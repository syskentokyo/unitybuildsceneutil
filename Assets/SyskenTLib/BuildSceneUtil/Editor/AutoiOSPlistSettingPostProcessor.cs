using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif
namespace SyskenTLib.BuildSceneUtilEditor
{
    public class AutoiOSPlistSettingPostProcessor 
    {
#if UNITY_IOS
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {

            if (buildTarget == BuildTarget.iOS
                && CustomBuild._lastBuildConfig != null)
            {
                string teamID = CustomBuild._lastBuildConfig.overwrittenTeamID_ONIOS;
                string appID = CustomBuild._lastBuildConfig.overwrittenAppID_ONIOS;

                

                string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
                PBXProject pbxProject = new PBXProject();
                pbxProject.ReadFromFile(projectPath);
                
                if (teamID != "" && appID != "")
                {
                    Debug.Log("iOS:設定書き換え");
                    Debug.Log("アプリID " + appID);
                    Debug.Log("TeamID " + teamID);
                    

                    //Main
                    string target = pbxProject.GetUnityMainTargetGuid();
                    pbxProject.SetTeamId(target, teamID);
                    pbxProject.SetBuildProperty(target, "PRODUCT_BUNDLE_IDENTIFIER", appID);
                }

                pbxProject.WriteToFile(projectPath);
                
            }
        }

#endif
    }
}
