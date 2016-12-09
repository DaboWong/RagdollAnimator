using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MappingRagdoll
{
    [System.Serializable]
    public class MappingRagdollKeyFrame
    {
        public float keyTime;        
        public string BoneName;     
        public Vector3 force;
        //TODO
        [HideInInspector]
        public ForceMode foceMode = ForceMode.Impulse;
        [HideInInspector]
        public bool isTorque;
        [HideInInspector]
        public bool isRelative;
    }
    
    [System.Serializable]    
    public class RagdollMappingClip : ScriptableObject
    {
        public float duration;
        public string clipName;
        public MappingRagdollKeyFrame[] eventKeyFrames;
        public MappingRagdollKeyFrame[] fixedKeyFrames;
    }
}
