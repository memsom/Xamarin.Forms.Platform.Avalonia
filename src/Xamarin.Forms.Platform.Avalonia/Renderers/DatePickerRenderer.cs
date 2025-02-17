﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Xamarin.Forms.Platform.Avalonia
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, AvaloniaForms.Controls.DatePicker>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new AvaloniaForms.Controls.DatePicker());
					Control.SelectedDateChanged += OnNativeSelectedDateChanged; //+= OnNativeSelectedDateChanged;
				}

				// Update control property 
				UpdateDate();
				UpdateMinimumDate();
				UpdateMaximumDate();
				UpdateTextColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName)
				UpdateDate();
			else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
				UpdateMaximumDate();
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
				UpdateMinimumDate();
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
		}
		
		void UpdateDate()
		{
			Control.SelectedDate = Element.Date;
		}

		void UpdateMaximumDate()
		{
			Control.DisplayDateEnd = Element.MaximumDate;
		}

		void UpdateMinimumDate()
		{
			Control.DisplayDateStart = Element.MinimumDate;
		}

		void UpdateTextColor()
		{
			Control.UpdateDependencyColor(AvaloniaForms.Controls.DatePicker.ForegroundProperty, Element.TextColor);
		}

		private void OnNativeSelectedDateChanged(object sender, global::Avalonia.Controls.SelectionChangedEventArgs e)
		{
			if (Control.SelectedDate.HasValue)
			{
				((IElementController)Element).SetValueFromRenderer(DatePicker.DateProperty, Control.SelectedDate.Value);
			}
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
					Control.SelectedDateChanged -= OnNativeSelectedDateChanged;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}
	}
}
