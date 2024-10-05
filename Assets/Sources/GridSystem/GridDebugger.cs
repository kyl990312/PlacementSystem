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

        private GridReader _gridReader;
        private Dictionary<Vector2Int, List<SpriteRenderer>> _cells;
        private GameObjectPool _poolCell;
        private Vector3 _prevPosition;
        private int _prevLayerCount;
        private Vector2 _prevCellSize;
        private List<bool> _layerVisibled = new List<bool>();
        private Transform _root;
        private List<Transform> _cellsParents;
        private bool _inited = false;

        public int LayerCount
        {
            get
            {
                if (_gridReader == null)
                    return 0;
                return _gridReader.LayerCount;
            }
        }
        public bool Enabled
        {
            get
            {
                if(_root == null) 
                    return false;
                return _root.gameObject.activeSelf;
            }
            set
            {
                if (_root == null) 
                    return;
                _root.gameObject.SetActive(value);
            }
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (_gridReader == null)
                return;
            if (_prevLayerCount != _gridReader.LayerCount)
            {
                SetGrid(_gridReader);
            }

            UpdateColor();
            if(Vector3.Distance(_prevPosition, _gridReader.Center) > 0.1f)
            {
                UpdateTransform();
            }

            if(Vector2.Distance(_prevCellSize, _gridReader.CellSize) > 0.01f)
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

            var parent = new GameObject("Grid Debugger Root");
            _root = parent.transform;
            _root.SetParent(transform, false);
            _inited = true;
        }

        public void SetGrid(GridReader viwer)
        {
            Initialize();

            _gridReader = viwer;
            _prevPosition = _gridReader.Center;
            _prevLayerCount = _gridReader.LayerCount;

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

            for(int i = 0; i<_gridReader.LayerCount; i++)
            {
                if(_layerVisibled.Count >= i)
                    _layerVisibled.Add(true);
            }

            // Create Cell Parent
            if (_cellsParents == null)
                _cellsParents = new List<Transform>();
            for(int i = _cellsParents.Count; i<_gridReader.LayerCount; i++)
            {
                var cellParent = new GameObject($"Layer {i}");
                cellParent.transform.SetParent(_root, false);
                _cellsParents.Add(cellParent.transform);
            }

            // Create Cell
            foreach(Vector2Int coord in viwer.CellCoordinates)
            {
                _cells.Add(coord, new List<SpriteRenderer>());
                for (int i = 0; i<_gridReader.LayerCount; i++)
                {
                    var cellGObj = _poolCell.Get();
                    if (cellGObj == null)
                        continue;
                    cellGObj.transform.SetParent(_cellsParents[i], false);
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
            for(int i = 0; i< _gridReader.LayerCount;i++)
            {
                foreach (Vector2Int coord in _cells.Keys)
                {
                    if (_cells[coord].Count <= i)
                        continue;
                    _cells[coord][i].color = GetColor(i, _gridReader.GetCellState(i, coord));
                }
            }
        }

        private void UpdateTransform()
        {
            _prevPosition = _gridReader.Center;
            for (int i = 0; i < _gridReader.LayerCount; i++)
            {
                foreach (Vector2Int coord in _cells.Keys)
                {
                    if (_cells[coord].Count <= i)
                        continue;
                    _cells[coord][i].transform.position = _gridReader.CellToWorld(coord);
                }
            }
        }

        private void UpdateCellSize()
        {
            _prevCellSize = _gridReader.CellSize;
            for (int i = 0; i < _gridReader.LayerCount; i++)
            {
                foreach (Vector2Int coord in _cells.Keys)
                {
                    if (_cells[coord].Count <= i)
                        continue;
                    _cells[coord][i].transform.localScale = new Vector3(_gridReader.CellSize.x, _gridReader.CellSize.y, 1.0f) * 0.9f;
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

