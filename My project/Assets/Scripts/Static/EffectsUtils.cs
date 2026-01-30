using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class EffectsUtils
{
    public static List<object> ObjectList( object[] objects, params object[] param)
    {
        List<object> list = new List<object>();
        foreach (object obj in param)
        {
            list.Add(obj);
        }
        foreach (object obj in objects)
        {
            list.Add(obj);
        }
        return list;

    }

    public static string StandardString(string str)
    {
        return str.ToLower().Trim();
    }

    public static string PresetToString(Preset preset)
    {
        return preset.ToString();
    }

   


   
}