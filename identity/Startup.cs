using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;

namespace identity
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddIdentityServer()
                .AddInMemoryClients(Clients.Get())                         
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddInMemoryApiScopes(Resources.GetApiScopes())
                .AddTestUsers(Users.Get())                     
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }

    internal class Clients
        {
            public static IEnumerable<Client> Get()
            {
                return new List<Client>
                {
                    new Client
                    {
                        ClientId = "oauthClient",
                        ClientName = "Example client application using client credentials",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = new List<Secret> {new Secret("SuperSecretPassword".Sha256())}, // change me!
                        AllowedScopes = new List<string> {"api1.read"}
                    },
                    new Client
                    {
                        ClientId = "oidcClient",
                        ClientName = "Example Client Application",
                        ClientSecrets = new List<Secret> {new Secret("SuperSecretPassword".Sha256())}, // change me!
    
                        AllowedGrantTypes = GrantTypes.Code,
                        RedirectUris = new List<string> {"https://localhost:6001/signin-oidc"},
                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Email,
                            "role",
                            "api1.read"
                        },

                        RequirePkce = true,
                        AllowPlainTextPkce = false
                    },
                        new Client
                    {
                        ClientId = "client2",
                        ClientName = "Example Client Application",
                        ClientSecrets = new List<Secret> {new Secret("SuperSecretPassword".Sha256())}, // change me!
    
                        AllowedGrantTypes = GrantTypes.Code,
                        RedirectUris = new List<string> {"https://localhost:8001/signin-oidc"},
                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Email,
                            "role",
                            "api1.read"
                        },

                        RequirePkce = true,
                        AllowPlainTextPkce = false
                    }
                };
            }
        }


    internal class Resources
        {
            public static IEnumerable<IdentityResource> GetIdentityResources()
            {
                return new[]
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email(),
                    new IdentityResource
                    {
                        Name = "role",
                        UserClaims = new List<string> {"role"}
                    }
                };
            }

            public static IEnumerable<ApiResource> GetApiResources()
            {
                return new[]
                {
                    new ApiResource
                    {
                        Name = "api1",
                        DisplayName = "API #1",
                        Description = "Allow the application to access API #1 on your behalf",
                        Scopes = new List<string> {"api1.read", "api1.write"},
                        ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                        UserClaims = new List<string> {"role"}
                    }
                };
            }
	
	        public static IEnumerable<ApiScope> GetApiScopes()
            {
                return new[]
                {
                    new ApiScope("api1.read", "Read Access to API #1"),
			        new ApiScope("api1.write", "Write Access to API #1")
                };
            }
        }


    internal class Users
        {
            public static List<TestUser> Get()
            {
                return new List<TestUser> {
                    new TestUser {
                        SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                        Username = "scott",
                        Password = "password",
                        Claims = new List<Claim> {
                            new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
                            new Claim(JwtClaimTypes.Role, "admin")
                        }
                    }
                };
            }
        }
}
