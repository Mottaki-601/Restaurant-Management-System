using _1291387.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace _1291387
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<ApplicationDbContext>(null);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            var context = new _1291387.Models.ApplicationDbContext();
            var roleManager = new Microsoft.AspNet.Identity.RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new Microsoft.AspNet.Identity.EntityFramework.RoleStore<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(context));
            var userManager = new Microsoft.AspNet.Identity.UserManager<_1291387.Models.ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<_1291387.Models.ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Admin"));
            }
            if (!roleManager.RoleExists("User"))
            {
                roleManager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("User"));
            }

            var user = new _1291387.Models.ApplicationUser();

            user.UserName = "admin";
            user.Email = "admin";

            userManager.PasswordValidator = new Microsoft.AspNet.Identity.PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            userManager.UserValidator = new Microsoft.AspNet.Identity.UserValidator<_1291387.Models.ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            var check = userManager.Create(user, "admin");

            if (check.Succeeded)
            {
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
