using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlacementSystem
{
    [CustomEditor(typeof(PlacementMapSettings))]
    public class PlacementMapSettings_Editor : Editor
    {
        private PlacementMapSettings _target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _target = target as PlacementMapSettings;
            if (_target == null)
                return;

            EditorGUILayout.Space(10.0f);
            GUIFunctions();
        }

        private void GUIFunctions()
        {
            if (GUILayout.Button("Create Grid"))
            {
                _target.SetGridData(CreateGridData());
                serializedObject.FindProperty("_gridData").serializedObject.ApplyModifiedProperties();
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private GridData CreateGridData()
        {
            if(_target.GetGridData(out var data) == false)
            {
                data = new GridData();
                data.CellSize = Vector2.one;
                data.Cells = new List<CellData>();
                data.CenterOffset = Vector2.zero;
                data.CellCount = new Vector2Int(10, 10);
            }

            for (int i = 0; i < data.CellCount.y; i++)
            {
                for (int j = 0; j < data.CellCount.x; j++)
                {
                    data.Cells.Add(new CellData()
                    {
                        Coordinate = new Vector2Int(j - data.CellCount.x / 2, i - data.CellCount.y / 2),
                        State = 1
                    });
                }
            }

            return data;
        }
    }
}

