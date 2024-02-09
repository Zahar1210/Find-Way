using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class UWResizableElement : ResizableElement
{
    public new class UxmlFactory : UxmlFactory<UWResizableElement, UxmlTraits> { }
}