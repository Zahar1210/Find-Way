using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UWWindow : EditorWindow
{
    private static UWWindow _window;
    
    private bool _needToUpdate;
    
    public static WorkspaceView WorkspaceView;

    private void OnEnable()
    {
        _window = this;
        
        Undo.undoRedoPerformed += OnUndoRedo;
        EditorApplication.projectChanged += UpdateGUI;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
        EditorApplication.projectChanged -= UpdateGUI;
    }

    [MenuItem("Window/Unity Workspace")]
    public static void OpenWindow()
    {
        _window = GetWindow<UWWindow>();
        _window.titleContent = new GUIContent("Unity Workspace");
        _window.minSize = new Vector2(400f, 300f);
    }

    private void Update()
    {
        WorkspaceView?.Update();
    }
    
    private void OnGUI()
    {
        WorkspaceView?.OnGUI();
        
        if (_needToUpdate)
        {
            _needToUpdate = false;
            UpdateGUI();
        }
    }
    
    private void OnUndoRedo()
    {
        _needToUpdate = true;
        AssetDatabase.SaveAssets();
    }
    
    private void ShowInitializationScreen()
    {
        VisualTreeAsset visualTree = UWContainer.Instance.InitializationScreenUXML;
        visualTree.CloneTree(rootVisualElement);
    }
    
    private void ShowWorkspaceScreen(Workspace workspace)
    {
        VisualTreeAsset visualTree = UWContainer.Instance.WorkspaceUXML;
        visualTree.CloneTree(rootVisualElement);
        
        WorkspaceView = rootVisualElement.Q<WorkspaceView>();
        WorkspaceView.UpdateWorkspace(workspace);
    }

    private void CreateGUI()
    {
        UpdateGUI();
    }
    
    public static void UpdateGUI()
    {
        UWDebug.Log("UpdateGUI");
        _window.rootVisualElement.Clear();
        
        Workspace workspace = WorkspaceSystem.GetCurrentWorkspace();
        if (workspace == null)
            _window.ShowInitializationScreen();
        else
            _window.ShowWorkspaceScreen(workspace);
    }
    
    public static void FocusWindow()
    {
        _window.Focus();
    }
}