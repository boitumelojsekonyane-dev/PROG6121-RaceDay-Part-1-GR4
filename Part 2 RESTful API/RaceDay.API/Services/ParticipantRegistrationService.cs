
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Identity;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class ParticipantRegistrationService
    {
        private readonly RaceDayDbContext _raceDayContext;
        private readonly RaceDayIdentityDbContext _identityContext;
        private readonly UserManager<RaceDayUser> _userManager;

        public ParticipantRegistrationService(
            RaceDayDbContext raceDayContext,
            RaceDayIdentityDbContext identityContext,
            UserManager<RaceDayUser> userManager)
        {
            _raceDayContext = raceDayContext;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterAsync(
            RegisterRequest request)
        {
            // Ensure both contexts share the same connection
            // and transaction.


            if (!ReferenceEquals(
                _raceDayContext.Database.GetDbConnection(),
                _identityContext.Database.GetDbConnection()))
            {
                throw new InvalidOperationException(
                    "Registration requires both database contexts to share the same connection.");
            }

            await using var transaction =
                await _raceDayContext.Database.BeginTransactionAsync();

            await _identityContext.Database.UseTransactionAsync(
                transaction.GetDbTransaction());

            try
            {


                var email = request.Email.Trim();
                var userName = request.UserName.Trim();

                var identityNumber = string.IsNullOrWhiteSpace(request.IdentityNumber)
                    ? null
                    : request.IdentityNumber.Trim();

                var passportNumber = string.IsNullOrWhiteSpace(request.PassportNumber)
                    ? null
                    : request.PassportNumber.Trim();

                // Check existing participant records.
                var duplicateParticipant =
                    await _raceDayContext.Participants.AnyAsync(p =>
                        p.Email == email ||
                        p.UserName == userName ||
                        (identityNumber != null &&
                         p.IdentityNumber == identityNumber) ||
                        (passportNumber != null &&
                         p.PassportNumber == passportNumber));

                // Check existing login accounts.
                var duplicateIdentityUser =
                    await _userManager.FindByEmailAsync(email) != null ||
                    await _userManager.FindByNameAsync(userName) != null;

                if (duplicateParticipant || duplicateIdentityUser)
                {
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "RegistrationConflict",
                            Description = "Registration could not be completed."
                        });
                }

                var participant = new Participant
                {
                    FirstName = request.FirstName,
                    Surname = request.Surname,
                    Email = email,
                    UserName = userName,
                    PhoneNumber = request.PhoneNumber,
                    EmergencyContact = request.EmergencyContact,
                    DateOfBirth = DateOnly.FromDateTime(
                        request.DateOfBirth),
                    IdentityNumber = identityNumber,
                    PassportNumber = passportNumber,
                    Gender = request.Gender,
                    Province = request.Province
                };

                _raceDayContext.Participants.Add(participant);
                await _raceDayContext.SaveChangesAsync();

                var user = new RaceDayUser
                {
                    UserName = userName,
                    Email = email,
                    ParticipantID = participant.ParticipantID,
                    AdminID = null
                };

                var createResult =
                    await _userManager.CreateAsync(
                        user, request.Password);

                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return createResult;
                }

                var roleResult =
                    await _userManager.AddToRoleAsync(
                        user, "Participant");

                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return roleResult;
                }

                await transaction.CommitAsync();

                return IdentityResult.Success;
            }


            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RegistrationDatabaseError",
                        Description = "Registration could not be completed."
                    });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}

