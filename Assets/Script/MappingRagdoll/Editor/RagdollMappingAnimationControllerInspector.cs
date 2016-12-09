using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.MappingRagdoll;

[CustomEditor(typeof(Assets.Scripts.MappingRagdoll.RagdollMappingAnimationController))]
class RagdollMappingAnimationControllerInspector : Editor
{
    RagdollMappingAnimationController ragdollMappingCtlr;    

    public override void OnInspectorGUI()
    {
        if (ragdollMappingCtlr == null)
            ragdollMappingCtlr = target as RagdollMappingAnimationController;

        base.OnInspectorGUI();       
    }
}
