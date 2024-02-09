using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public static class UWAssetContainerSystem
{
    private static IAssetContainer _currentAssetContainer;
    
    private static bool _canClick;
    private static MouseButton _button;
    
    private static long _lastClickTime;
    private static long _clickDelay = 150; 
    
    private static bool _waitingAssetOnSecondClick;
    // private static long _doubleClickDelay = 200;
    private static long _doubleClickDelay = 0;
    
    private static bool _canAssetDrag;
    private static float _assetDragTreshold = 20f;
    private static Vector2 _assetDragStartMousePosition;
    
    static UWAssetContainerSystem()
    {
        UWGraph.MouseDownEventGlobal -= OnMouseDownEventGlobal;
        UWGraph.MouseUpEventGlobal -= OnMouseUpEventGlobal;
        UWGraph.MouseMoveEventGlobal -= OnMouseMoveEventGlobal;
        
        UWGraph.MouseDownEventGlobal += OnMouseDownEventGlobal;
        UWGraph.MouseUpEventGlobal += OnMouseUpEventGlobal;
        UWGraph.MouseMoveEventGlobal += OnMouseMoveEventGlobal;
    }
    
    private static void SingleClick()
    {
        UWSystem.SelectAsset(_currentAssetContainer.Asset);
    }
    
    private static void DoubleClick()
    {
        UWSystem.OpenAsset(_currentAssetContainer.Asset);
    }
    
    private static void OnMouseDownEventGlobal(MouseDownEvent evt)
    {
        if (evt.button != (int) MouseButton.LeftMouse)
            return;

        _canAssetDrag = false;
    }

    private static void OnMouseMoveEventGlobal(MouseMoveEvent evt)
    {
        if (evt.pressedButtons != 1)
            return;

        if (!_canAssetDrag)
            return;
        
        bool isAssetDrag = Vector2.Distance(_assetDragStartMousePosition, evt.mousePosition) > _assetDragTreshold;
        
        if (!isAssetDrag)
            return;
        
        _canClick = false;

        UWSystem.StartDrag(_currentAssetContainer);
        _currentAssetContainer.UnpressAsset();
        
        evt.StopPropagation();
    }
    
    private static void OnMouseUpEventGlobal(MouseUpEvent evt)
    {
        _currentAssetContainer?.UnpressAsset();
    }
    
    public static void AssetDown(this IAssetContainer assetContainer, MouseDownEvent evt)
    {
        _currentAssetContainer = assetContainer;
        
        _canAssetDrag = true;
        _canClick = true;
        _lastClickTime = evt.timestamp;
        _assetDragStartMousePosition = evt.mousePosition;
        _button = (MouseButton) evt.button;
        
        _currentAssetContainer.PressAsset();
    }
    
    public static async void AssetUp(this IAssetContainer assetContainer, MouseUpEvent evt)
    {
        if (!_canClick)
            return;
        
        if (evt.button != (int) _button)
            return;
        
        Object asset = _currentAssetContainer.Asset;
        switch (_button)
        {
            case MouseButton.LeftMouse:
                if (evt.shiftKey)
                    UWSystem.SelectAsset(asset);
                else if (evt.altKey)
                    UWSystem.ShowAsset(asset);
                else
                    UWSystem.OpenAsset(asset);
                break;
            case MouseButton.RightMouse:
                UWSystem.SelectAsset(asset);
                break;
            case MouseButton.MiddleMouse:
                UWSystem.ShowAsset(asset);
                break;
        }
        
        return;
        
        if (!_waitingAssetOnSecondClick)
        {
            if (evt.timestamp - _lastClickTime > _clickDelay)
                return;
            
            _waitingAssetOnSecondClick = true;
            
            await Task.Delay((int) _doubleClickDelay);

            if (_waitingAssetOnSecondClick)
            {
                SingleClick();
             
                _waitingAssetOnSecondClick = false;
            }
        }
        else
        {
            DoubleClick();
         
            _waitingAssetOnSecondClick = false;
        }
    }
}