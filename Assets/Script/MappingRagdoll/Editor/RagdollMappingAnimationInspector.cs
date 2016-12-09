using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.MappingRagdoll;
using System.IO;

[CustomEditor(typeof(Assets.Scripts.MappingRagdoll.RagdollMappingAnimation))]
class RagdollMappingAnimationInspector : Editor
{
    string fieldName = "uselessField";
    Type type = typeof(Assets.Scripts.MappingRagdoll.RagdollMappingAnimationController);
    RagdollMappingAnimation rma;
    SerializedObject serializedclip;  

    public RagdollMappingAnimationInspector()
    {
    }

    public override void OnInspectorGUI()
    {
        rma = target as RagdollMappingAnimation;
        if (rma.ragdollMappingClip == null)
            rma.ragdollMappingClip = ScriptableObject.CreateInstance(typeof(RagdollMappingClip)) as RagdollMappingClip;
        if(string.IsNullOrEmpty(rma.ragdollMappingClip.clipName))//Rename
        {
            rma.ragdollMappingClip.clipName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(rma.GetInstanceID()));
        }
        if (serializedclip==null)
            serializedclip = new SerializedObject(rma.ragdollMappingClip);
        EditorGUILayout.LabelField("属性名称:"+ fieldName);
        EditorGUILayout.LabelField("类型："+type.ToString());
        EditorGUILayout.Space();
        serializedclip.Update();
        DrawPropertiesExcluding(serializedclip);
        if(GUI.changed)
        {
            if (serializedclip.ApplyModifiedProperties())
                rma.Init(fieldName, type);
            serializedObject.ApplyModifiedProperties();         
        }
    }

    string DrawLabel(string label, ref string text )
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);
        text = GUILayout.TextField(text);
        EditorGUILayout.EndHorizontal();
        return text;
    }

    void DrawArray(string propertyName, string button)
    {
        SerializedProperty clipDuration = serializedclip.FindProperty(propertyName);
        if(clipDuration.isArray)
        {
            if (GUILayout.Button(button))
            {
                clipDuration.InsertArrayElementAtIndex(0);                
            }           
        }
    }
}
