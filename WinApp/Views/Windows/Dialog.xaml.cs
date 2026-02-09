using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vst.Controls;

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        static Stack<Dialog> _navigation = new Stack<Dialog>();
        static public void Pop(bool result = true)
        {
            if (_navigation.Count > 0)
            {
                var top = _navigation.Pop();
                top.DialogResult = result;
            }
        }
        public bool Show(UIElement content, string ok = "OK", string cancel = null)
        {
            Body.Child = content;

            ButtonAccept.IsVisible = !string.IsNullOrEmpty(ok);
            ButtonCancel.IsVisible = !string.IsNullOrEmpty(cancel);

            ButtonAccept.Text = ok;
            ButtonCancel.Text = cancel;

            return base.ShowDialog() == true;
        }

        public static Dialog BeginEdit(EditContext context, Action<Document> submit)
        {
            var editorPanel = new EditorPanel {
                DataContext = context,
                Margin = new Thickness(30),
            };
            var w = context.Width;
            if (w == 0) w = editorPanel.GetScreenWidth() / 3;
            editorPanel.Width = w;

            var dialog = new Dialog(editorPanel, "Submit", "Cancel");
            dialog.DataContext = context;

            dialog.Accept += () => {

                bool err = false;

                dialog.ClearComment();
                editorPanel.ErrorFound(lst => {

                    err = true;
                    dialog.ShowError("Cần điền đủ các trường bắt buộc");
                    
                    return;
                });

                if (!err && submit != null)
                {
                    dialog.Fade.Visibility = Visibility.Visible;
                    submit.Invoke(editorPanel.GetEditedDocument());
                }
            };

            return dialog;
        }

        protected void ClearComment() 
        { 
            Comment.StopBlink(); 
        }
        protected void ShowError(string text)
        {
            Comment.Text = text;
            Comment.Foreground = Brushes.Red;
            Comment.Blink(0.5, 1);
        }

        public event Action Accept;
        protected virtual void RaiseAccept()
        {
            Accept?.Invoke();
        }
        protected virtual void RaiseCancel() => Pop(false);


        new public bool? Show()
        {
            return this.ShowDialog();
        }
        new public bool? ShowDialog()
        {
            _navigation.Push(this);
            return base.ShowDialog();
        }

        public Dialog()
        {
            InitializeComponent();

            this.ButtonX.Click += (_, __) => RaiseCancel();
            this.ButtonAccept.Click += (_, __) => { 
                RaiseAccept();
            };
            this.ButtonCancel.Click += (_, __) => RaiseCancel();

            PreviewKeyUp += (s, e) => {
                if (e.Key == Key.Escape)
                {
                    RaiseCancel();
                    return;
                }
                if (e.Key == Key.Enter)
                {
                    RaiseAccept();
                }
            };

            Closing += (s, e) => {
                Body.Child = null;
            };
        }


        public Dialog(UIElement body, string ok = "OK", string cancel = null) : this()
        {
            Body.Child = body;

            ButtonAccept.IsVisible = !string.IsNullOrEmpty(ok);
            ButtonCancel.IsVisible = !string.IsNullOrEmpty(cancel);

            ButtonAccept.Text = ok;
            ButtonCancel.Text = cancel;
        }
    }
}
