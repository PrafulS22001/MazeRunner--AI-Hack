using UnityEditor;
using UnityEngine;

public class DebugMCP : MonoBehaviour
{
    [MenuItem("Tools/Enable MCP Debug")]
    public static void EnableDebug()
    {
        EditorPrefs.SetBool("Unity.AI.UnityMCP.DebugLogs", true);
        Debug.Log("MCP Debug logging enabled!");
    }
}