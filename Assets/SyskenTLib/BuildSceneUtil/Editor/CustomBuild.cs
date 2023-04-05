using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SyskenTLib.CommonAppConfig;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SyskenTLib.BuildSceneUtilEditor
{
    public enum CustomBuildType
    {
        Unknown,
        Release,
        Adhoc1,
        Adhoc2,
        Development1,
        Development2,
        Development3,
        Private1,
        Private2,
        Private3,
            
    }
    public class CustomBuild : EditorWindow
    {


        public const string SAVEKEY_CURRENT_BUILD_ROOT_DIR_PATH = "currentBuildTargetRootDirectoryPath";
        public const string SAVEKEY_LAST_BUILD_DIR_PATH = "lastBuildDirectoryPath";

        /// <summary>
        ///  ビルド保存先のルートフォルダのパス
        /// </summary>
        private static string currentBuildTargetRootDirectoryPath
        {
            get
            {
                string saveValue=  EditorUserSettings.GetConfigValue(SAVEKEY_CURRENT_BUILD_ROOT_DIR_PATH);
                if (saveValue == null)
                {
                    return "";
                }

                return saveValue;
            }

            set
            {
                EditorUserSettings.SetConfigValue(SAVEKEY_CURRENT_BUILD_ROOT_DIR_PATH, value);
            }
            
        }

        /// <summary>
        /// 最後ビルドしたときの設定
        /// </summary>
        public static CustomBuildConfig _lastBuildConfig = null;

        public static CustomBuildType _lastBuildType = CustomBuildType.Unknown;

        /// <summary>
        /// 最後ビルドして保存したディレクトリのパス
        /// </summary>
        public static string _lastBuildDirectoryPath   
        {
            get
            {
                string saveValue=  EditorUserSettings.GetConfigValue(SAVEKEY_LAST_BUILD_DIR_PATH);
                if (saveValue == null)
                {
                    return "";
                }

                return saveValue;
            }

            set
            {
                EditorUserSettings.SetConfigValue(SAVEKEY_LAST_BUILD_DIR_PATH, value);
            }
            
        }

        /// <summary>
        /// プラットフォーム検知を登録済みか
        /// </summary>
        public static bool _isDoneRegistChangePlatformAction = false;
        

        private static readonly string privateRootDirPath = "Assets/SyskenTLib/BuildSceneUtil/Editor/1_PrivateConfig/";
        private static readonly string private1ConfigFileName = "Private1BuildConfig.asset";
        private static readonly string private2ConfigFileName = "Private2BuildConfig.asset";
        private static readonly string private3ConfigFileName = "Private3BuildConfig.asset";
        private static readonly string privateGitignoreFileName = ".gitignore";

        //
        // アプリ設定
        //
        private static AppConfig _backupAppConfig;
        
        //
        // マクロ定義の控え
        //
        private static string _backupDefaultDefines = "";
        private static string _backupAddDefines = "";
        private static string _resultEditDefines = "";
        
        //
        // Android用の設定控え
        //
        private static string _backupAndroidAppID = "";
        private static string _backupAndroidKeyStoreName = "";
        private static string _backupAndroidKeyStorePass = "";
        private static string _backupAndroidKeyStoreKeyAliasName = "";
        private static string _backupAndroidKeyStoreKeyAliasPass = "";

        #region メニュー

        [MenuItem("SyskenTLib/CustomBuild/OpenLastDirectory",priority = 30)]
        private static void OpenLastDirectory()
        {
            RegistChangedPlatform();//プラットフォーム変更を検知開始
            Debug.Log("最後のビルド保存先："+_lastBuildConfig);
            if (_lastBuildDirectoryPath != "")
            {
                Application.OpenURL(_lastBuildDirectoryPath);
            }
        }
        
        [MenuItem("SyskenTLib/CustomBuild/LogOutputLastBuildInfo",priority = 40)]
        private static void LogOutputLastBuildInfo()
        {
            RegistChangedPlatform();//プラットフォーム変更を検知開始
            Debug.Log("最後のビルド保存先："+_lastBuildConfig);
            Debug.Log("最後のビルド種類："+_lastBuildType);

        }

        
        [MenuItem("SyskenTLib/CustomBuild/FindRootConfig",priority = 100)]
        private static void FindRootConfig()
        {
            RegistChangedPlatform();//プラットフォーム変更を検知開始
            SyskenTLibCustomRootConfig rootConfig = GetRootConfig();
            if (rootConfig == null)
            {
                return;
            }

            Selection.activeObject = rootConfig;//UnityEditor上で選択したことにする

        }
        
        [MenuItem("SyskenTLib/CustomBuild/FindRootPrivateConfig",priority = 100)]
        private static void FindRootPrivateConfig()
        {
            RegistChangedPlatform();//プラットフォーム変更を検知開始
            SyskenTLibCustomPrivateRootConfig rootConfig = GetRootPrivateConfig();
            if (rootConfig == null)
            {
                return;
            }

            Selection.activeObject = rootConfig;//UnityEditor上で選択したことにする

        }
        
        [MenuItem("SyskenTLib/CustomBuild/AutoCreatePrivateConfig",priority = 210)]
        private static void AutoCreatePrivateConfig()
        {
            RegistChangedPlatform();//プラットフォーム変更を検知開始
            InitPrivateConfig();

        }

        [MenuItem("SyskenTLib/CustomBuild/ReSelectBuildTargetDirectory", priority = 320)]
        private static void ReSelectBuildTargetRoot()
        {
            RegistChangedPlatform(); //プラットフォーム変更を検知開始
            SelectBuildRootDir();
        }
        
        
        [MenuItem("SyskenTLib/CustomBuild/SelectAppConfigDirtory",priority = 400)]
        private static void SelectAppConfigDirectory()
        {
            RegistChangedPlatform();//プラットフォーム変更を検知開始

            Selection.activeObject = GetAppRootConfig();//UnityEditor上で選択したことにする
        }


        #endregion

        #region プラットフォーム変更検知

        
        private　static void RegistChangedPlatform()
        {
            if (_isDoneRegistChangePlatformAction == false)
            {
                _isDoneRegistChangePlatformAction = true;
                ChangeBuildTargetListener.OnChangedPlatform += OnChangedPlatform;
            }
         
        }

        private static void OnChangedPlatform()
        {
            ChangeBuildTargetListener.OnChangedPlatform -= OnChangedPlatform;
            
            
            Debug.Log("プラットフォーム変更を検知");
            
            //プラットフォームが変わったので、
            //一時保存データをリセットする
            _isDoneRegistChangePlatformAction = false;

            currentBuildTargetRootDirectoryPath = "";
            _lastBuildConfig = new CustomBuildConfig();
            _lastBuildType = CustomBuildType.Unknown;
            _lastBuildDirectoryPath = "";
        }
        
        
        #endregion
        
        
        
        private static void InitPrivateConfig()
        {
            SyskenTLibCustomPrivateRootConfig rootConfig = GetRootPrivateConfig();

            string private1Path = privateRootDirPath + private1ConfigFileName;
            CustomBuildConfig private1config  = AssetDatabase.LoadAssetAtPath<CustomBuildConfig> (private1Path);
            if (private1config == null)
            {
                //設定ファイルがなかったので作成
                Debug.Log("プライベート１の設定ファイル作成");
                CustomBuildConfig config =  CreateInstance<CustomBuildConfig>();
                AssetDatabase.CreateAsset(config, private1Path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            if (rootConfig.private1BuildConfig == null)
            {
                //設定ファイルを紐付ける
                Debug.Log("プライベート１の設定ファイルをルートコンフィグに設定");
                rootConfig.private1BuildConfig = AssetDatabase.LoadAssetAtPath<CustomBuildConfig> (private1Path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            string private2Path = privateRootDirPath + private2ConfigFileName;
            CustomBuildConfig private2config  = AssetDatabase.LoadAssetAtPath<CustomBuildConfig> (private2Path);
            if (private2config == null)
            {
                //設定ファイルがなかったので作成
                Debug.Log("プライベート2の設定ファイル作成");
                CustomBuildConfig config =  CreateInstance<CustomBuildConfig>();
                AssetDatabase.CreateAsset(config, private2Path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            if (rootConfig.private2BuildConfig == null)
            {
                //設定ファイルを紐付ける
                Debug.Log("プライベート2の設定ファイルをルートコンフィグに設定");
                rootConfig.private2BuildConfig =  AssetDatabase.LoadAssetAtPath<CustomBuildConfig> (private2Path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            string private3Path = privateRootDirPath + private3ConfigFileName;
            CustomBuildConfig private3config  = AssetDatabase.LoadAssetAtPath<CustomBuildConfig> (private3Path);
            if (private3config == null)
            {
                //設定ファイルがなかったので作成
                Debug.Log("プライベート3の設定ファイル作成");
                CustomBuildConfig config =  CreateInstance<CustomBuildConfig>();
                AssetDatabase.CreateAsset(config, private3Path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            if (rootConfig.private3BuildConfig == null)
            {
                //設定ファイルを紐付ける
                Debug.Log("プライベート3の設定ファイルをルートコンフィグに設定");
                rootConfig.private3BuildConfig = AssetDatabase.LoadAssetAtPath<CustomBuildConfig> (private3Path);;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }


            string gitignorePath = privateRootDirPath + privateGitignoreFileName;
            if (File.Exists(gitignorePath) == false)
            {
                //設定ファイルがなかったので作成
                Debug.Log("プライベート用に、Gitignoreを作成する");
                string fileText = "";
                fileText += private1ConfigFileName + "\n";
                fileText += private2ConfigFileName + "\n";
                fileText += private3ConfigFileName + "\n";
                
                File.WriteAllText(gitignorePath,fileText);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
        }

        #region パス取得系

        



        private static string GetRootConfigPath()
        {
            string[] guids = AssetDatabase.FindAssets("t:SyskenTLibCustomRootConfig");
            if (guids.Length != 1)
            {
                Debug.Log("ルート設定ファイルがありません。");
                return "";
            }

            string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);

            return filePath;

        }

        private static SyskenTLibCustomRootConfig GetRootConfig()
        {
            string rootConfigPath = GetRootConfigPath();
            if (rootConfigPath == "")
            {
                return null;
            }
            
            Debug.Log("ビルドのルートコンフィグパス："+rootConfigPath);
            return AssetDatabase.LoadAssetAtPath<SyskenTLibCustomRootConfig> (rootConfigPath);
           
        }

        private static string GetRootPrivateConfigPath()
        {
            string[] guids = AssetDatabase.FindAssets("t:SyskenTLibCustomPrivateRootConfig");
            if (guids.Length != 1)
            {
                Debug.Log("プライベートのルート設定ファイルがありません。");
                return "";
            }

            string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);

            return filePath;

        }

        private static SyskenTLibCustomPrivateRootConfig GetRootPrivateConfig()
        {
            string rootConfigPath = GetRootPrivateConfigPath();
            if (rootConfigPath == "")
            {
                return null;
            }
            
            Debug.Log("ビルドのプライベートルートコンフィグパス："+rootConfigPath);
            return AssetDatabase.LoadAssetAtPath<SyskenTLibCustomPrivateRootConfig> (rootConfigPath);
           
        }
        
        
        private static string GetAppRootConfigPath()
        {
            string[] guids = AssetDatabase.FindAssets("t:AppRootConfig");
            if (guids.Length != 1)
            {
                Debug.Log("アプリルート設定ファイルがありません。");
                return "";
            }

            string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);

            return filePath;

        }

        private static AppRootConfig GetAppRootConfig()
        {
            string rootConfigPath = GetAppRootConfigPath();
            if (rootConfigPath == "")
            {
                return null;
            }
            
            Debug.Log("アプリ設定のルートコンフィグパス："+rootConfigPath);
            return AssetDatabase.LoadAssetAtPath<AppRootConfig> (rootConfigPath);
           
        }
        
        
        #endregion

        private static void SelectBuildRootDir()
        {
            //フォルダ選択
            var selectDirpath = EditorUtility.OpenFolderPanel("Select Build Root Directory" ,
                Application.dataPath, string.Empty);
            currentBuildTargetRootDirectoryPath = selectDirpath;
            
            Debug.Log("ビルド保存先のルートディレクトリのパス："+selectDirpath);
            

        }
        

        private static string CreateBuildDirectory(string preDirectoryName,SaveDirectoryType directoryType,bool isNotDirectoryBuild,string appName)
        {
            string buildAppName = "";
            if (isNotDirectoryBuild == true)
            {
                //アプリケーションファイルとして保存する場合
                //アプリファイル名を作成する
                if (appName == "")
                {
                    string datePath = DateTime.Now.ToString( "yyyyMMddHHmmss" );
                    buildAppName = "/main" + datePath;
                }
                else
                {
                    buildAppName = "/" + appName;
      
                }
                            
            }
            

            string buildDirectoryPath = currentBuildTargetRootDirectoryPath;
            
            switch (directoryType)
            {
                case SaveDirectoryType.OverwriteLastDir:
                {
                    if (_lastBuildDirectoryPath == "")
                    {
                        //ビルドしたことがなく、まだパスがなかった場合
                        string datePath = DateTime.Now.ToString( "yyyyMMddHHmmss" );
                        buildDirectoryPath += "/" + preDirectoryName+datePath+buildAppName;
                    }
                    else
                    {
                        //前回のフォルダを選ぶ
                        buildDirectoryPath = _lastBuildDirectoryPath;
                    }
                    
                    
                    break;
                }
                
                case SaveDirectoryType.OverwriteSelectDir:
                {
                    //選択する
                    var selectDirpath = EditorUtility.OpenFolderPanel("Select Save Directory" ,
                        Application.dataPath, string.Empty);
                    if (string.IsNullOrEmpty(selectDirpath))
                    {
                        //選択しなかった場合は、日時フォルダ
                        string datePath = DateTime.Now.ToString( "yyyyMMddHHmmss" );
                        buildDirectoryPath += "/" + preDirectoryName+datePath+buildAppName;
                    }
                    else
                    {
                        buildDirectoryPath = selectDirpath+buildAppName;
                    }

                    break;
                }
                
                case SaveDirectoryType.AutoCreateDateDir:
                {
                    //日時フォルダをつくってつかう
                    string datePath = DateTime.Now.ToString( "yyyyMMddHHmmss" );
                    buildDirectoryPath += "/" +preDirectoryName+ datePath+buildAppName;
                    break;
                }
                
            }


            return buildDirectoryPath;
        }



        #region ここからビルド

        


        
        
        
        [MenuItem("File/BuildRelease",priority = 310)]
        private static void BuildRelease()
        {
            StartBuild(CustomBuildType.Release);
        }
        
        [MenuItem("File/BuildAdhoc1",priority = 310)]
        private static void BuildAdhoc1()
        {
            StartBuild(CustomBuildType.Adhoc1);
        }
        
        [MenuItem("File/BuildAdhoc2",priority = 310)]
        private static void BuildAdhoc2()
        {
            StartBuild(CustomBuildType.Adhoc2);
        }
        
        [MenuItem("File/BuildDevelopment1",priority = 410)]
        private static void BuildDevelopment1()
        {
            StartBuild(CustomBuildType.Development1);
        }
        
        [MenuItem("File/BuildDevelopment2",priority = 410)]
        private static void BuildDevelopment2()
        {
            StartBuild(CustomBuildType.Development2);
        }
        
        [MenuItem("File/BuildDevelopment3",priority = 410)]
        private static void BuildDevelopment3()
        {
            StartBuild(CustomBuildType.Development3);
        }
        
        [MenuItem("File/BuildPrivate1",priority = 510)]
        private static void BuildPrivate1()
        {
            StartBuild(CustomBuildType.Private1);
        }
        
        [MenuItem("File/BuildPrivate2",priority = 510)]
        private static void BuildPrivate2()
        {
            StartBuild(CustomBuildType.Private2);
        }
        
        [MenuItem("File/BuildPrivate3",priority = 510)]
        private static void BuildPrivate3()
        {
            StartBuild(CustomBuildType.Private3);
        }





        private static void StartBuild(CustomBuildType buildType)
        {
            RegistChangedPlatform();//プラットフォーム変更を検知開始
            
            Debug.Log("ビルドスタート:"+buildType);

            //ルートフォルダ選択
            if (string.IsNullOrEmpty(currentBuildTargetRootDirectoryPath))
            {
                SelectBuildRootDir();
            }

            if (string.IsNullOrEmpty(currentBuildTargetRootDirectoryPath))
            {
                Debug.Log("保存先が選ばれてません。");
                return;
            }
            

            CustomBuildConfig nextBuildConfig = null;
            
            switch (buildType)
            {
                case CustomBuildType.Release:
                {
                    nextBuildConfig = GetRootConfig().releaseBuildConfig;
                }
                    break;
                case CustomBuildType.Adhoc1:
                {
                    nextBuildConfig = GetRootConfig().adhoc1BuildConfig;
                }
                    break;
                case CustomBuildType.Adhoc2:
                {
                    nextBuildConfig = GetRootConfig().adhoc2BuildConfig;
                }
                    break;
                case CustomBuildType.Development1:
                {
                    nextBuildConfig = GetRootConfig().development1BuildConfig;
                }
                    break;
                
                case CustomBuildType.Development2:
                {
                    nextBuildConfig = GetRootConfig().development2BuildConfig;
                }
                    break;
                case CustomBuildType.Development3:
                {
                    nextBuildConfig = GetRootConfig().development3BuildConfig;
                }
                    break;
                case CustomBuildType.Private1:
                {
                    InitPrivateConfig();//プライベートコンフィグファイルを作成（ない場合のみ）
                    nextBuildConfig = GetRootPrivateConfig().private1BuildConfig;
                }
                    break;
                
                case CustomBuildType.Private2:
                {
                    InitPrivateConfig();//プライベートコンフィグファイルを作成（ない場合のみ）
                    nextBuildConfig = GetRootPrivateConfig().private2BuildConfig;
                }
                    break;
                
                case CustomBuildType.Private3:
                {
                    InitPrivateConfig();//プライベートコンフィグファイルを作成（ない場合のみ）
                    nextBuildConfig = GetRootPrivateConfig().private3BuildConfig;
                }
                    break;
            }


            if (nextBuildConfig == null)
            {
                Debug.Log("ビルドの設定ファイルがない");
                return;
             
            }
            
            //
            // 最後のビルド設定を覚えておく
            //
            _lastBuildConfig = nextBuildConfig;
            _lastBuildType = buildType;
            
            //ここからビルドしていく
            BuildOnConfig(nextBuildConfig,buildType);

        }


        private static void BuildOnConfig(CustomBuildConfig config,CustomBuildType buildType)
        {

            
            //ビルドシーン整理
            List<SceneAsset> buildSceneList = config._targetSceneList;
            if (buildSceneList.Count == 0)
            {
                Debug.LogError("ビルド対象のシーンが設定されていません。");
                return;
            }

            EditorBuildSettingsScene[] buildSceneArray = buildSceneList.Select(buildScene =>
                new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(buildScene), true)).ToArray();
            
            
            //プラットフォーム
            BuildTarget buildTarget= EditorUserBuildSettings.activeBuildTarget;
            bool isSaveAppFileTypePlatform = buildTarget == BuildTarget.Android
                                             || buildTarget == BuildTarget.StandaloneWindows64
                                             || buildTarget == BuildTarget.StandaloneWindows
                                             || buildTarget == BuildTarget.StandaloneOSX;//アプリファイルとして保存するプラットフォームか。


            //保存先
            string appName = config.saveAppName;
            string saveDirectoryPath = CreateBuildDirectory(config.saveDirectoryPreName,config.saveDirectoryType,isSaveAppFileTypePlatform,appName);
           
            //
            // アプリ設定ファイル切り替え
            //
           
            _backupAppConfig = null;
            if (config._appConfig != null)
            {
                AppRootConfig appRootConfig = GetAppRootConfig();
                _backupAppConfig = appRootConfig._appConfig;
                appRootConfig._appConfig = config._appConfig;
                Debug.Log("アプリ設定書き換え:"+appRootConfig._appConfig.GetConfigType());
                EditorUtility.SetDirty(appRootConfig);

            }

            //
            // マクロ定義
            //
            AutoAddDefinesPreBuild(buildTarget, config);
            
            //
            // ユーザオリジナルの処理
            //
            UserCustomProcessManager userCustomProcessManager = new UserCustomProcessManager();
            if (config.isActiveUserOriginalProcessPreBuild)
            {
                userCustomProcessManager.StartUserOriginalProcessPreBuild(buildType,config.userOriginalParamList,config.userOriginalConfig);
            }

            //ビルドオプション
            BuildOptions nextBuildOptions = BuildOptions.None;
            
            if (config.isClearBuildCache)
            {
                nextBuildOptions |= BuildOptions.CleanBuildCache;
            }
            
            //ビルドオプション：デバッグビルド
            if (config.isDevelopmentBuild)
            {
                nextBuildOptions |= BuildOptions.Development;
            }
            
                        
            //ビルドオプション：ビルド後のフォルダを開く処理
            if (IsAutoOpenDirectoryAfterBuild(buildTarget, saveDirectoryPath, config))
            {
                nextBuildOptions |= BuildOptions.ShowBuiltPlayer;
            }
            
            //ビルドオプション：ビルド後アプリを実行する
            if (IsBuildAndRun(buildTarget, saveDirectoryPath, config))
            {
                nextBuildOptions |= BuildOptions.AutoRunPlayer;
            }
            
            if (config.isConnectWithProfiler)
            {
                nextBuildOptions |= BuildOptions.ConnectWithProfiler;
            }
            
            if (config.isDeepPriling)
            {
                nextBuildOptions |= BuildOptions.EnableDeepProfilingSupport;
            }
            
            if (config.isAllowDebugging)
            {
                nextBuildOptions |= BuildOptions.AllowDebugging;
            }
            
            //
            // Android用の設定
            //
            AutoChangeConfigPreBuildOnAndroid(buildTarget, config);
            
                        
            //ここまでの変更をプロジェクト反映
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
           //
           // ビルド実行
           //
            BuildReport buildReport = BuildPipeline.BuildPlayer(
                buildSceneArray
                ,saveDirectoryPath
                ,buildTarget
                ,nextBuildOptions     
            );

            AppRootConfig currentappRootConfig = GetAppRootConfig();
            Debug.Log("ビルド結果:アプリ設定："+   currentappRootConfig._appConfig.GetConfigType());
            
            string buildSceneLog = "";
            buildSceneArray.ToList().ForEach(buildScene => { buildSceneLog+= "" + buildScene.path+"\n"; });
            Debug.Log("ビルド結果："+buildReport.summary.result);
            Debug.Log("ビルド結果:シーン="+buildSceneLog);
            Debug.Log("ビルド結果:ビルド時間="+buildReport.summary.totalTime);
            Debug.Log("ビルド結果:ビルドしたファイル数="+buildReport.files.Length);
            Debug.Log("ビルド結果:エラー数="+buildReport.summary.totalErrors);
            Debug.Log("ビルド結果:警告数="+buildReport.summary.totalWarnings);
            Debug.Log("ビルド結果:ビルドオプション="+nextBuildOptions);
            Debug.Log("ビルド結果:マクロ定義："+ _resultEditDefines);
            Debug.Log("ビルド結果:保存先："+ saveDirectoryPath);


            //ビルド保存先をおぼえておく
            _lastBuildDirectoryPath = saveDirectoryPath;

            
            //一部のプラットフォームは、独自Runする
            AutoRunAfterBuild(buildTarget, saveDirectoryPath, config);
            
                        
            //
            // ユーザオリジナルの処理
            //
            if (config.isActiveUserOriginalProcessAfterBuild)
            {
                userCustomProcessManager.StartUserOriginalProcessAfterBuild(buildType,config.userOriginalParamList,config.userOriginalConfig);
            }
            
            //
            // Android用の設定を戻す
            //
            AutoChangeConfigAfterBuildOnAndroid(buildTarget, config);
            
            //
            // マクロ定義を戻す
            //
            AutoAddDefinesAfterBuild(buildTarget, config);
            
            
            
            //
            // アプリの設定戻す
            //
            if (_backupAppConfig != null)
            {
                AppRootConfig appRootConfig = GetAppRootConfig();
                appRootConfig._appConfig = _backupAppConfig;
                EditorUtility.SetDirty(appRootConfig);
            }
            
            
            //プロジェクト反映
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private static bool IsAutoOpenDirectoryAfterBuild(BuildTarget buildTarget, string savePath, CustomBuildConfig config)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                {
                    return config.isAutoOpenDirectoryAfterBuild_ONANDROID;

                }
                    
                
                case BuildTarget.iOS:
                {
                    return config.isAutoOpenDirectoryAfterBuild_ONIOS;
                }
           
                
                case BuildTarget.StandaloneWindows:
                {
                    return config.isAutoOpenDirectoryAfterBuild_ONWINDOWS;
                }
                   
                
                case BuildTarget.StandaloneWindows64:
                {
                    return config.isAutoOpenDirectoryAfterBuild_ONWINDOWS;
                }
                
                case BuildTarget.StandaloneOSX:
                {
                    return config.isAutoOpenDirectoryAfterBuild_ONOSX;
                }
                    
            }

            return false;
        }

        private static bool IsBuildAndRun(BuildTarget buildTarget, string savePath, CustomBuildConfig config)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                {
                    return config.isAutoRunAfterBuild_ONANDROID;
                }


                case BuildTarget.iOS:
                {
                    return config.isAutoRunAfterBuild_ONIOS;
                }


                case BuildTarget.StandaloneWindows:
                {
                    break;
                }


                case BuildTarget.StandaloneWindows64:
                {
                    break;
                }


                case BuildTarget.StandaloneOSX:
                {
                    break;
                }

            }
            return false;
        }

        
        /// <summary>
        /// 一部のプラットフォームは、Build&Runがうまくうごかないので、独自で実行する
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <param name="savePath"></param>
        /// <param name="config"></param>
        private static void AutoRunAfterBuild(BuildTarget buildTarget, string savePath, CustomBuildConfig config)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                {
                    if (config.isAutoRunAfterBuild_ONANDROID)
                    {

                    }
                    
                }
                    break;
                
                case BuildTarget.iOS:
                {
                    if (config.isAutoRunAfterBuild_ONIOS)
                    {
                        
                    }
                }
                    break;
                
                case BuildTarget.StandaloneWindows:
                {
                    if (config.isAutoRunAfterBuild_ONWINDOWS)
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = savePath+".exe";
                        proc.Start();
                        // Application.OpenURL("file://"+savePath+".exe");
                    }
                }
                    break;
                
                case BuildTarget.StandaloneWindows64:
                {
                    if (config.isAutoRunAfterBuild_ONWINDOWS)
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = savePath+".exe";
                        proc.Start();
                        // Application.OpenURL("file://"+savePath+".exe");
                    }
                }
                    break;
                
                case BuildTarget.StandaloneOSX:
                {
                    if (config.isAutoRunAfterBuild_ONOSX)
                    {
                        Application.OpenURL("file://"+savePath+".app");
                    }
                }
                    break;
            }
        }
        
        
        #endregion

        #region マクロ定義

        private static void AutoAddDefinesPreBuild(BuildTarget buildTarget, CustomBuildConfig config)
        {
            BuildTargetGroup buildTargetGroup = ConvertToBuildTargetGroup(buildTarget);
            _backupDefaultDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            _backupAddDefines = "";
            


            for (int i = 0; i < config.addDefineList.Count; i++)
            {
                _backupAddDefines +=config.addDefineList[i];
                if (i != (config.addDefineList.Count - 1))
                {
                    //最後以外はくっつける
                    _backupAddDefines += ";";
                }
            }
            
            _resultEditDefines = _backupDefaultDefines;
            if (_backupDefaultDefines != "" && config.addDefineList.Count > 0)
            {
                _resultEditDefines += ";"+_backupAddDefines;
            }
            else
            {
                _resultEditDefines += _backupAddDefines;
            }
            
            
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,_resultEditDefines);
        }

        private static void AutoAddDefinesAfterBuild(BuildTarget buildTarget, CustomBuildConfig config)
        {
            BuildTargetGroup buildTargetGroup = ConvertToBuildTargetGroup(buildTarget);
            Debug.Log("戻したマクロ定義："+_backupDefaultDefines);
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,_backupDefaultDefines);
            
        }

        private static BuildTargetGroup ConvertToBuildTargetGroup(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSX:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                
            }


            return BuildTargetGroup.Unknown;
        }
        #endregion


        #region Android

        private static void AutoChangeConfigPreBuildOnAndroid(BuildTarget buildTarget, CustomBuildConfig config)
        {
            if (buildTarget != BuildTarget.Android) return;
            
            
            Debug.Log("Androidはビルド前に、Unityのプロジェクト設定を書き換えます");
            
            //設定上書き
            
            //ビルドオプション
            EditorUserBuildSettings.buildAppBundle = config.isOverwrittenBuildAppBundle_ONANDROID;

            if (config.isOverwrittenAppID_ONANDROID)
            {
                Debug.Log("AndroidのアプリID書き換え："+config.overwrittenAppID_ONANDROID);
                _backupAndroidAppID = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                
                //アプリID書き換え
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, config.overwrittenAppID_ONANDROID);
            }

            if (config.isOverwrittenKeystoreConfig_ONANDROID)
            {
                Debug.Log("AndroidのKeyStore設定書き換え："+config.overwrittenAppID_ONANDROID);
                
                _backupAndroidKeyStoreName = PlayerSettings.Android.keystoreName;
                _backupAndroidKeyStorePass = PlayerSettings.Android.keystorePass;
                _backupAndroidKeyStoreKeyAliasName = PlayerSettings.Android.keyaliasName;
                _backupAndroidKeyStoreKeyAliasPass = PlayerSettings.Android.keyaliasPass;
                
                //KeyStore書き換え
                PlayerSettings.Android.keystoreName = config.overwrittenKeystoreName_ONANDROID;
                PlayerSettings.Android.keystorePass = config.overwrittenKeystorePass_ONANDROID;
                PlayerSettings.Android.keyaliasName = config.overwrittenKeystoreKeyAliasName_ONANDROID;
                PlayerSettings.Android.keyaliasPass = config.overwrittenKeystoreKeyAliasPass_ONANDROID;
            }
        }
        
        private static void AutoChangeConfigAfterBuildOnAndroid(BuildTarget buildTarget, CustomBuildConfig config)
        {
            if (buildTarget != BuildTarget.Android) return;
            
            Debug.Log("Androidはビルド後に、Unityのプロジェクト設定を書き換えたものを戻します");

            if (config.isOverwrittenAppID_ONANDROID)
            {
                //アプリID書き換え戻す
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, _backupAndroidAppID);
            }

            if (config.isOverwrittenKeystoreConfig_ONANDROID)
            {
                //KeyStore書き換え戻す
                PlayerSettings.Android.keystoreName = _backupAndroidKeyStoreName;
                PlayerSettings.Android.keystorePass = _backupAndroidKeyStorePass;
                PlayerSettings.Android.keyaliasName = _backupAndroidKeyStoreKeyAliasName;
                PlayerSettings.Android.keyaliasPass = _backupAndroidKeyStoreKeyAliasPass;
            }
            
        }
        

        #endregion

    }
}