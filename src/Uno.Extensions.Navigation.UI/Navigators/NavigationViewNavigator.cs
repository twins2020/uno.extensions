﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Uno.Extensions.Navigation;
using Uno.Extensions.Navigation.UI;
using Uno.Extensions.Navigation.Regions;
#if !WINUI
using Windows.UI.Xaml;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace Uno.Extensions.Navigation.Navigators;

public class NavigationViewNavigator : ControlNavigator<Microsoft.UI.Xaml.Controls.NavigationView>, ICompositeNavigator
{
	protected override FrameworkElement? CurrentView => Control?.SelectedItem as FrameworkElement;

	public override void ControlInitialize()
	{
		if (Control is not null)
		{
			Control.SelectionChanged += ControlSelectionChanged;
		}
	}

	private void ControlSelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
	{
		var tbi = args.SelectedItem as FrameworkElement;

		var path = tbi?.GetName() ?? tbi?.Name;
		if (path is not null &&
			!string.IsNullOrEmpty(path))
		{
			Region.Navigator()?.NavigateRouteAsync(sender, path);
		}
	}

	public NavigationViewNavigator(
		ILogger<NavigationViewNavigator> logger,
		IRegion region,
		IMappings mappings,
		RegionControlProvider controlProvider)
		: base(logger, region, mappings, controlProvider.RegionControl as Microsoft.UI.Xaml.Controls.NavigationView)
	{
	}

	protected override async Task<string?> Show(string? path, Type? viewType, object? data)
	{
		if (Control is null)
		{
			return null;
		}

		Control.SelectionChanged -= ControlSelectionChanged;
		try
		{
			var item = (from mi in Control.MenuItems.OfType<FrameworkElement>()
						where (mi.GetName() ?? mi.Name) == path
						select mi).FirstOrDefault();
			if (item != null)
			{
				Control.SelectedItem = item;
			}

			// Don't return path, as we need for path to be passed down to children
			return default;
		}
		finally
		{
			await (Control.Content as FrameworkElement).EnsureLoaded();
			Control.SelectionChanged += ControlSelectionChanged;
		}
	}
}
