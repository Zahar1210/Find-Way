using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UWCustomView<T> : UWFileView<T> where T : UWCustom
{
    private IMGUIContainer _guiContainer;

    protected UWCustomView(T file) : base(file)
    {
        EditorApplication.update += _guiContainer.MarkDirtyRepaint;
        RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
    }

    private void OnDetachFromPanelEvent(DetachFromPanelEvent evt)
    {
        EditorApplication.update -= _guiContainer.MarkDirtyRepaint;
    }

    protected override void LoadUXML()
    {
        LoadUXML(UWContainer.Instance.CustomUXML);
    }
    
    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        
        _guiContainer = this.Q<IMGUIContainer>();
        _guiContainer.onGUIHandler = File.DrawTotalGUI;
    }

    protected override void OnCollapseValueChanged(bool collapsed)
    {
        base.OnCollapseValueChanged(collapsed);

        _guiContainer.style.display = File.Collapsed ? DisplayStyle.None : DisplayStyle.Flex;
    }
}

public abstract class UWCustom : UWFile
{
    protected abstract Type ViewType { get; }
    
    public override Vector2 InitialSize => new Vector2(200f, 200f);
    
    public override UWFileView CreateView()
    {
        return Activator.CreateInstance(ViewType, (object) this) as UWFileView;
    }
    
    protected abstract void DrawGUI();
    
    public void DrawTotalGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawGUI();
        if (EditorGUI.EndChangeCheck())
        {
            Save();
        }
    }
}

public abstract class UWCustom<T> : UWCustom where T : UWFileView
{
    protected sealed override Type ViewType => typeof(T);
}