using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp.Views;

namespace WinApp
{
    internal class StationManagerViewModel : BaseViewModel
    {
        public Station Current => (Station)Table.SelectedItem;

        ListContext _tableContext = new ListContext("device") {
            Value = Global.Stations.ToList()
        };
        public ListContext Table => _tableContext;

        internal void BeginAddNew()
        {
            var current = Current;

            Action<Document> submit = (doc) => {

                var vm = new PhoneNumberService();
                vm.AddNumber(current, doc, () => { 
                    
                });
                Dialog.Pop(true);
            };

            var dlg = Dialog.BeginEdit(new EditContext("add-phone"), submit);
            dlg.ShowDialog();
        }
    }
}
