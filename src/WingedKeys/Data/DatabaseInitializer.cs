using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityModel;
using WingedKeys.Models;

namespace WingedKeys.Data
{
	public static class DatabaseInitializer
	{
		public static void Initialize(
			PersistedGrantDbContext persistedGrantDbContext,
			ConfigurationDbContext configurationDbContext,
			WingedKeysContext wingedKeysContext,
			UserManager<ApplicationUser> userMgr,
			Config config)
		{
			persistedGrantDbContext.Database.EnsureCreated();
			configurationDbContext.Database.EnsureCreated();
			wingedKeysContext.Database.EnsureCreated();

			if (!configurationDbContext.Clients.Any())
			{
				foreach (var client in config.GetClients())
				{
					configurationDbContext.Clients.Add(client.ToEntity());
				}
				configurationDbContext.SaveChanges();
			}

			if (!configurationDbContext.IdentityResources.Any())
			{
				foreach (var resource in config.GetIdentityResources())
				{
					configurationDbContext.IdentityResources.Add(resource.ToEntity());
				}
				configurationDbContext.SaveChanges();
			}

			if (!configurationDbContext.ApiResources.Any())
			{
				foreach (var resource in config.GetApis())
				{
					configurationDbContext.ApiResources.Add(resource.ToEntity());
				}
				configurationDbContext.SaveChanges();
			}

			if (!wingedKeysContext.ApplicationUsers.Any())
			{
				var voldemort = new ApplicationUser
				{
					Id = "2c0ec653-8829-4aa1-82ba-37c8832bbb88",
					UserName = "voldemort",
					Email = "voldemort@hogwarts.uk.co",
					EmailConfirmed = true
				};
				var result = userMgr.CreateAsync(voldemort, "thechosenone").Result;
				if (!result.Succeeded)
				{
						throw new Exception(result.Errors.First().Description);
				}

				result = userMgr.AddClaimsAsync(voldemort, new Claim[]{
					new Claim(JwtClaimTypes.Name, "Tom Marvolo Riddle"),
					new Claim(JwtClaimTypes.GivenName, "Voldemort"),
					new Claim(JwtClaimTypes.FamilyName, "Riddle"),
					new Claim(JwtClaimTypes.Email, "voldemort@hogwarts.uk.co"),
					new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
					new Claim("role", "developer")
				}).Result;

				if (!result.Succeeded)
				{
						throw new Exception(result.Errors.First().Description);
				}
			}
		}
	}
}
