using UnityEngine;

namespace SyskenTLib.CommonAppConfig
{
    public enum ConfigType
    {
        UnityEditor=-1,
        Release=0,
        Adhoc1=1,
        Adhoc2=2,
        Dev1=3,
        Dev2=4,
        Dev3=5,
        Private1=6,
        Private2=7,
    }
    
    
    [UnityEngine.CreateAssetMenu(fileName = "AppConfig", menuName = "AppConfig", order = 0)]
    public class AppConfig : ScriptableObject
    {


        [Header("設定ファイルについて")]
        [SerializeField]private ConfigType _configType = ConfigType.Dev1;

        [Header("共通")] 
        

        [Header("API")] 
        [SerializeField]
        private  string API_URL1 = "";
        [SerializeField]private  string API_URL2 = "";
        [SerializeField]private  string API_URL3 = "";
        [SerializeField]private  string API_URL4 = "";
        [SerializeField] private  string API_URL5 = "";
        
        [SerializeField] private  string API_KEY1 = "";
        [SerializeField] private  string API_KEY2 = "";


        [Header("Auth")] [SerializeField] private string USER_NAME1 = "";
        [SerializeField] private string PASSWORD_NAME1 = "";
        [SerializeField] private string USER_NAME2 = "";
        [SerializeField] private string PASSWORD_NAME2 = "";


        public ConfigType GetConfigType()
        {
            return _configType;
        }

        public string GetAPIURL1()
        {
            return API_URL1;
        }
        public string GetAPIURL2()
        {
            return API_URL2;
        }
        public string GetAPIURL3()
        {
            return API_URL3;
        }
        public string GetAPIURL4()
        {
            return API_URL4;
        }
        
        public string GetAPIURL5()
        {
            return API_URL5;
        }
        
        public string GetAPIKey1()
        {
            return API_KEY1;
        }
        
        public string GetAPIKey2()
        {
            return API_KEY2;
        }
        
        public string GetUserName1()
        {
            return USER_NAME1;
        }
        public string GetPassword1()
        {
            return PASSWORD_NAME1;
        }
        public string GetUserName2()
        {
            return USER_NAME2;
        }
        public string GetPassword2()
        {
            return PASSWORD_NAME2;
        }



    }
}