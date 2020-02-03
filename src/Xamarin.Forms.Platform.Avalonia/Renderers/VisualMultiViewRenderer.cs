﻿using Avalonia.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Avalonia.Controls;
using ASelectionChangedEventArgs = Xamarin.Forms.Platform.Avalonia.Controls.SelectionChangedEventArgs;

namespace Xamarin.Forms.Platform.Avalonia
{
    public class VisualMultiViewRenderer<TElement, TNativeElement> : ViewRenderer<TElement, TNativeElement>
                where TElement : ItemsView where TNativeElement : FormsMultiView
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
        {
            if (e.OldElement != null) // Clear old element event
            {
                ((INotifyCollectionChanged)e.OldElement.ItemsSource).CollectionChanged -= OnPagesChanged;
            }

            if (e.NewElement != null)
            {
                // Subscribe control event
                Control.SelectionChanged += Control_SelectionChanged;

                // Subscribe element event
                ((INotifyCollectionChanged)Element.ItemsSource).CollectionChanged += OnPagesChanged;
            }

            base.OnElementChanged(e);
        }

        protected override void Appearing()
        {
            base.Appearing();

            OnPagesChanged(Element.ItemsSource, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Element.ItemsSource != null)
            {
                Control.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<object>(Element.ItemsSource.Cast<object>());
            }
            else
            {
                Control.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<object>();
            }
        }

        private void Control_SelectionChanged(object sender, ASelectionChangedEventArgs e)
        {
        }

        bool _isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                if (Control != null)
                {
                    Control.SelectionChanged -= Control_SelectionChanged;
                }

                if (Element != null)
                {
                    ((INotifyCollectionChanged)Element.ItemsSource).CollectionChanged -= OnPagesChanged;
                }
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
