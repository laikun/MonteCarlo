using UnityEngine;
using System.Collections.Generic;
using System;

public static class K_OptionData
{
    static Dictionary<string, object> data = new Dictionary<string, object>();

    public static T Get<T>(string name, T? def = null) where T : struct
    {
        object value;
        if (!data.TryGetValue(name, out value))
        {
            if (!def.HasValue)
                throw new UnityException(name + " 키가 존재하지 않음", new KeyNotFoundException());
            else
                value = def.Value;
        }

        K_Report.Log("<color=teal><b>[OptionData]</b> " + name + " : " + value + "</color>");

        return (T)value;
    }

    public static void Set<T>(string name, T value)
    {
        if (data.ContainsKey(name))
            data[name] = value;
        else
            data.Add(name, value);
    }
}
