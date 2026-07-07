using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DotComTrading.Models;
using DotComTrading.Data;

namespace DotComTrading.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DotComTradingDBContext _context;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, DotComTradingDBContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        //Initiates OAuth Login Flow
        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string returnUrl = "https://localhost:7021/home")
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", new { returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return Challenge(properties, "Google");
        }

        //Handles Google's Response After Login
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "https://localhost:7021/home")
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return Content("Google Login Failed.");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false); //Attempts to sign in user if account already linked

            ApplicationUser? user = null;

            if (result.Succeeded) 
            {
                user = await _userManager.GetUserAsync(User); //Existing User Signed In Successfully

                if(user == null)
                {
                    return Content("Google Login Failed.");
                }
			}
            else
            {
                var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrWhiteSpace(email))
                {
                    return Content("Google Login Failed.");
                }

                user = await _userManager.FindByEmailAsync(email); //Checks if user already exists

                if (user == null)
                {
                    user = new ApplicationUser //Creates new user if does not exist
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    var createUserResult = await _userManager.CreateAsync(user);

                    if (!createUserResult.Succeeded)
                    {
                        return Content("Google Login Failed.");
                    }
                }

                var existingLogins = await _userManager.GetLoginsAsync(user);
                var alreadyLinked = existingLogins.Any(login => login.LoginProvider == loginInfo.LoginProvider && login.ProviderKey == loginInfo.ProviderKey);

                if (!alreadyLinked)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

                    if (!addLoginResult.Succeeded)
                    {
                        return Content("Google Login Failed.");
                    }
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            var existingPortfolio = _context.Portfolios.FirstOrDefault(p => p.UserId == user.Id);

            if (existingPortfolio == null)
            {
                existingPortfolio = new Portfolio
                {
                    UserId = user.Id,
                    Balance = 10000m
                };

                _context.Portfolios.Add(existingPortfolio);
                await _context.SaveChangesAsync();
            }

            string username = "";

            if(!string.IsNullOrWhiteSpace(user.UserName))
            {
                username = user.UserName;
            }
            else if (!string.IsNullOrWhiteSpace(user.Email))
            {
                username = user.Email;  
            }

            string urlSeparator = "?";

            if (returnUrl.Contains("?"))
            {
                urlSeparator = "&";
            }

            string returnUrlWithData = returnUrl + urlSeparator + "username=" + Uri.EscapeDataString(username) + "&portfolioId=" + existingPortfolio.Id; //Creates URL to return with user data for frontend

            return Redirect(returnUrlWithData);
        }
    }
}
