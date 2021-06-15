using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using AutoMapper;
using Autofac.Integration.WebApi;

namespace WebApp
{
    public class AutofacConfig
    {
        /// <summary>
        /// Autofac依赖注入
        /// </summary>
        public static void Register()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule(Assembly.Load("WebApp")));
            builder.Register(r => new AutoMapperConfig().Register()).As<IMapper>().SingleInstance();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            #region WebApi注入

            ////初始化容器
            //var builder = new ContainerBuilder();
            ////注册服务
            //var baseType = typeof(ISuperService);
            //var interfaces = ConfigurationManager.AppSettings["IServices"];
            //var services = ConfigurationManager.AppSettings["Services"];//从配置文件中加载
            //var assembly = Assembly.Load(services);//加载类库中所有类
            ///* builder.RegisterAssemblyTypes(assembly).Where(p=>baseType.IsAssignableFrom(p)).AsImplementedInterfaces();*///service必须以Service结尾，必须继承自ISuperService &&p.Name.EndsWith("Service")
            //builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
            //var instance = new MyContext().GetInstance();

            //builder.RegisterType<BaseClient>().As<IBaseClient>().InstancePerLifetimeScope();
            ////builder.RegisterInstance<ISqlSugarClient>(instance).SingleInstance();

            //builder.Register(c => new MyContext().GetInstance()).As<ISqlSugarClient>().InstancePerLifetimeScope();

            //builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

            ////automapper
            ////module 注入   自动注入，将AutofacWebApi模块中所有的类都configuration，但是还是需要注册有对应的映射关系
            //builder.RegisterModule(new AutofacModule(Assembly.Load("AutofacWebApi")));

            //// builder.Register(r => new AutoMapperRegist().Register()).As<IMapper>().SingleInstance();
            ////获取http配置
            //var configuration = GlobalConfiguration.Configuration;
            ////注册api控制器
            //builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            ////注册Autofac筛选器提供程序。
            ////builder.Register(r => new AutofacActionFilter()).AsWebApiActionFilterForAllControllers();//将过滤器放到容器中
            //builder.RegisterWebApiFilterProvider(configuration);
            ////注册Autofac模型绑定器提供程序。
            //builder.RegisterWebApiModelBinderProvider();
            ////将依赖关系解析程序设置为Autofac。
            //var container = builder.Build();
            //configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            #endregion
        }
    }
}