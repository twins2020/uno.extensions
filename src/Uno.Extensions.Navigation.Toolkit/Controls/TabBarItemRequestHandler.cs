﻿using Uno.Extensions.Navigation.UI;
using Uno.Toolkit.UI;

namespace Uno.Extensions.Navigation.Toolkit.Controls;

public class TabBarItemRequestHandler : ActionRequestHandlerBase<TabBarItem>
{
	public override void Bind(FrameworkElement view)
	{
		var viewButton = view as TabBarItem;
		if (viewButton is null)
		{
			return;
		}

		BindAction(viewButton,
			action => new RoutedEventHandler((sender, args) => action((TabBarItem)sender)),
			(element, handler) => element.Click += handler,
			(element, handler) => element.Click -= handler);
	}
}
