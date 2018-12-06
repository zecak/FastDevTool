
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FastDevTool.Common
{
    [TemplatePart(Name = PART_ItemsControl, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_NextPageButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_PreviousPageButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_FirstPageButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_LastPageButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_GoPageButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_TextBlock, Type = typeof(TextBlock))]
    /// <summary>
    /// Pager.xaml 的交互逻辑
    /// </summary>
    public partial class Pager : Control
    {
        private const string PART_ItemsControl = "PART_ItemsControl";
        private const string PART_NextPageButton = "PART_NextPageButton";
        private const string PART_PreviousPageButton = "PART_PreviousPageButton";
        private const string PART_FirstPageButton = "PARY_FirstPageButton";
        private const string PART_LastPageButton = "PART_LastPageButton";
        private const string PART_GoPageButton = "PART_GoPageButton";
        private const string PART_TextBlock = "PART_TextBlock";
        private const string PART_TextBox = "PART_TextBox";

        private ItemsControl _itemsControl;
        private Button _nextPageButton;
        private Button _previousButton;
        private Button _firstPageButton;
        private Button _lastPageButton;
        private Button _goPageButton;
        private TextBox _textBox;
        private TextBlock _textBlock;

        private ObservableCollection<Button> _pageButtons;

        static Pager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pager), new FrameworkPropertyMetadata(typeof(Pager)));
        }

        public Pager() : base()
        {
        }

        public static readonly DependencyProperty TotalPagesProperty = DependencyProperty.Register(
            "TotalPages", typeof(int), typeof(Pager), new PropertyMetadata(1, OnTotalPagesPropertyChanged, (d, value) =>
            {
                try
                {
                    return (int)value < 1 ? 1 : value;
                }
                catch
                {
                    return 1;
                }

            }));

        private static void OnTotalPagesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pager)d).OnTotalPagesPropertyChanged(e);
        }

        private void OnTotalPagesPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            _pageButtons = new ObservableCollection<Button>();
            var totalPages = (int)e.NewValue;
            var buttonCount = Math.Min(totalPages, MaxDisplayPageCount);
            for (int i = 1; i <= buttonCount; i++)
            {
                var button = new Button
                {
                    Style = ButtonStyle,
                    Content = i,
                    Height = this.Height
                };
                button.Click += (s, arg) =>
                {
                    InternalCurrentPage = (int)(((Button)s).Content); PageNoToGo = InternalCurrentPage;
                };
                _pageButtons.Add(button);
            }

            if (_itemsControl != null)
            {
                _itemsControl.ItemsSource = _pageButtons;
            }

            UpdateButtonEnableState(InternalCurrentPage, totalPages);
            SetTextBlockText(MessagePattern, TotalRecordCount, totalPages);
        }

        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }


        private void OnCurrentChanged(int oldValue, int newValue)
        {
            SetPageNoButtons(newValue);
            OnPageChanged(new RoutedPropertyChangedEventArgs<int>(oldValue, newValue, PageChangeedEvent));
        }

        private void SetPageNoButtons(int currentPage)
        {
            if (MaxDisplayPageCount < TotalPages)
            {
                var mid = MaxDisplayPageCount / 2;

                if (currentPage >= mid + 1 && currentPage <= TotalPages - mid - 1)
                {
                    SetPageNoButtons(currentPage - 2, currentPage - 3 + MaxDisplayPageCount);
                }
                else if (currentPage < mid + 1)
                {
                    SetPageNoButtons(1, MaxDisplayPageCount);
                }
                else
                {
                    SetPageNoButtons(TotalPages - MaxDisplayPageCount + 1, TotalPages);
                }
            }

            UpdateButtonEnableState(currentPage, TotalPages);
        }

        private void SetPageNoButtons(int startNo, int endNo)
        {
            _pageButtons = new ObservableCollection<Button>();
            for (int i = startNo; i <= endNo; i++)
            {
                var button = new Button
                {
                    Style = ButtonStyle,
                    Content = i,
                    Height = Height
                };
                button.Click += (s, arg) => { InternalCurrentPage = (int)(((Button)s).Content); PageNoToGo = InternalCurrentPage; };
                _pageButtons.Add(button);
            }
            _itemsControl.ItemsSource = _pageButtons;
        }

        private int _internalCurrentPage = 1;
        private int InternalCurrentPage
        {
            get => _internalCurrentPage;
            set
            {
                if (_internalCurrentPage == value)
                    return;
                var oldValue = _internalCurrentPage;
                _internalCurrentPage = value;
                OnCurrentChanged(oldValue, _internalCurrentPage);
            }
        }

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(
            "CurrentPage", typeof(int), typeof(Pager), new PropertyMetadata(1, OnCurrentPagePropertyChanged));

        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        private static void OnCurrentPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Pager).OnCurrentPagePropertyChanged(e);
        }

        private void OnCurrentPagePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            _internalCurrentPage = (int)e.NewValue;
            SetPageNoButtons((int)e.NewValue);
        }


        public int MaxDisplayPageCount { get; set; } = 5;

        public static readonly DependencyProperty MessagePatternProperty = DependencyProperty.Register(
            "MessagePattern", typeof(string), typeof(Pager),
            new PropertyMetadata("找到{0}条记录，共{1}页", OnMessagePatternPropertyChanged));

        private static void OnMessagePatternPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pager)d).OnMessagePatternPropertyChanged(e);
        }

        private void OnMessagePatternPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var pattern = (string)e.NewValue;
            SetTextBlockText(pattern, TotalRecordCount, TotalPages);
        }

        private void SetTextBlockText(string pattern, object value1, object value2)
        {
            if (_textBlock != null)
            {

                try
                {
                    _textBlock.Text = string.Format(pattern, value1, value2);
                }
                catch
                {
                    _textBlock.Text = "";
                }
            }
        }

        public string MessagePattern
        {
            get => (string)GetValue(MessagePatternProperty);
            set => SetValue(MessagePatternProperty, value);
        }

        public static readonly DependencyProperty TotalRecordCountProperty = DependencyProperty.Register(
            "TotalRecordCount", typeof(int), typeof(Pager), new PropertyMetadata(0, OnTotalRecordCountProperty));

        private static void OnTotalRecordCountProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pager)d).OnTotalRecordCountProperty(e);
        }

        private void OnTotalRecordCountProperty(DependencyPropertyChangedEventArgs e)
        {
            var value = (int)e.NewValue;
            SetTextBlockText(MessagePattern, value, TotalPages);
        }

        public int TotalRecordCount
        {
            get { return (int)GetValue(TotalRecordCountProperty); }
            set { SetValue(TotalRecordCountProperty, value); }
        }

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            "ButtonStyle", typeof(Style), typeof(Pager), new PropertyMetadata(null, OnButtonStylePropertyChanged));

        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        private static void OnButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pager)d).OnButtonStylePropertyChanged(e);
        }

        private void OnButtonStylePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var style = (Style)e.NewValue;
            if (_nextPageButton != null)
            {
                _nextPageButton.Style = style;
            }
            if (_previousButton != null)
            {
                _previousButton.Style = style;
            }
            if (_firstPageButton != null)
            {
                _firstPageButton.Style = style;
            }
            if (_lastPageButton != null)
            {
                _lastPageButton.Style = style;
            }
            if (_goPageButton != null)
            {
                _goPageButton.Style = style;
            }

            if (_pageButtons != null && _pageButtons.Count > 0)
            {
                foreach (var button in _pageButtons)
                {
                    button.Style = style;
                }
            }
        }

        public static readonly DependencyProperty PageNoBoxStyleProperty = DependencyProperty.Register(
            "PageNoBoxStyle", typeof(Style), typeof(Pager), new PropertyMetadata(default(Style), OnPageNoBoxStylePropertyChanged));

        public Style PageNoBoxStyle
        {
            get => (Style)GetValue(PageNoBoxStyleProperty);
            set => SetValue(PageNoBoxStyleProperty, value);
        }

        private static void OnPageNoBoxStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pager)d).OnPageNoBoxStylePropertyChanged(e);
        }

        private void OnPageNoBoxStylePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_textBox != null)
            {
                _textBox.Style = (Style)e.NewValue;
            }
        }

        public static readonly DependencyProperty PageNoToGoProperty = DependencyProperty.Register(
            "PageNoToGo", typeof(int), typeof(Pager), new PropertyMetadata(1, OnPageNoToGoPropertyChanged));

        private static void OnPageNoToGoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pager)d).OnPageNoToGoPropertyChanged(e);
        }

        private void OnPageNoToGoPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue.ToString();

            if (int.TryParse(newValue, out var value))
            {
                if (value >= 1 && value <= TotalPages)
                {
                    if (_goPageButton != null)
                    {
                        _goPageButton.IsEnabled = true;
                        return;
                    }
                }
            }
            else
            {
                PageNoToGo = 1;
            }

            if (_goPageButton != null)
                _goPageButton.IsEnabled = false;
        }

        public int PageNoToGo
        {
            get => (int)GetValue(PageNoToGoProperty);
            set => SetValue(PageNoToGoProperty, value);
        }

        public static readonly RoutedEvent PageChangeedEvent = EventManager.RegisterRoutedEvent("PageChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(Pager));

        public event RoutedPropertyChangedEventHandler<int> PageChanged
        {
            add => AddHandler(PageChangeedEvent, value);
            remove => RemoveHandler(PageChangeedEvent, value);
        }

        protected void OnPageChanged(RoutedPropertyChangedEventArgs<int> args)
        {
            //RoutedEventArgs routedEventArgs = new RoutedEventArgs(PageChangeedEvent, this);
            RaiseEvent(args);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _nextPageButton = Template.FindName(PART_NextPageButton, this) as Button;
            if (_nextPageButton != null)
            {
                _nextPageButton.IsEnabled = false;
                _nextPageButton.Click += (s, e) =>
                {
                    if (InternalCurrentPage < TotalPages)
                        InternalCurrentPage++;
                    else
                        InternalCurrentPage = TotalPages;

                    PageNoToGo = InternalCurrentPage;
                };
            }
            _previousButton = Template.FindName(PART_PreviousPageButton, this) as Button;
            if (_previousButton != null)
            {
                _previousButton.IsEnabled = false;
                _previousButton.Click += (s, e) =>
                {
                    if (InternalCurrentPage > 0)
                        InternalCurrentPage--;
                    else
                        InternalCurrentPage = 1;
                    PageNoToGo = InternalCurrentPage;
                };
            }
            _firstPageButton = Template.FindName(PART_FirstPageButton, this) as Button;
            if (_firstPageButton != null)
            {
                _firstPageButton.IsEnabled = false;
                _firstPageButton.Click += (s, e) => { InternalCurrentPage = 1; PageNoToGo = InternalCurrentPage; };
            }
            _lastPageButton = Template.FindName(PART_LastPageButton, this) as Button;
            if (_lastPageButton != null)
            {
                _lastPageButton.IsEnabled = false;
                _lastPageButton.Click += (s, e) => { InternalCurrentPage = TotalPages; PageNoToGo = InternalCurrentPage; };
            }
            _goPageButton = Template.FindName(PART_GoPageButton, this) as Button;
            if (_goPageButton != null)
            {
                _goPageButton.IsEnabled = false;
                _goPageButton.Click += (s, e) => { InternalCurrentPage = PageNoToGo; };
            }
            _textBox = Template.FindName(PART_TextBox, this) as TextBox;
            if (_textBox != null)
            {
                var binding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath("PageNoToGo"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                _textBox.SetBinding(TextBox.TextProperty, binding);
                _textBox.GotFocus += _textBox_GotFocus;
            }
            _itemsControl = Template.FindName(PART_ItemsControl, this) as ItemsControl;
            _pageButtons = new ObservableCollection<Button>();
            var buttonCount = Math.Min(TotalPages, MaxDisplayPageCount);
            for (int i = 1; i <= buttonCount; i++)
            {
                var button = new Button
                {
                    Style = ButtonStyle,
                    Content = i,
                    Height = this.Height
                };
                button.Click += (s, arg) => { InternalCurrentPage = (int)(((Button)s).Content); PageNoToGo = InternalCurrentPage; };
                _pageButtons.Add(button);
            }

            _itemsControl.ItemsSource = _pageButtons;
            _textBlock = Template.FindName(PART_TextBlock, this) as TextBlock;
            if (_textBlock != null)
            {
                SetTextBlockText(MessagePattern, TotalRecordCount, TotalPages);
            }
        }

        private void _textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void UpdateButtonEnableState(int currentPage, int totalPages)
        {
            if (_firstPageButton != null)
                _firstPageButton.IsEnabled = currentPage > 1;
            if (_previousButton != null)
                _previousButton.IsEnabled = currentPage > 1;
            if (_nextPageButton != null)
                _nextPageButton.IsEnabled = currentPage < totalPages;
            if (_lastPageButton != null)
                _lastPageButton.IsEnabled = currentPage < totalPages;
            if (_goPageButton != null)
                _goPageButton.IsEnabled = _textBox != null && int.TryParse(_textBox.Text, out var pageNo);
            if (_pageButtons != null && _pageButtons.Count > 0)
            {
                foreach (var button in _pageButtons)
                {
                    button.IsEnabled = (int)button.Content != currentPage;
                }
            }
        }
    }
}

