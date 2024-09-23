using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Component
{
    public class GridDrawer : MonoBehaviour
    {
        static readonly int HASH_LINE_COLOR = Shader.PropertyToID("_LineColor");
        static readonly int HASH_LINE_THICKNESS = Shader.PropertyToID("_LineThickness");
        static readonly int HASH_LINE_CELL_COUNT = Shader.PropertyToID("_CellCount");

        public GridDrawerSettings settings;
        public GameObject gridPlanePrefab;
        private GridViwer _gridViewer;
        private Vector3 _prevCenter;
        private bool _inited = false;
        private Transform _gridPlane;
        private Material _gridMaterial;

        private void Update()
        {
            if (_gridViewer == null)
                return;
            if (!_inited)
            {
                Initialize();
            }
            if (!_inited)
                return;

            if (Vector3.Distance(_gridViewer.Center, _prevCenter) > 0.1f)
            {
                DrawGrid();
                _prevCenter = _gridViewer.Center;
            }
        }

        public void SetGrid(GridViwer gridViewer)
        {
            _gridViewer = gridViewer; 
        }

        public virtual void Initialize()
        {
            SetRenderer();
            _inited = true;
        }

        public virtual void DrawGrid()
        {
            if (_gridViewer == null)
                return;

            if (_gridMaterial != null)
            {
                _gridMaterial.SetVector(HASH_LINE_CELL_COUNT, new Vector4(_gridViewer.CellCount.x, _gridViewer.CellCount.y, 0.0f, 0.0f));
            }
            if(_gridPlane != null)
            {
                _gridPlane.localScale = new Vector3(_gridViewer.CellCount.x * _gridViewer.CellSize.x, _gridViewer.CellCount.y * _gridViewer.CellSize.y, 1.0f);
                _gridPlane.localPosition = new Vector3(_gridViewer.CellSize.x * 0.5f, 0.0f, _gridViewer.CellSize.y * 0.5f) * -1f;
            }
        }

        private void SetRenderer()
        {
            var instance = Instantiate(gridPlanePrefab);
            if (instance == null)
                return;
            _gridPlane = instance.transform;
            _gridPlane.SetParent(transform, false);
            _gridPlane.localRotation = Quaternion.Euler(90.0f,0.0f,0.0f);
            var meshRenderer = instance.GetComponent<MeshRenderer>();
            _gridMaterial = meshRenderer.material;

            if(_gridMaterial != null)
            {
                if(settings != null)
                {
                    _gridMaterial.SetColor(HASH_LINE_COLOR, settings.lineColor);
                    _gridMaterial.SetFloat(HASH_LINE_THICKNESS, settings.lineThickness);
                }
            }
        }
    }
}

