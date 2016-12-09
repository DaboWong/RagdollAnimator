using System;
using UnityEngine;

namespace Assets.Scripts.MappingRagdoll
{
    [CreateAssetMenu]
    public class RagdollMappingAnimation : ScriptableObject
    {
        AnimationClip animationClip;
        [SerializeField]
        public RagdollMappingClip ragdollMappingClip;
        public delegate void OnKeyFrameTriggered(MappingRagdollKeyFrame mrb);
        public OnKeyFrameTriggered onKeyFrameTriggered;
        public OnKeyFrameTriggered onFixedKeyFrameTriggered;
        public float duration { get { return animationClip.length; } }
        public AnimationClip clip { get { return animationClip; } }
        public string clipName
        {
            get
            {
                if (ragdollMappingClip == null)
                    return string.Empty;
                return ragdollMappingClip.clipName;
            }           
        }

        public RagdollMappingAnimation()            
        { 
            ragdollMappingClip = CreateInstance(typeof(RagdollMappingClip)) as RagdollMappingClip;
        }

        public void Init(string fieldName, Type monoType, WrapMode wm = WrapMode.Loop)
        {
            if(ragdollMappingClip==null)
            {
                Debug.LogError("clip is null");
                return;
            }

            //clipName = ragdollMappingClip.clipName;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0, 0);
            curve.AddKey(ragdollMappingClip.duration, 1);

            animationClip = new AnimationClip();
            animationClip.wrapMode = wm;
            animationClip.name = clipName;
            animationClip.legacy = true;
            animationClip.SetCurve(string.Empty, monoType, fieldName, curve);

            if (!Application.isEditor)
            {
                for (int i = 0; i < ragdollMappingClip.eventKeyFrames.Length; ++i)
                {
                    AnimationEvent evt = new AnimationEvent();
                    evt.functionName = "OnAnimationEvent";
                    evt.objectReferenceParameter = this;
                    evt.messageOptions = SendMessageOptions.RequireReceiver;
                    evt.intParameter = i;
                    evt.time = ragdollMappingClip.eventKeyFrames[i].keyTime;
                    animationClip.AddEvent(evt);
                }
            }
        }

        void OnAnimationEvent(int index)
        {
            if (index < 0 || index >= ragdollMappingClip.eventKeyFrames.Length)
            {
                Debug.LogError("index is out of range" + index.ToString());
                return;
            }
            Debug.Log("trigger item, " + ragdollMappingClip.eventKeyFrames[index].BoneName);
            if (onKeyFrameTriggered != null)
                onKeyFrameTriggered(ragdollMappingClip.eventKeyFrames[index]);
        }
        public void FixedUpdate()
        {
            if (onFixedKeyFrameTriggered != null)
            {
                foreach (var clip in ragdollMappingClip.fixedKeyFrames)
                    onFixedKeyFrameTriggered(clip);
            }
        }

        public void Play(Animation animation)
        {
            animation.Play(clipName);
        }
    }
}