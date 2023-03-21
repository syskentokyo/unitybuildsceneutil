using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SyskenTLib.BuildSceneUtilEditor
{
    public enum SaveDirectoryType
    {
        AutoCreateDateDir,
        OverwriteLastDir,
        OverwriteSelectDir
    }
    
    
    [CreateAssetMenu(fileName = "CustomBuildConfig", menuName = "CustomBuildConfig", order = 0)]
    public class CustomBuildConfig : ScriptableObject
    {
        [Header("共通")] public bool isDevelopmentBuild = false;
        
        
        [Tooltip("保存先の名前の前につける文字")]
        public string saveDirectoryPreName = "";
        
        [Tooltip("アプリファイルとして保存する場合の名前")]
        public string saveAppName = "";
        
        [Tooltip("保存ディレクトリの決め方")]
        public SaveDirectoryType saveDirectoryType = SaveDirectoryType.AutoCreateDateDir;
        
        [Header("シーン")]
        public List<SceneAsset> _targetSceneList;
        
        
        
        [Header("プラットフォーム別")] 
        [Header("Windows")] 
        
        [Header("Android")]
        
        
        [Header("iOS")] 
        public bool isOverwrittenAppIDAndTeamID_ONIOS = false;
        public string overwrittenAppID_ONIOS = "";
        public string overwrittenTeamID_ONIOS = "";
        
        
        
        
        

    }
}