using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// SecondWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SecondWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _cache;

        public SecondWindow(IServiceProvider serviceProvider, IMemoryCache cache)
        {
            InitializeComponent();
            this._serviceProvider = serviceProvider;
            this._cache = cache;
            this.Loaded += SecondWindow_Loaded;
        }

        private void SecondWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._cache.TryGetValue(nameof(MySettings), out MySettings mySettings))
            {
                
            }
        }
    }
}
