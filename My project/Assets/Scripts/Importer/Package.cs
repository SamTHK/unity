using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Package
{
    public List<string> presets_key; ///for preset
    public List<string> presets;
    public Package()
    {
        
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

[Serializable]
public class LocalizationPack
{
    public List<string> localization_key;
    public List<string> localization;
}

