/*
 * Modified from docs.identityserver.io
 */
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace WingedKeys
{
	public class Config
	{
		private readonly IConfiguration Configuration;

		public Config(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public List<TestUser> GetUsers()
		{
			return new List<TestUser>
			{
				new TestUser
				{
					SubjectId = "1",
					Username = "voldemort",
					Password = "thechosenone",

					Claims = new []
					{
						new Claim("name", "Voldemort"),
						new Claim("allowed_apps", "hedwig"),
						new Claim("role", "developer"),
						new Claim("role", "oec-admin"),
					}
				}
			};
		}

		public IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
			};
		}

		// http://docs.identityserver.io/en/latest/topics/resources.html#defining-api-resources
		public IEnumerable<ApiResource> GetApis()
		{
			return new List<ApiResource>
			{
				new ApiResource
				{
					Name = "hedwig_backend",

					Description = "Hedwig API",

					// include the following using claims in access token (in addition to subject id)
					UserClaims = { "role" },

					Scopes =
					{
						new Scope()
						{
							Name = "hedwig_backend",
							DisplayName = "Full access to Hedwig API",
							UserClaims = new [] { "role", "allowed_apps" }
						}
					}
				}
			};
		}

		public IEnumerable<Client> GetClients()
		{
			var accessTokenLifetime = Configuration.GetValue<int>("AccessTokenLifetime");
			var baseUri = Configuration.GetValue<string>("BaseUri");
			var clientUrisRaw = Configuration.GetValue<string>("ClientUris");
			var clientUris = clientUrisRaw.Split(",", StringSplitOptions.RemoveEmptyEntries);

			var redirectEndpoint = "/login/callback";

			var allowedCorsOrigins = clientUris;
			var postLogoutRedirectUris = clientUris;
			string[] redirectUris = clientUris.Select(uri => $"{uri}{redirectEndpoint}").ToArray();

			return new List<Client>
			{
				// Hedwig Client
				new Client
				{
					ClientId = "hedwig",
					ClientName = "Hedwig Client",
					AllowedGrantTypes = GrantTypes.Code,
					RequireClientSecret = false,
					RequireConsent = false,

					RedirectUris =           redirectUris,
					PostLogoutRedirectUris = postLogoutRedirectUris,
					AllowedCorsOrigins =     allowedCorsOrigins,

					AccessTokenLifetime = accessTokenLifetime,

					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"hedwig_backend"
					},

					AllowOfflineAccess = true,
				}
			};
		}
	}
}
