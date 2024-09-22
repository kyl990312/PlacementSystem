using GridSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Debug
{
    public class GridDebugger : MonoBehaviour
    {
        public SpriteRenderer _cellDummy;
        public GridDebuggerSettings _settings;

        private GridViwer _gridViewer;
        private Dictionary<Vector2Int, List<SpriteRenderer>> _cells;
        private GameObjectPool _poolCell;
        private Vector3 _prevPosition;
        private int _prevLayerCount;
        private Vector2 _prevCellSize;
        private List<bool> _layerVisibled = new List<bool>();
        private Transform _cellParent;
        private bool _inited = false;

        public int LayerCount
        {
            get
            {
                if (_gridViewer == null)
                    return 0;
                return _gridViewer.LayerCount;
            }
        }
        public bool Enabled
        {
            get
            {
                if(_cellParent == null) 
                    return false;
                return _cellParent.gameObject.activeSelf;
            }
            set
            {
                if (_cellParent == null) 
                    return;
                _cellParent.gameObject.SetActive(value);
            }
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (_gridViewer == null)
                return;
            if (_prevLayerCount != _gridViewer.LayerCount)
            {
                SetGrid(_gridViewer);
            }

            UpdateColor();
            if(Vector3.Distance(_prevPosition, _gridViewer.Center) > 0.1f)
            {
                UpdateTransform();
            }

            if(Vector2.Distance(_prevCellSize, _gridViewer.CellSize) > 0.01f)
            {
                UpdateCellSize();
                UpdateTransform();
            }
        }

        private void Initialize()
        {
            if (_inited)
                return;
            _poolCell = new GameObjectPool(_cellDummy.gameObject);
            _poolCell.Capacity = 10000;

            var parent = new GameObject("Grid Debugger Cells");
            _cellParent = parent.transform;
            _cellParent.SetParent(transform, false);
            _inited = true;
        }

        public void SetGrid(GridViwer viwer)
        {
            Initialize();

            _gridViewer = viwer;
            _prevPosition = _gridViewer.Center;
            _prevLayerCount = _gridViewer.LayerCount;

            if (_cells == null)
            {
                _cells= new Dictionary<Vector2Int, List<SpriteRenderer>>();
            }
            foreach(var cells in _cells.Values)
            {
                for(int i = 0; i<cells.Count; i++)
                {
                    cells[i].color = Color.clear;
                    cells[i].transform.localScale = Vector3.zero;
                    _poolCell.Return(cells[i].gameObject);
                }
            }
            _cells.Clear();

            for(int i = 0; i<_gridViewer.LayerCount; i++)
            {
                if(_layerVisibled.Count >= i)
                    _layerVisibled.Add(true);
            }

            // Create Cell
            foreach(Vector2Int coord in viwer.CellCoordinates)
            {
                _cells.Add(coord, new List<SpriteRenderer>());
                for (int i = 0; i<_gridViewer.LayerCount; i++)
                {
                    var cellGObj = _poolCell.Get();
                    if (cellGObj == null)
                        continue;
                    cellGObj.transform.SetParent(_cellParent, false);
                    cellGObj.transform.localScale = Vector3.one;
                    cellGObj.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);

                    if (cellGObj.TryGetComponent(out SpriteRenderer renderer))
                    {
                        _cells[coord].Add(renderer);
                    }
                }
            }

            UpdateTransform();
        }

        public void SetVisible(int layer, bool visible)
        {
            if (_layerVisibled.Count < layer)
                return;
            _layerVisibled[layer] = visible;
        }

        public bool GetVisible(int layer)
        {
            if (_layerVisibled.Count < layer)
                return false;
            return _layerVisibled[layer];
        }

        private void UpdateColor()
        {
            for(int i = 0; i< _gridViewer.LayerCount;i++)
            {
                foreach (Vector2Int coord in _cells.Keys)
                {
                    if (_cells[coord].Count <= i)
                        continue;
                    _cells[coord][i].color = GetColor(i, _gridViewer.GetCellState(i, coord));
                }
            }
        }

        private void UpdateTransform()
        {
            _prevPosition = _gridViewer.Center;
            for (int i = 0; i < _gridViewer.LayerCount; i++)
            {
                foreach (Vector2Int coord in _cells.Keys)
                {
                    if (_cells[coord].Count <= i)
                        continue;
                    _cells[coord][i].transform.position = _gridViewer.CellToWorld(coord);
                }
            }
        }

        private void UpdateCellSize()
        {
            _prevCellSize = _gridViewer.CellSize;
            for (int i = 0; i < _gridViewer.LayerCount; i++)
            {
                foreach (Vector2Int coord in _cells.Keys)
                {
                    if (_cells[coord].Count <= i)
                        continue;
                    _cells[coord][i].transform.localScale = new Vector3(_gridViewer.CellSize.x, _gridViewer.CellSize.y, 1.0f) * 0.9f;
                }
            }
        }

        private Color GetColor(int layer, int state)
        {
            if (_settings == null)
                return Color.clear;
            if (_settings.colorGroups.Count == 0)
                return Color.clear;

            int idx = _settings.colorGroups.FindIndex(x=>x.layer == layer);
            if(idx < 0)
                return Color.clear;
            if(state < 0)
                return Color.clear;
            if(_settings.colorGroups[idx].colors.Count <= state)
                return Color.clear;
            Color color = _settings.colorGroups[idx].colors[state];
            color.a *= _settings.colorGroups[idx].alpha;
            
            if(_layerVisibled.Count > layer)
            {
                if (!_layerVisibled[layer])
                    color.a = 0.0f;
            }
            return color;
        }

        [Serializable]
        public struct StateColor
        {
            public int state;
            public Color color;
        }
    }
}

