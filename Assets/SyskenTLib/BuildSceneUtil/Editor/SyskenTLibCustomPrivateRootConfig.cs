using UnityEngine;

namespace SyskenTLib.BuildSceneUtilEditor
{
    
    public class SyskenTLibCustomPrivateRootConfig : ScriptableObject
    {
        [Header("プライベートビルド設定")]
        public CustomBuildConfig private1BuildConfig;
        public CustomBuildConfig private2BuildConfig;
        public CustomBuildConfig private3BuildConfig;
    }
}