#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Custom.Tool.AutoBuild
{
    public class AutoBuildEditorWindow : OdinMenuEditorWindow
    {
        /// <summary>
        /// Method that gets called by Menu and created main window
        /// </summary>
        [MenuItem("TBS/AutoBuild")]
        public static void OpenEditorWindow()
        {
            AutoBuildEditorWindow _window = GetWindow<AutoBuildEditorWindow>();
            _window.Show();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            _window.titleContent = new GUIContent("Automatic Build Tool");
        }

        /// <summary>
        /// Method that created a tree for editor window that is just a list.
        /// </summary>
        /// <returns>returns a OdingMneuTree that is created in this window</returns>
        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            OdinMenuStyle style =
            new OdinMenuStyle()
            {
                Height = 23,
                Offset = 20.00f,
                IndentAmount = 15.00f,
                IconSize = 16.00f,
                IconOffset = 0.00f,
                NotSelectedIconAlpha = 0.85f,
                IconPadding = 0.1f,
                TriangleSize = 16.00f,
                TrianglePadding = 0.00f,
                AlignTriangleLeft = true,
                Borders = true,
                BorderPadding = 0.00f,
                BorderAlpha = 0.32f,
                SelectedColorDarkSkin = new Color(0.736f, 0.101f, 0.038f, 1.000f),
                SelectedColorLightSkin = new Color(0.736f, 0.101f, 0.038f, 1.000f)
            };
            tree.DefaultMenuStyle = style;
            tree.Add("Android Parameters", new AndroidParameter());
            tree.Add("iOS Parameters", new IOSParameter());
            tree.Add("Windows Parameters", new WindowsParameter());
            return tree;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnBeginDrawEditors()
        {
            base.OnBeginDrawEditors();
            OdinMenuItem selected = this.MenuTree.Selection.FirstOrDefault();
            int toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                    GUILayout.Label(selected.Name);

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Switch Platform")))
                {
                    BuildManager.Instance.SwitchPlatform(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Upgrade Version")))
                {
                    VersionManager.Instance.UpdateVersion();
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Make a build", "Button used to make a build for current selected platform")))
                {
                    Close();
                    BuildManager.Instance.Build();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
#endif
