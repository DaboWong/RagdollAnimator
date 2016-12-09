using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.MappingRagdoll.Editor
{
    class RagdollMappingAnimationWindow : EditorWindow
    {
        [MenuItem("Tools/RagdollMappingAnimation")]
        static void CreateWindow()
        {
            RagdollMappingAnimationWindow window = EditorWindow.GetWindow<RagdollMappingAnimationWindow>();
        }

        string selectName = string.Empty;

        string[] menu = new string[]
        {
            "add fixed key frame",
            "add event key frame",
        };
        int selectMenu=-1;

        public string[] boneType;


        Vector2 scrollPosV = Vector2.zero;
        Vector2 scrollPosH = Vector2.zero;

        public RagdollMappingAnimationWindow()
        {
            Selection.selectionChanged += OnSelectionChanged;

            Type data = typeof(RagdollMappingData);

            var fields = data.GetFields();
            boneType = new string[fields.Length];
            for(int i =0; i < fields.Length;++i)
            {
                boneType[i] = fields[i].Name;
            }
        }

        ~RagdollMappingAnimationWindow()
        {
            Debug.Log("destroy RagdollMappingAnimationWindow.");
            Selection.selectionChanged -= OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            Debug.Log("selection changed", Selection.activeGameObject);

            Reselect(0);

            Repaint();
        }  
        
        void OnGUI()
        {
            Draw();          
        }

        void Draw()
        {
            scrollPosV = EditorGUILayout.BeginScrollView(scrollPosV);
            DrawPlayBar();
            EditorGUILayout.BeginHorizontal();            
            DrawPanelLeft();            
            EditorGUILayout.Space();
            DrawPanelRight();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }   
        void DrawPlayBar()
        {
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("▷"))
            {
                RagdollMappingAnimationController controller = null;
                if (Selection.activeGameObject != null)
                    controller = Selection.activeGameObject.GetComponent<RagdollMappingAnimationController>();
                if (controller == null)
                    return;
                var sel = GetSelect();
                if (sel == null)
                    return;
                controller.Play(sel.clipName);            
            }
            if(GUILayout.Button("‖"))
            {
                RagdollMappingAnimationController controller = null;
                if (Selection.activeGameObject != null)
                    controller = Selection.activeGameObject.GetComponent<RagdollMappingAnimationController>();
                if (controller == null)
                    return;
                controller.Stop();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawPanelLeft()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            DrawName();
            DrawMenu();
            EditorGUILayout.EndHorizontal();
            DropProperty();
            EditorGUILayout.EndVertical();            
        }

        void DrawPanelRight()
        {
            EditorGUILayout.BeginVertical();
            DrawTimeLine();
            DrawKeyFrames();
            EditorGUILayout.EndVertical();
        }

        void DrawTimeLine()
        {
            EditorGUILayout.LabelField("0:00");
        }

        void DrawKeyFrames()
        {
            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(0, position.yMin, 0), new Vector3(0, position.yMax, 0 ));
        }

        void DrawName()
        {
            int selectIndex = GetIndex(selectName);
            int currSel = EditorGUILayout.Popup(Mathf.Max(selectIndex, 0), GetNameArray());
            if(currSel!= selectIndex)
            {
                Reselect(currSel);

                Repaint();
            }
        }     
        void DrawMenu()
        {
            int selectIndex = selectMenu;
            selectIndex = EditorGUILayout.Popup(selectIndex, menu);           
            if (selectIndex != selectMenu)
            {
                selectMenu = selectIndex;
                RagdollMappingAnimation selAnim = GetSelect();
                if (selAnim == null)
                    return;
                var kf = new MappingRagdollKeyFrame();
                kf.BoneName = boneType[0];
                switch (selectMenu)
                {
                    case 0:
                        {
                            Debug.Log("add kf fixed");
                            var clips = new List<MappingRagdollKeyFrame>(selAnim.ragdollMappingClip.fixedKeyFrames);
                            clips.Add(kf);
                            selAnim.ragdollMappingClip.fixedKeyFrames = clips.ToArray();
                            
                        }
                        break;
                    case 1:
                        {
                            Debug.Log("add kf event");
                            var clips = new List<MappingRagdollKeyFrame>(selAnim.ragdollMappingClip.eventKeyFrames);
                            clips.Add(kf);
                            selAnim.ragdollMappingClip.eventKeyFrames = clips.ToArray();
                        }
                        break;
                }
                Repaint();
                selectMenu = -1;
            }
        }
        
        void DropProperty()
        {
            RagdollMappingAnimation selAnim = GetSelect();
            if (selAnim == null)
                return;
            DrawClip(selAnim.ragdollMappingClip);
        }   

        void DrawClip(RagdollMappingClip ragdollMappingClip)
        {
            if (ragdollMappingClip == null)
                return;
            ragdollMappingClip.duration = EditorGUILayout.FloatField("duration",ragdollMappingClip.duration);

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < ragdollMappingClip.eventKeyFrames.Length; ++i)
            {
                var et = ragdollMappingClip.eventKeyFrames[i];
                DrawProperty(ref ragdollMappingClip.eventKeyFrames, et, ragdollMappingClip.duration, true);
            }
            for (int i =0; i < ragdollMappingClip.fixedKeyFrames.Length; ++ i)
            {
                var et = ragdollMappingClip.fixedKeyFrames[i];
                DrawProperty(ref ragdollMappingClip.fixedKeyFrames, et, ragdollMappingClip.duration, false);
            }

            EditorGUILayout.EndVertical();
        }

        void DrawProperty(ref MappingRagdollKeyFrame[] container, MappingRagdollKeyFrame mrkf, float duration, bool showTimer )
        {
            //name            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("boneType");
            int boneIndex = IndexOfBone(mrkf.BoneName);            
            int currSel = EditorGUILayout.Popup(boneIndex, boneType);
            if (currSel != boneIndex)
            {
                mrkf.BoneName = boneType[currSel];
                Repaint();
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
            if(showTimer)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("time");
                float timeSlider = EditorGUILayout.Slider(mrkf.keyTime, 0, duration);
                mrkf.keyTime = Mathf.Min(timeSlider, duration);
                EditorGUILayout.EndHorizontal();
            }

            //force
            EditorGUILayout.BeginVertical();
            mrkf.force.x =EditorGUILayout.FloatField("x", mrkf.force.x);
            mrkf.force.y = EditorGUILayout.FloatField("y", mrkf.force.y);
            mrkf.force.z = EditorGUILayout.FloatField("z", mrkf.force.z);
            EditorGUILayout.EndVertical();

            //TODO properties
            //mrkf.isRelative = EditorGUILayout.Toggle("isRelative", mrkf.isRelative);
        }

        public int IndexOfBone(string name)
        {
            for(int i =0; i < boneType.Length; ++ i)
            {
                if (boneType[i] == name)
                    return i;
            }
            return 0;
        }

        void Reselect(int selectIndex)
        {
            RagdollMappingAnimationController controller = null;
            if (Selection.activeGameObject != null)
                controller = Selection.activeGameObject.GetComponent<RagdollMappingAnimationController>();
            if (controller != null && controller.animations.Length > 0)
            {
                int index = Mathf.Min(selectIndex, controller.animations.Length);
                if(controller.animations[index]!=null)
                    selectName = controller.animations[index].clipName;
            }
        }


        int GetIndex(string name)
        {
            RagdollMappingAnimationController controller = null;
            if (Selection.activeGameObject != null)
                controller = Selection.activeGameObject.GetComponent<RagdollMappingAnimationController>();
            if (controller == null)
                return -1;
            if (string.IsNullOrEmpty(selectName))
                return -1;
            var anim = controller.FindByName(selectName);
            if (anim == null)
                return -1;
            return new List<RagdollMappingAnimation>(controller.animations).IndexOf(anim);
        }
        RagdollMappingAnimation GetSelect()
        {
            RagdollMappingAnimationController controller = null;
            if (Selection.activeGameObject != null)
                controller = Selection.activeGameObject.GetComponent<RagdollMappingAnimationController>();
            if (controller == null)
                return null;
            if (string.IsNullOrEmpty(selectName))
                return null;
            return controller.FindByName(selectName);
        }

        string[] GetNameArray()
        {
            RagdollMappingAnimationController controller = null;
            if (Selection.activeGameObject != null)
                controller = Selection.activeGameObject.GetComponent<RagdollMappingAnimationController>();
            if (controller == null)
                return new string[] { };
            List<string> nameArry = new List<string>();
            foreach(var anim in controller.animations)
            {
                if(anim!=null)
                {
                    nameArry.Add(anim.clipName);
                }
            }
            return nameArry.ToArray();
        }
    }
}
