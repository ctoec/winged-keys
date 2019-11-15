using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WingedKeys
{
	public class Startup
	{
		public IWebHostEnvironment Environment { get; }
		public IConfiguration Configuration { get; }

		private readonly string WingedKeysConnectionString;
		private readonly string MigrationsAssembly;
		private readonly string Issuer;

		public Startup(IWebHostEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;

			WingedKeysConnectionString = Configuration.GetConnectionString("WINGEDKEYS");
			MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
			Issuer = Configuration.GetValue<string>("Issuer");
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
			});

			// MVC
			services.AddMvc(option => option.EnableEndpointRouting = false);

			// Identity Server
			var identityServerServices = services
				.AddIdentityServer(options => { options.IssuerUri = Issuer; })
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
				});

				if (Environment.IsDevelopment())
				{
					identityServerServices.AddTestUsers(new Config(Configuration).GetUsers());
					identityServerServices.AddDeveloperSigningCredential();
				}				
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseCors("AllowAll");
			}

			app.UseStaticFiles();
			app.UseIdentityServer();
			app.UseMvcWithDefaultRoute();
		}
	}
}
