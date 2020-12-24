using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private IConfiguration Configuration { get; set; }
        private IServiceProvider ServiceProvider { get; set; }
        private IHost host { get; set; }
        public App()
        {
            host = Host.CreateDefaultBuilder()
                 .ConfigureHostConfiguration(builder =>
                 { })
                 .ConfigureServices((context, services) =>
                 {
                     this.Configuration = context.Configuration;
                     this.ConfigureServices(context, services);
                 }).ConfigureAppConfiguration((context, configuration) =>
                 {
                     configuration.AddJsonFile("appsetting.json", false, true)
                     .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                 })
                 .Build();
            this.ServiceProvider = host.Services;
            host.Start();
        }
        async protected override void OnStartup(StartupEventArgs e)
        {
            using (var scope = this.ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EFDbContext>();
                var mainWindow = this.ServiceProvider.GetRequiredService<MainWindow>();
                try
                {
                    if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
                    {
                        await dbContext.Database.MigrateAsync();
                        await dbContext.Database.ExecuteSqlRawAsync(@"
                             CREATE TRIGGER SetRowVersionTimestamp
                             AFTER UPDATE ON Products
                                BEGIN
                                    UPDATE Products SET RowVersion = randomblob(8) WHERE rowid = NEW.rowid;
                                END");
                        mainWindow.Show();
                    }
                    else
                    {
                        mainWindow.Show();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.AddConfiguration(this.Configuration.GetSection("Logging"))
                .AddConsole().AddDebug();
#if DEBUG
                configure.AddDebug();
#endif

            });

            services.AddDbContext<EFDbContext>(opt =>
            {
                var connectionString = this.Configuration.GetConnectionString(nameof(EFDbContext));
                opt.UseSqlite(connectionString);
            }, ServiceLifetime.Transient);

            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<MySettings>(this.Configuration.GetSection("MySettings"));
            services.AddTransient<MainWindow>();
            services.AddTransient<SecondWindow>();
        }
        protected async override void OnExit(ExitEventArgs e)
        {
            using (host) await host.StopAsync();

            base.OnExit(e);
        }
    }
}
