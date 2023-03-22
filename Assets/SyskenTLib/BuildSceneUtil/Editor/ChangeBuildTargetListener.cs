using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;


namespace SyskenTLib.BuildSceneUtilEditor
{
    public class ChangeBuildTargetListener: IActiveBuildTargetChanged
    {
        public static Action OnChangedPlatform;
        
        public int callbackOrder { get { return 0; } }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Debug.Log("OnActiveBuildTargetChanged");
            OnChangedPlatform?.Invoke();;
        }
    }
}