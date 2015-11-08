using System;
using Turbina.Editors.PinControls;
using Turbina.Editors.Ropes;

namespace Turbina.Editors
{
    public class ControlTypesResolver : IControlTypesResolver
    {
        public Type GetLinkControlType()
        {
            return typeof(Rope);
        }

        public Type GetNodeEditorType(Node node)
        {
            if (node is CompositeNode)
            {
                return typeof (CompositeNodeEditor);
            }

            return typeof(NodeEditor);
        }

        public Type GetPinEditorType(IPin pin)
        {
            if (pin.Direction == PinDirection.Output)
            {
                return typeof (PinEditorControl);
            }

            if (pin.Type == typeof (bool))
            {
                return typeof (OnOffPinControl);
            }

            if (pin.Type == typeof (TimeSpan))
            {
                return typeof (TimeSpanPinControl);
            }

            if (pin.Type == typeof (Uri))
            {
                return typeof (WebBrowserPinControl);
            }

            return typeof (TextBoxPinControl);
        }
    }
}