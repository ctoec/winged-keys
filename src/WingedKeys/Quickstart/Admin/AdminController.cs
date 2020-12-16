using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WingedKeys.Models;
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
				public async Task<IActionResult> TriggerInitialLogin(
					bool? success,
					string emailSent
				)
				{

						var vm = new TriggerInitialLoginViewModel();

						vm.Success = success;
						vm.EmailSent = email.sent;

						return View(vm);
				}

				[HttpPost]
				[ValidateAntiForgeryToken]
				public async Task<IActionResult> TriggerInitialLogin(TriggerInitialLoginInputModel model, string button)
				{
					// the user clicked the "cancel" button
					if (button != "create")
					{
						return Redirect("~/Admin/TriggerInitialLogin");
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
				public IActionResult AccessDenied()
				{
					return View();
				}

				/*****************************************/
				/* helper APIs for the AdminController */
				/*****************************************/
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