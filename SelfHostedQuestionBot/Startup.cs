using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SelfHostedQuestionBot
{
    public class Startup
    {
        private const String _GamePageUrl = "Http://www.yourgameurl.com";
        private static String _Authpage = File.ReadAllText(Path.Combine(Path.Combine(Environment.CurrentDirectory, @"..\.."), @"Auth/Index.html"));

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            string lContentPath = Path.Combine(Environment.CurrentDirectory, @"..\..");
            var fileOptions = new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(lContentPath),
            };

            app.UseStaticFiles(fileOptions);
            if (!String.IsNullOrEmpty(_Authpage))
                _Authpage = File.ReadAllText(Path.Combine(lContentPath, @"Auth/Index.html"));

            CookieAuthenticationOptions lOptions = new CookieAuthenticationOptions()
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Auth/Login"),
                LogoutPath = new PathString("/Auth/Logout"),
            };

            app.UseCookieAuthentication(lOptions);

            app.Use(async (context, next) =>
            {
                var redirectUri = context.Request.Query["ReturnUrl"];

                //If accesses the login path and is not logged in and is a POST request
                if (context.Request.Path.Value.Contains(lOptions.LoginPath.Value) && context.Authentication.User == null)
                {
                    if (context.Request.Method == "POST")
                    {
                        //Do we have the needed form data to validate the user
                        var lForm = await context.Request.ReadFormAsync();
                        if (!String.IsNullOrEmpty(lForm["input_login"]) && !String.IsNullOrEmpty(lForm["input_pass"]))
                        {
                            //Validate him
                            var lAuthenticatedUser = new UserManager().Authenticate(lForm["input_login"], lForm["input_pass"]);
                            if (lAuthenticatedUser != null)
                            {
                                //If the user exists, set claims
                                ClaimsIdentity lIdentity = new ClaimsIdentity(lOptions.AuthenticationType);
                                lIdentity.AddClaim(new Claim(ClaimTypes.Name, lAuthenticatedUser.Username));

                                //log user in
                                context.Authentication.SignIn(lIdentity);

                                //send user (authorized) to our game site
                                context.Response.Redirect(_GamePageUrl);

                                //At this point the user will be able to connect to the hub AND access methods with the [Authorize] tag
                            }
                        }
                    }
                    else
                    {
                        //If not, send him back to the login page
                        context.Response.Redirect("/Auth/");
                    }
                }
                else if (context.Request.Path.Value.Contains(lOptions.LogoutPath.Value))
                {//If the user is accessing the logout path

                    //log him out
                    context.Authentication.SignOut(lOptions.AuthenticationType);

                    //and send him back to the login screen
                    context.Response.Redirect("/Auth");
                }
                else if (context.Request.Path.Value == "/Auth/")
                {
                    //Is the user already logged in, send him to the game site
                    if (context.Authentication.User != null)
                        context.Response.Redirect(_GamePageUrl);


                    context.Response.ContentType = "text/html";
                    //Send him the login form
                    await context.Response.WriteAsync(_Authpage);
                }
                else
                {
                    //If none of the above applies
                    await next();
                }
            });

            app.MapSignalR();
        }
    }
}
