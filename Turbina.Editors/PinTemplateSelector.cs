//using System.Windows;
//using System.Windows.Controls;
//
//namespace Turbina.Editors
//{
//    public class PinTemplateSelector : DataTemplateSelector
//    {
//        public override DataTemplate SelectTemplate(object item, DependencyObject container)
//        {
////            var pin = item as Pin;
////            if (pin != null)
////            {
////                var presenterType = pin.PresenterType;
////                return presenterType != null
////                    ? new ItemContainerTemplate { VisualTree = new FrameworkElementFactory(presenterType) }
////                    : null;
////            }
//
//            return base.SelectTemplate(item, container);
//        }
//    }
//}