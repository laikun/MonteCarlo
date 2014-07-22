using UnityEngine;
using System.Linq;
using System.Collections;

public static class K_Report {
    
    public static bool IsConsolePrint {get; set;}

    static string getName(MonoBehaviour mono)
    {
        return string.Join("/", mono.GetComponentsInParent<Transform>().Reverse().Select(x => x.name).ToArray());
    }

    public static void Log(object msg)
    {
        Debug.Log(msg);
    }

    public static void Log(this MonoBehaviour mono, string msg, string color = "black")
    {
        Debug.Log("<color=" + color + "><b>" + getName(mono) + "</b> : " + msg + "</color>");
    }

    public static void LogPurple(this MonoBehaviour mono, string msg)
    {        
        Debug.Log("<color=purple><b>" + getName(mono) + "</b> : " + msg + "</color>");
    }

    public static void FBlog(this MonoBehaviour mono, string msg)
    {
        FbDebug.Log("<color=darkblue><b>" + getName(mono) + "</b> : " + msg + "</color>");
    }

    public static void FBlog(string msg)
    {
        FbDebug.Log("<color=darkblue>" + msg + "</color>");
    }

    public static void DLog_Event(this MonoBehaviour mono, string msg)
    { 
        K_Report.Log("<color=brown>[" + msg.PadRight(10, ' ') + "]<b> " + mono.name + "</b></color>");
    }

}
    