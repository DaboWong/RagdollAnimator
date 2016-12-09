using Assets.Scripts.MappingRagdoll;
using Assets.Scripts.MappingRagdoll.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Script.MappingRagdoll.Editor
{
    class MappingRagdollKeyFrameCtrl
    {
        //base property
        Rect ctrlRect;
        public virtual void Draw()
        {
            //name            
            ctrlRect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("boneType");
            int boneIndex = parentWindow.IndexOfBone(mrkf.BoneName);
            int currSel = EditorGUILayout.Popup(boneIndex, parentWindow.boneType);
            if (currSel != boneIndex)
            {
                mrkf.BoneName = parentWindow.boneType[currSel];                
            }
            //menu delete
            if (GUILayout.Button("删"))
            {
                var clips = new List<MappingRagdollKeyFrame>(container);
                clips.Remove(mrkf);
                container = clips.ToArray();
            }
            EditorGUILayout.EndHorizontal();

            //time
            if (showTimer)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("time");
                float timeSlider = EditorGUILayout.Slider(mrkf.keyTime, 0, duration);
                mrkf.keyTime = Mathf.Min(timeSlider, duration);
                EditorGUILayout.EndHorizontal();
            }

            //force
            EditorGUILayout.BeginVertical();
            mrkf.force.x = EditorGUILayout.FloatField("x", mrkf.force.x);
            mrkf.force.y = EditorGUILayout.FloatField("y", mrkf.force.y);
            mrkf.force.z = EditorGUILayout.FloatField("z", mrkf.force.z);
            EditorGUILayout.EndVertical();
        }

        public MappingRagdollKeyFrameCtrl(RagdollMappingAnimationWindow parent, 
            MappingRagdollKeyFrame[] c, MappingRagdollKeyFrame mrkf, 
            float d, float st)
        {

        }
        //inheritance property
        MappingRagdollKeyFrame[] container;
        MappingRagdollKeyFrame mrkf;
        float duration;
        bool showTimer;
        RagdollMappingAnimationWindow parentWindow;        
    }
}
