using System.Collections.Generic;
using System.Linq;

public static class EffectsUtils
{
    public static List<object> ObjectList(object[] objects, params object[] param)
    {
        List<object> list;
        if (objects == null)
        {
            list = new();
        }
        else
        {
            list = objects.ToList();
        }

        for (int i = 0; i < param.Length; i++)
        {
            list.Insert(i, param[i]);
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