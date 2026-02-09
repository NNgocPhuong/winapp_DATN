using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Vst.Controls;

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for StationListLayout.xaml
    /// </summary>
    public partial class StationListLayout : UserControl
    {
        StationManagerViewModel vm = null;
        TemplateTableView table;

        class PhoneListView : MyListView
        {
            protected override FrameworkElement CreateItem()
            {
                return new PhoneItemView();
            }
        }
        EditorPanel infoEditor;

        void FlyIn()
        {
            flyingColumn.FlyIn(() => {
                infoEditor.Show(false);
                PhoneListContent.Children.Clear();
            });
        }

        void OnPhoneNumbersChanged(string action, Document phone)
        {
            Dispatcher.InvokeAsync(() => {
                PhoneListContent.Children.Clear();
                foreach (var e in vm.Current.GetPhoneNumbers())
                {
                    var item = new PhoneItemView { DataContext = e };
                    PhoneListContent.Children.Add(item);
                }
            });
        }

        void Refresh()
        {
            vm = (StationManagerViewModel)DataContext;
            table = new TemplateTableView {
                DataContext = vm.Table
            };
            table.RegisterClickEvent(() => {
                if (table.SelectedItem != null)
                {
                    RightPanel.DataContext = vm.Current;

                    infoEditor = new EditorPanel { 
                        DataContext = new EditContext("device") { Value = vm.Current }
                    };
                    FormContent.Child = infoEditor;
                    OnPhoneNumbersChanged("load", null);

                    flyingColumn.Show();
                }
                else
                {
                    FlyIn();
                }
            });
            LeftPanel.Child = table;
        }

        public StationListLayout()
        {
            InitializeComponent();
            BtnClose.Click += (_, __) => FlyIn();

            BtnUpdateInfo.Click += (_, __) => {
                if (!infoEditor.ErrorFound(err => { }))
                {
                    //var vm = new UpdateStation();
                    //vm.Update(infoEditor.GetEditedDocument());
                }
            };
            BtnAddPhone.Click += (_, __) => {
                
                flyingColumn.Show(false);
                vm.BeginAddNew();
            };

            this.DataModelChangedTo<StationManagerViewModel>(vm => Refresh());
        }
    }
}
