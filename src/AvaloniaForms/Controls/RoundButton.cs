using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvaloniaForms.Controls
{
    public class RoundButton : Button, IStyleable
    {
		static RoundButton()
		{
			CornerRadiusProperty.Changed.AddClassHandler<RoundButton>((x, e) => x.OnCornerRadiusPropertyChanged(e));
		}

		Type IStyleable.StyleKey => typeof(Button);
		
		ContentPresenter _contentPresenter;

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);
			_contentPresenter = VisualChildren.OfType<ContentPresenter>().FirstOrDefault();
			UpdateCornerRadius();
		}

		private void OnCornerRadiusPropertyChanged(AvaloniaPropertyChangedEventArgs e)
		{
			UpdateCornerRadius();
		}

		void UpdateCornerRadius()
		{
			if (_contentPresenter != null)
			{
				_contentPresenter.CornerRadius = new CornerRadius(
					CornerRadius.TopLeft,
					CornerRadius.TopRight,
					CornerRadius.BottomRight,
					CornerRadius.BottomLeft);
			}
		}
	}
}
