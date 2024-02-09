using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public abstract class UWFile : ScriptableObject
{
    [SerializeField] private string _title;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _size;
    [SerializeField] private bool _collapsed;

    public Vector3 Position { get => _position; set => SetAndSave(ref _position, value); }
    public Vector3 Size { get => _size; set => SetAndSave(ref _size, value); }

    public bool Collapsed { get => _collapsed; set => SetAndSave(ref _collapsed, value); }

    protected abstract string DefaultTitle { get; }
    
    public string Title => string.IsNullOrEmpty(_title) ? DefaultTitle : _title;

    public abstract Vector2 InitialSize { get; }

    public abstract UWFileView CreateView();
    
    protected void SetAndSave<T>(ref T field, T value)
    {
        field = value;
        Save();
    }

    public void Save()
    {
        EditorUtility.SetDirty(this);
    }
    
    private void DrawBaseInspectorGUI()
    {
        _title = EditorGUILayout.TextField("Title", _title);
        EditorGUILayout.Space(10f);
    }
    
    protected virtual void DrawInspectorGUI(SerializedObject serializedObject, ref UnityAction updateCallback, ref bool needUpdate) { }

    public void DrawInspectorGUI(float labelWidth, ref UnityAction updateCallback, ref bool needUpdate)
    {
        updateCallback += Save;
        
        SerializedObject serializedObject = new SerializedObject(this);
        
        float lastLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = labelWidth;
        
        DrawBaseInspectorGUI();
        DrawInspectorGUI(serializedObject, ref updateCallback, ref needUpdate);
        
        EditorGUIUtility.labelWidth = lastLabelWidth;
        
        serializedObject.ApplyModifiedProperties();
    }
}
