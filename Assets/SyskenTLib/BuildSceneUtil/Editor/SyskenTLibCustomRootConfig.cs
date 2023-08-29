using UnityEngine;

namespace SyskenTLib.BuildSceneUtilEditor
{

    public class SyskenTLibCustomRootConfig : ScriptableObject
    {
        
        [Header("プロジェクト共有のビルド設定")]
        public CustomBuildConfig releaseBuildConfig;
        public CustomBuildConfig adhoc1BuildConfig;
        public CustomBuildConfig adhoc2BuildConfig;
        public CustomBuildConfig development1BuildConfig;
        public CustomBuildConfig development2BuildConfig;
        public CustomBuildConfig development3BuildConfig;
        
        
    }
}