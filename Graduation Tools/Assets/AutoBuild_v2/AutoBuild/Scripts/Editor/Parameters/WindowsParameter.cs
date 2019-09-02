#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Custom.Tool.AutoBuild
{
    public class WindowsParameter : Parameter
    {
        public WindowsParameter()
        {
            SetSettings();
            ParameterManager.Instance.RegisterParameter(this);
        }

        public override void SetSettings()
        {
            base.SetSettings();
        }

        protected override void OnValueChange()
        {
            base.OnValueChange();
        }

    }
}
#endif