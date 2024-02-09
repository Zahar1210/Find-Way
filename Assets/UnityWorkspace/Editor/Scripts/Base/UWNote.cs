using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UWNote : UWCustom<UWNoteView>, IResizable, ICollapsible
{
    protected override string DefaultTitle => "Note";
    public override Vector2 InitialSize => new Vector2(200f, 200f);
    public Vector2 MinSize => new Vector2(100f, 100f);
    public Vector2 MaxSize => new Vector2(700f, 700f);
    public string CollapsedInfo
    {
        get
        {
            if (string.IsNullOrEmpty(Text))
                return "";
            
            string line = Text.Split('\n')[0];
            string info = line.Substring(0, Mathf.Min(16, line.Length));
            return info.Length < Text.Length ? info + "..." : info;
        }
    }

    public string Text;
    public int FontSize = 16;
    
    protected override void DrawGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.textArea);
        style.wordWrap = true;
        style.fontSize = FontSize;
        Text = EditorGUILayout.TextArea(Text, style, GUILayout.ExpandHeight(true));
    }

    protected override void DrawInspectorGUI(SerializedObject serializedObject, ref UnityAction updateCallback, ref bool needUpdate)
    {
        EditorGUILayout.LabelField("Font Size");
        FontSize = EditorGUILayout.IntSlider("", FontSize, 8, 48);
    }
}

public class UWNoteView : UWCustomView<UWNote>
{
    public UWNoteView(UWNote file) : base(file) { }
}