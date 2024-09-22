using UnityEngine;
using UnityEditor;

namespace GridSystem.Debug
{
    [CustomEditor(typeof(GridDebugger))]
    public class GridDebugger_Editor : Editor
    {
        private GridDebugger _target;

        private bool _foldVisibleLayer;

        public override void OnInspectorGUI()
        {
            if (_target == null)
                _target = target as GridDebugger;

            base.OnInspectorGUI();
            EditorGUILayout.Space(10.0f);
            if (_target == null)
                return;

            GUIFunction();
        }

        private void GUIFunction()
        {
            if(_target.LayerCount > 0)
            {
                _foldVisibleLayer = EditorGUILayout.Foldout(_foldVisibleLayer, new GUIContent("µð¹ö±ë Ç¥½Ã"));
                if (_foldVisibleLayer)
                {
                    for (int i = 0; i < _target.LayerCount; i++)
                    {
                        var visible = EditorGUILayout.Toggle(string.Format("layer {0}", i), _target.GetVisible(i));
                        _target.SetVisible(i, visible);
                    }
                }
            }

            _target.Enabled = GUILayout.Toggle(_target.Enabled, _target.Enabled ? "µð¹ö±ë ÁßÁö" : "µð¹ö±ë È°¼ºÈ­", "Button");
        }
    }
}

