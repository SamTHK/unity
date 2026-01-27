using System.Collections.Generic;
using UnityEngine;

public class Package
{
    public List<string> presets_key; ///for preset
    public List<string> presets;

    public List<string> keys; /// for localization
    public List<string> values;

    public Package()
    {
        keys = new List<string>();
        values = new List<string>();
        presets = new List<string>();
        presets_key = new List<string>();
    }

    public void AddValues(string key, string value)
    { this.keys.Add(key); this.values.Add(value); }

    public string GetValues(string key)
    {
        string language = ObjectUtils.Manager.Language;
        key += "_-." + language;

        int i = keys.FindIndex(x => x == key);
        if (i != -1)
        {
            return values[i];
        }
        return default;
    }

    public void AddPreset(Preset preset)
    {
        presets_key.Add(preset.filename);
    }

    public Something GetPreset<Something>(string name) where Something : Preset
    {
        int i = presets_key.FindIndex(x => x == name);
        if (i != -1)
        {
            return JsonUtility.FromJson<Something>(presets[i]);
        }
        return default;
    }


}