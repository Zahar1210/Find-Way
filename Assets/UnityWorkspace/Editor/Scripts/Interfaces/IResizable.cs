using UnityEngine;

public interface IResizable
{
    Vector2 MinSize { get; }
    Vector2 MaxSize { get; }
}