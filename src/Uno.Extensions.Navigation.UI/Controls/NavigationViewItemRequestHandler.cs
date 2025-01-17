﻿using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using Windows.Foundation;

namespace Uno.Extensions.Navigation.UI
{
    public class NavigationViewItemRequestHandler : ActionRequestHandlerBase<NavigationViewItem>
    {
        public override void Bind(FrameworkElement view)
        {
            var viewButton = view as NavigationViewItem;
            if (viewButton is null)
            {
                return;
            }

            var parent = VisualTreeHelper.GetParent(view);
            while (parent is not null && parent is not NavigationView)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent is null)
            {
                return;
            }
			BindAction((NavigationView)parent,
                action => new TypedEventHandler<NavigationView, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs>((sender, args) =>
                {
                    if ((args.InvokedItemContainer is FrameworkElement navItem && navItem == viewButton))
                    {
                        action((FrameworkElement)args.InvokedItemContainer);
                    }
                }),
                (element, handler) => element.ItemInvoked += handler,
                (element, handler) => element.ItemInvoked -= handler);
        }
    }
}
