﻿using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;

namespace AvaloniaForms.Controls
{
    public class PageControl : ContentControl, IToolbarProvider, ITitleViewRendererController, IStyleable
    {
        public static readonly StyledProperty<bool> TitleVisibilityProperty = AvaloniaProperty.Register<PageControl, bool>(nameof(TitleVisibility));
        public static readonly StyledProperty<Brush> ToolbarBackgroundProperty = AvaloniaProperty.Register<PageControl, Brush>(nameof(ToolbarBackground));
        public static readonly StyledProperty<string> BackButtonTitleProperty = AvaloniaProperty.Register<PageControl, string>(nameof(BackButtonTitle));
        public static readonly StyledProperty<Thickness> ContentMarginProperty = AvaloniaProperty.Register<PageControl, Thickness>(nameof(ContentMargin));
        public static readonly StyledProperty<Bitmap> TitleIconProperty = AvaloniaProperty.Register<PageControl, Bitmap>(nameof(TitleIcon));
        public static readonly StyledProperty<object> TitleViewContentProperty = AvaloniaProperty.Register<PageControl, object>(nameof(TitleViewContent));
        public static readonly StyledProperty<bool> TitleViewVisibilityProperty = AvaloniaProperty.Register<PageControl, bool>(nameof(TitleViewVisibility));
        public static readonly StyledProperty<double> TitleInsetProperty = AvaloniaProperty.Register<PageControl, double>(nameof(TitleInset));
        public static readonly StyledProperty<Brush> TitleBrushProperty = AvaloniaProperty.Register<PageControl, Brush>(nameof(TitleBrush));

        static PageControl()
        {
        }

        Type IStyleable.StyleKey => typeof(PageControl);

        protected CommandBar _commandBar;
        public CommandBar CommandBar => _commandBar;

        protected Control _titleViewPresenter;
        public Control TitleViewPresenter => _titleViewPresenter;

        ToolbarPlacement _toolbarPlacement;
        bool _toolbarDynamicOverflowEnabled = true;
        readonly ToolbarPlacementHelper _toolbarPlacementHelper = new ToolbarPlacementHelper();

        public bool ShouldShowToolbar
        {
            get => _toolbarPlacementHelper.ShouldShowToolBar;
            set => _toolbarPlacementHelper.ShouldShowToolBar = value;
        }

        public Bitmap TitleIcon
        {
            get => GetValue(TitleIconProperty);
            set => SetValue(TitleIconProperty, value);
        }

        public object TitleViewContent
        {
            get => GetValue(TitleViewContentProperty);
            set => SetValue(TitleViewContentProperty, value);
        }

        TaskCompletionSource<CommandBar> _commandBarTcs;
        ContentPresenter _presenter;
        TitleViewManager _titleViewManager;

        public PageControl()
        {
            LayoutUpdated += OnLayoutUpdated;
        }

        #region Loaded & Unloaded
        //public event EventHandler<RoutedEventArgs> Loaded;
        //public event EventHandler<RoutedEventArgs> Unloaded;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            OnLoaded(new RoutedEventArgs());
            Appearing();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            OnUnloaded(new RoutedEventArgs());
            Disappearing();
        }

        //protected virtual void OnLoaded(RoutedEventArgs e) { Loaded?.Invoke(this, e); }
        //protected virtual void OnUnloaded(RoutedEventArgs e) { Unloaded?.Invoke(this, e); }
        #endregion

        #region Appearing & Disappearing
        protected virtual void Appearing()
        {
        }

        protected virtual void Disappearing()
        {
        }
        #endregion

        #region LayoutUpdated & SizeChanged
        //public event EventHandler<EventArgs> SizeChanged;
        //protected virtual void OnSizeChanged(EventArgs e) { SizeChanged?.Invoke(this, e); }

        protected virtual void OnLayoutUpdated(object sender, EventArgs e)
        {
            OnSizeChanged(new SizeChangedEventArgs(null));
        }
        #endregion

        public string BackButtonTitle
        {
            get => GetValue(BackButtonTitleProperty);
            set => SetValue(BackButtonTitleProperty, value);
        }

        public double ContentHeight => _presenter != null ? _presenter.Height : 0;

        public Thickness ContentMargin
        {
            get => GetValue(ContentMarginProperty);
            set => SetValue(ContentMarginProperty, value);
        }

        public double ContentWidth => _presenter != null ? _presenter.Width : 0;

        public Brush ToolbarBackground
        {
            get => GetValue(ToolbarBackgroundProperty);
            set => SetValue(ToolbarBackgroundProperty, value);
        }

        public ToolbarPlacement ToolbarPlacement
        {
            get => _toolbarPlacement;
            set
            {
                _toolbarPlacement = value;
                _toolbarPlacementHelper.UpdateToolbarPlacement();
            }
        }

        public bool ToolbarDynamicOverflowEnabled
        {
            get => _toolbarDynamicOverflowEnabled;
            set
            {
                _toolbarDynamicOverflowEnabled = value;
                UpdateToolbarDynamicOverflowEnabled();
            }
        }

        public bool TitleVisibility
        {
            get => GetValue(TitleVisibilityProperty);
            set => SetValue(TitleVisibilityProperty, value);
        }

        public bool TitleViewVisibility
        {
            get => GetValue(TitleViewVisibilityProperty);
            set => SetValue(TitleViewVisibilityProperty, value);
        }

        public Brush TitleBrush
        {
            get => GetValue(TitleBrushProperty);
            set => SetValue(TitleBrushProperty, value);
        }

        public double TitleInset
        {
            get => GetValue(TitleInsetProperty);
            set => SetValue(TitleInsetProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _presenter = e.NameScope.Find<ContentPresenter>("presenter");

            _titleViewPresenter = e.NameScope.Find<Control>("TitleViewPresenter");

            _commandBar = e.NameScope.Find<CommandBar>("CommandBar");

            _titleViewManager = new TitleViewManager(this);

            _toolbarPlacementHelper.Initialize(_commandBar, () => ToolbarPlacement, name => e.NameScope.Find(name) as AvaloniaObject);
            UpdateToolbarDynamicOverflowEnabled();

            TaskCompletionSource<CommandBar> tcs = _commandBarTcs;
            tcs?.SetResult(_commandBar);
        }

        void UpdateToolbarDynamicOverflowEnabled()
        {
            if (_commandBar != null)
            {
                _commandBar.IsDynamicOverflowEnabled = ToolbarDynamicOverflowEnabled;
            }
        }

        public Task<CommandBar> GetCommandBarAsync()
        {
            if (_commandBar != null)
                return Task.FromResult(_commandBar);

            _commandBarTcs = new TaskCompletionSource<CommandBar>();
            ApplyTemplate();
            return _commandBarTcs.Task;
        }
    }
}
