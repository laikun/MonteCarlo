using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class K_Report {
    
    public static bool IsConsolePrint {get; set;}

    static System.Text.StringBuilder log = new System.Text.StringBuilder("\n");

    static void add(string str)
    {
#if !UNITY_EDITOR
        if (K_ReportDebug.Instance != null)
            K_ReportDebug.Instance.Add(str);
#endif
        log.AppendLine(System.DateTime.Now.ToShortTimeString() + " :: " + str);
    }

    static string getName(MonoBehaviour mono)
    {
        return string.Join("/", mono.GetComponentsInParent<Transform>().Reverse().Select(x => x.name).ToArray());
    }

    public static void Log(object msg)
    {
        add(msg.ToString());
        Debug.Log(msg);
    }

    public static void Log(this MonoBehaviour mono, string msg, string color = "black")
    {
        add(getName(mono) + " : " + msg);
        Debug.Log("<color=" + color + "><b>" + getName(mono) + "</b> : " + msg + "</color>");
    }

    public static void LogPurple(this MonoBehaviour mono, string msg)
    {
        add(getName(mono) + " : " + msg);
        Debug.Log("<color=purple><b>" + getName(mono) + "</b> : " + msg + "</color>");
    }

    public static void FBlog(this MonoBehaviour mono, string msg)
    {
        add(getName(mono) + " : " + msg);
        FbDebug.Log("<color=darkblue><b>" + getName(mono) + "</b> : " + msg + "</color>");
    }

    public static void FBlog(string msg)
    {
        add(msg);
        FbDebug.Log("<color=darkblue>" + msg + "</color>");
    }

    public static void DLog_Event(this MonoBehaviour mono, string msg)
    {
        add(getName(mono) + " : " + msg);
        K_Report.Log("<color=brown>[" + msg.PadRight(10, ' ') + "]<b> " + mono.name + "</b></color>");
    }


}

public class K_ReportDebug : SingletonInGame<K_ReportDebug>
{
    GUIStyle textStyle = new GUIStyle();
    List<string> log = new List<string>(new string[]{"", "", "", "", "", "", "", "", });

    public void Add(string str)
    {
        log.RemoveAt(0);
        log.Add(str);
    }

    protected override void awake()
    {
        textStyle.alignment = TextAnchor.LowerLeft;
        textStyle.fixedWidth = 600f;
        textStyle.clipping = TextClipping.Clip;
    }

#if !UNITY_EDITOR
    void OnGUI()
    {
        GUI.Box(new Rect(0, Screen.height - 100, 600, 100), string.Join("\n", log.ToArray()), textStyle);
    }
#endif
}
    