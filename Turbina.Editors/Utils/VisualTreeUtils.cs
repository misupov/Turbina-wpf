using System.Windows;
using System.Windows.Media;

namespace Turbina.Editors.Utils
{
    public static class VisualTreeUtils
    {
        public static T FindParent<T>(DependencyObject child) where T : class
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }

    }
}