using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

#if UNITY_EDITOR
public static class DrawInspectorExcept
    {
    public static void DrawInspectorExceptArray(this SerializedObject serializedObject, string[] fieldsToSkip)
    {
        serializedObject.Update();
        SerializedProperty prop = serializedObject.GetIterator();
        if (prop.NextVisible(true))
        {
            do
            {
                if (fieldsToSkip.Any(prop.name.Contains))
                    continue;

                EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
            }
            while (prop.NextVisible(false));
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

