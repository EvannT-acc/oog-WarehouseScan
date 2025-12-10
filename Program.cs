using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Oog.DataAccess;
using Oog.Domain.Interfaces;
using Oog.Infrastructure.InversionOfControl;
using Oog.Tools.Configuration;
using Oog.Web.WebHelper;
using Oog.WebService.Core.WebService;
using System.Reflection;

namespace Oog.WarehouseScan
{
    internal static class Program
    {
        public static IEnumerable<AutoRegisterTarget> AutoRegistrationTargetsAllowed
        {
            get
            {
                yield return AutoRegisterTarget.All;
                yield return AutoRegisterTarget.SearchWebClass;
                yield return AutoRegisterTarget.CommandWebClass;
                yield return AutoRegisterTarget.SearchErpClass;
            }
        }

        public static IEnumerable<Type> ExceptClasses
        {
            get
            {
                yield return typeof(OoGardenErpDbContext);
            }
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            var assemblyFolder = Path.GetDirectoryName(AppContext.BaseDirectory);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
#if DEBUG
            var configFileName = "log4net.Debug.config";
#else
            var configFileName = "log4net.release.config";
#endif
            XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(assemblyFolder, configFileName)));

            var host = CreateHost(args);

            using var serviceScope = host.Services.CreateScope();

            var services = serviceScope.ServiceProvider;
            OogConfig.ServiceProvider = services;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        public static IHost CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());

                    if (hostingContext.HostingEnvironment.EnvironmentName == Microsoft.Extensions.Hosting.Environments.Production)
                    {
                        config.AddJsonFile("appsettings.Production.json", false, true);
                    }
                    else
                    {
                        config.AddJsonFile("appsettings.development.json", false, true);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Ajout du contexte pour EF Core !
                    services.AddDbContext<OoGardenWebDbContext>(options =>
                    {
                        var connectionString = hostContext.Configuration.GetConnectionString(nameof(OoGardenWebDbContext));
                        options.UseSqlServer(connectionString, (o) =>
                        {
                            o.CommandTimeout(300);
                            o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                        });
#if DEBUG
                        options.EnableSensitiveDataLogging();
#endif
                    }, ServiceLifetime.Transient);

                    services.AddDbContext<OoGardenErpDbContext>(options =>
                    {
                        var connectionString = hostContext.Configuration.GetConnectionString(nameof(OoGardenErpDbContext));
                        options.UseSqlServer(connectionString, (o) =>
                        {
                            o.CommandTimeout(300);
                            o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                        });
#if DEBUG
                        options.EnableSensitiveDataLogging();
#endif
                    }, ServiceLifetime.Transient);

                    #region Auto Registration

                    services.AddScoped<IWebHelper, WebHelper>();

                    services.Scan(scan =>
                    {
                        // créer une liste des binaires en commencant par le projet en cours
                        List<Assembly> assemblies = new()
                        {
                            Assembly.GetEntryAssembly()
                        };
                        // Puis en ajoutant les dll référencées en les chargeant
                        assemblies.AddRange(Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(o => o.FullName.StartsWith("Oog.")).Select(aname => Assembly.Load(aname)));

                        List<Type> types = new();
                        assemblies.ForEach((assembly) => types.AddRange(assembly.GetTypes()));

                        var implementedAssemblies = scan.FromAssemblies(assemblies).AutoRegister(types, AutoRegistrationTargetsAllowed, ExceptClasses);
                    });

                    // https://stackoverflow.com/questions/52007836/dbproviderfactories-getfactoryclasses-returns-no-results-after-installing-net-s
                    System.Data.Common.DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

                    SageX3WebService.SageX3CalledFromNetCore = true;

                    #endregion
                })
                .Build();
        }
    }
}