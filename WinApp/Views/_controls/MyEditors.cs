using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SWC = System.Windows.Controls;

namespace Vst.Controls
{
    public class MyDialog : Window
    {
        public MyDialog()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }

}
namespace Vst.Controls
{
    public class EditorInfo : Document
    {
        public int Layout { get => GetValue<int>("layout"); set => Push("layout", value); }
        public string Type { get => GetString("type") ?? "text"; set => Push("type", value); }
        public string Placeholder { get => GetString("placeholder"); set => Push("placeholder", value); }
        public bool Required { 
            get => GetValue("required", true) && (Type != "check"); 
            set => Push("required", value); 
        }
        public object Options
        {
            get => GetValue<object>("options");
            set => Push("options", value);
        }

        public string DisplayName { get => GetString("displayName"); set => Push("displayName", value); }
        public string ValueName { get => GetString("valueName"); set => Push("valueName", value); }
        public MyEditor Control { get; set; }
    }
    public class EditorPanel : InlineBlockView
    {
        public EditorPanel()
        {
            this.DataModelChangedTo<EditContext>(context => {
                
                ItemsSource = context.Editors;

                var model = context.ValueContext;
                if (model != null)
                {
                    ForEach((f, c) => {
                        var k = f.EditorInfo.Name;
                        if (model.TryGetValue(k, out object v))
                        {
                            c.Value = v;
                        }
                    });
                }
                else
                {
                    context.Value = model = new Document();
                }

            });
        }

        public void ForEach(Action<FormControl, MyEditor> callback)
        {
            foreach (FormControl c in Children)
            {
                callback(c, c.EditorInfo.Control);
            }
        }
        public FormControl Find(string name)
        {
            foreach (FormControl c in Children)
            {
                if (c.EditorInfo.Name == name)
                    return c;
            }
            return null;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            ForEach((c, e) => {
                var w = c.EditorInfo.Layout;
                if (w == 0) w = 12;

                c.Width = constraint.Width / 12 * w;
            });
            return base.MeasureOverride(constraint);
        }

        protected override FrameworkElement CreateItem()
        {
            return new FormControl();
        }

        public MyEditor Add(EditorInfo context)
        {
            Children.Add(new FormControl { EditorInfo = context });
            return context.Control;
        }

        public bool ErrorFound(Action<List<EditorInfo>> callback)
        {
            var error = new List<EditorInfo>();
            ForEach((f, c) => {
                if (f.EditorInfo.Required == true && string.IsNullOrWhiteSpace(c.Text))
                {
                    error.Add(f.EditorInfo);
                }
            });
            if (error.Count > 0)
            {
                callback?.Invoke(error);
                return true;
            }
            return false;
        }
        public Document GetEditedDocument()
        {
            var model = new Document();
            ForEach((f, c) => {
                var k = f.EditorInfo.Name;
                var v = c.Value;
                model.Push(k, v);
            });
            return model;
        }
    }

    public class FormControlContent : StackPanel
    {

    }
    public class FormControl : MyPanel<FormControlContent>
    {
        static Dictionary<string, Type> temp = new Dictionary<string, Type> {
            {"text", typeof(TextBox) },
            {"select", typeof(SelectBox) },
            {"check", typeof(CheckBox) },
            {"number", typeof(NumberBox) },
            {"password", typeof(PasswordBox) },
        };

        public FormControl()
        {
            this.DataModelChangedTo<EditorInfo>(_info => {

                _info.Type = _info.Type.ToLower();

                Children.Clear();

                var caption = new EditorLabel();
                caption.Add(_info.Caption);
                if (_info.Required == true)
                    caption.Add(" *", Brushes.Red);

                var editor = _info.Control;
                if (editor == null)
                {
                    editor = (MyEditor)Activator.CreateInstance(temp[_info.Type]);
                    _info.Control = editor;
                }
                editor.SetEditorInfo(_info);

                caption.Click += (s, e) => {
                    editor.SelectAll();
                    editor.GetInput().Focus();
                };

                Children.Add(caption);
                Children.Add(editor);
            });
        }

        public EditorInfo EditorInfo
        {
            get => DataContext as EditorInfo;
            set => DataContext = value;
        }
    }
    public class EditorLabel : TextBase
    {
        public EditorLabel()
        {
            Padding = new Thickness(0, 0, 0, 3);
            Cursor = Cursors.Hand;
        }
    }
    public class EditorFrame : MyElement
    {
        public void SetContent(UIElement element)
        {
            element.SetValue(BorderThicknessProperty, default(Thickness));
            element.SetValue(BackgroundProperty, Brushes.Transparent);
            element.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            Action<bool> raise_activate = b => {
                Activated = b;
                element.SetValue(ForegroundProperty, Foreground);
            };

            element.GotFocus += (s, e) => raise_activate(true);
            element.LostFocus += (s, e) => {
                raise_activate(false);
            };

            PreviewMouseDown += (s, e) => element.Focus();

            raise_activate(false);
        }

        public EditorFrame()
        {
            BorderThickness = new Thickness(1);
        }
    }
    public class Placeholder : SWC.TextBlock
    {
        public Placeholder()
        {
            VerticalAlignment = VerticalAlignment.Center;
        }

        new public bool IsVisible
        {
            get => base.IsVisible;
            set => base.Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
    }
    public abstract class MyEditor : GridLayout
    {
        #region dependency
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.RegisterAttached(nameof(Caption),
            typeof(string),
            typeof(MyEditor),
            new PropertyMetadata(default(string)));
        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(nameof(Placeholder),
            typeof(string),
            typeof(MyEditor),
            new PropertyMetadata(default(string)));
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set
            {
                SetValue(PlaceholderProperty, value);
                if (placeholder != null)
                {
                    placeholder.Text = value;
                }
            }
        }

        #endregion

        protected EditorFrame frame;
        protected Placeholder placeholder;

        public EditorFrame Frame => frame;
        public bool IsEmpty
        {
            get
            {
                var v = GetEditValue();
                return (v == null || v.Equals(string.Empty));
            }
        }
        public object Value
        {
            get => GetEditValue(); set => SetEditValue(value);
        }
        public string Text
        {
            get => GetEditValue()?.ToString() ?? string.Empty;
            set => SetEditValue(value);
        }
        protected virtual object GetEditValue() => null;
        protected virtual void SetEditValue(object v) { }
        
        
        protected virtual void CreateFrame()
        {
            placeholder = new Placeholder();
            frame = new EditorFrame { 
                Child = placeholder
            };
            Children.Add(frame);
        }
        public MyEditor()
        {
            CreateFrame();
        }
        public abstract UIElement GetInput();
        public virtual void SelectAll() { }
        public virtual void SetEditorInfo(EditorInfo i)
        {
            Name = i.Name;
            Placeholder = i.Placeholder;

            SetEditValue(i.Value);
            SetOptions(i.Options);
        }
        public virtual void SetOptions(object values) { }

        #region EVENTS
        public event EventHandler ValueChanged;
        protected virtual void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
    public abstract class MyEditor<TInput> : MyEditor
        where TInput : SWC.Control, new()
    {
        public override UIElement GetInput() => input;
        protected TInput input { get; private set; } = new TInput();
        public double FontSize
        {
            get => (double)input.GetValue(SWC.Control.FontSizeProperty);
            set => input.SetValue(SWC.Control.FontSizeProperty, value);
        }

        public MyEditor()
        {
            frame.SetContent(input);
            Children.Add(input);
            input.LostFocus += (s, e) => {
                object v = GetEditValue();
                Func<bool> empty = () => v == null || v.Equals(string.Empty);
                placeholder.IsVisible = empty();
            };

            input.GotFocus += (s, e) => placeholder.IsVisible = false;

            FontSize = 16;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            input.SetValue(MarginProperty, frame.Padding);
            placeholder.IsVisible = IsEmpty;
            return base.MeasureOverride(constraint);
        }
    }
    public class TextBox : MyEditor<SWC.TextBox>
    {
        protected virtual bool IsKeyInvalid(Key key) => false;
        protected override Size MeasureOverride(Size constraint)
        {
            placeholder.FontSize = input.FontSize;
            return base.MeasureOverride(constraint);
        }
        public TextBox()
        {
            input.PreviewKeyDown += (s, e) => {
                switch (e.Key)
                {
                    case Key.Enter:
                        return;

                    case Key.Tab:
                        return;

                    case Key.Delete:
                        return;

                    case Key.Back:
                        return;
                }
                e.Handled = IsKeyInvalid(e.Key);
            };
            input.TextChanged += (s, e) => RaiseValueChanged();
        }
        protected override object GetEditValue() => input.Text;
        protected override void SetEditValue(object v) => input.Text = $"{v}";
    }
    public class NumberBox : TextBox
    {
        protected override bool IsKeyInvalid(Key key)
        {
            switch (key)
            {
                case Key.OemPeriod: 
                    return input.Text.IndexOf('.') >= 0;

                case Key.OemMinus:
                    return input.Text != string.Empty;
            }    
            return !((key >= Key.D0 && key <= Key.D9) 
                || (key >= Key.NumPad0 && key <= Key.NumPad9));
        }
        public override void SelectAll() => input.SelectAll();
    }
    public class PasswordBox : MyEditor<SWC.PasswordBox>
    {
        public PasswordBox()
        {
        }    
        protected override object GetEditValue() => input.Password;
        protected override void SetEditValue(object v) => input.Password = $"{v}";
        public override void SelectAll() => input.SelectAll();
    }
    public class SelectBox : MyEditor<SWC.ComboBox>
    {
        public SelectBox()
        {
            input.IsEditable = true;
            input.Padding = new Thickness(0);
        }
        protected override object GetEditValue()
        {
            if (string.IsNullOrEmpty(ValueName))
                return input.Text;

            var v = input.SelectedValue;
            if (v != null && !v.Equals(string.Empty))
            {
                var p = v.GetType().GetProperty(ValueName);
                if (p != null)
                {
                    return p.GetValue(v);
                }
            }

            return v;
        }
        protected override void SetEditValue(object v)
        {
            if (string.IsNullOrEmpty(ValueName) || input.ItemsSource == null)
            {
                input.Text = v?.ToString();
                return;
            }

            PropertyInfo p = null;
            foreach (var e in input.ItemsSource)
            {
                if (p == null)
                {
                    p = e.GetType().GetProperty(ValueName);
                    if (p == null)
                        return;
                }

                if (p.GetValue(e).Equals(v))
                {
                    input.SelectedValue = e;
                    return;
                }
            }
        }

        public bool Editable
        {
            get => input.IsEditable;
            set => input.IsEditable = value;
        }
        public string DisplayName
        {
            get => input.DisplayMemberPath;
            set => input.DisplayMemberPath = value;
        }
        public string ValueName { get; set; }
        public override void SetEditorInfo(EditorInfo i)
        {
            base.SetEditorInfo(i);
            DisplayName = i.DisplayName;
            ValueName = i.ValueName;
        }
        public override void SetOptions(object value)
        {
            if (value is string)
            {
                input.ItemsSource = ((string)value).Split(';');
                return;
            }
            if (value is IDictionary)
            {

                return;
            }
            input.ItemsSource = (IEnumerable)value;
        }
    }
    class CheckBoxIndicator : EditorFrame
    {
        public CheckBoxIndicator()
        {
            Width = Height = 16;
            HorizontalAlignment = HorizontalAlignment.Left;

            BorderBrush = Brushes.LightGray;
            BorderThickness = new Thickness(1);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            CornerRadius = new CornerRadius(Height = Width);
            return base.MeasureOverride(constraint);
        }
    }

    public class CheckBox : MyEditor
    {
        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register(
                nameof(Checked),                      // Tên property
                typeof(bool),                         // Kiểu dữ liệu
                typeof(CheckBox),               // Chủ sở hữu
                new PropertyMetadata(false, OnCheckedChanged)); // Giá trị mặc định + callback

        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set => SetValue(CheckedProperty, value);
        }

        private static void OnCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CheckBox)d;
            var inp = control.frame;
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;

            // Xử lý khi Checked thay đổi
            if (newValue)
            {
                // Ví dụ: đổi màu nền khi được check
                inp.BorderThickness = new Thickness(inp.Width / 3);
                inp.BorderBrush = control.ActiveColor;
            }
            else
            {
                inp.BorderThickness = new Thickness(1);
                inp.BorderBrush = Brushes.LightGray;
            }
        }

        public static readonly DependencyProperty ActiveColorProperty =
             DependencyProperty.Register(
                 nameof(ActiveColor),                   // tên property
                 typeof(Brush),                         // kiểu dữ liệu
                 typeof(CheckBox),                     // chủ sở hữu
                 new PropertyMetadata(Brushes.Blue));

        public Brush ActiveColor
        {
            get => (Brush)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        public override UIElement GetInput() => frame;
        public CheckBox()
        {
        }

        protected override void CreateFrame()
        {
            Children.Add(frame = new CheckBoxIndicator { });
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null && (Parent is FormControlContent p))
            {
                var cap = p.Children[0];
                p.Children.Remove(cap);

                p.Children.Add(cap);
                p.Orientation = Orientation.Horizontal;

                p.RegisterClickEvent(() => { 
                    Checked ^= true;
                });
            }
        }

        protected override void SetEditValue(object v)
        {
            if ((v is int) || (v is long))
            {
                Checked = (long)v != 0;
                return;
            }
            Checked = v != null;
        }
        protected override object GetEditValue() => Checked ? 1 : 0;
    }
}
