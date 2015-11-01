using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Turbina.Editors.Ropes;

namespace Turbina.Editors
{
    public class EditorTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, FrameworkElementFactory> _editors = new Dictionary<Type, FrameworkElementFactory>();
        private readonly FrameworkElementFactory ropeFactory = new FrameworkElementFactory(typeof(Rope));

        public EditorTemplateSelector()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetCustomAttributes<ContainsNodeEditorsAttribute>().Any())
                {
                    LoadNodeEditors(assembly);
                }
            }
        }

        private void LoadNodeEditors(Assembly assembly)
        {
            foreach (var exportedType in assembly.ExportedTypes)
            {
                foreach (var attribute in exportedType.GetCustomAttributes<EditorForAttribute>(true))
                {
                    foreach (var nodeType in attribute.NodeTypes)
                    {
                        _editors[nodeType] = new FrameworkElementFactory(exportedType);
                    }
                }
            }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var type = item.GetType();

                while (type != null)
                {
                    FrameworkElementFactory factory;
                    if (_editors.TryGetValue(type, out factory))
                    {
                        return new ItemContainerTemplate { VisualTree = factory };
                    }

                    type = type.BaseType;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}