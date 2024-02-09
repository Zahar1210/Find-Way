using UnityEngine;

public static class UWDebug
{
    public static void Log(string message)
    {
#if UW_DEBUG
        Debug.Log(message);
#endif
    }
}