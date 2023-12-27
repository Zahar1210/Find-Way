using UnityEngine;

public class TrafficDot : MonoBehaviour
{
    public DotType Type;
    public CenterDot CenterDot => transform.parent.GetComponent<CenterDot>();
}