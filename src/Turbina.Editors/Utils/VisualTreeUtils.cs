using System.Windows;
using System.Windows.Media;

namespace Turbina.Editors.Utils
{
    public static class VisualTreeUtils
    {
        public static T FindParent<T>(DependencyObject child) where T : class
        {
            while (true)
            {
                var parentObject = VisualTreeHelper.GetParent(child);

                if (parentObject == null)
                {
                    return null;
                }

                var parent = parentObject as T;
                if (parent != null)
                {
                    return parent;
                }

                child = parentObject;
            }
        }

        public static TDataContextType FindDataContext<TDataContextType>(FrameworkElement element) where TDataContextType : class
        {
            while (true)
            {
                if (element == null)
                {
                    return null;
                }

                var dataContext = element.DataContext as TDataContextType;

                if (dataContext != null)
                {
                    return dataContext;
                }

                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
        }
    }
}