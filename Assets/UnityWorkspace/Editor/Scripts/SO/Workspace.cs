using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Workspace : ScriptableObject
{
    [SerializeField] private List<UWFile> _files = new List<UWFile>();
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _zoom = Vector2.one;
    [SerializeField] private bool _inspectorVisible = true;
    [SerializeField] private bool _zoomActive = true;
    [SerializeField] private bool _added;
    [SerializeField] private bool _opened;
    [SerializeField] private int _order;
    
    public Vector3 Position { get => _position; set => SetAndSave(ref _position, value); }
    public Vector3 Zoom { get => _zoom; set => SetAndSave(ref _zoom, value); }
    public bool InspectorVisible { get => _inspectorVisible; set => SetAndSave(ref _inspectorVisible, value); }
    public bool ZoomActive { get => _zoomActive; set => SetAndSave(ref _zoomActive, value); }
    public bool Added { get => _added; set => SetAndSave(ref _added, value); }
    public bool Opened { get => _opened; set => SetAndSave(ref _opened, value); }
    public int Order { get => _order; set => SetAndSave(ref _order, value); }
    
    public List<UWFile> CurrentFiles => _files;
    
    public UnityAction<float> ZoomChanged;
    
    private void SetAndSave<T>(ref T field, T value)
    {
        field = value;
        Save();
    }

    private void Save()
    {
        EditorUtility.SetDirty(this);
    }
    
    public void AddFile(UWFile file, bool undo = false)
    {
        _files.Add(file);

        AssetDatabase.AddObjectToAsset(file, this);
        
        if (undo)
            Undo.RegisterCreatedObjectUndo(file, "Create file");
        
        AssetDatabase.SaveAssets();
    }
    
    public UWFile CreateFile(System.Type type)
    {
        UWFile file = CreateInstance(type) as UWFile;
        file.name = type.Name;
        
        Undo.RecordObject(this, "Create file");
        
        AddFile(file, true);
        
        return file;
    }

    public void RemoveFile(UWFile fileViewFile)
    {
        Undo.RecordObject(this, "Remove file");
        
        _files.Remove(fileViewFile);
        
        Undo.DestroyObjectImmediate(fileViewFile);
        
        AssetDatabase.SaveAssets();
    }
}