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
    
    

    public class CustomBuildConfig : ScriptableObject
    {
        [Header("共通")] public bool isClearBuildCache = false;
        public bool isDevelopmentBuild = false;

        public bool isConnectWithProfiler = false;
        public bool isDeepPriling = false;
        public bool isAllowDebugging = false;
        
        
        [Tooltip("保存先の名前の前につける文字")]
        public string saveDirectoryPreName = "";
        
        [Tooltip("アプリファイルとして保存する場合の名前")]
        public string saveAppName = "";
        
        [Tooltip("保存ディレクトリの決め方")]
        public SaveDirectoryType saveDirectoryType = SaveDirectoryType.AutoCreateDateDir;

        [Header("アプリ内定義")] public CommonAppConfig.AppConfig _appConfig;
        
        
        [Header("ユーザオリジナル処理")]
        public List<string> userOriginalParamList;
        public BaseUserOriginalConfig userOriginalConfig;
        
        
        [Header("マクロ定義")] public List<string> addDefineList=new List<string>();
        
        [Header("シーン")]
        public List<SceneAsset> _targetSceneList;



        [Header("プラットフォーム別")]
        
        [Header("Windows")]
        public bool isAutoOpenDirectoryAfterBuild_ONWINDOWS = false;
        public bool isAutoRunAfterBuild_ONWINDOWS = false;
        
        [Header("Mac OSX")]
        public bool isAutoOpenDirectoryAfterBuild_ONOSX = false;
        public bool isAutoRunAfterBuild_ONOSX = false;
        
        [Header("Android")]
        public bool isAutoOpenDirectoryAfterBuild_ONANDROID = false;
        public bool isAutoRunAfterBuild_ONANDROID = false;
        
        public bool isOverwrittenBuildAppBundle_ONANDROID = false;
        
        public bool isOverwrittenAppID_ONANDROID = false;
        public string overwrittenAppID_ONANDROID = "";
        
        public bool isOverwrittenKeystoreConfig_ONANDROID = false;
        public string overwrittenKeystoreName_ONANDROID = "";
        public string overwrittenKeystorePass_ONANDROID = "";
        public string overwrittenKeystoreKeyAliasName_ONANDROID = "";
        public string overwrittenKeystoreKeyAliasPass_ONANDROID = "";

        [Header("iOS")] 
        public bool isAutoOpenDirectoryAfterBuild_ONIOS = false;
        public bool isAutoRunAfterBuild_ONIOS = false;
        
        public bool isOverwrittenAppIDAndTeamID_ONIOS = false;
        public string overwrittenAppID_ONIOS = "";
        public string overwrittenTeamID_ONIOS = "";
        
        
        
        
        

    }
}