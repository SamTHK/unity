/*

using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class PresetMaker : MonoBehaviour
{

    

    [NonSerialized] public object[] variables;
    [NonSerialized] public int variables_numbers;
    [NonSerialized] public PresetTypeMake preset_type = PresetTypeMake.None;

    [LabelOverride("Effect's File Name")] public string effectfilename;
    [LabelOverride("Base Effect")] public string baseeffect;
    public List<string> effect_variables;
    [Space(20)]
    [LabelOverride("Condition's File Name")] public string conditionfilename;
    [LabelOverride("Base Condition")] public string basecondition;
    public List<string> condition_variables;
    public List<string> condition_firstproc;
    [Space(20)]
    [LabelOverride("Localization's File Name")] public string localfilename;
    [LabelOverride("Localization's Language")] public string locallanguage = "english";
    [LabelOverride("Localization's Content")] public string localcontent;
    [Space(20)]
    [LabelOverride("Default Language")] public string default_language = "english";
    public List<string> package_preset;
    public List<string> package_localization;
    [NonSerialized] public string created_effect = "";
    [NonSerialized] public string created_condition = "";
    [NonSerialized] public string created_preset = "";
    [NonSerialized] public string created_package = "";
    [NonSerialized] public string created_localization = "";
    public enum PresetTypeMake
    {
        None,
        Card
    }
    private void Update()
    {
        
    }

    public void ChangeType()
    {
        ResetPreset();
        created_preset = "";
    }

    public void ResetEffect()
    {
        effectfilename = default;
        baseeffect = default;
        effect_variables = default;
    }

    public void ResetCondition()
    {
        conditionfilename = default;
        basecondition = default;
        condition_variables = default;
        condition_firstproc = default;
    }
    public void ResetPreset()
    {
        switch (preset_type)
        {
            case PresetTypeMake.None:
                variables_numbers = 0;
                variables = null;
                break;
            default:
                variables_numbers = 0;
                variables = null;
                break;
        }
    }

    public void ResetPackage()
    {
        default_language = "english";
        package_preset = default;
        package_localization = default;
    }

    public void CreatePreset()
    {
        switch (preset_type)
        {
             
        }
    }

    public void CreateEffect()
    {
        EffectPreset effectPreset = new EffectPreset()
        {
            filename = effectfilename,
            baseeffect = baseeffect,
            variables = effect_variables.ToArray(),
        };
        created_effect = JsonUtility.ToJson(effectPreset);
    }

    public void CreateCondition()
    {
        ConditionPreset conditionPreset = new ConditionPreset()
        {
            filename = conditionfilename,
            basecondition = basecondition,
            firstproc = condition_firstproc.ToArray(),
            variables = condition_variables.ToArray(),
        };
        created_condition = JsonUtility.ToJson(conditionPreset);
    }

    public void ResetLocalization()
    {
        locallanguage = "english";
        localfilename = default;
        localcontent = default;
    }    

    public void CreateLocalization()
    {
        LocalizationPack localizationPack = new LocalizationPack(localfilename,locallanguage,localcontent);
        created_localization = JsonUtility.ToJson(localizationPack);
    }
    public void AddEffectToPackage()
    {
        package_preset.Add(created_effect);
    }    

    public void AddConditionToPackage()
    {
        package_preset.Add(created_condition);
    }

    public void AddPresetToPackage()
    {
        package_preset.Add(created_preset);
    }

    public void AddLocalizationToPackage()
    {
        package_preset.Add(created_localization);
    }    

    public void CreatePackage()
    {
        List<string> keys = new();
        foreach (string preset in package_preset) {
            keys.Add(preset.Split("\"")[5]);
        }
        Package pack = new Package()
        {
            default_language = default_language,
            presets = package_preset,
            presets_key = keys
        };
        created_package = JsonUtility.ToJson(pack);
    }

    public void ReadPackage()
    {
        Package pack = JsonUtility.FromJson<Package>(created_package);
        default_language = pack.default_language;
        package_preset = pack.presets;
        package_localization = pack.values;
    }
}
#endif

*/