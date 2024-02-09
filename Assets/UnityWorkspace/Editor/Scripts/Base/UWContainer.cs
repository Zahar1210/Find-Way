using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UWContainer : ScriptableObject
{
    [SerializeField] private Object _root;
    [Space(20f)]
    [SerializeField] private VisualTreeAsset _initializationScreenUXML;
    [SerializeField] private VisualTreeAsset _workspaceUXML;
    [SerializeField] private VisualTreeAsset _workspaceTabUXML;
    [SerializeField] private VisualTreeAsset _assetUXML;
    [SerializeField] private VisualTreeAsset _customUXML;
    [SerializeField] private VisualTreeAsset _folderUXML;
    [SerializeField] private VisualTreeAsset _folderAssetContainerUXML;
    [SerializeField] private VisualTreeAsset _folderAssetContainerLineUXML;
    [Space(20f)]
    [SerializeField] private VisualTreeAsset[] _assetStyles;
    [Space(20f)]
    [SerializeField] private StyleSheet _folderCollapse;
    [SerializeField] private StyleSheet _collapseButton;
    
    public Object Root => _root;
    public VisualTreeAsset InitializationScreenUXML => _initializationScreenUXML;
    public VisualTreeAsset WorkspaceUXML => _workspaceUXML;
    public VisualTreeAsset WorkspaceTabUXML => _workspaceTabUXML;
    public VisualTreeAsset AssetUXML => _assetUXML;
    public VisualTreeAsset CustomUXML => _customUXML;
    public VisualTreeAsset FolderUXML => _folderUXML;
    public VisualTreeAsset FolderAssetContainerUXML => _folderAssetContainerUXML;
    public VisualTreeAsset FolderAssetContainerLineUXML => _folderAssetContainerLineUXML;
    public VisualTreeAsset[] AssetStyles => _assetStyles;
    public StyleSheet FolderCollapse => _folderCollapse;
    public StyleSheet CollapseButton => _collapseButton;
    
    private static UWContainer _instance;

    public static UWContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = AssetDatabase.LoadAssetAtPath<UWContainer>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:" + typeof(UWContainer))[0]));
            }

            return _instance;
        }
    }
}