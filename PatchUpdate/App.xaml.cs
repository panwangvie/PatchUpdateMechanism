using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UpdateFile;
using UpdateFile.Log;

namespace PatchUpdate
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, App.LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref App.KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(IntPtr path);

        private static int LowLevelKeyboardProc(int nCode, int wParam, ref App.KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0)
            {
                switch (wParam)
                {
                    case 256:
                    case 257:
                    case 260:
                    case 261:
                        if ((lParam.vkCode == 9 && lParam.flags == 32) || (lParam.vkCode == 27 && lParam.flags == 32) || (lParam.vkCode == 115 && lParam.flags == 32))
                        {
                            return 1;
                        }
                        break;
                }
            }
            return App.CallNextHookEx(0, nCode, wParam, ref lParam);
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static Process RunningInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            foreach (Process process in Process.GetProcessesByName(currentProcess.ProcessName))
            {
                if (process.Id != currentProcess.Id && process.MainModule.FileName == currentProcess.MainModule.FileName)
                {
                    return process;
                }
            }
            return null;
        }

        public static void HandleRunningInstance()
        {
            Process process = App.RunningInstance();
            if (process != null)
            {
                App.ShowWindowAsync(process.MainWindowHandle, 3);
                App.SetForegroundWindow(process.MainWindowHandle);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string name = string.Format(CultureInfo.InvariantCulture, "Local\\{{{0}}}{{{1}}}", new object[]
            {
                executingAssembly.GetType().GUID,
                executingAssembly.GetName().Name
            });
            bool flag;
            this._mutex = new Mutex(true, name, out flag);
            if (!flag)
            {
                App.HandleRunningInstance();
                Environment.Exit(0);
                return;
            }
            base.OnStartup(e);
             
            new MainUpdate(e).ShowDialog();
        }

        private void LoadResources()
        {
            string text = ConfigurationManager.AppSettings["Language"];
            if (string.IsNullOrEmpty(text))
            {
                text = "zh-CHS";
            }
            //ExpendMethod.CurrentLanguage = ((text == "zh-CHS") ? LanguageType.zh_CHS : LanguageType.en_US);
            Application.Current.Properties["CurrentLanguageType"] = text;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(text);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(text);
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            string uriString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Language", "SEngine_" + Application.Current.Properties["CurrentLanguageType"] + ".xaml");
            resourceDictionary.Source = new Uri(uriString);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            App.UnhookWindowsHookEx(this._hHook);
            base.OnExit(e);
        }

        public App()
        {
           // XmlConfigurator.Configure();
            Application.Current.DispatcherUnhandledException += this.Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
            IntPtr moduleHandle = App.GetModuleHandle(IntPtr.Zero);
            this._hookProc = new App.LowLevelKeyboardProcDelegate(App.LowLevelKeyboardProc);
            this._hHook = App.SetWindowsHookEx(13, this._hookProc, moduleHandle, 0);
            _ = this._hHook == IntPtr.Zero;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            Logs.WriteLog($"意外的操作:Message:{e.Exception.Message},StackTrace:{e.Exception.StackTrace} ");
            MessageBox.Show(e.Exception.Message, "意外的操作", MessageBoxButton.OK);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Logs.WriteLog($"意外的操作:{e.ExceptionObject.ToString()} ");
                base.Dispatcher.Invoke(delegate ()
                {
                    MessageBox.Show(e.ExceptionObject.ToString(), "意外的异常", MessageBoxButton.OK);
                });
            }
            catch(Exception ex)
            {
                Logs.WriteLog($" Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);

            }
        }




        private const int WH_KEYBOARD_LL = 13;

        private IntPtr _hHook;

        private App.LowLevelKeyboardProcDelegate _hookProc;

        private const int SW_MAXIMIZE = 3;

        private Mutex _mutex;

        private bool _contentLoaded;

        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;

            private int scanCode;

            public int flags;

            private int time;

            private int dwExtraInfo;
        }

        private delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref App.KBDLLHOOKSTRUCT lParam);
    }
}
