using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 플래그 보조 클래스
/// 플래그 값은 Integer형태로 저장
/// </summary>
public static class K_Flag
{
    static Dictionary<string, Action<int>> handler = new Dictionary<string, Action<int>>();
    static Dictionary<string, int> state = new Dictionary<string, int>();

    /// <summary>
    /// 지정된 이름의 플래그 값을 가져온다
    /// </summary>
    /// <param name="name">플래그 명</param>
    /// <returns></returns>
    public static int State(string name)
    {
        int value;

        if (state.TryGetValue(name, out value))
            return value;

        throw new UnityException("<color=red>[FLAG] 설정되지 않은 플래그를 요구하고 있음 : "+ name + "</color>", new KeyNotFoundException());
    }

    /// <summary>
    /// 플래그 설정
    /// </summary>
    /// <param name="name">플래그 명</param>
    /// <param name="flag"></param>
    public static void On(string name, bool flag)
    {
        int value;

        if (!state.TryGetValue(name, out value))
            state.Add(name, flag ? 1 : 0);
        else
            K_Flag.On(name, flag ? 1 : 0);
    }

    /// <summary>
    /// 플래그 설정
    /// </summary>
    /// <param name="name">플래그 명</param>
    /// <param name="flag"></param>
    public static void On(string name, int flag)
    {
        int value;

        if (!state.TryGetValue(name, out value))
            state.Add(name, flag);
        else
            state[name] = flag;

        Action<int> fh;

        if (handler.TryGetValue("name", out fh) && fh != null) fh(flag);

        K_Report.Log("<color=teal><b>[FLAG\t]</b> " + name + " = " + state[name] + "</color>");
    }
     
    /// <summary>
    /// 플래그에 핸들러를 등록함
    /// </summary>
    /// <param name="name">플래그 명</param>
    /// <param name="handle">등록할 핸들러 Action(int)></param>
    public static void SetHandler(string name, Action<int> act)
    {
        int value;

        if (!state.TryGetValue(name, out value))
            state.Add(name, 0);

        Action<int> fh;

        if (!handler.TryGetValue("name", out fh))
            handler.Add("name", act);
        else
            fh += act;
    }

    public static void Reset(string name)
    {
        state.Remove(name);
        handler.Remove(name);

        state.Add(name, 0);
   }

    /// <summary>
    /// 등록된 핸들러 리셋
    /// </summary>
    /// <param name="name">플래그 명</param>
    public static void Remove(string name)
    {
        int value;

        if (state.TryGetValue(name, out value))
            state.Remove(name);
        else
            K_Report.Log("<color=red><b>[FLAG\t]</b> 설정되지 않은 플래그를 요구하고 있음 : " + name + "</color>");

        Action<int> fh;

        if (handler.TryGetValue("name", out fh))
            handler.Remove(name);
        else
            K_Report.Log("<color=red><b>[FLAG\t]</b> 설정되지 않은 플래그를 요구하고 있음 : " + name + "</color>");
    }

    /// <summary>
    /// 전체 플래그 초기화
    /// </summary>
    public static void Init()
    {
        handler = new Dictionary<string, Action<int>>();
        state = new Dictionary<string, int>();
    }
}

