using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UWInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<UWInspectorView, UxmlTraits> { }
    
    private UWGraph _graph;
    
    private Label _title;
    private IMGUIContainer _container;
    
    private UWFile _currentFile;
    private bool _visible = true;
    
    public UWInspectorView()
    {
        UWFileView.Selected += OnFileSelected;
        UWFileView.Unselected += OnFileUnselected;
    }
    
    public void Initialize(UWGraph graphView, bool visible)
    {
        _graph = graphView;
        
        _title = this.Q<Label>();
        _container = this.Q<IMGUIContainer>();
        
        _visible = visible;
    }
    
    private void UpdateInspector()
    {
        UpdateVisibility();
        if (CanVisible()) DrawInspector();
        else DisableInspector();
    }

    private void UpdateTitle()
    {
        _title.text = _currentFile.Title;
    }
    
    private void UpdateVisibility()
    {
        if (CanVisible())
            AddToClassList("inspector-active");
        else
            RemoveFromClassList("inspector-active");
    }

    private bool CanVisible()
    {
        return _visible && _graph.selection.Count == 1;
    }

    private void DrawInspector()
    {
        if (_graph.selection.Count != 1)
        {
            _container.onGUIHandler = DrawMultipleInspectorGUI;
            return;
        }

        UWFileView fileView = (UWFileView) _graph.selection[0];
        UWFile file = fileView.File;
        
        if (_currentFile == file) return;
        _currentFile = file;
        
        UWDebug.Log("Change inspector");
        _container.onGUIHandler = () => DrawInspectorGUI(fileView);
        
        UpdateTitle();
        UpdateVisibility();
    }

    private void DrawInspectorGUI(UWFileView fileView)
    {
        if (fileView == null || fileView.File == null)
        {
            _container.onGUIHandler = null;
            return;
        }
        
        UnityAction updateCallback = null;
        bool needUpdate = false;
        
        EditorGUI.BeginChangeCheck();
        fileView.File.DrawInspectorGUI(90f, ref updateCallback, ref needUpdate);
        EditorGUILayout.Space(10f);
        if (EditorGUI.EndChangeCheck() || needUpdate)
        {
            updateCallback?.Invoke();
            
            fileView.UpdateFile();
            
            UpdateTitle();
        }
    }
    
    private void DisableInspector()
    {
        _currentFile = null;
        _container.onGUIHandler = null;
    }
    
    private void DrawMultipleInspectorGUI()
    {
        EditorGUILayout.LabelField("Multiple selection not supported");
    }
    
    private void OnFileSelected(UWFileView fileView)
    {
        UpdateInspector();
    }
    
    private async void OnFileUnselected(UWFileView fileView)
    {
        await Task.Delay(10);
        if (_graph == null) return;
        
        UpdateInspector();
    }

    public void SetVisibility(bool visible)
    {
        _visible = visible;
        
        UpdateInspector();
    }
}