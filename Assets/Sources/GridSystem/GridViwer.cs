using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem.Internal;

namespace GridSystem
{
    public class GridViwer
    {
        private StateGrid _grid;

        public Vector3 Center
        {
            get
            {
                if (_grid == null)
                    return Vector3.zero;
                return _grid.transform.position;
            }
        }
        public Vector2 CellSize 
        {
            get
            {
                if(_grid == null)
                    return Vector2.zero;
                return _grid.CellSize;
            }
        }
        public Vector2Int CellCount
        {
            get
            {
                if (_grid == null)
                    return Vector2Int.zero;
                return _grid.CellCount;
            }
        }
        public Vector2Int MinCell
        {
            get
            {
                if(_grid == null)
                    return Vector2Int.zero;
                return _grid.MinCell;
            }
        }
        public Vector2Int MaxCell
        {
            get
            {
                if(_grid == null)
                    return Vector2Int.zero;
                return _grid.MaxCell;
            }
        }
        public int LayerCount
        {
            get
            {
                if(_grid == null)
                    return 0;
                return _grid.LayerCount;
            }
        }
        public IEnumerable<Vector2Int> CellCoordinates
        {
            get
            {
                if(_grid == null)
                    return new Vector2Int[0];
                return _grid.CellCoordinates;
            }
        }
        public GridAxis Axis
        {
            get
            {
                if (_grid == null)
                    return GridAxis.XZ;
                return _grid.Axis;
            }
        }
        public int CellVertexCount
        {
            get
            {
                if (_grid == null)
                    return 0;
                return _grid.CellVertexCount;
            }
        }

        public GridViwer(StateGrid grid)
        {
            _grid = grid;
        }

        public int GetCellState(int layer, Vector3 worldPosition)
        {
            if (_grid == null)
                return 0;
            var cellCoord = _grid.WorldToCell(worldPosition);
            return _grid[layer, cellCoord];
        }

        public int GetCellState(int layer, Vector2Int cellCoord)
        {
            if(_grid == null)
                return 0;
            return _grid[layer, cellCoord];
        }

        public bool IsContains(Vector3 worldPosition)
        {
            if (_grid == null)
                return false;
            return _grid.IsContains(worldPosition);
        }

        public bool IsExistCell(Vector2Int cellCoord)
        {
            if (_grid == null)
                return false;
            return _grid.IsExistCell(cellCoord);
        }

        public Vector3 CellToWorld(Vector2Int coord)
        {
            if (_grid == null)
                return Vector3.zero;
            return _grid.CellToWorld(coord);
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            if (_grid == null)
                return Vector2Int.zero;
            return _grid.WorldToCell(worldPosition);
        }
    }
}

