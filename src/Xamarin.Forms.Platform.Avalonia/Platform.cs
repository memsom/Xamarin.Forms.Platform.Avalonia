﻿using AvaloniaForms;
using AvaloniaForms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Avalonia.Controls;

namespace Xamarin.Forms.Platform.Avalonia
{
    public abstract class Platform : BindableObject, INavigation
#pragma warning disable CS0618 // Type or member is obsolete
		, IPlatform
#pragma warning restore CS0618 // Type or member is obsolete
	{
		readonly global::Avalonia.Controls.ContentControl _page;
		ApplicationWindow ParentWindow => _page?.GetParentWindow() as ApplicationWindow;

		Page Page { get; set; }

		public IReadOnlyList<Page> NavigationStack
		{
			get { throw new InvalidOperationException("NavigationStack is not supported globally on Windows, please use a NavigationPage."); }
		}

		public IReadOnlyList<Page> ModalStack
		{
			get
			{
				return ParentWindow?.InternalChildren.Cast<Page>().ToList();
			}
		}

		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer));

		internal static Platform Current => (DefaultNavigation.MainWindow as FormsApplicationPage)?.Platform;

		internal Platform(global::Avalonia.Controls.ContentControl page)
		{
			_page = page;


			var busyCount = 0;
			MessagingCenter.Subscribe(this, Page.BusySetSignalName, (Page sender, bool enabled) =>
			{
				busyCount = Math.Max(0, enabled ? busyCount + 1 : busyCount - 1);
			});

			SubscribeAlertsAndActionSheets();
		}


		internal void UpdateToolbarItems()
		{
			// TODO:
		}


		internal void SubscribeAlertsAndActionSheets()
		{
			MessagingCenter.Subscribe<Page, AlertArguments>(this, Page.AlertSignalName, OnPageAlert);
			MessagingCenter.Subscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName, OnPageActionSheet);
		}

		async void OnPageAlert(Page sender, AlertArguments options)
		{
			string content = options.Message ?? options.Title ?? string.Empty;

			FormsContentDialog dialog = new FormsContentDialog();

			if (options.Message == null || options.Title == null)
			{
				dialog.Content = content;
			}
			else
			{
				dialog.Title = options.Title;
				dialog.Content = options.Message;
			}

			if (options.Accept != null)
			{
				dialog.IsPrimaryButtonEnabled = true;
				dialog.PrimaryButtonText = options.Accept;
			}

			if (options.Cancel != null)
			{
				dialog.IsSecondaryButtonEnabled = true;
				dialog.SecondaryButtonText = options.Cancel;
			}

			var dialogResult = await dialog.ShowAsync();

			options.SetResult(dialogResult == ContentDialogResult.Primary);
		}

		async void OnPageActionSheet(Page sender, ActionSheetArguments options)
		{
			var list = new global::Avalonia.Controls.ListBox
			{
				//Style = (System.Windows.Style)System.Windows.Application.Current.Resources["ActionSheetList"],
			};

			foreach (var item in options.Buttons)
			{
				list.Items.Add(item);
			}

			var dialog = new FormsContentDialog
			{
				Content = list,
			};

			if (options.Title != null)
			{
				dialog.Title = options.Title;
			}

			list.SelectionChanged += (s, e) =>
			{
				if (list.SelectedItem != null)
				{
					dialog.Hide();
					options.SetResult((string)list.SelectedItem);
				}
			};

			/*_page.KeyDown += (window, e) =>
			 {
				 if (e.Key == System.Windows.Input.Key.Escape)
				 {
					 dialog.Hide();
					 options.SetResult(LightContentDialogResult.None.ToString());
				 }
			 };*/

			if (options.Cancel != null)
			{
				dialog.IsSecondaryButtonEnabled = true;
				dialog.SecondaryButtonText = options.Cancel;
			}

			if (options.Destruction != null)
			{
				dialog.IsPrimaryButtonEnabled = true;
				dialog.PrimaryButtonText = options.Destruction;
			}

			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.Secondary)
			{
				options.SetResult(options.Cancel);
			}
			else if (result == ContentDialogResult.Primary)
			{
				options.SetResult(options.Destruction);
			}
		}

		public static SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			if (widthConstraint > 0 && heightConstraint > 0 && GetRenderer(view) != null)
			{
				IVisualElementRenderer element = GetRenderer(view);
				return element.GetDesiredSize(widthConstraint, heightConstraint);
			}

			return new SizeRequest();
		}


		public static IVisualElementRenderer GetOrCreateRenderer(VisualElement element)
		{
			if (GetRenderer(element) == null)
			{
				SetRenderer(element, CreateRenderer(element));
			}

			return GetRenderer(element);
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			IVisualElementRenderer result = Registrar.Registered.GetHandlerForObject<IVisualElementRenderer>(element) ?? new DefaultViewRenderer();
			result.SetElement(element);
			return result;
		}

		public static IVisualElementRenderer GetRenderer(VisualElement self)
		{
			return (IVisualElementRenderer)self.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement self, IVisualElementRenderer renderer)
		{
			self.SetValue(RendererProperty, renderer);
			self.IsPlatformEnabled = renderer != null;
		}
		
		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				return;

			Page = newRoot;

			ParentWindow?.SetStartupPage(Page);
			Application.Current.NavigationProxy.Inner = this;
		}


		Task INavigation.PushAsync(Page root)
		{
			return ((INavigation)this).PushAsync(root, true);
		}

		Task<Page> INavigation.PopAsync()
		{
			return ((INavigation)this).PopAsync(true);
		}

		Task INavigation.PopToRootAsync()
		{
			return ((INavigation)this).PopToRootAsync(true);
		}

		Task INavigation.PushAsync(Page root, bool animated)
		{
			throw new InvalidOperationException("PushAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopAsync(bool animated)
		{
			throw new InvalidOperationException("PopAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PopToRootAsync(bool animated)
		{
			throw new InvalidOperationException("PopToRootAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		void INavigation.RemovePage(Page page)
		{
			throw new InvalidOperationException("RemovePage is not supported globally on Windows, please use a NavigationPage.");
		}

		void INavigation.InsertPageBefore(Page page, Page before)
		{
			throw new InvalidOperationException("InsertPageBefore is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync(Page page)
		{
			return ((INavigation)this).PushModalAsync(page, true);
		}

		Task<Page> INavigation.PopModalAsync()
		{
			return ((INavigation)this).PopModalAsync(true);
		}

		Task INavigation.PushModalAsync(Page page, bool animated)
		{
			if (page == null)
				throw new ArgumentNullException(nameof(page));

			var tcs = new TaskCompletionSource<bool>();

#pragma warning disable CS0618 // Type or member is obsolete
			// The Platform property is no longer necessary, but we have to set it because some third-party
			// library might still be retrieving it and using it
			page.Platform = this;
#pragma warning restore CS0618 // Type or member is obsolete

			ParentWindow?.PushModal(page, animated);
			tcs.SetResult(true);
			return tcs.Task;
		}

		Task<Page> INavigation.PopModalAsync(bool animated)
		{
			var tcs = new TaskCompletionSource<Page>();
			var page = ParentWindow?.PopModal(animated) as Page;
			tcs.SetResult(page);
			return tcs.Task;
		}

		#region Obsolete 

		SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			return GetNativeSize(view, widthConstraint, heightConstraint);
		}

		#endregion
	}
}
