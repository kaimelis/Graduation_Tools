#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Custom.Tool.AutoBuild
{
    public class ParameterManager
    {
        private static ParameterManager _instance;
        private List<Parameter> _allParameters = new List<Parameter>();

        public static ParameterManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ParameterManager();
                return _instance;
            }
            private set
            {
                _instance = new ParameterManager();
            }
        }
        

        public void RegisterParameter(Parameter param)
        {
            if (param != null)
                _allParameters.Add(param);
        }


        public void PrepareSettings()
        {
            if (_allParameters.Count == 0)
                return;
            foreach (var item in _allParameters)
            {
                item?.PrepareSettings();
            }
        }

        public void UpdateGeneralSettings()
        {
            if (_allParameters.Count == 0)
                return;
            foreach (var item in _allParameters)
            {
                item?.SetSettings();
            }
        }

        public void UpdateAndroidPasswords(string key, string alias)
        {
            PlayerSettings.Android.keystorePass = key;
            PlayerSettings.Android.keyaliasPass = alias;
        }
    }
}
#endif
