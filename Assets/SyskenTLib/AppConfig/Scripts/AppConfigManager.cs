using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SyskenTLib.CommonAppConfig
{
    [DefaultExecutionOrder(-999)]//一番最初に生成する。定数であるため。
    public class AppConfigManager : MonoBehaviour
    {

        [SerializeField] private AppRootConfig _appRootConfig;

        public static AppConfigManager instance;
        

        private void Awake()
        {
            if( instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public AppConfig GetCurrentConfig()
        {
            
            return _appRootConfig._appConfig;
        }
        
    }
}
