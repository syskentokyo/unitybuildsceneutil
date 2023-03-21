using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SyskenTLib.BuildSceneUtilEditor
{
    public class CustomBuild : EditorWindow
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

        

        private static readonly string privateRootDirPath = "Assets/SyskenTLib/BuildSceneUtil/Editor/1_PrivateConfig/";
        private static readonly string private1ConfigFileName = "Private1BuildConfig.asset";
        private static readonly string private2ConfigFileName = "Private2BuildConfig.asset";
        private static readonly string private3ConfigFileName = "Private3BuildConfig.asset";
        private static readonly string privateGitignoreFileName = ".gitignore";


        #region メニュー

        [MenuItem("SyskenTLib/CustomBuild/OpenLastDirectory",priority = 30)]
        private static void OpenLastDirectory()
        {
            Debug.Log("最後のビルド保存先："+_lastBuildConfig);
            if (_lastBuildDirectoryPath != "")
            {
                Application.OpenURL(_lastBuildDirectoryPath);
            }
        }
        
        [MenuItem("SyskenTLib/CustomBuild/LogOutputLastBuildInfo",priority = 40)]
        private static void LogOutputLastBuildInfo()
        {
            Debug.Log("最後のビルド保存先："+_lastBuildConfig);
            Debug.Log("最後のビルド種類："+_lastBuildType);

        }

        
        [MenuItem("SyskenTLib/CustomBuild/FindRootConfig",priority = 100)]
        private static void FindRootConfig()
        {
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
            InitPrivateConfig();

        }

        [MenuItem("SyskenTLib/CustomBuild/ReSelectBuildTargetDirectory",priority = 320)]
        private static void ReSelectBuildTargetRoot()
        {
            SelectBuildRootDir();
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
            BuildOnConfig(nextBuildConfig);

        }


        private static void BuildOnConfig(CustomBuildConfig config)
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
           
            
            //ビルドオプション
            
            
           
            BuildReport buildReport = BuildPipeline.BuildPlayer(
                buildSceneArray
                ,saveDirectoryPath
                ,buildTarget
                ,BuildOptions.None        
            );
            
            Debug.Log("ビルド結果："+buildReport.summary.result);
            Debug.Log("ビルド結果: ビルド時間="+buildReport.summary.totalTime);
            Debug.Log("ビルド結果: ビルドしたファイル数="+buildReport.files.Length);
            
            
            
            //ビルド保存先をおぼえておく
            _lastBuildDirectoryPath = saveDirectoryPath;
        }
        

        
        #endregion

        private void OnGUI()
        {
            
        }
    }
}