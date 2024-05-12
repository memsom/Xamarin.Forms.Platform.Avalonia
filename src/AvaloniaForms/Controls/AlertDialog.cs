using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaForms.Controls
{
    public partial class AlertDialog : ContentDialog
    {
		public ScrollBarVisibility VerticalScrollBarVisibility { get; set; }

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);

			var scrollName = "ContentScrollViewer";
			if (e.NameScope.Find(scrollName) is ScrollViewer contentScrollViewer)
			{
				contentScrollViewer.VerticalScrollBarVisibility = VerticalScrollBarVisibility;
			}
		}
	}
}
