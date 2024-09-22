using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 셀의 위치 및 상태 데이터를 저장합니다. 
 * 유니티의 Grid 시스템의 Position 변환 기능 사용을 위해 Grid 시스템을 한 번 감싸는 형태입니다.
 * 
 */

namespace GridSystem.Internal
{
    public class StateGrid
    {
        private Grid _grid;
        private Transform _transform;
        private Dictionary<Vector2Int, List<int>> _cellStates;
        private Vector2Int _cellCount;
        private int _layerCount;
        private CellBound _cellBounds;
        private CellStyle _cellStyle;

        public Vector2 CellSize
        {
            get
            {
                if (_grid == null)
                    return Vector2.zero;
                return new Vector2(_grid.cellSize.x, _grid.cellSize.y);
            }
            set
            {
                if (_grid == null)
                    return;
                _grid.cellSize = new Vector3(value.x, value.y, 1.0f);
            }
        }
        public Vector2Int CellCount
        {
            get => _cellCount;
        }
        public int LayerCount
        {
            get => _layerCount;
        }
        public int CellVertexCount
        {
            get
            {
                return _cellStyle switch
                {
                    CellStyle.Square => 4,
                    _ => 0
                };
            }
        }
        public Vector2Int MinCell
        {
            get =>new Vector2Int(_cellBounds.xMin, _cellBounds.yMin);
        }
        public Vector2Int MaxCell
        {
            get => new Vector2Int(_cellBounds.xMax, _cellBounds.yMax);
        }
        public GridAxis Axis
        {
            get => _grid.cellSwizzle switch
            {
                GridLayout.CellSwizzle.XYZ => GridAxis.XY,
                GridLayout.CellSwizzle.XZY => GridAxis.XZ,
                _ => GridAxis.XZ
            };
        }
        public int this[int index, Vector2Int coord]
        {
            get
            {
                if (!_cellStates.ContainsKey(coord))
                    return -1;
                if (_cellStates[coord] == null)
                    return -1;
                if (_cellStates[coord].Count < 0 || _cellStates[coord].Count <= index)
                    return -1;
                return _cellStates[coord][index];
            }
            set
            {
                if (!_cellStates.ContainsKey(coord))
                    return;
                if (_cellStates[coord] == null)
                    return;
                if (_cellStates[coord].Count < 0 || _cellStates[coord].Count <= index)
                    return;
                _cellStates[coord][index] = value;
            }
        }
        public Transform transform
        {
            get => _transform;
        }
        public IEnumerable<Vector2Int> CellCoordinates
        {
            get
            {
                if (_cellStates == null)
                    return new Vector2Int[0];
                return _cellStates.Keys;
            }
        }


        public void Initialize(GridData data, Transform parent, int layerCount = 1)
        {
            var gridObj = new GameObject();
            if(parent != null)
            {
                gridObj.transform.SetParent(parent, false);
            }
            _transform = gridObj.transform;
            _transform.position = data.CenterOffset;

            _grid = gridObj.AddComponent<Grid>();
            _grid.cellSwizzle = data.Axis switch
            {
                GridAxis.XY => GridLayout.CellSwizzle.XYZ,
                GridAxis.XZ => GridLayout.CellSwizzle.XZY,
                _ => GridLayout.CellSwizzle.XZY
            };
            _grid.cellSize = new Vector3(data.CellSize.x,1.0f,data.CellSize.y);
            _cellStyle = data.CellStyle;

            _cellCount = data.CellCount;
            _cellBounds = new CellBound();
            foreach(var cell in data.Cells)
            {
                if(_cellBounds.xMin > cell.Coordinate.x)
                    _cellBounds.xMin = cell.Coordinate.x;
                if(_cellBounds.xMax <  cell.Coordinate.x)
                    _cellBounds.xMax = cell.Coordinate.x;
                if(_cellBounds.yMin > cell.Coordinate.y)
                    _cellBounds.yMin = cell.Coordinate.y;
                if(_cellBounds.yMax < cell.Coordinate.y)
                    _cellBounds.yMax = cell.Coordinate.y;
            }

            _cellStates = new Dictionary<Vector2Int, List<int>>();
            _layerCount = layerCount;
            for (int i = 0; i < data.Cells.Count; i++)
            {
                _cellStates.Add(data.Cells[i].Coordinate, new List<int>());
                for (int j = 0; j < _layerCount; j++)
                {
                    _cellStates[data.Cells[i].Coordinate].Add(0);
                }
                if (_cellStates[data.Cells[i].Coordinate].Count > 0)
                {
                    _cellStates[data.Cells[i].Coordinate][0] = data.Cells[i].State;
                }
            }
        }

        public void AddLayer(List<CellData> cells)
        {
            _layerCount++;
            foreach(var coord in _cellStates.Keys)
            {
                _cellStates[coord].Add(0);
            }

            if (cells != null)
            {
                foreach (var cell in cells)
                {
                    if (!_cellStates.ContainsKey(cell.Coordinate))
                        continue;
                    _cellStates[cell.Coordinate][_layerCount - 1] = cell.State;
                }
            }
        }

        public void Clear()
        {
            GameObject.DestroyImmediate(_grid.gameObject);
        }

        public Vector3 CellToWorld(Vector2Int coord)
        {
            if (_grid == null)
                return Vector3.zero;
            return _grid.CellToWorld(new Vector3Int(coord.x, coord.y, 0));
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            if (_grid == null)
                return Vector2Int.zero;
            var cellCoord = _grid.WorldToCell(worldPosition);
            return new Vector2Int(cellCoord.x, cellCoord.y);
        }

        public bool IsContains(Vector3 worldPosition)
        {
            return true;
        }

        public bool IsExistCell(Vector2Int cellCoord)
        {
            if (cellCoord.x < MinCell.x)
                return false;
            if (cellCoord.x > MaxCell.x)
                return false;
            if (cellCoord.y < MinCell.y)
                return false;
            if (cellCoord.y > MaxCell.y)
                return false;
            return true;
        }

        private struct CellBound
        {
            public int xMin;
            public int xMax;
            public int yMin;
            public int yMax;
        }
    }
}
