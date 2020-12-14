using Microsoft.AspNetCore.Identity;
using System;

namespace WingedKeys.Models
{
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// Gets or sets the last login timestamp for the given user.
		/// </summary>
		public virtual DateTimeOffset? LastLogin
		{
			get;
			set;
		}
	}
}
