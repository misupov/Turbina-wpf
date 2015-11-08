using System;

namespace Turbina.Editors
{
    public interface IControlTypesResolver
    {
        Type GetLinkControlType();

        Type GetNodeEditorType(Node node);

        Type GetPinEditorType(IPin pin);
    }
}