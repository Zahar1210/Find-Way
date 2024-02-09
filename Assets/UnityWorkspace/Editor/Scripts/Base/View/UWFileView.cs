using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class UWFileView<T> : UWFileView where T : UWFile
{
    protected new T File => base.File as T;

    protected UWFileView(T file) : base(file)
    {
        
    }
}

public abstract class UWFileView : GraphElement, ICollectibleElement
{
    public UWFile File { get; protected set; }

    private UWGraph GraphView;
    
    protected VisualElement Panel;
    private Label _title;
    private ResizableElement _resizableElement;
    private UWCollapseButton _collapseButton;
    private Label _collapsedInfo;
    private VisualElement _dropIndicator;

    protected bool PanelHover;
    private bool _layoutInitialized;
    
    private Vector3 _offset;
    
    private bool IsSizeInitialized => File.Size != Vector3.zero;

    protected virtual Vector2 InitialSize => File.InitialSize;
    public static UnityAction<UWFileView, MouseDownEvent> MouseDown;
    
    public static UnityAction<UWFileView> Selected;
    public static UnityAction<UWFileView> Unselected;

    protected UWFileView(UWFile file)
    {
        File = file;
        
        LoadUXML();
        InitializeComponents();
        AddManipulators();
        InitializeCapabilities();
        RegisterCallbacks();
        TrySetUpResizableElement();
        OnCollapseValueChanged(File.Collapsed);
    }
    
    protected void LoadUXML(VisualTreeAsset uxml)
    {
        uxml.CloneTree(this);
    }

    protected abstract void LoadUXML();
    
    protected virtual void InitializeComponents()
    {
        Panel = this.Q("Panel");
        _title = this.Q<Label>("Title");
        
        _resizableElement = this.Q<ResizableElement>();
        
        _collapseButton = this.Q<UWCollapseButton>("Collapse");
        if (_collapseButton != null)
        {
            _collapseButton.IsCollapsed = File.Collapsed;
            if (File is not ICollapsible)
                _collapseButton.style.display = DisplayStyle.None;
        }
        
        _dropIndicator = this.Q("DropIndicator");
        UpdateDragAndDropIndicator(false);
        
        _collapsedInfo = this.Q<Label>("CollapsedInfo");
    }

    private void InitializeCapabilities()
    {
        capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable |
                        Capabilities.Ascendable | Capabilities.Snappable;
    }

    protected virtual void AddManipulators()
    {
        this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
    }
    
    protected virtual void RegisterCallbacks()
    {
        RegisterCallback<GeometryChangedEvent>(InitializeLayout);
        
        RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
        RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
        
        RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
        
        RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
        RegisterCallback<DragPerformEvent>(OnDragPerform);
        RegisterCallback<DragExitedEvent>(OnDragExited);
        
        Panel.RegisterCallback<GeometryChangedEvent>(OnPanelGeometryChanged);

        if (_resizableElement != null)
        {
            _resizableElement.RegisterCallback<GeometryChangedEvent>(_ => OnSizeChanged());
            foreach (VisualElement child in _resizableElement.Query().ToList())
                child.RegisterCallback<MouseUpEvent>(_ => ResizeComplete());
        }
        
        if (_collapseButton != null)
            _collapseButton.ValueChanged += OnCollapseValueChanged;
    }
    
    protected virtual void InitializeLayout(GeometryChangedEvent evt)
    {
        UnregisterCallback<GeometryChangedEvent>(InitializeLayout);
        
        _layoutInitialized = true;

        Initialize();
    }

    protected virtual void Initialize()
    {
        if (!IsSizeInitialized)
        {
            File.Size = InitialSize;
            File.Position -= File.Size / 2f;
        }

        UpdateFile();
    }

    protected virtual void OnMouseDownEvent(MouseDownEvent evt)
    {
        MouseDown?.Invoke(this, evt);
    }
    
    protected virtual void OnMouseUpEvent(MouseUpEvent evt)
    {
        
    }

    protected virtual void OnMouseEnterEvent(MouseEnterEvent evt)
    {
        PanelHover = true;
        UpdateHover();
        
        UpdateDragAndDropIndicator(true);
    }
    
    protected virtual void OnMouseLeaveEvent(MouseLeaveEvent evt)
    {
        PanelHover = false;
        UpdateHover();
        
        UpdateDragAndDropIndicator(false);
    }
    
    protected virtual void OnDragUpdated(DragUpdatedEvent evt)
    {
        
    }
    
    protected virtual void OnDragPerform(DragPerformEvent evt)
    {
        UpdateDragAndDropIndicator(false);
    }
    
    protected virtual void OnDragExited(DragExitedEvent evt)
    {
        UpdateDragAndDropIndicator(false);
    }
    
    private void UpdateDragAndDropIndicator(bool active)
    {
        if (_dropIndicator != null)
            _dropIndicator.style.display = active && CanAddDragAndDropAssets() ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    protected virtual bool CanAddDragAndDropAssets()
    {
        return false;
    }
    
    private void UpdateLayoutByPanel()
    {
        if (!File.Collapsed) return;

        Rect rect = new Rect(layout.position, Panel.layout.size);
        base.SetPosition(rect);
        _resizableElement.parent.style.width = rect.width;
        _resizableElement.parent.style.height = rect.height;
    }
    
    private void OnPanelGeometryChanged(GeometryChangedEvent evt)
    {
        UpdateLayoutByPanel();
    }
    
    protected virtual void OnSizeChanged()
    {
        if (!IsSizeInitialized) return;
        if (File.Collapsed) return;
        
        File.Size = new Vector2(_resizableElement.parent.layout.width, _resizableElement.parent.layout.height);
    }
    
    protected virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        UWFileView[] filesView;
        if (GraphView.selection.Count > 0)
            filesView = selected ? GraphView.selection.Select(selectable => (selectable as UWFileView)).ToArray() : Array.Empty<UWFileView>();
        else
            filesView = new[] {this};
        UWFile[] files = filesView.Select(fileView => fileView.File).ToArray();

        if (files.Length > 0)
        {
            string move = files.Length > 1 ? $"Move ({files.Length}) to/" : "Move to/";
            string copy = files.Length > 1 ? $"Copy ({files.Length}) to/" : "Copy to/";
            foreach (Workspace workspace in WorkspaceSystem.GetAllWorkspaces())
            {
                if (workspace == WorkspaceSystem.GetCurrentWorkspace()) continue;
                
                evt.menu.AppendAction(move + workspace.name, _ => WorkspaceSystem.MoveFiles(files, workspace), DropdownMenuAction.AlwaysEnabled);
                evt.menu.AppendAction(copy + workspace.name, _ => WorkspaceSystem.CopyFiles(files, workspace), DropdownMenuAction.AlwaysEnabled);
            }
        }
        
        string delete = files.Length > 1 ? $"Delete ({files.Length})" : "Delete";
        evt.menu.AppendAction(delete, _ => GraphView.RemoveFiles(filesView), DropdownMenuAction.AlwaysEnabled);
    }

    private void EnableResizableElement()
    {
        if (_resizableElement == null) return;
        if (_resizableElement.style.display == DisplayStyle.Flex) return;

        _resizableElement.style.display = DisplayStyle.Flex;
    }

    private void DisableResizableElement()
    {
        if (_resizableElement == null) return;
        if (_resizableElement.style.display == DisplayStyle.None) return;
        
        _resizableElement.style.display = DisplayStyle.None;
    }
    
    public virtual void UpdateFile()
    {
        UpdateTitle();
        UpdateLayout();
        UpdateCollapsedInfo();
        TrySetUpResizableElement();
    }

    private void UpdateTitle()
    {
        _title.text = File.Title;
    }

    private void UpdateLayout()
    {
        Rect rect = new Rect(File.Position.x, File.Position.y, File.Size.x, File.Size.y);
        if (File.Collapsed)
        {
            rect.size = Panel.layout.size;
        }
        
        base.SetPosition(rect);
    }
    
    private void UpdateCollapsedInfo()
    {
        if (_collapsedInfo != null && File is ICollapsible collapsible)
        {
            _collapsedInfo.text = collapsible.CollapsedInfo;
        }
    }
    
    private void UpdateResizableElementLayout()
    {
        if (_resizableElement == null) return;
        
        _resizableElement.parent.style.width = File.Size.x;
        _resizableElement.parent.style.height = File.Size.y;
    }

    protected void UpdateHover()
    {
        UpdateHover(PanelHover);
    }
    
    protected virtual void UpdateHover(bool panelHover)
    {
        if (panelHover) Panel.AddToClassList("panel-hover");
        else Panel.RemoveFromClassList("panel-hover");
    }
    
    private void TrySetUpResizableElement()
    {
        if (_resizableElement == null) return;
        if (!IsSizeInitialized) return;

        _resizableElement.parent.style.width = File.Size.x;
        _resizableElement.parent.style.height = File.Size.y;
        
        if (File is IResizable resizablePanel)
        {
            _resizableElement.parent.style.minWidth = resizablePanel.MinSize.x;
            _resizableElement.parent.style.minHeight = resizablePanel.MinSize.y;
                
            _resizableElement.parent.style.maxWidth = resizablePanel.MaxSize.x;
            _resizableElement.parent.style.maxHeight = resizablePanel.MaxSize.y;
        }
        else
        {
            _resizableElement.style.display = DisplayStyle.None;
        }
    }
    
    private void ResizeComplete()
    {
        if (_resizableElement == null) return;
        if (!IsSizeInitialized) return;
        
        SetPosition(new Rect(layout.position + _resizableElement.parent.layout.position - Vector2.one, _resizableElement.parent.layout.size));
        _resizableElement.parent.style.left = 0f;
        _resizableElement.parent.style.top = 0f;

        UWDebug.Log("Resize Complete");
    }
    
    protected virtual void OnCollapseValueChanged(bool collapsed)
    {
        bool isSwitching = File.Collapsed != collapsed;
        File.Collapsed = collapsed;

        if (File.Collapsed)
        {
            styleSheets.Add(UWContainer.Instance.FolderCollapse);
            
            DisableResizableElement();
            
            if (!isSwitching)
                Panel.RegisterCallback<GeometryChangedEvent>(UpdateLayoutByPanelEvent);
        }
        else
        {
            UpdateLayout();
            UpdateResizableElementLayout();
            EnableResizableElement();
            
            if (isSwitching) RegisterCallback<GeometryChangedEvent>(RemoveStyleFolderCollapse);
            else styleSheets.Remove(UWContainer.Instance.FolderCollapse);
        }
        
        if (isSwitching && collapsed)
            UpdateCollapsedInfo();

        return;
        
        void UpdateLayoutByPanelEvent(GeometryChangedEvent evt)
        {
            RegisterCallback<GeometryChangedEvent>(UpdateLayoutByFileEvent);
            UnregisterCallback<GeometryChangedEvent>(UpdateLayoutByPanelEvent);
        }
        
        void UpdateLayoutByFileEvent(GeometryChangedEvent evt)
        {
            UpdateLayoutByPanel();
            UnregisterCallback<GeometryChangedEvent>(UpdateLayoutByFileEvent);
        }

        void RemoveStyleFolderCollapse(GeometryChangedEvent evt)
        {
            styleSheets.Remove(UWContainer.Instance.FolderCollapse);
            UnregisterCallback<GeometryChangedEvent>(RemoveStyleFolderCollapse);
        }
    }
    
    public void SaveOffset()
    {
        _offset = GraphView.viewTransform.position / GraphView.viewTransform.scale.x + File.Position;
    }

    public void UpdatePosition()
    {
        SetPosition(GetPosition());
    }
    
    public override Rect GetPosition()
    {
        Rect position = base.GetPosition();
        if (GraphView.IsFileMovement && !selected)
        {
            position.position = -GraphView.viewTransform.position / GraphView.viewTransform.scale.x + _offset;
        }
        return position;
    }

    public override void SetPosition(Rect newPos)
    {
        Undo.RecordObject(File, "Change position");

        UWDebug.Log("SetPosition");
        base.SetPosition(newPos);

        File.Position = new Vector2(newPos.x, newPos.y);
        
        if (!File.Collapsed)
            File.Size = new Vector2(newPos.width, newPos.height);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        
        Panel.AddToClassList("panel-selected");
        
        Selected?.Invoke(this);
    }

    public override void OnUnselected()
    {
        base.OnUnselected();

        Panel.RemoveFromClassList("panel-selected");
        
        Unselected?.Invoke(this);
    }
    
    public void SetDragAndDropActive(bool active)
    {
        if (active) DisableResizableElement();
        else EnableResizableElement();
    }
    
    public void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
    {
        
    }
    
    public void SetGraphView(UWGraph graphView)
    {
        GraphView = graphView;
    }
}