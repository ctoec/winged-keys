using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WingedKeys.Models;

namespace WingedKeys.Data
{
	public class WingedKeysContext : IdentityDbContext<ApplicationUser>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public WingedKeysContext(
			DbContextOptions<WingedKeysContext> options,
			IHttpContextAccessor httpContextAccessor
		) : base(options)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
				base.OnModelCreating(builder);
				// Customize the ASP.NET Identity model and override the defaults if needed.
				// For example, you can rename the ASP.NET Identity table names and more.
				// Add your customizations after calling base.OnModelCreating(builder);
				builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
		}
	}
}
