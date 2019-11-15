using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WingedKeys.Data;

namespace WingedKeys
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateWebHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var persistedGrantDbContext = services.GetRequiredService<PersistedGrantDbContext>();
					var configurationDbContext = services.GetRequiredService<ConfigurationDbContext>();
					var configuration = services.GetRequiredService<IConfiguration>();
					var config = new Config(configuration);
					DatabaseInitializer.Initialize(persistedGrantDbContext, configurationDbContext, config);
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while seeding the database.");
				}
			}

			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
		}
			
	}
}
