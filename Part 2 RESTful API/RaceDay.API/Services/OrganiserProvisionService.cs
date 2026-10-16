
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Identity;

namespace RaceDay.API.Services
{
    public class OrganiserProvisioningService
    {
        private readonly UserManager<RaceDayUser> _userManager;
        private readonly RaceDayDbContext _context;

        public OrganiserProvisioningService(
            UserManager<RaceDayUser> userManager,
            RaceDayDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<IdentityResult> CreateOrganiserAccountAsync(
            int adminId,
            string password)
        {
            // Verify that the Organiser exists.
            var admin = await _context.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AdminID == adminId);

            if (admin == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Organiser record not found."
                    });
            }

            // Prevent multiple Identity accounts for one Organiser.
            var existingAccount = await _userManager.Users
                .AnyAsync(u => u.AdminID == adminId);

            if (existingAccount)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "This Organiser already has an Identity account."
                    });
            }

            // Prevent duplicate email accounts.
            var existingEmail = await _userManager
                .FindByEmailAsync(admin.Email);

            if (existingEmail != null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "An Identity account already uses this email."
                    });
            }

            // Create the Organiser's Identity account.
            var user = new RaceDayUser
            {
                UserName = admin.Email,
                Email = admin.Email,
                AdminID = admin.AdminID,
                ParticipantID = null,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(
                user, password);

            if (!result.Succeeded)
            {
                return result;
            }

            // Assign the Organiser role.
            var roleResult = await _userManager.AddToRoleAsync(
                user, "Organiser");


            if (!roleResult.Succeeded)
            {
                // Remove the newly created account if
                // Organiser role assignment fails.
                var deleteResult = await _userManager.DeleteAsync(user);

                if (!deleteResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Organiser role assignment failed, and " +
                        "the incomplete Identity account could not be removed.");
                }

                return roleResult;
            }

            return IdentityResult.Success;

        }

    }
}
