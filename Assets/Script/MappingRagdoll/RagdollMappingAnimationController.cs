using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MappingRagdoll
{

    [RequireComponent(typeof(RagdollMappingData))]
    public class RagdollMappingAnimationController : MonoBehaviour
    {
        public RagdollMappingAnimation[] animations;
        protected RagdollMappingAnimation currAnimation;
        protected Dictionary<string, Rigidbody> relativeTransformBind = new Dictionary<string, Rigidbody>();
        protected float interval;
        private bool isInit;
        [HideInInspector]
        Animation animControll;
        public float uselessField;
        public string defaultAnim;

        protected bool isPlaying { get { return currAnimation != null || animControll.isPlaying; } }
        void Start()
        {
            if (!isInit)
                Init();
        }
        protected virtual void FixedUpdate()
        {
            if (currAnimation != null)
                currAnimation.FixedUpdate();
        }
        protected virtual void Init()
        {
            if (isInit)
                return;
            isInit = true;
            RagDollMappingUtil.AutoMapping(gameObject, animations, ref relativeTransformBind);
            animControll = GetComponent<Animation>();
            if (animControll == null)
            {
                animControll = gameObject.AddComponent<Animation>();
                animControll.playAutomatically = false;            
            }
            foreach(AnimationState cs in animControll)
            {
                animControll.RemoveClip(cs.clip.name);
            }
            foreach (var anim in animations )
            {
                anim.Init("uselessField", typeof(RagdollMappingAnimationController), WrapMode.Loop);
                print("add clip, name:" + anim.clipName);
                animControll.AddClip(anim.clip, anim.clipName);
            }
        }

        public virtual void Play(string animationName)
        {
            if (Application.isEditor)
                isInit = false;

            if(!isInit)
                Init();
            print("being play " + animationName);
            RagdollMappingAnimation rma = FindByName(animationName);
            if (rma == null)
            {
                Debug.LogError("animation does not exist! name:" + animationName);
                return;
            }
            interval = 0;
            if (isPlaying)
                Stop();
            currAnimation = rma;
            currAnimation.Play(animControll);
            RegCallBack(currAnimation);
        }

        public RagdollMappingAnimation FindByName(string name)
        {
            //animations.Find(t => t.clipName == animationName);
            for (int i = 0; i < animations.Length; ++i)
            {
                if (name == animations[i].name)
                    return animations[i];
            }
            return null;
        }

        void Play(RagdollMappingAnimation rma)
        {
            rma.Play(animControll);
        }

        public virtual void Stop()
        {
            if (currAnimation != null)
                print("stop currAnimation:" + currAnimation.clipName + "interval:" + interval);
            UnRegCallback(currAnimation);
            animControll.Stop(currAnimation.clipName);
            currAnimation = null;
        }
        protected Rigidbody GetBone(string boneName)
        {
            Rigidbody result;
            if (relativeTransformBind.TryGetValue(boneName, out result))
                return result;
            return null;
        }

        protected void RegCallBack(RagdollMappingAnimation rma)
        {
            rma.onKeyFrameTriggered += OnKeyFrameTriggered;
            rma.onFixedKeyFrameTriggered += OnFixedFrameTriggered;
        }

        protected void UnRegCallback(RagdollMappingAnimation rma)
        {
            rma.onKeyFrameTriggered += OnKeyFrameTriggered;
            rma.onFixedKeyFrameTriggered += OnFixedFrameTriggered;
        }

        protected virtual void OnFixedFrameTriggered(MappingRagdollKeyFrame mk)
        {
            AddForce(mk);
        }
        protected virtual void OnKeyFrameTriggered(MappingRagdollKeyFrame mk)
        {
            AddForce(mk);
        }
        #region addforce
        protected virtual void AddForce(MappingRagdollKeyFrame mk)
        {
            if (mk == null)
                return;
            Rigidbody bone = GetBone(mk.BoneName);
            if (bone == null)
            {
                Debug.LogError("missing bone name:" + mk.BoneName);
                return;
            }
            if (bone.isKinematic)
            {
                Debug.LogWarning(string.Format("Bone {0} is Kinematic, key frame is ignored!", mk.BoneName));
            }
            print("add force to bone:" + mk.BoneName.ToString() + "|" + mk.force.ToString());
            bone.AddForce(mk.force, ForceMode.Impulse);
        }
        #endregion
    }
}