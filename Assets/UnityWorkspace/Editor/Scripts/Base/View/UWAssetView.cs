using UnityEngine;
using UnityEngine.UIElements;

public class UWAssetView : UWFileView<UWAsset>
{
    public Object a;
    public int b;
    public StyleSheet c;
    protected override Vector2 InitialSize => new Vector2(Panel.resolvedStyle.width, Panel.resolvedStyle.height);
    
    private UWAssetContainer _asset;

    public UWAssetView(UWAsset file) : base(file)
    {
        _asset.UpdateContainer(File.Asset);
    }
    
    protected override void LoadUXML()
    {
        if (File.Style == null)
            File.Style = UWContainer.Instance.AssetStyles[0];
        
        LoadUXML(File.Style);
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        _asset = this.Q<UWAssetContainer>("Asset");
    }

    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();
        
        _asset.HoverUpdated += OnUpdateAssetHover;
    }

    public override void UpdateFile()
    {
        base.UpdateFile();
        
        UpdateAsset();
    }

    private void UpdateAsset()
    {
        _asset.UpdateContainer(File.Asset);
    }
    
    protected override void UpdateHover(bool panelHover)
    {
        base.UpdateHover(panelHover && !_asset.Hover);
    }
    
    private void OnUpdateAssetHover(bool hover)
    {
        UpdateHover(PanelHover);
    }

    protected override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        foreach (VisualTreeAsset assetStyle in UWContainer.Instance.AssetStyles)
        {
            evt.menu.AppendAction("View/" + assetStyle.name, _ => UpdateStyle(assetStyle), File.Style == assetStyle ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
        }

        base.BuildContextualMenu(evt);
    }
    
    private void UpdateStyle(VisualTreeAsset style)
    {
        if (style == null) return;
        
        File.Style = style;
        
        Clear();
        LoadUXML(style);
        InitializeComponents();
        UpdateFile();
    }

    public override bool IsSelectable()
    {
        Event evt = Event.current;
        if (_asset != null && evt != null && _asset.Hover)
            return false;
        
        return base.IsSelectable();
    }
}