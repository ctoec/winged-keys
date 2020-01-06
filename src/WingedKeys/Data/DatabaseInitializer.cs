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

			DeleteAllData(
				configurationDbContext,
				wingedKeysContext,
				userMgr);

			AddClients(config, configurationDbContext);
			AddIdentityResources(config, configurationDbContext);
			AddApiResources(config, configurationDbContext);
			AddTestApplicationUsers(userMgr);
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

		private static void AddTestApplicationUsers(
			UserManager<ApplicationUser> userMgr
		)
		{
			AddUser(
				userMgr,
				"Tom Marvolo Riddle",
				"Voldemort",
				"Riddle",
				"voldemort@hogwarts.uk.co",
				"voldemort",
				"thechosenone",
				"2c0ec653-8829-4aa1-82ba-37c8832bbb88"
			);
		}

		private static void AddUser(
			UserManager<ApplicationUser> userMgr,
			string fullName,
			string givenName,
			string familyName,
			string username,
			string email,
			string password,
			string id = null)
		{
			ApplicationUser user;
			if (id != null)
			{
				user = new ApplicationUser
				{
					Id = id,
					UserName = username,
					Email = email,
					EmailConfirmed = true
				};
			}
			else
			{
				user = new ApplicationUser
				{
					UserName = username,
					Email = email,
					EmailConfirmed = true
				};
			}
			
			var result = userMgr.CreateAsync(user, password).Result;
			if (!result.Succeeded)
			{
				Console.WriteLine("Unable to create {0}", username);
				return;
			}

			result = userMgr.AddClaimsAsync(user, new Claim[]{
				new Claim(JwtClaimTypes.Name, fullName),
				new Claim(JwtClaimTypes.GivenName, givenName),
				new Claim(JwtClaimTypes.FamilyName, familyName),
				new Claim(JwtClaimTypes.Email, email),
				new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
			}).Result;

			if (!result.Succeeded)
			{
				Console.WriteLine(result.Errors.First().Description);
				return;
			}
		}

		private static void DeleteAllData(
			ConfigurationDbContext configurationDbContext,
			WingedKeysContext wingedKeysContext,
			UserManager<ApplicationUser> userMgr)
		{
			// Delete tables that are modified in Config.cs
			configurationDbContext.Clients.RemoveRange(
				configurationDbContext.Clients.ToList());
			configurationDbContext.IdentityResources.RemoveRange(
				configurationDbContext.IdentityResources.ToList());
			configurationDbContext.ApiResources.RemoveRange(
				configurationDbContext.ApiResources.ToList());

			// Delete Users and AspNetUserClaims
			var applicationUsers = wingedKeysContext.ApplicationUsers.ToList();
			foreach (var user in applicationUsers)
			{
				var claims = userMgr.GetClaimsAsync(user).GetAwaiter().GetResult();
				userMgr.RemoveClaimsAsync(user, claims).GetAwaiter().GetResult();
			}
			wingedKeysContext.ApplicationUsers.RemoveRange(applicationUsers);
		}
	}
}
