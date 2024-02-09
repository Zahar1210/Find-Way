using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public static class UWSystem
{
    public static T[] GetAssetsAtPath<T>(string path, bool deepSearch = false) where T : Object
    {
        List<T> assets = new List<T>();
        
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFileSystemEntries(path, "*", deepSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (string assetPath in files)
            {
                if (assetPath.Contains(".meta"))
                    continue;
                
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) 
                    assets.Add(asset);
            }
        }

        return assets.ToArray();
    }
    
    public static T[] GetAssetsAtPath<T>(string path, bool deepSearch, FilterList whiteList) where T : Object
    {
        List<T> assets = new List<T>();
        T[] allAssets = GetAssetsAtPath<T>(path, deepSearch);
        
        if ((int) whiteList == 0) 
            return assets.ToArray();
        
        foreach (T asset in allAssets)
        {
            if (asset.name.Contains(".meta"))
                continue;
            
            if (asset == null)
                continue;

            if (CheckMatchAsset(asset, whiteList))
            {
                assets.Add(asset);
            }
        }

        return assets.ToArray();
    }
    
    private static bool CheckMatchAsset(Object asset, FilterList filterList)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        string assetExtension = Path.GetExtension(path);
        
        if (filterList.HasFlag(FilterList.Folder) && IsDirectory(asset)) return true;
        if (filterList.HasFlag(FilterList.Scene) && assetExtension.Contains(".unity")) return true;
        if (filterList.HasFlag(FilterList.Prefab) && asset is GameObject) return true;
        if (filterList.HasFlag(FilterList.ScriptableObject) && assetExtension.Contains(".asset")) return true;
        if (filterList.HasFlag(FilterList.Material) && asset is Material) return true;
        if (filterList.HasFlag(FilterList.Texture) && asset is Texture) return true;
        if (filterList.HasFlag(FilterList.Script) && assetExtension.Contains(".cs")) return true;
        if (filterList.HasFlag(FilterList.Animation) && asset is AnimationClip) return true;
        if (filterList.HasFlag(FilterList.Animator) && asset is RuntimeAnimatorController) return true;
        if (filterList.HasFlag(FilterList.Audio) && asset is AudioClip) return true;

        return false;
    }
    
    public static bool IsDirectory(Object asset)
    {
        return AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(asset));
    }
    
    private static void OpenFolder(int instanceId)
    {
        Type pt = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
        object ins = pt.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public).GetValue(null);
        MethodInfo method = pt.GetMethod("ShowFolderContents", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(ins, new object[] {instanceId, true});
    }
    
    public static void StartDrag(IAssetContainer assetContainer)
    {
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.objectReferences = new[] { assetContainer.Asset };
        DragAndDrop.StartDrag(assetContainer.DragTitle);
        DragAndDrop.SetGenericData("UW", assetContainer.FileView);
    }
    
    public static void SelectAsset(Object asset)
    {
        Selection.activeObject = asset;
    }
    
    public static void OpenAsset(Object asset)
    {
        if (IsDirectory(asset))
        {
            OpenFolder(asset.GetInstanceID());
        }
        else if (asset is GameObject)
        {
            AssetDatabase.OpenAsset(asset);
        }
        else if (asset is SceneAsset)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(asset));
        }
        else
        {
            ShowAsset(asset);
        }
    }
    
    public static void ShowAsset(Object asset)
    {
        EditorUtility.OpenPropertyEditor(asset);
    }
}