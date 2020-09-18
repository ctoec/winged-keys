using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityModel;
using WingedKeys.Models;

namespace WingedKeys.Data
{
	public static class DatabaseInitializer
	{
		private static readonly string TEST_USER_ID = "2c0ec653-8829-4aa1-82ba-37c8832bbb88";

		public static async void Initialize(
			PersistedGrantDbContext persistedGrantDbContext,
			ConfigurationDbContext configurationDbContext,
			WingedKeysContext wingedKeysContext,
			UserManager<ApplicationUser> userMgr,
			Config config)
		{
			persistedGrantDbContext.Database.EnsureCreated();
			configurationDbContext.Database.EnsureCreated();
			wingedKeysContext.Database.EnsureCreated();

			DeleteAllData(configurationDbContext);

			AddClients(config, configurationDbContext);
			AddIdentityResources(config, configurationDbContext);
			AddApiResources(config, configurationDbContext);

			if ((await userMgr.FindByIdAsync(DatabaseInitializer.TEST_USER_ID)) == null)
			{
				AddTestApplicationAdminUser(userMgr);
			}
		}

		private static void AddClients(
			Config config,
			ConfigurationDbContext configurationDbContext)
		{
			foreach (var client in config.GetClients())
			{
				configurationDbContext.Clients.Add(client.ToEntity());
			}
			configurationDbContext.SaveChanges();
		}

		private static void AddIdentityResources(
			Config config,
			ConfigurationDbContext configurationDbContext)
		{
			foreach (var resource in config.GetIdentityResources())
			{
				configurationDbContext.IdentityResources.Add(resource.ToEntity());
			}
			configurationDbContext.SaveChanges();
		}

		private static void AddApiResources(
			Config config,
			ConfigurationDbContext configurationDbContext)
		{
			foreach (var resource in config.GetApis())
			{
				configurationDbContext.ApiResources.Add(resource.ToEntity());
			}
			configurationDbContext.SaveChanges();
		}

		private static void AddTestApplicationAdminUser(
			UserManager<ApplicationUser> userMgr
		)
		{
			var voldemort = new ApplicationUser
			{
				Id = DatabaseInitializer.TEST_USER_ID,
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
				new Claim("role", "developer"),
				new Claim("role", "admin"),
			}).Result;

			if (!result.Succeeded)
			{
					throw new Exception(result.Errors.First().Description);
			}
		}

		private static void DeleteAllData(
			ConfigurationDbContext configurationDbContext)
		{
			// Delete tables that are modified in Config.cs
			configurationDbContext.Clients.RemoveRange(
				configurationDbContext.Clients.ToList());
			configurationDbContext.IdentityResources.RemoveRange(
				configurationDbContext.IdentityResources.ToList());
			configurationDbContext.ApiResources.RemoveRange(
				configurationDbContext.ApiResources.ToList());
		}
	}
}
