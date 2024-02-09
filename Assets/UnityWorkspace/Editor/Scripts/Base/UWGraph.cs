using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class UWGraph : GraphView
{
    public new class UxmlFactory : UxmlFactory<UWGraph, UxmlTraits> { }

    private Workspace _workspace;
    
    private ContentZoomer _contentZoomer;

    private Vector3 _lastPosition;
    private Vector3 _savedPosition;
    private bool _isFileMovement;
    private bool _isDragAndDropActive;

    public bool IsFileMovement => _isFileMovement;
    
    public readonly float MinZoom = 0.4f;
    public readonly float MaxZoom = 1.5f;

    public static UnityAction<MouseDownEvent> MouseDownEventGlobal;
    public static UnityAction<MouseUpEvent> MouseUpEventGlobal;
    public static UnityAction<MouseMoveEvent> MouseMoveEventGlobal;

    private readonly float _creationOffset = 30f;
    
    public UWGraph()
    {
#if UW_DEBUG
        Insert(0, new GridBackground());
#endif

        ResetEvents();
        AddManipulators();

        RegisterCallbacks();
        
        UWFileView.MouseDown += OnFileMouseDown;
    }
    
    public void UpdateWorkspace(Workspace workspace)
    {
        _workspace = workspace;
        
        SetZoomActive(_workspace.ZoomActive);
        
        PopulateView();
        
        viewTransform.position = _workspace.Position;
        viewTransform.scale = _workspace.Zoom;
    }
    
    private void ResetEvents()
    {
        UWFileView.MouseDown = null;
        UWFileView.Selected = null;
        UWFileView.Unselected = null;
    }
    
    private void AddManipulators()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    private void RegisterCallbacks()
    {
        RegisterCallback<KeyDownEvent>(OnKeyDown);
        
        RegisterCallback<MouseDownEvent>(OnMouseDownEvent, TrickleDown.TrickleDown);
        RegisterCallback<MouseUpEvent>(OnMouseUpEvent, TrickleDown.TrickleDown);
        RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent, TrickleDown.TrickleDown);
        
        RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
        RegisterCallback<DragPerformEvent>(OnDragPerform);
    }

    public void Update()
    {
        TryUpdateWorkspace();
        FixOffset();
    }

    public void OnGUI()
    {
        FixDragAndDrop();
    }

    private void TryUpdateWorkspace()
    {
        if (_workspace == null) return;
        
        if (_workspace.Position != viewTransform.position) 
            _workspace.Position = viewTransform.position;

        if (_workspace.Zoom != viewTransform.scale)
        {
            _workspace.Zoom = viewTransform.scale;

            _workspace.ZoomChanged?.Invoke(_workspace.Zoom.x);
        }
    }
    
    private void FixOffset()
    {
        if (viewTransform.position != _lastPosition && _isFileMovement)
        {
            _lastPosition = viewTransform.position;
            
            foreach (UWFileView element in graphElements)
            {
                if (!element.selected) element.UpdatePosition();
            }
            
            UWDebug.Log("fix");
        }
    }

    private void FixDragAndDrop()
    {
        bool isDragAndDropActive = DragAndDrop.objectReferences.Length > 0;
        if (_isDragAndDropActive != isDragAndDropActive)
        {
            _isDragAndDropActive = isDragAndDropActive;
            
            foreach (UWFileView element in graphElements)
            {
                element.SetDragAndDropActive(isDragAndDropActive);
            }
        }
    }
    
    private void OnFileMouseDown(UWFileView fileView, MouseDownEvent evt)
    {
        if (evt.button == (int) MouseButton.LeftMouse)
            StartFileMovement();
    }
    
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.R)
        {
            UWWindow.UpdateGUI();
        }
        
        if (evt.keyCode == KeyCode.F)
        {
            evt.StopImmediatePropagation();
            
            Frame();
        }
    }
    
    private void OnDragUpdated(DragUpdatedEvent evt)
    {
        object data = DragAndDrop.GetGenericData("UW");
        if (data is UWAssetView) return;
        if (evt.target != this) return;
        if (DragAndDrop.objectReferences.Length == 0) return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
    }
    
    private void OnDragPerform(DragPerformEvent evt)
    {
        object data = DragAndDrop.GetGenericData("UW");
        if (data is UWAssetView) return;
        
        if (DragAndDrop.objectReferences.Length > 0 && evt.target == this)
        {
            DropFiles(DragAndDrop.objectReferences, ConvertMousePositionToLocal(evt.localMousePosition));
        
            DragAndDrop.AcceptDrag();
        }
    }
    
    private void OnMouseDownEvent(MouseDownEvent evt)
    {
        if ((evt.pressedButtons & (1 << (int) MouseButton.LeftMouse)) != 0 && evt.button != (int) MouseButton.LeftMouse)
        {
            UWDebug.Log("Mouse is already pressed");
            evt.StopImmediatePropagation();
        }

        MouseDownEventGlobal?.Invoke(evt);
    }
    
    private void OnMouseUpEvent(MouseUpEvent evt)
    {
        MouseUpEventGlobal?.Invoke(evt);

        if (evt.button == (int) MouseButton.LeftMouse)
        {
            EndFileMovement();
        }
    }
    
    private void OnMouseMoveEvent(MouseMoveEvent evt)
    {
        MouseMoveEventGlobal?.Invoke(evt);
    }
    
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            RemoveFiles(graphViewChange.elementsToRemove.Select(e => e as UWFileView).ToArray(), false);
        }
        
        return graphViewChange;
    }
    
    private void StartFileMovement()
    {
        _isFileMovement = true;
        
        foreach (UWFileView element in graphElements)
        {
            if (!element.selected) element.SaveOffset();
        }
        
        _savedPosition = viewTransform.position;
    }
    
    private void EndFileMovement()
    {
        if (!_isFileMovement) return;
        
        _isFileMovement = false;
        
        foreach (UWFileView element in graphElements)
        {
            Rect rect = element.GetPosition();
            rect.position += (Vector2) (viewTransform.position - _savedPosition) / viewTransform.scale.x;
            element.SetPosition(rect);
        }
        
        viewTransform.position = _savedPosition;
    }

    private void Restart()
    {
        UpdateWorkspace(_workspace);
    }

    public void Frame()
    {
        float offset = 47f;
        
        Rect rect = layout;
        rect.size -= Vector2.up * offset;
        
        CalculateFrameTransform(CalculateRectToFitAll(contentViewContainer), rect, 10, out Vector3 frameTranslation, out Vector3 frameScaling);
        UpdateViewTransform(frameTranslation + Vector3.up * offset, frameScaling);
    }
    
    private UWFileView CreateFile(Type type, Vector2 position)
    {
        UWFile file = _workspace.CreateFile(type);
        file.Position = position;
        
        return CreateFileView(file);
    }
    
    private UWFileView CreateFileView(UWFile file)
    {
        UWFileView fileView = file.CreateView();
        fileView.SetGraphView(this);
        
        AddElement(fileView);
        
        return fileView;
    }
    
    private void DropFiles(Object[] objects, Vector2 position)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            PrefabAssetType type = PrefabUtility.GetPrefabAssetType(objects[i]);
            if (objects[i] is GameObject && type != PrefabAssetType.Regular && type != PrefabAssetType.Variant)
            {
                objects[i] = PrefabUtility.GetCorrespondingObjectFromSource(objects[i]);
                continue;
            }
            
            if (!AssetDatabase.Contains(objects[i]))
                objects[i] = null;
        }

        objects = objects.Where(o => o != null).Distinct().ToArray();

        if (objects.Length > 1)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add folder"), false, () => { AddFiles(objects, position, true); });
            menu.AddItem(new GUIContent($"Add assets ({objects.Length})"), false, () => { AddFiles(objects, position); });
            menu.ShowAsContext();
        }
        else if (objects.Length == 1 && UWSystem.IsDirectory(objects[0]))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add folder"), false, () => { AddFiles(objects, position, true, objects[0]); });
            menu.AddItem(new GUIContent("Add asset"), false, () => { AddFiles(objects, position); });
            menu.ShowAsContext();
        }
        else
        {
            AddFiles(objects, position);
        }
    }

    private void AddFiles(Object[] objects, Vector2 position, bool isFolder = false, Object directory = null)
    {
        if (isFolder)
        {
            CreateFileFolder(position, objects, directory);
        }
        else
        {
            for (int i = 0; i < objects.Length; i++)
            {
                CreateFileAsset(position + Vector2.one * _creationOffset * i, objects[i]);
            }
        }
    }
    
    private void CreateFileFolder(Vector2 position, Object[] objects = null, Object directory = null)
    {
        UWFileView fileView = CreateFile(typeof(UWFolder), position);
        UWFolder folder = fileView.File as UWFolder;
        
        if (directory) folder.Initialize(directory);
        else folder.Initialize(objects);
        
        fileView.UpdateFile();
    }

    private void CreateFileAsset(Vector2 position, Object obj)
    {
        UWFileView fileView = CreateFile(typeof(UWAsset), position);
        UWAsset asset = fileView.File as UWAsset;
        asset.Initialize(obj);
        fileView.UpdateFile();
    }
    
    private void PopulateView()
    {
        UWDebug.Log("Populate View");
        
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        foreach (UWFile file in _workspace.CurrentFiles)
        {
            CreateFileView(file);
        }
    }

    public void RemoveFiles(UWFileView[] fileViews, bool removeView = true)
    {
        foreach (UWFileView fileView in fileViews)
        {
            _workspace.RemoveFile(fileView.File);
            if (removeView)
                RemoveElement(fileView);
        }
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        
        if (evt.target != this)
            return;
        
        Vector2 mousePosition = ConvertMousePositionToLocal(evt.localMousePosition);
        
        // evt.menu.ClearItems();
        AddDefaultFiles(evt, mousePosition);
        AddCustomFiles(evt, mousePosition);
    }
    
    private void AddDefaultFiles(ContextualMenuPopulateEvent evt, Vector2 mousePosition)
    {
        evt.menu.AppendAction("Asset", _ => { CreateFile(typeof(UWAsset), mousePosition); });
        evt.menu.AppendAction("Folder", _ => { CreateFileFolder(mousePosition); });
        evt.menu.AppendAction("Note", _ => { CreateFile(typeof(UWNote), mousePosition); });
    }
    
    private void AddCustomFiles(ContextualMenuPopulateEvent evt, Vector2 mousePosition)
    {
        List<Type> ignoredTypes = new List<Type>
        {
            typeof(UWAsset),
            typeof(UWFolder),
            typeof(UWNote)
        };
        
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(domainAssembly => domainAssembly.GetTypes())
            .Where(type => typeof(UWFile).IsAssignableFrom(type) && !type.IsAbstract && !ignoredTypes.Contains(type))
            .ToArray();
        
        foreach (Type type in types)
        {
            evt.menu.AppendAction("Custom/ " + type, _ => { CreateFile(type, mousePosition); });
        }
    }
    
    private Vector2 ConvertMousePositionToLocal(Vector2 mousePosition)
    {
        return viewTransform.matrix.inverse.MultiplyPoint(mousePosition);
    }
    
    public void UpdateZoom(float zoom)
    {
        float currentScale = viewTransform.scale.x;
        Vector3 graphSize = contentRect.size;
            
        Vector3 scale = zoom * Vector3.one;
        Vector3 offset = graphSize / scale.x - graphSize / currentScale;
        Vector3 position = viewTransform.position / currentScale * scale.x + offset * scale.x / 2f;
            
        UpdateViewTransform(position, scale);
        _workspace.Position = position;
        _workspace.Zoom = scale;
    }

    public void SetZoomActive(bool active)
    {
        if (active)
        {
            if (_contentZoomer == null)
            {
                _contentZoomer = new ContentZoomer();
                _contentZoomer.minScale = MinZoom;
                _contentZoomer.maxScale = MaxZoom;
                _contentZoomer.scaleStep = 0.05f;
            }
            this.AddManipulator(_contentZoomer);
        }
        else
        {
            this.RemoveManipulator(_contentZoomer);
        }
    }

    public void SelectFiles(UWFile[] newFiles)
    {
        ClearSelection();
        
        foreach (UWFileView fileView in graphElements)
        {
            if (newFiles.Contains(fileView.File))
                AddToSelection(fileView);
        }
    }
}