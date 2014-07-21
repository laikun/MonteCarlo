using UnityEngine;
using System.Collections;

public static class K_Report {
    
    public static bool IsConsolePrint {get; set;}

    public static void Log(object msg)
    {
        Debug.Log(msg);
    }

    public static void DLog_Event(this MonoBehaviour mono, string msg) { 
        K_Report.Log("<color=brown>[" + msg.PadRight(10, ' ') + "]<b> " + mono.name + "</b></color>");
    }

}
    