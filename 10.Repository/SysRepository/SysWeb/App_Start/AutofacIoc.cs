using Autofac;
using Autofac.Integration.Mvc;
using SysApplication;
using SysEntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;

namespace SysWeb.App_Start
{
    public class AutofacIoc
    {
        /// <summary>
        /// Autofac依赖注入
        /// </summary>
        public static void AutofacRegister()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SysDbContext>();
            var baseType = typeof(IDependency);
            var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            builder.RegisterControllers(assemblies).PropertiesAutowired();
            builder.RegisterAssemblyTypes(assemblies)
                   .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
                   .Where(c => c.Name.EndsWith("Service"))
                   .AsSelf()
                   .AsImplementedInterfaces()
                   .PropertiesAutowired()
                   .InstancePerLifetimeScope();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //Web Api注入
            //var builder = new ContainerBuilder();
            //builder.RegisterType<GPEntities>();
            //var baseType = typeof(IDependency);
            //var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //builder.RegisterControllers(assemblies).PropertiesAutowired();
            //builder.RegisterApiControllers(assemblies).PropertiesAutowired();
            //builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            //builder.RegisterWebApiModelBinderProvider();
            //builder.RegisterAssemblyTypes(assemblies)
            //       .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
            //       .Where(c => c.Name.EndsWith("Service"))
            //       .AsSelf()
            //       .AsImplementedInterfaces()
            //       .PropertiesAutowired()
            //       .InstancePerLifetimeScope();
            //var container = builder.Build();
            //GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}