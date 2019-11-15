using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace WingedKeys.Data
{
	public static class DatabaseInitializer
	{
		public static void Initialize(
			PersistedGrantDbContext persistedGrantDbContext,
			ConfigurationDbContext configurationDbContext,
			Config config)
		{
			persistedGrantDbContext.Database.EnsureCreated();
			configurationDbContext.Database.EnsureCreated();

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
		}
	}
}
