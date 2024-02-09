using UnityEngine.Events;
using UnityEngine.UIElements;

public class UWCollapseButton : VisualElement
{
    public new class UxmlFactory : UxmlFactory<UWCollapseButton, UxmlTraits> { }
    
    private bool _isCollapsed;

    public bool IsCollapsed
    {
        get => _isCollapsed;
        set
        {
            _isCollapsed = value;
            UpdateState();
            
            ValueChanged?.Invoke(_isCollapsed);
        }
    }
    
    public UnityAction<bool> ValueChanged;
    
    public UWCollapseButton()
    {
        RegisterCallback<MouseDownEvent>(OnMouseDown);
        styleSheets.Add(UWContainer.Instance.CollapseButton);
    }
    
    private void OnMouseDown(MouseDownEvent evt)
    {
        IsCollapsed = !IsCollapsed;
    }
    
    private void UpdateState()
    {
        if (_isCollapsed)
            AddToClassList("collapsed");
        else
            RemoveFromClassList("collapsed");
    }
}