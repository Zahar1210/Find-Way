using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class UWFolder : UWFile, IResizable, ICollapsible
{
    [SerializeField] private Object[] _assets = Array.Empty<Object>();
    [SerializeField] private float _assetScale = 0.8f;
    [SerializeField] private FolderType _folderType;
    [SerializeField] private List<DirectoryContainer> _directories = new List<DirectoryContainer>();
    [SerializeField] private SortByName _sortByName = SortByName.Ascending;
    [SerializeField] private SortByType _sortByType = SortByType.Ascending;
    [SerializeField] private bool _deepSearch;
    [SerializeField] private FilterList _filter = (FilterList) (-1);
    [SerializeField] private string _searchFilter = "";
    [SerializeField] private Vector2 _scrollPosition;
    [SerializeField] private bool _directoriesFoldout = true;
    
    private Object[] _filteredAssets = Array.Empty<Object>();
    private Vector2 _inspectorAssetsScrollPosition;
    private Vector2 _inspectorDirectoriesScrollPosition;
    private static Object _newDirectory;
    
    public bool CanAddAssets => _folderType == FolderType.Custom;
    
    public Vector2 ScrollPosition { get => _scrollPosition; set => SetAndSave(ref _scrollPosition, value); }
    public string SearchFilter { get => _searchFilter; set => SetAndSave(ref _searchFilter, value); }
    public float AssetScale { get => _assetScale; set => SetAndSave(ref _assetScale, value); }

    public override Vector2 InitialSize => new Vector2(200f, 300f);
    public Vector2 MinSize => new Vector2(150f, 150f);
    public Vector2 MaxSize => new Vector2(900f, 900f);
    protected override string DefaultTitle => _folderType == FolderType.Directory && _directories.Count >= 1 ? _directories[0].Directory.name : "Folder";
    public string CollapsedInfo => $"{FilteredAssetCount} assets";
    
    public int AssetCount => _assets.Length;
    public int FilteredAssetCount => _filteredAssets.Length;
    
    public override UWFileView CreateView()
    {
        return new UWFolderView(this);
    }

    public void Initialize(Object directory)
    {
        _folderType = FolderType.Directory;
        _directories = new List<DirectoryContainer>(new[] {new DirectoryContainer(directory)});
        
        UpdateAssets();
        
        Save();
    }

    public void Initialize(Object[] assets)
    {
        _assets = assets ?? Array.Empty<Object>();
        
        UpdateAssets();
        
        Save();
    }
    
    private void UpdateFilteredAssets()
    {
        _filteredAssets = _assets.Where(a => a.name.ToLower().Contains(_searchFilter.ToLower())).ToArray();
    }
    
    protected override void DrawInspectorGUI(SerializedObject serializedObject, ref UnityAction updateCallback, ref bool needUpdate)
    {
        updateCallback += UpdateAssets;
        
        SerializedProperty folderType = serializedObject.FindProperty("_folderType");
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(folderType);
        if (EditorGUI.EndChangeCheck())
        {
            if (folderType.enumValueIndex == (int) FolderType.Directory && _assets.Length > 0)
            {
                if (!EditorUtility.DisplayDialog("Warning", "Changing folder type will remove all assets from this folder. Are you sure?", "Yes", "No"))
                {
                    folderType.enumValueIndex = (int) FolderType.Custom;
                }
            }
        }

        if (folderType.enumValueIndex == (int) FolderType.Directory)
        {
            _directoriesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_directoriesFoldout, "Directories");

            Rect dropArea = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.DragUpdated)
            {
                if (dropArea.Contains(Event.current.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Any(UWSystem.IsDirectory))
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                if (dropArea.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.AcceptDrag();
                    
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (UWSystem.IsDirectory(draggedObject))
                        {
                            _directories.Add(new DirectoryContainer(draggedObject));
                            
                            needUpdate = true;
                        }
                    }
                    
                    Event.current.Use();
                }
            }

            if (EditorGUIUtility.GetObjectPickerControlID() == 1) _directoriesFoldout = true;
            if (_directoriesFoldout)
            {
                EditorGUILayout.Space(5f);
                
                _inspectorDirectoriesScrollPosition = EditorGUILayout.BeginScrollView(_inspectorDirectoriesScrollPosition, false, false);
                for (int i = 0; i < _directories.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    Object newDirectory = _directories[i].Directory;
                    EditorGUILayout.ObjectField(_directories[i].Directory, typeof(Folder), false);

                    if (EditorGUIUtility.GetObjectPickerControlID() != 0 && EditorGUIUtility.GetObjectPickerControlID() != 1)
                        newDirectory = EditorGUIUtility.GetObjectPickerObject();
                    
                    if (newDirectory != _directories[i].Directory && UWSystem.IsDirectory(newDirectory))
                        _directories[i] = new DirectoryContainer(newDirectory);

                    if (GUILayout.Button("X", GUILayout.Width(20f)))
                    {
                        Undo.RecordObject(this, "Change folder directories");
                        
                        _directories.Remove(_directories[i]);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                
                if (GUILayout.Button("Add"))
                {
                    EditorGUIUtility.ShowObjectPicker<Folder>(null, false, "", 1);
                }

                if (EditorGUIUtility.GetObjectPickerControlID() == 1)
                {
                    Object directory = EditorGUIUtility.GetObjectPickerObject();
                    if (directory != null && directory != _newDirectory && UWSystem.IsDirectory(directory) && _directories.All(d => d.Directory != directory))
                    {
                        _newDirectory = directory;
                    }
                }
                else
                {
                    if (_newDirectory != null && Event.current.type == EventType.Repaint)
                    {
                        Undo.RecordObject(this, "Change folder directories");
                        
                        _directories.Add(new DirectoryContainer(_newDirectory));
                        _newDirectory = null;

                        _inspectorDirectoriesScrollPosition.y = float.MaxValue;
                        
                        needUpdate = true;
                    }
                }
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        EditorGUILayout.Space(10f);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_sortByName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_sortByType"));
        
        if (folderType.enumValueIndex == (int) FolderType.Directory)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_deepSearch"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_filter"));
        }
        
        if (folderType.enumValueIndex == (int) FolderType.Custom && _assets.Length > 0)
        {
            EditorGUILayout.Space(10f);
        
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            GUILayout.Label($"List ({AssetCount})", style);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(3f);
            
            EditorGUI.BeginChangeCheck();
            _searchFilter = EditorGUILayout.TextField("Search", _searchFilter);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateFilteredAssets();
            }
            
            List<Object> assetsList = _assets.ToList();
            
            EditorGUI.BeginChangeCheck();
            _inspectorAssetsScrollPosition = EditorGUILayout.BeginScrollView(_inspectorAssetsScrollPosition, false, false);
            for (int i = 0; i < _filteredAssets.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                int index = assetsList.IndexOf(_filteredAssets[i]);
                Object newAsset = EditorGUILayout.ObjectField(_filteredAssets[i], typeof(Object), false);
                if (newAsset != _filteredAssets[i] && newAsset != null && !assetsList.Contains(newAsset))
                {
                    Undo.RecordObject(this, "Change folder assets");
                    
                    _assets[index] = newAsset;
                }
                
                if (GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    Undo.RecordObject(this, "Change folder assets");
                    
                    _assets[index] = null;
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                _assets = _assets.Where(a => a != null).ToArray();
            }
        }
    }
    
    public void UpdateAssets()
    {
        UWDebug.Log("Update assets");

        if (_folderType == FolderType.Directory)
        {
            List<DirectoryContainer> directories = new List<DirectoryContainer>();
            foreach (DirectoryContainer directory in _directories)
            {
                if (directory != null && directory.Directory != null && directories.All(d => d.Directory != directory.Directory))
                    directories.Add(directory);
            }
            _directories = directories;
            
            List<Object> assets = new List<Object>();
            foreach (DirectoryContainer directory in _directories)
            {
                directory.TryNormalize();
                assets.AddRange(UWSystem.GetAssetsAtPath<Object>(directory.Path, _deepSearch, _filter));
            }
            _assets = assets.ToArray();
        }
        
        _assets = _assets.Where(a => a != null).ToArray();

        IOrderedEnumerable<Object> orderedEnumerable = null;
        switch (_sortByType)
        {
            case SortByType.None:
                if (_sortByName == SortByName.Ascending) orderedEnumerable = _assets.OrderBy(a => a.name);
                else if (_sortByName == SortByName.Descending) orderedEnumerable = _assets.OrderByDescending(a => a.name);
                orderedEnumerable = orderedEnumerable.ThenBy(a => a.GetType().Name);
                break;
            case SortByType.Ascending:
                orderedEnumerable = _assets.OrderBy(a => a.GetType().Name);
                break;
            case SortByType.Descending:
                orderedEnumerable = _assets.OrderByDescending(a => a.GetType().Name);
                break;
        }

        if (_sortByType != SortByType.None)
        {
            if (_sortByName == SortByName.Ascending) orderedEnumerable = orderedEnumerable.ThenBy(a => a.name);
            else if (_sortByName == SortByName.Descending) orderedEnumerable = orderedEnumerable.ThenByDescending(a => a.name);
        }
        
        _assets = orderedEnumerable.Distinct().ToArray();
        UpdateFilteredAssets();

        Save();
    }

    public void AddAssets(Object[] assets)
    {
        Undo.RecordObject(this, "Change folder assets");
        
        _assets = _assets.Concat(assets).Distinct().ToArray();
        UpdateAssets();
    }

    public Object GetAsset(int index)
    {
        return _assets[index];
    }
    
    public Object GetFilteredAsset(int index)
    {
        return _filteredAssets[index];
    }
    
    [Serializable]
    private class DirectoryContainer
    {
        [SerializeField] private Object _directory;
        [SerializeField] private string _path;
        
        public Object Directory => _directory;
        public string Path => _path;
        
        public DirectoryContainer(Object directory)
        {
            _directory = directory;
            _path = AssetDatabase.GetAssetPath(_directory);
        }
        
        public DirectoryContainer(string path)
        {
            _path = path;
            _directory = AssetDatabase.LoadAssetAtPath<Object>(_path);
        }
        
        public void TryNormalize()
        {
            if (_directory == null)
                _directory = AssetDatabase.LoadAssetAtPath<Object>(_path);
            else if (string.IsNullOrEmpty(_path))
                _path = AssetDatabase.GetAssetPath(_directory);
        }
    }
    
    private enum FolderType
    {
        Custom,
        Directory
    }
    
    private enum SortByName
    {
        Ascending,
        Descending
    }
    
    private enum SortByType
    {
        None,
        Ascending,
        Descending
    }

    private class Folder : Object {}
}