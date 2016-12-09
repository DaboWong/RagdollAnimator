using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MappingRagdoll
{
    public static class RagDollMappingUtil
    {
        public static void AutoMapping(GameObject scanRoot, RagdollMappingAnimation[] animations, ref Dictionary<string, Rigidbody> boneMap)
        {
            if (scanRoot == null)
            {
                Debug.LogError("null reference of scan root!");
                return;
            }
            if (animations == null)
            {
                Debug.LogError("null reference of animations");
            }
            if (boneMap == null)
            {
                Debug.LogError("null reference of boneMap");
            }

            foreach (var anim in animations)
            {
                if (anim.ragdollMappingClip != null)
                {
                    foreach (var kf in anim.ragdollMappingClip.eventKeyFrames)
                    {
                        AddBone(scanRoot, kf, ref boneMap);
                    }
                    foreach (var kf in anim.ragdollMappingClip.fixedKeyFrames)
                    {
                        AddBone(scanRoot, kf, ref boneMap);
                    }
                }
            }
        }
        public static void AddBone(GameObject scanRoot, MappingRagdollKeyFrame mkf, ref Dictionary<string, Rigidbody> boneMap)
        {
            Rigidbody result;
            if (boneMap.TryGetValue(mkf.BoneName, out result))
                return;
            var transform = scanRoot.transform.RecursiveFindChild(mkf.BoneName);
            if (transform == null)
            {
                Debug.LogError("missing bone name:" + mkf.BoneName);
                return;
            }
            var rig = transform.GetComponent<Rigidbody>();
            if (rig == null)
            {
                Debug.LogError("transform missing rigidbody, name:" + mkf.BoneName);
            }
            boneMap.Add(mkf.BoneName, rig);
        }

        public static Transform RecursiveFindChild(this Transform transform, string name)
        {
            foreach (Transform t in transform)
            {
                if (t.name == name)
                    return t;
                return RecursiveFindChild(t, name);
            }
            return null;
        }
    }
}