using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vst.Controls;

namespace WinApp.Views
{
    interface IAppView : System.Mvc.IView
    {
        bool IsLoading { get; }
    }

    class Loading : IAppView
    {
        public bool IsLoading => true;

        public object Content => null;

        public virtual void Render(object model)
        {
        }
    }
    class BaseView<TModel, TView> : IAppView
        where TModel : BaseViewModel, new()
        where TView : FrameworkElement, new()
    {
        public object Content => MainView;
        protected virtual BaseViewModel CreateViewModel() => new TModel();

        protected virtual void Waiting()
        {
        }
        public BaseView()
        {
            ViewModel = (TModel)CreateViewModel();
            MainView = new TView();

            MainView.IsVisibleChanged += (s, e) => { 
                if (e.NewValue.Equals(false))
                {
                    ViewModel.Dispose();
                }
            };
        }
        public virtual void Render(object model)
        {
            RenderCore();
            if (ViewModel is AsyncViewModel vm)
            {
                if (vm.IsReady)
                {
                    MainView.DataContext = vm.GetDataContext();
                    return;
                }

                IsLoading = true;
                vm.Execute();

                var timer = new Vst.Timer();
                timer.OnTick += () => {

                    if (vm.IsReady)
                    {
                        timer.Stop();

                        IsLoading = false;

                        MainView.Dispatcher.InvokeAsync(() => {
                            MainView.DataContext = vm.GetDataContext();
                        });
                    }
                };
                timer.Start();
            }
            else
            {
                MainView.DataContext = ViewModel;
            }
        }

        public bool IsLoading { get; protected set; }
        protected TModel ViewModel { get; set; }
        protected TView MainView { get; set; }
        protected Document Document => (Document)ViewModel.Value;
        protected DocumentList DocumentList => ViewModel.ValueList;
        protected virtual void RenderCore()
        {
        }
    }

    class BaseView<TView> : BaseView<BaseViewModel, TView>
        where TView: FrameworkElement, new()
    {
    }
}
