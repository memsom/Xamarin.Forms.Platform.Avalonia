﻿using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;

namespace AvaloniaForms.Controls
{
    public class PageContentPresenter : ContentPresenter
    {
        public PageContentPresenter()
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
			OnSizeChanged(new SizeChangedEventArgs(null)); // this is probably wrong
		}
		#endregion
	}
}
