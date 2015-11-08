using System.Windows;
using System.Windows.Controls;
using Turbina.Editors.ViewModels;

namespace Turbina.Editors
{
    public class LinkTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var linkViewModel = item as LinkViewModel;
            if (linkViewModel != null)
            {
                return new DataTemplate(typeof(LinkViewModel))
                {
                    VisualTree = new FrameworkElementFactory(linkViewModel.ControlTypesResolver.GetLinkControlType())
                };
            }

            return base.SelectTemplate(item, container);
        }
    }

    public class NodeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var nodeViewModel = item as NodeViewModel;
            if (nodeViewModel != null)
            {
                return new DataTemplate(typeof (NodeViewModel))
                {
                    VisualTree = new FrameworkElementFactory(nodeViewModel.ControlTypesResolver.GetNodeEditorType(nodeViewModel.Node))
                };
            }

            return base.SelectTemplate(item, container);
        }
    }

    public class PinTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var pinViewModel = item as PinViewModel;
            if (pinViewModel != null)
            {
                return new DataTemplate(typeof (PinViewModel))
                {
                    VisualTree = new FrameworkElementFactory(pinViewModel.ControlTypesResolver.GetPinEditorType(pinViewModel.Pin))
                };
            }

            return base.SelectTemplate(item, container);
        }
    }

//    public class EditorTemplateSelector : DataTemplateSelector
//    {
//        private readonly Dictionary<Type, FrameworkElementFactory> _editors = new Dictionary<Type, FrameworkElementFactory>();
//
//        public EditorTemplateSelector()
//        {
//            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//            {
//                if (assembly.GetCustomAttributes<ContainsNodeEditorsAttribute>().Any())
//                {
//                    LoadNodeEditors(assembly);
//                }
//            }
//        }
//
//        private void LoadNodeEditors(Assembly assembly)
//        {
//            foreach (var exportedType in assembly.ExportedTypes)
//            {
//                foreach (var attribute in exportedType.GetCustomAttributes<EditorForAttribute>(true))
//                {
//                    foreach (var nodeType in attribute.NodeTypes)
//                    {
//                        _editors[nodeType] = new FrameworkElementFactory(exportedType);
//                    }
//                }
//            }
//        }
//
//        public override DataTemplate SelectTemplate(object item, DependencyObject container)
//        {
//            if (item != null)
//            {
//                var type = item.GetType();
//
//                while (type != null)
//                {
//                    FrameworkElementFactory factory;
//                    if (_editors.TryGetValue(type, out factory))
//                    {
//                        return new ItemContainerTemplate { VisualTree = factory };
//                    }
//
//                    type = type.BaseType;
//                }
//            }
//
//            return base.SelectTemplate(item, container);
//        }
//    }
}