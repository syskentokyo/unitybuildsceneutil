using System.Collections;
using System.Collections.Generic;
using SyskenTLib.CommonAppConfig;
using UnityEngine;

namespace SyskenTLib.BuildSceneUtilDemo
{
    public class DebugAppConfigManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("アプリ設定："+ AppConfigManager.instance.GetCurrentConfig().GetConfigType());
            Debug.Log("アプリ設定 API_URL1："+ AppConfigManager.instance.GetCurrentConfig().GetAPIURL1());
        }
    
    }
}
