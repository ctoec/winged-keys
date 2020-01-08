using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using WingedKeys.Data;
using WingedKeys.Models;

namespace WingedKeys
{
	public class Startup
	{
		public IWebHostEnvironment Environment { get; }
		public IConfiguration Configuration { get; }

		private readonly string WingedKeysConnectionString;
		private readonly string MigrationsAssembly;

		public Startup(IWebHostEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;

			WingedKeysConnectionString = Configuration.GetConnectionString("WINGEDKEYS");
			MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// CORS
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					builder =>
					{
						builder
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowAnyOrigin();
					}
				);
				options.AddPolicy("Production",
					builder =>
					{
						builder.WithOrigins(Configuration.GetValue<string>("ClientUris"));
					}
				);
			});

			services.AddDbContext<WingedKeysContext>(options =>
				options.UseSqlServer(WingedKeysConnectionString)
			);

			// MVC
			services.AddMvc(option => option.EnableEndpointRouting = false);

			 services.AddIdentity<ApplicationUser, IdentityRole>(config =>
				{
						config.SignIn.RequireConfirmedEmail = true;
						config.User.RequireUniqueEmail = true;
						// Password requirements
						config.Password.RequireDigit = false;
						config.Password.RequiredLength = 6;
						config.Password.RequiredUniqueChars = 1;
						config.Password.RequireLowercase = false;
						config.Password.RequireNonAlphanumeric = false;
						config.Password.RequireUppercase = false;
				})
				.AddEntityFrameworkStores<WingedKeysContext>()
				.AddDefaultTokenProviders();

			// Identity Server
			var identityServerServices = services
				.AddIdentityServer(options =>
				{
					var baseUri = Configuration.GetValue<string>("BaseUri");
					var issuerUri = Configuration.GetValue<string>("IssuerUri");
					if (issuerUri != "")
					{
						options.IssuerUri = issuerUri;
					}
					else
					{
						options.PublicOrigin = baseUri;
					}

					if (Environment.IsDevelopment())
					{
						IdentityModelEventSource.ShowPII = true;
					}
				})
				.AddConfigurationStore(options =>
				{
					options.ConfigureDbContext = b =>
						b.UseSqlServer(
							WingedKeysConnectionString,
							sql => sql.MigrationsAssembly(MigrationsAssembly)
						);
				})
				// this adds the operational data from DB (codes, tokens, consents)
				.AddOperationalStore(options =>
				{
					options.ConfigureDbContext = b =>
						b.UseSqlServer(
							WingedKeysConnectionString,
							sql => sql.MigrationsAssembly(MigrationsAssembly)
						);

					// this enables automatic token cleanup. this is optional.
					options.EnableTokenCleanup = true;
				})
				.AddAspNetIdentity<ApplicationUser>();

				if (Environment.IsDevelopment())
				{
					identityServerServices.AddDeveloperSigningCredential();
				}
				else
				{
					var currentDirectory = Path.GetDirectoryName(
						System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase
					).Replace("file:\\\\", "");
					var certificateFileName = Configuration.GetValue<string>("CertificateFileName");
					var certificatePath = Path.Join(currentDirectory, certificateFileName);
					var certificatePassword = Configuration.GetValue<string>("CertificatePassword");
					var certificate = new X509Certificate2(certificateFileName, certificatePassword, X509KeyStorageFlags.MachineKeySet);
					identityServerServices.AddSigningCredential(certificate);
				}

				services.AddAuthentication();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseCors("AllowAll");
			}
			else
			{
				app.UseCors("Production");
			}

			app.UseRouting();
			app.UseStaticFiles();
			app.UseIdentityServer();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
