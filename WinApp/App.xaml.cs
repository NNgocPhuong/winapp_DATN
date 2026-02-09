using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Mvc;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinApp.ViewModels;
using WinApp.Views;

namespace Vst.Controls
{
    public static class VstControlExtention
    {
        static public void Request(this FrameworkElement element, string url, params object[] args)
        {
            WinApp.App.Request(url, args);
        }
    }
}

namespace WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public void Request(string url, params object[] args)
        {
            if (url[0] == '/')
            {
                url = Engine.RequestContext.ControllerName + url;
            }
            Engine.Execute(url, args);
        }
        static public void DispatcherRequest(string url, params object[] args)
        {
            Browser.Dispatcher.InvokeAsync(() => Request(url, args));
        }

        static public void ShowError(string content)
        {
            MessageBox.Show("     " + content + "     ", "Error");
        }
        static public void DispatcherAsync(Action callback)
        {
            App.Current.MainWindow.Dispatcher.InvokeAsync(callback);
        }
        static public Vst.Timer SysClock { get; private set; } = new Vst.Timer();

        static public void OpenLoginDialog()
        {
            #region LOGIN
            Dialog loginDialog = null;

            var service = new LoginService();
            service.ResponseSuccess += (res) => {
                Global.User = new AppUserModel(service.Engine.ID, res.ValueContext);
                
                loginDialog.Dispatcher.InvokeAsync(() => {
                    Browser.DataContext = Global.User;
                    Browser.Start();
                    Dialog.Pop(true);
                });
            };

            var model = new LoginModel();
            service.Engine.Start(model.Token);

            var context = new EditContext("login", "+", model);
            loginDialog = Dialog.BeginEdit(context, service.Execute);
            if (loginDialog.ShowDialog() == false)
            {
                App.Current.Shutdown();
            }
            #endregion
        }
        static internal MainWindow Browser { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DB.Start(Environment.CurrentDirectory + "/App_Data");
            Vst.Controls.SvgIcon.Register(DB.Main.DataPath("Svg"));

            Engine.Register(this, result => {

                var view = result.View;
                if (view != null)
                {
                    Dispatcher.Invoke(() => {

                        if (Current.MainWindow == null) return;

                        view.Render(result.Model);

                        //if (view is IAppView v)
                        //{
                        //    Browser.BeginWaiting();
                        //    while (v.IsLoading)
                        //    {
                        //        System.Threading.Thread.Sleep(100);
                        //    }
                        //}

                        Browser.UpdateView(view.Content);
                    });
                }
            });

            Browser = new MainWindow();
            SysClock.Start();

            OpenLoginDialog();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            DB.End();
            SysClock.Stop();

            base.OnExit(e);
        }
    }
}
