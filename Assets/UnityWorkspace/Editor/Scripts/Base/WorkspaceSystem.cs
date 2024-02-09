using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class WorkspaceSystem
{
    private static List<Workspace> _workspaces;
    private static List<Workspace> _addedWorkspaces;
    private static Workspace _currentWorkspace;

    private static string WorkspaceRelativePath => "/Workspaces";
    private static string WorkspacePath => AssetDatabase.GetAssetPath(UWContainer.Instance.Root) + WorkspaceRelativePath;
    
    static WorkspaceSystem()
    {
        InitializeWorkspaces();
        TryAddWorkspacesToGitignore();
    }
    
    private static void InitializeWorkspaces()
    {
        _workspaces = UWSystem.GetAssetsAtPath<Workspace>(WorkspacePath).ToList();
        
        _addedWorkspaces = _workspaces.Where(workspace => workspace.Added).OrderBy(workspace => workspace.Order).ToList();
        
        _currentWorkspace = _addedWorkspaces.FirstOrDefault(w => w.Opened);
    }
    
    private static void TryAddWorkspacesToGitignore()
    {
        try
        {
            string gitignorePath = ".gitignore";
            if (!File.Exists(gitignorePath))
            {
                File.Create(gitignorePath).Close();
                UWDebug.Log(".gitignore file was created");
            }
            
            string[] lines = File.ReadAllLines(gitignorePath);
            if (!Array.Exists(lines, line => line.Trim() == WorkspacePath))
            {
                File.AppendAllText(gitignorePath, WorkspacePath + Environment.NewLine);
                UWDebug.Log($"File {WorkspacePath} was added to .gitignore.");
            }
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(WorkspaceRelativePath) && lines[i].Trim() != WorkspacePath)
                {
                    lines[i] = "";
                }
            }
        }
        catch (Exception ex)
        {
            UWDebug.Log($"Error: {ex.Message}");
        }
    }
    
    private static Workspace CreateWorkspaceFile()
    {
        if (!Directory.Exists(WorkspacePath))
            Directory.CreateDirectory(WorkspacePath);
        
        Workspace workspace = ScriptableObject.CreateInstance<Workspace>();

        _workspaces.Add(workspace);
        
        AssetDatabase.CreateAsset(workspace, WorkspacePath + "/" + GetNewWorkspaceName() + ".asset");
        AssetDatabase.SaveAssets();
        
        UWDebug.Log("Workspace created");
        
        return workspace;
    }
    
    private static string GetNewWorkspaceName()
    {
        string workspaceName = "Workspace_";
        int number = 1;
        while (_workspaces.Any(w => w.name == workspaceName + number))
            number++;
        workspaceName += number;
        return workspaceName;
    }
    
    public static void CreateWorkspace()
    {
        Workspace workspace = CreateWorkspaceFile();
        AddWorkspace(workspace);
        UWWindow.UpdateGUI();
        
        UWWindow.WorkspaceView.StartRenaming(workspace);
    }
    
    public static void DeleteWorkspace(Workspace workspace)
    {
        bool delete = EditorUtility.DisplayDialog("Delete workspace", "Are you sure you want to delete this workspace?", "Delete", "Cancel");
        if (!delete) return;
        
        _workspaces.Remove(workspace);
        RemoveWorkspace(workspace);
        
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(workspace));
        AssetDatabase.SaveAssets();
    }
    
    public static void AddWorkspace(Workspace workspace)
    {
        _addedWorkspaces.Add(workspace);
        
        workspace.Added = true;
        
        OpenWorkspace(workspace);
    }
    
    public static void RemoveWorkspace(Workspace workspace)
    {
        _addedWorkspaces.Remove(workspace);
        
        workspace.Added = false;

        if (workspace.Opened)
        {
            if (_addedWorkspaces.Count > 0)
            {
                OpenWorkspace(_addedWorkspaces[0]);
            }
            else
            {
                _currentWorkspace = null;
                
                UWWindow.UpdateGUI();
            }
        }
        else
        {
            UWWindow.WorkspaceView.UpdateTabs();
        }
    }
    
    public static void OpenWorkspace(Workspace workspace)
    {
        if (_currentWorkspace == workspace) return;
        if (!workspace.Added) AddWorkspace(workspace);
        
        if (_currentWorkspace != null)
            _currentWorkspace.Opened = false;
        
        workspace.Opened = true;
        workspace.Order = _addedWorkspaces.IndexOf(workspace);
        
        _currentWorkspace = workspace;
        
        UWWindow.UpdateGUI();
    }

    public static Workspace GetCurrentWorkspace()
    {
        return _currentWorkspace;
    }
    
    public static Workspace[] GetOpenedWorkspaces()
    {
        return _addedWorkspaces.ToArray();
    }
    
    public static Workspace[] GetClosedWorkspaces()
    {
        return _workspaces.Where(workspace => !_addedWorkspaces.Contains(workspace)).ToArray();
    }

    public static Workspace[] GetAllWorkspaces()
    {
        return _workspaces.ToArray();
    }
    
    private static async void SelectFiles(UWFile[] files)
    {
        await Task.Delay(1);

        UWWindow.WorkspaceView.Graph.SelectFiles(files);
    }
    
    private static void MoveFiles(UWFile[] files, Workspace to, bool copy)
    {
        UWFile[] newFiles = new UWFile[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            newFiles[i] = Object.Instantiate(files[i]);
            to.AddFile(newFiles[i]);

            if (!copy)
                UWWindow.WorkspaceView.Workspace.RemoveFile(files[i]);
        }

        OpenWorkspace(to);
        
        UWWindow.UpdateGUI();
        
        SelectFiles(newFiles);
    }

    public static void MoveFiles(UWFile[] files, Workspace to)
    {
        MoveFiles(files, to, false);
    }
    
    public static void CopyFiles(UWFile[] files, Workspace to)
    {
        MoveFiles(files, to, true);
    }
    
    public static void ShiftWorkspace(Workspace workspace, bool right)
    {
        int index = _addedWorkspaces.IndexOf(workspace);
        if (index == -1) return;
        
        int newIndex = index + (right ? 1 : -1);
        if (newIndex < 0 || newIndex >= _addedWorkspaces.Count) return;
        
        _addedWorkspaces[index] = _addedWorkspaces[newIndex];
        _addedWorkspaces[newIndex] = workspace;
        
        workspace.Order = newIndex;
        _addedWorkspaces[index].Order = index;
        
        UWWindow.WorkspaceView.UpdateTabs();
    }
    
    private static string GetFreeName(string name, string directory)
    {
        string extension = Path.GetExtension(name);
        name = Path.GetFileNameWithoutExtension(name);
        
        bool isDirectory = extension == "";
        
        string currentName = name + extension;
        int number = 1;
        while (isDirectory ? Directory.Exists(directory + "/" + currentName) : File.Exists(directory + "/" + currentName))
        {
            currentName = name + " (" + number + ")" + extension;
            
            number++;
        }
        
        return currentName;
    }
    
    private static void SaveWorkspace(Workspace workspace, string directoryPath)
    {
        string workspacePath = directoryPath + "/" + GetFreeName(workspace.name + ".asset", directoryPath);
        File.Copy(AssetDatabase.GetAssetPath(workspace), workspacePath, true);
    }
    
    public static void SaveAllWorkspaces()
    {
        string directoryPath = EditorUtility.OpenFolderPanel("Save workspace", "", "");
        if (string.IsNullOrEmpty(directoryPath)) return;
        
        directoryPath += "/" + GetFreeName("Workspaces", directoryPath);
        
        Directory.CreateDirectory(directoryPath);

        foreach (Workspace workspace in _workspaces)
        {
            SaveWorkspace(workspace, directoryPath);
        }
    }

    public static void SaveWorkspace(Workspace workspace)
    {
        string directoryPath = EditorUtility.OpenFolderPanel("Save workspace", "", workspace.name);
        if (string.IsNullOrEmpty(directoryPath)) return;
        
        SaveWorkspace(workspace, directoryPath);
    }
    
    private static void LoadWorkspace(string workspacePath)
    {
        string workspaceName = GetFreeName(Path.GetFileName(workspacePath), WorkspacePath);
        string path = WorkspacePath + "/" + workspaceName;
        File.Copy(workspacePath, path, true);
        
        AssetDatabase.Refresh();
        
        Workspace workspace = AssetDatabase.LoadAssetAtPath<Workspace>(path);
        workspace.name = workspaceName;
        
        AddWorkspace(workspace);
    }
    
    public static void LoadWorkspaces()
    {
        string directoryPath = EditorUtility.OpenFolderPanel("Load workspace", "", "");
        if (string.IsNullOrEmpty(directoryPath)) return;
        
        string[] workspacePaths = Directory.GetFiles(directoryPath, "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string workspacePath in workspacePaths)
        {
            LoadWorkspace(workspacePath);
        }
        
        AssetDatabase.Refresh();
        
        UWWindow.UpdateGUI();
    }
    
    public static void LoadWorkspace()
    {
        string workspacePath = EditorUtility.OpenFilePanel("Load workspace", "", "asset");
        if (string.IsNullOrEmpty(workspacePath)) return;
        
        LoadWorkspace(workspacePath);
        
        UWWindow.UpdateGUI();
    }
}