using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class WindowManager
    {
        private static Hashtable RegisterWindow = new Hashtable();

        public static void Register<T>(WindowType windowKey)
        {
            if (RegisterWindow.Contains(windowKey)) return;
            RegisterWindow.Add(windowKey, typeof(T));
        }

        public static void Show(WindowType windowType, object viewModel)
        {
            if (!RegisterWindow.Contains(windowType)) return;
            if (Activator.CreateInstance((Type)(RegisterWindow[windowType])) is Window window)
            {
                window.DataContext = viewModel;
                window.Show();
            }
        }

        public static void Close(WindowType windowType)
        {
            if (!RegisterWindow.ContainsKey(windowType)) return;
            var win = (Window)RegisterWindow[windowType];
            win?.Close();
            RegisterWindow.Remove(windowType);
        }
    }

    public enum WindowType
    {
        LoginType,

        MainType,

    }
}
