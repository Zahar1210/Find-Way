using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InitializationScreen : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InitializationScreen, UxmlTraits> { }
    
    private Button _buttonCreateWorkspace;
    private Button _buttonOpenWorkspace;
    
    public InitializationScreen()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }
    
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        
        _buttonCreateWorkspace = this.Q<Button>("button-create-workspace");
        _buttonOpenWorkspace = this.Q<Button>("button-open-workspace");

        _buttonCreateWorkspace.clickable.clicked += CreateWorkspace;
        
        if (WorkspaceSystem.GetClosedWorkspaces().Length == 0)
            _buttonOpenWorkspace.SetEnabled(false);
        
        _buttonOpenWorkspace.clickable.clicked += Open;
    }
    
    private void CreateWorkspace()
    {
        WorkspaceSystem.CreateWorkspace();
    }
    
    private void Open()
    {
        GenericMenu menu = new GenericMenu();
        
        foreach (Workspace workspace in WorkspaceSystem.GetClosedWorkspaces())
            menu.AddItem(new GUIContent(workspace.name), false, () => WorkspaceSystem.AddWorkspace(workspace));

        menu.DropDown(_buttonOpenWorkspace.worldBound);
    }
}