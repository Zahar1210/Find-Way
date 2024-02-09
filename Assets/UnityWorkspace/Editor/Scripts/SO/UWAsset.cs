using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class UWAsset : UWFile
{
    [SerializeField] private Object _asset;
    [SerializeField] private VisualTreeAsset _style;

    public Object Asset { get => _asset; private set => SetAndSave(ref _asset, value); }
    public VisualTreeAsset Style { get => _style; set => SetAndSave(ref _style, value); }

    protected override string DefaultTitle => Asset != null ? Asset.name : "None";
    public override Vector2 InitialSize => new Vector2(0f, 0f);

    public override UWFileView CreateView()
    {
        return new UWAssetView(this);
    }

    public void Initialize(Object asset)
    {
        Asset = asset;
    }

    protected override void DrawInspectorGUI(SerializedObject serializedObject, ref UnityAction updateCallback, ref bool needUpdate)
    {
        _asset = EditorGUILayout.ObjectField(_asset, typeof(Object), false);
    }
}
