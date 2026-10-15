
using Microsoft.AspNetCore.Identity;

namespace RaceDay.API.Identity
{
    public class RaceDayUser : IdentityUser
    {
        // Links the login account to an existing
        // participant profile when applicable.
        public int? ParticipantID { get; set; }

        // Links the login account to an existing
        // organiser profile when applicable.
        public int? AdminID { get; set; }

        // Records when the account was created.
        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}
