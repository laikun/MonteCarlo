//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
    public static class K_GameState
    {
        private static Dictionary<string, string> state = new Dictionary<string, string>();
        public static string Get(string str)
        {
            string value = null;
            if (state.TryGetValue(str, out value))
                return value;
            else
                throw new Exception("존재하지 않는 상태");
        }
    }
}
