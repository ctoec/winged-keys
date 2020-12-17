using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WingedKeys.Models;
using WingedKeys.Services;
using System.Security.Claims;

namespace IdentityServer4.Quickstart.UI
{
		[SecurityHeaders]
		[Authorize(Policy = "AdminOnly")]
		public class AdminController : Controller
		{
				private readonly UserManager<ApplicationUser> _userManager;

				public AdminController(UserManager<ApplicationUser> userManager)
				{
						_userManager = userManager;
				}

				/// <summary>
				/// Entry point into the admin workflow
				/// </summary>
				[HttpGet]
				public async Task<IActionResult> NewAccount(
					bool? success
				)
				{
						// build a model so we know what to show on the login page
						var vm = BuildNewAccountViewModel();
						vm.Success = success;
						return View(vm);
				}

				/// <summary>
				/// Handle postback from new account creation
				/// </summary>
				[HttpPost]
				[ValidateAntiForgeryToken]
				public async Task<IActionResult> NewAccount(NewAccountInputModel model, string button)
				{
					// the user clicked the "cancel" button
					if (button != "create")
					{
						return Redirect("~/Admin/NewAccount");
					}

					string error = null;
					if (ModelState.IsValid)
					{
						var username = model.Username;
						var user = await _userManager.FindByNameAsync(username);
						if (user != null)
						{
							error = "User with username already exists";
						}
						else
						{
							user = new ApplicationUser
							{
								UserName = model.Username,
								Email = model.Email,
								EmailConfirmed = true
							};
							var result = _userManager.CreateAsync(user, model.Password).Result;
							if (!result.Succeeded)
							{
								error = result.Errors.First().Description;
							}
							else
							{
								result = _userManager.AddClaimsAsync(user, new Claim[]{
									new Claim(JwtClaimTypes.Name, model.GivenName + " " + model.FamilyName),
									new Claim(JwtClaimTypes.GivenName, model.GivenName),
									new Claim(JwtClaimTypes.FamilyName, model.FamilyName),
									new Claim(JwtClaimTypes.Email, model.Email),
									new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
								}).Result;

								if (!result.Succeeded)
								{
									error = result.Errors.First().Description;
								}
								else
								{
									return Redirect("~/Admin/NewAccount?success=true");
								}
							}
						}
					}
					else
					{
						error = "Missing information. All fields are required";
					}

					// something went wrong, show form with error
					var errorVm = BuildNewAccountViewModel(error);
					return View(errorVm);
				}

				[HttpGet]
				public IActionResult InviteUser(
					bool? success,
					string emailRecipient = null
				)
				{
					return View(new InviteUserViewModel { Success = success, EmailRecipient = emailRecipient });
				}

				[HttpPost]
				[ValidateAntiForgeryToken]
				public async Task<IActionResult> InviteUser(InviteUserInputModel model)
				{
					string error;

					if (ModelState.IsValid)
					{
						var user = await _userManager.FindByNameAsync(model.Username);

						if (user == null)
						{
							error = "No user found for '" + model.Username + "'.";
						} else if (await _userManager.IsEmailConfirmedAsync(user))
						{
							error = "User '" + model.Username + "' has not yet confirmed their email address.";
						}
						else
						{
							string token = await _userManager.GeneratePasswordResetTokenAsync(user);
							var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, protocol: Request.Scheme);

							await new EmailService().SendEmailAsync(user.Email, "Welcome to ECE Reporter!", BuildInviteUserEmail(callbackUrl));

							return View(new InviteUserViewModel { Success = true, EmailRecipient = user.Email });
						}
					}
					else
					{
						error = "Username is required.";
					}

					return View(new InviteUserViewModel { Error = error });
				}

				[HttpGet]
				public IActionResult AccessDenied()
				{
					return View();
				}

				/*****************************************/
				/* helper APIs for the AdminController */
				/*****************************************/
				private string BuildInviteUserEmail(string callbackUrl)
				{
					return "<h1>Welcome to ECE Reporter!</h1> <p>Good news - your account setup is nearly complete!  Once you update your password, you will be able to access the application.<br/><a href=\"" + callbackUrl + "\">Click here to set your password.</a></p>";
				}

				private NewAccountViewModel BuildNewAccountViewModel()
				{
					return new NewAccountViewModel();
				}

				private NewAccountViewModel BuildNewAccountViewModel(string error)
				{
					var vm = BuildNewAccountViewModel();
					if (error != null) 
					{
						vm.Success = false;
						vm.Error = error;
					}
					return vm;
				}
		}
}