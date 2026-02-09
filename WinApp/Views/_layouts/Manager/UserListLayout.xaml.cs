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
    class StationSelectList : StationItemList
    {
        UserManagerViewModel vm => (UserManagerViewModel)DataContext;
        public StationSelectList()
        {
            ItemClick += (e) => {
                vm.DeviceToUser(e, s => this.Async(() => {
                    Children.Remove(this.Find(e));
                }));
            };
        }
        protected override FrameworkElement CreateItem()
        {
            return new SelectStationItem();
        }
        public void BeginSelect()
        {
            this.ItemsSource = vm.FutureStations;
        }

        public void EndSelect()
        {
            this.ItemsSource = vm.Stations;
        }
    }

    /// <summary>
    /// Interaction logic for UserListLayout.xaml
    /// </summary>
    public partial class UserListLayout : UserControl
    {
        UserManagerViewModel vm = null;
        FrameworkElement deleteMessage;
        TemplateTableView table;

        StationSelectList DeviceSelector;


        void OnPersonsChanged()
        {
            Dispatcher.InvokeAsync(() => table.ItemsSource = vm.Persons);
        }
        void FlyIn()
        {
            flyingColumn.FlyIn(null);
        }
        void Refresh()
        {
            vm = (UserManagerViewModel)DataContext;
            table = new TemplateTableView { 
                DataContext = vm.Table
            };
            table.RegisterClickEvent(() => {
                if (table.SelectedItem != null)
                {
                    flyingColumn.Show();
                    RightPanel.DataContext = (Document)table.SelectedItem;

                    DeviceSelector.EndSelect();
                }
                else
                {
                    FlyIn();
                }
            });

            LeftPanel.Child = table;
            DeviceSelector.DataContext = vm;
        }

        public UserListLayout()
        {
            InitializeComponent();

            BtnClose.Click += (_, __) => FlyIn();

            btnSelectDevice.Click += (_, __) => {
                DeviceSelector.BeginSelect();
            };

            SelectorContent.Child = DeviceSelector = new StationSelectList();

            deleteMessage = (FrameworkElement)LeftPanel.Child;
            LeftPanel.Child = null;

            BtnDelete.Click += (_, __) => {

                flyingColumn.Show(false);
                deleteMessage.DataContext = vm.Current;
                
                Dialog dlg = new Dialog(deleteMessage, "Đồng ý", "Thôi");
                dlg.Accept += () => Dialog.Pop();
                
                if (dlg.ShowDialog() == true)
                {
                    vm.DeleteUser(OnPersonsChanged);
                }
            };

            BtnAddUser.Click += (_, __) => {
                Action<Document> submit = doc => {

                    vm.AddUser(doc, OnPersonsChanged);

                    Dialog.Pop();
                };

                var dlg = Dialog.BeginEdit(new EditContext("add-user"), submit);
                dlg.ShowDialog();
            };

            this.DataModelChangedTo<UserManagerViewModel>(vm => {
                Refresh();
            });
        }
    }
}
