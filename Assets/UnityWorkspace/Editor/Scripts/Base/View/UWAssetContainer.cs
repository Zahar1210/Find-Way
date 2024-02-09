using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UWAssetContainer : VisualElement, IAssetContainer
{
    public new class UxmlFactory : UxmlFactory<UWAssetContainer, UxmlTraits> { }

    private UWFileView _fileView;
    private VisualElement _icon;
    private Label _title;
    
    private bool _hover;
    private bool _initialized;
    private float _titleSize;
    
    private float MinTitleSize = 10f;
    private float MaxTitleSize = 24f;
    
    public bool Hover => _hover;
    
    public Object Asset { get; set; }
    public UWFileView FileView => _fileView;
    public string DragTitle => Asset.name;

    public UnityAction<bool> HoverUpdated;
    
    public UWAssetContainer()
    {
        RegisterCallback<MouseDownEvent>(OnMouseDownEventAsset);
        RegisterCallback<MouseUpEvent>(OnMouseUpEventAsset);
        
        RegisterCallback<PointerEnterEvent>(OnPointerEnterEventAsset);
        RegisterCallback<PointerLeaveEvent>(OnPointerLeaveEventAsset);
    }

    private void TryInitialize()
    {
        if (!_initialized)
        {
            _initialized = true;
            
            _fileView = GetFirstAncestorOfType<UWFileView>();
            _icon = this.Q<VisualElement>("AssetIcon") ?? this;
            _title = this.Query<Label>("AssetTitle");

            if (_title != null)
                _titleSize = _title.resolvedStyle.fontSize;
        }
    }
    
    private void OnPointerEnterEventAsset(PointerEnterEvent evt)
    {
        AddToClassList("asset-hover");

        _hover = true;
        
        HoverUpdated?.Invoke(true);
    }
    
    private void OnPointerLeaveEventAsset(PointerLeaveEvent evt)
    {
        RemoveFromClassList("asset-hover");

        _hover = false;
        
        HoverUpdated?.Invoke(false);
    }
    
    private void OnMouseDownEventAsset(MouseDownEvent evt)
    {
        evt.StopImmediatePropagation();
        
        if (Asset == null)
            return;
        
        this.AssetDown(evt);
    }
    
    private void OnMouseUpEventAsset(MouseUpEvent evt)
    {
        evt.StopImmediatePropagation();
        
        if (Asset == null)
            return;
        
        if (!_hover)
            return;
        
        this.AssetUp(evt);
    }

    private Texture2D GetIcon()
    {
        if (!Asset) return null;

        Texture2D icon = AssetPreview.GetAssetPreview(Asset);
        if (!icon) icon = AssetPreview.GetMiniThumbnail(Asset);
        
        return icon;
    }
    
    private void UpdateIcon()
    {
        _icon.style.backgroundImage = GetIcon();
    }
    
    private void UpdateTitle()
    {
        if (_title != null)
        {
            _title.text = Asset.name;
            UpdateTitleSize();
        }
    }
    
    private void UpdateTitleSize()
    {
        if (_title != null)
        {
            float titleSize = _titleSize / parent.transform.scale.x;
            titleSize = Mathf.Clamp(titleSize, MinTitleSize, MaxTitleSize);
            
            _title.style.fontSize = titleSize;
        }
    }
    
    public void UpdateContainer()
    {
        TryInitialize();

        UpdateIcon();
        UpdateTitle();
    }
    
    public void UpdateContainer(Object asset)
    {
        UpdateTitleSize();
        
        if (asset && Asset == asset) return;
        
        Asset = asset;
        
        UpdateContainer();
    }
    
    public void PressAsset()
    {
        AddToClassList("asset-pressed");
    }
    
    public void UnpressAsset()
    {
        RemoveFromClassList("asset-pressed");
    }
}