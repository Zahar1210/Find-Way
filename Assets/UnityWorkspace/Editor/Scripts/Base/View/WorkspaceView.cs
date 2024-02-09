using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkspaceView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<WorkspaceView, UxmlTraits> { }
    
    private Slider _sliderZoom;
    
    private UWGraph _graph;
    private UWInspectorView _inspector;
    private VisualElement _tabContainer;
    private Button _buttonSettings;
    
    private static Workspace _needToRename;
    
    private WorkspaceTab[] _tabs;
    
    private Workspace _workspace;
    
    public UWGraph Graph => _graph;
    public Workspace Workspace => _workspace;
    
    public WorkspaceView()
    {
        RegisterCallbacks();
    }
    
    private void RegisterCallbacks()
    {
        RegisterCallback<GeometryChangedEvent>(Initialize);
        RegisterCallback<KeyDownEvent>(OnKeyDown);
    }
    
    private void Initialize(GeometryChangedEvent evt)
    {
        if (_workspace == null)
            return;
        
        UnregisterCallback<GeometryChangedEvent>(Initialize);
        
        _graph = this.Q<UWGraph>();
        _inspector = this.Q<UWInspectorView>("Inspector");
        _tabContainer = this.Q("WorkspaceTabContainer");
        _buttonSettings = this.Q<Button>("Settings");

        SetUpTabs();
        UpdateTabs();
        SetUpToolbar();
        SetUpSettings();
        UpdateWorkspace(_workspace);
    }
    
    private void SetUpTabs()
    {
        Button buttonAddTab = _tabContainer.Q<Button>("Add");
        buttonAddTab.clickable.clicked += () =>
        {
            Workspace[] closedWorkspaces = WorkspaceSystem.GetClosedWorkspaces();

            if (closedWorkspaces.Length == 0)
            {
                WorkspaceSystem.CreateWorkspace();
            }
            else
            {
                GenericMenu menu = new GenericMenu();
                
                menu.AddItem(new GUIContent("Create"), false, WorkspaceSystem.CreateWorkspace);
                
                foreach (Workspace workspace in WorkspaceSystem.GetClosedWorkspaces())
                    menu.AddItem(new GUIContent("Open/" + workspace.name), false, () => WorkspaceSystem.AddWorkspace(workspace));

                menu.DropDown(buttonAddTab.worldBound);
            }
        };
    }

    public void UpdateTabs()
    {
        Workspace[] openedWorkspaces = WorkspaceSystem.GetOpenedWorkspaces();
        WorkspaceTab[] currentTabs = _tabContainer.Query<WorkspaceTab>().ToList().ToArray();

        _tabs = new WorkspaceTab[openedWorkspaces.Length];
        
        for (int i = 0; i < Mathf.Max(openedWorkspaces.Length, currentTabs.Length); i++)
        {
            if (i < openedWorkspaces.Length)
            {
                WorkspaceTab tab;
                
                if (i >= currentTabs.Length)
                {
                    VisualTreeAsset visualTreeAsset = UWContainer.Instance.WorkspaceTabUXML;
                    tab = visualTreeAsset.CloneTree().Q<WorkspaceTab>();
                    foreach (StyleSheet styleSheet in visualTreeAsset.stylesheets) 
                        tab.styleSheets.Add(styleSheet);

                    _tabContainer.Insert(_tabContainer.childCount - 2, tab);
                    _tabs[i] = tab;
                }
                else
                {
                    tab = currentTabs[i];
                    _tabs[i] = currentTabs[i];
                }

                tab.Initialize(openedWorkspaces[i], openedWorkspaces[i].Opened);
                
                if (_needToRename == tab.Workspace)
                {
                    tab.StartRenaming();
                    _needToRename = null;
                }
            }
            else
            {
                _tabContainer.Remove(currentTabs[i]);
            }
        }
    }
    
    private void SetUpToolbar()
    {
        SetUpGraphSettings();
        SetUpToggleInspector();
        SetUpSliderZoom();
        SetUpButtonFit();
    }
    
    private void SetUpSettings()
    {
        _buttonSettings.clicked += () =>
        {
            GenericMenu menu = new GenericMenu();
            Workspace[] workspaces = WorkspaceSystem.GetAllWorkspaces();
            
            menu.AddItem(new GUIContent("Save/All workspaces"), false, WorkspaceSystem.SaveAllWorkspaces);
            menu.AddSeparator("Save/");
            foreach (Workspace workspace in workspaces) menu.AddItem(new GUIContent("Save/" + workspace.name), false, () => WorkspaceSystem.SaveWorkspace(workspace));
            
            menu.AddItem(new GUIContent("Load/Folder workspaces"), false, WorkspaceSystem.LoadWorkspaces);
            menu.AddItem(new GUIContent("Load/One worskpace"), false, WorkspaceSystem.LoadWorkspace);
            
            menu.AddSeparator("");
            
            foreach (Workspace workspace in workspaces) menu.AddItem(new GUIContent("Delete/" + workspace.name), false, () => WorkspaceSystem.DeleteWorkspace(workspace));
            
            menu.ShowAsContext();
        };
    }
    
    private void SetUpGraphSettings()
    {
        ToolbarMenu menu = this.Q<ToolbarMenu>("Settings");
        menu.menu.AppendAction("Zoom", _ => SetZoomActive(!_workspace.ZoomActive), _ => _workspace.ZoomActive ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
    }
    
    private void SetUpToggleInspector()
    {
        ToolbarToggle toggleInspector = this.Q<ToolbarToggle>("Inspector");
        toggleInspector.value = _workspace.InspectorVisible;
        toggleInspector.RegisterValueChangedCallback(evt =>
        {
            _inspector.SetVisibility(evt.newValue);
            _workspace.InspectorVisible = evt.newValue;
        });
    }

    private void SetUpSliderZoom()
    {
        _sliderZoom = this.Q<Slider>("Zoom");
        _sliderZoom.labelElement.style.minWidth = 0f;
        _sliderZoom.lowValue = _graph.MinZoom;
        _sliderZoom.highValue = _graph.MaxZoom;
        _sliderZoom.value = _workspace.Zoom.x;
        _sliderZoom.RegisterValueChangedCallback(evt =>
        {
            _graph.UpdateZoom(evt.newValue);
        });
        _workspace.ZoomChanged += zoom =>
        {
            _sliderZoom.value = zoom;
        };
    }
    
    private void SetUpButtonFit()
    {
        Button buttonFit = this.Q<Button>("Fit");
        buttonFit.clickable.clicked += () =>
        {
            _graph.Frame();
        };
    }
    
    private void OnKeyDown(KeyDownEvent evt)
    {
        
    }

    private void SetZoomActive(bool active)
    {
        _workspace.ZoomActive = active;
        _graph.SetZoomActive(active);
    }

    public void Update()
    {
        _graph?.Update();
    }
    
    public void OnGUI()
    {
        _graph?.OnGUI();
    }
    
    public void UpdateWorkspace(Workspace workspace)
    {
        _workspace = workspace;

        _inspector?.Initialize(_graph, _workspace.InspectorVisible);
        _graph?.UpdateWorkspace(_workspace);
    }
    
    public void StartRenaming(Workspace workspace)
    {
        _needToRename = workspace;
    }
}