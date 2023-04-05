using UnityEngine;

namespace SyskenTLib.CommonAppConfig
{
    [CreateAssetMenu(fileName = "AppRootConfig", menuName = "AppRootConfig", order = 0)]
    public class AppRootConfig : ScriptableObject
    {
        public AppConfig _appConfig;
    }
}