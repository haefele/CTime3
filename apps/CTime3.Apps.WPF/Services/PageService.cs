using System.Windows;
using CommunityToolkit.Diagnostics;
using Wpf.Ui.Contracts;

namespace CTime3.Apps.WPF.Services
{
    /// <summary>
    /// Service that provides pages for navigation.
    /// </summary>
    public class PageService : IPageService
    {
        private readonly IServiceProvider _serviceProvider;

        public PageService(IServiceProvider serviceProvider)
        {
            Guard.IsNotNull(serviceProvider);

            this._serviceProvider = serviceProvider;
        }

        public T? GetPage<T>() where T : class
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException("The page should be a WPF control.");

            return (T?)_serviceProvider.GetService(typeof(T));
        }

        public FrameworkElement? GetPage(Type pageType)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
                throw new InvalidOperationException("The page should be a WPF control.");

            return _serviceProvider.GetService(pageType) as FrameworkElement;
        }
    }
}
