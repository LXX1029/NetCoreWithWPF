
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static object _obj = new object();
        private readonly IOptions<MySettings> _optionSetting;
        private readonly ILogger<MainWindow> _logger;
        private readonly EFDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _cache;
        private HttpClient _httpClient = new HttpClient();
        public MainWindow(IOptions<MySettings> optionSetting, ILogger<MainWindow> logger, EFDbContext context, IServiceProvider serviceProvider, IMemoryCache cache)
        {
            InitializeComponent();
            this._optionSetting = optionSetting;
            this._logger = logger;
            this._context = context;
            this._serviceProvider = serviceProvider;
            this._cache = cache;
            this.Loaded += MainWindow_Loaded;

        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this._logger.LogInformation("程序已启动");
            if (!this._cache.TryGetValue(nameof(MySettings), out MySettings _))
            {
                this._cache.Set<MySettings>("MySettings", new MySettings { token = "123456" });
            }

            this.dgCategory.ItemsSource = await this._context.Categorys.Include(p => p.Products).ToListAsync();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var options = this._optionSetting;
            var product = this._context.Products.Find(2);

            IDbContextTransaction transaction = null;
            try
            {
                using (var context = _serviceProvider.CreateScope().ServiceProvider.GetService<EFDbContext>())
                {
                    using (transaction = context.Database.BeginTransaction())
                    {
                        var product1 = context.Products.Find(2);
                        var price1 = new Random().Next(10);
                        this._logger.LogInformation($"price1:{price1.ToString()}");
                        product1.Price =  price1;
                        int result1 = context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateConcurrencyException)
                {
                    transaction.Rollback();
                    this._logger.LogDebug(ex.Message);
                }
            }
            var savedData = false;
            while (!savedData)
            {
                try
                {
                    var price2 = new Random().Next(10);
                    product.Price =  price2;
                    this._logger.LogInformation($"price2:{price2}");
                    int result = this._context.SaveChanges();
                    savedData = true;
                }
                catch (Exception ex)
                {
                    if (ex is DbUpdateConcurrencyException dbUpdate)
                    {
                        //dbUpdate.Entries.Single().Reload();
                        ////dbUpdate.Entries.Single().State = EntityState.Modified;
                        //this._context.SaveChanges();
                        //savedData = true;
                        //this._logger.LogDebug(ex.Message);

                        foreach (var item in dbUpdate.Entries)
                        {
                            if (item.Entity is Product entry)
                            {
                                var currentValues = item.CurrentValues;
                                var dbValues = item.GetDatabaseValues();
                                foreach (var prop in currentValues.Properties)
                                {
                                    var currentValue = currentValues[prop];
                                    var originalValue = item.OriginalValues[prop];
                                    var dbValue = dbValues[prop];
                                }
                                item.OriginalValues.SetValues(dbValues);
                            }
                        }
                    }
                }
            }


            //var secondWin = this._serviceProvider.GetService<SecondWindow>();
            //secondWin.Show();
            //Task.Run(() =>
            //{
            //    using (var context = _serviceProvider.CreateScope().ServiceProvider.GetService<EFDbContext>())
            //    {
            //        System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString()}---开始写入数据");
            //        for (int i = 0; i < 10; i++)
            //        {
            //            var category = new Category() { Name = $"肉类{i}" };
            //            context.Categorys.Add(category);
            //            int result = context.SaveChanges();
            //        }
            //        System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString()}---写入数据完毕");
            //    }
            //});

            // var httpClient = new HttpClient();

            //var param = JsonConvert.SerializeObject(new Login() { UserName = "admin", Password = "123456" });
            //var content = new StringContent(param);
            //content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            //httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");

            //var content = new StringContent(param, Encoding.UTF8, "application/json");
            //// content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            //var response = await httpClient.PostAsync("http://10.0.50.24:8085/api/Login/Login", content).ConfigureAwait(false);
            //var result = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(result);
        }
    }
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
