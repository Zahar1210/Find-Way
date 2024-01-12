using UnityEngine;
public static class CarTransform
{
    public static Vector3 GetPosition(float t, TrafficDot.Dot a, TrafficDot.Dot b, Vector3 centerDot)
    {
        if (centerDot.x != 0) {
            Vector3 dot1 = Vector3.Lerp(a.Pos, centerDot, t);
            Vector3 dot2 = Vector3.Lerp(centerDot, b.Pos, t);
            Vector3 dot = Vector3.Lerp(dot1, dot2, t);
            return dot;
        }
        else{
            Vector3 dot = Vector3.Lerp(a.Pos,b.Pos,t);
            return dot;
        }
    }

    public static Quaternion GetRotation(float t, TrafficDot.Dot a, TrafficDot.Dot b) {
        return Quaternion.Lerp(a.Rot, b.Rot, t);
    }
}