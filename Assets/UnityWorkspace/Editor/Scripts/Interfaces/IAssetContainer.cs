using UnityEngine;

public interface IAssetContainer
{
    Object Asset { get; }
    UWFileView FileView { get; }
    string DragTitle { get; }
    void PressAsset();
    void UnpressAsset();
}