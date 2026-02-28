/*
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using static PresetMaker;
using UnityEditor;
[CustomEditor(typeof(PresetMaker))]
public class PresetMakerEditor : Editor
{
    PresetMaker maker;
    float padding = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        maker = (PresetMaker)target;
    }
    public override void OnInspectorGUI()
    {
        GUILayout.Space(padding);
        PresetTypeMake preset_chosen = (PresetTypeMake)EditorGUILayout.EnumPopup("Type creating: ", maker.preset_type);
        if (preset_chosen != maker.preset_type)
        {
            maker.preset_type = preset_chosen;
            maker.ChangeType();
        }
        switch (maker.preset_type)
        {

        }


        GUILayout.Space(padding);
        serializedObject.DrawInspectorExceptArray(new string[] {"m_Script"});

        GUILayout.Space(padding);
        GUILayout.Label("Effect", EditorStyles.centeredGreyMiniLabel);
        if (GUILayout.Button("Create Effect"))
        {
            maker.CreateEffect();
        }
        GUILayout.Label("Created Effect: ");
        EditorGUILayout.TextArea(maker.created_effect);
        if (GUILayout.Button("Add Effect to Package"))
        {
            maker.AddEffectToPackage();
        }
        if (GUILayout.Button("Reset Effect"))
        {
            maker.ResetEffect();
        }

        GUILayout.Space(padding);
        GUILayout.Label("Condition", EditorStyles.centeredGreyMiniLabel);
        if (GUILayout.Button("Create Condition"))
        {
            maker.CreateCondition();
        }
        GUILayout.Label("Created Condition: ");
        EditorGUILayout.TextArea(maker.created_condition);
        if (GUILayout.Button("Add Condition to Package"))
        {
            maker.AddConditionToPackage();
        }
        if (GUILayout.Button("Reset Condition"))
        {
            maker.ResetCondition();
        }

        if (preset_chosen != PresetTypeMake.None)
        {
            GUILayout.Space(padding);
            GUILayout.Label("Preset", EditorStyles.centeredGreyMiniLabel);
            if (GUILayout.Button("Create Preset"))
            {
                maker.CreatePreset();
            }

            EditorGUILayout.TextArea(maker.created_preset);

            if (GUILayout.Button("Add Preset to Package"))
            {
                maker.AddPresetToPackage();
            }

            if (GUILayout.Button("Reset Preset"))
            {
                maker.ResetPreset();
            }
        }

        GUILayout.Space(padding);
        GUILayout.Label("Localization", EditorStyles.centeredGreyMiniLabel);
        if (GUILayout.Button("Create Localization"))
        {
            maker.CreateLocalization();
        }
        GUILayout.Label("Created Localization: ");
        EditorGUILayout.TextArea(maker.created_localization);
        if (GUILayout.Button("Add Localization to Package"))
        {
            maker.AddLocalizationToPackage();
        }
        if (GUILayout.Button("Reset Localization"))
        {
            maker.ResetLocalization();
        }

        GUILayout.Space(padding);
        GUILayout.Label("Package", EditorStyles.centeredGreyMiniLabel);
        if (GUILayout.Button("Create Package"))
        {
            maker.CreatePackage();
        }
        GUILayout.Label("Package to Read: ");
        maker.created_package = EditorGUILayout.TextArea(maker.created_package);
        if (GUILayout.Button("Read Package"))
        {
            maker.ReadPackage();
        }
        if (GUILayout.Button("Reset Package"))
        {
            maker.ResetPackage();
        }
    }

    
    public void ValueAdd(string name, int index)
    {
        maker.variables[index] = EditorGUILayout.TextField(name, (string)maker.variables[index]);
    }

    public void ListAdd(string name, int index)
    {
        GUILayout.Label(name);
        List<string> variables = ListVisualizer((List<string>)maker.variables[index]);
        if (variables.Last() == "")
        {
            variables.RemoveAt(variables.Count - 1);
        }
        maker.variables[index] = variables;
    }
    
    private List<string> ListVisualizer(List<string> list)
    {
        List<string> visualizers = new List<string>(list);

        visualizers.Add("");

        for (int i = 0; i < visualizers.Count; i++) 
        {
            visualizers[i] = EditorGUILayout.TextField(visualizers[i]);

            if (i < visualizers.Count - 1)
            {
                if (GUILayout.Button("-"))
                {
                    visualizers.RemoveAt(i);
                }
            }
        }
        return visualizers;
    }


   

   
    
}
#endif
*/
