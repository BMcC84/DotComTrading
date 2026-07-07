using Microsoft.AspNetCore.Identity;

namespace DotComTrading.Models
{
    //Represents an authenticated user and links their portfolio
    public class ApplicationUser : IdentityUser
    {
        public Portfolio? Portfolio { get; set; }
    }
}
