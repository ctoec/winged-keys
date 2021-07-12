using System;
using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;
using WingedKeys.Models;

namespace WingedKeys.Services
{
    public class TwoFactorService
    {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public TwoFactorService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task sendTwoFactorCode(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception("No user found for email address provided");
            }

            if (!await _signInManager.CanSignInAsync(user))
            {
                throw new Exception("User is not eligible to sign in at this time");
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                throw new Exception("No two factor provider found for email.  This should never happen.");
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await new EmailService().SendEmailAsync(user.Email, "ECE Reporter Verification Code", "Your ECE Reporter verification code is " + token);
        }
    }
}
