using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WingedKeys.Data;
using WingedKeys.Models;

namespace WingedKeys
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var host = CreateHostBuilder(args).Build();

			var environment = GetEnvironmentNameFromAppSettings();

			if (environment != Environments.Production)
			{
				using (var scope = host.Services.CreateScope())
				{
					var services = scope.ServiceProvider;
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogInformation("Detected environment " + environment);

					try
					{
						var persistedGrantDbContext = services.GetRequiredService<PersistedGrantDbContext>();
						var configurationDbContext = services.GetRequiredService<ConfigurationDbContext>();
						var wingedKeysContext = services.GetRequiredService<WingedKeysContext>();
						var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();
						var configuration = services.GetRequiredService<IConfiguration>();
						var config = new Config(configuration);
						DatabaseInitializer.Initialize(persistedGrantDbContext, configurationDbContext, wingedKeysContext, userMgr, config, configuration);
						logger.LogInformation("Database initialization complete");
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "An error occurred while seeding the database.");
					}
				}
			}

			host.Run();
		}

		public static IWebHostBuilder CreateHostBuilder(string[] args) {
			var environment = GetEnvironmentNameFromAppSettings();
			return WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging((context, logging) =>
				{
					logging.ClearProviders();

					logging.AddConfiguration(context.Configuration.GetSection("Logging"));
					logging.AddConsole();
					logging.AddDebug();

					if (environment != Environments.Development)
					{
						logging.AddAWSProvider(context.Configuration.GetAWSLoggingConfigSection());
						// logging.AddEventLog();
					}

			})
			.UseEnvironment(environment)
			.UseStartup<Startup>();
		}

		private static string GetEnvironmentNameFromAppSettings(string defaultValue = null)
		{
			return new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true)
				.Build()
				.GetValue<string>("EnvironmentName", defaultValue ?? "Development");
		}
	}
}
