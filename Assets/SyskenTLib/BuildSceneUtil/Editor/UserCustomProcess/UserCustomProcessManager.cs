using System;
using System.Collections.Generic;
using UnityEngine;

namespace SyskenTLib.BuildSceneUtilEditor
{
    public class UserCustomProcessManager
    {
        
        /// <summary>
        /// ビルド前の処理
        /// </summary>
        /// <param name="buildType"></param>
        public void StartUserOriginalProcessPreBuild(CustomBuildType buildType,List<string> userOriginalParamList,BaseUserOriginalConfig userOriginalConfig)
        {
            //
            // MEMO:ここに各ユーザオリジナルの処理を書いてください
            //
            Debug.Log("ビルド前のユーザオリジナル処理");
            
            switch (buildType)
            {
                case CustomBuildType.Unknown:
                    break;
                case CustomBuildType.Release:
                    break;
                case CustomBuildType.Adhoc1:
                    break;
                case CustomBuildType.Adhoc2:
                    break;
                case CustomBuildType.Development1:
                    break;
                case CustomBuildType.Development2:
                    break;
                case CustomBuildType.Development3:
                    break;
                case CustomBuildType.Private1:
                    break;
                case CustomBuildType.Private2:
                    break;
                case CustomBuildType.Private3:
                    break;
                default:
                    break;
            }
            
        }
        
        /// <summary>
        /// ビルド後の処理
        /// </summary>
        /// <param name="buildType"></param>
        public void StartUserOriginalProcessAfterBuild(CustomBuildType buildType,List<string> userOriginalParamList,BaseUserOriginalConfig userOriginalConfig)
        {
            //
            // MEMO:ここに各ユーザオリジナルの処理を書いてください
            //
            Debug.Log("ビルド後のユーザオリジナル処理");
            
            switch (buildType)
            {
                case CustomBuildType.Unknown:
                    break;
                case CustomBuildType.Release:
                    break;
                case CustomBuildType.Adhoc1:
                    break;
                case CustomBuildType.Adhoc2:
                    break;
                case CustomBuildType.Development1:
                    break;
                case CustomBuildType.Development2:
                    break;
                case CustomBuildType.Development3:
                    break;
                case CustomBuildType.Private1:
                    break;
                case CustomBuildType.Private2:
                    break;
                case CustomBuildType.Private3:
                    break;
                default:
                    break;
            }
        }
    }
}