using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    public class ObjectPlacer
    {
        static readonly int CLEAR_STATE = 0;

        private int _baseLayer;
        private int _targetLayer;
        private GridWriter _gridWriter;
        private GridReader _gridReader;
        private Dictionary<Vector2Int, int> _originalGrid;

        public void Initialize(GridGroup gridGroup, int baseLayer)
        {
            _baseLayer = baseLayer;

            if (gridGroup == null)
            {
                Debug.LogWarning("GridGroup is null.");
                return;
            }

            if(gridGroup.GetGridWriter(out _gridWriter) == false)
            {
                Debug.LogWarning("GridWriter is null");
                return;
            }

            if (gridGroup.GetGridReader(out _gridReader) == false)
            {
                Debug.LogWarning("GridReader is null");
                return;
            }
            
            _targetLayer = _gridWriter.AddLayer(CLEAR_STATE);
        }

        public void Clear()
        {
            _gridWriter?.RemoveLayer(_targetLayer);
        }

        public void UpdateMap(PlacementObject placementObject)
        {
            var cells = new ObjectCells(placementObject, _gridReader);
            foreach (Vector2Int cell in cells)
            {
                _gridWriter.SetCellState(_targetLayer, cell, placementObject.CellState);
            }
        }

        public void ClearMap(PlacementObject placementObject)
        {
            var cells = new ObjectCells(placementObject, _gridReader);
            foreach (Vector2Int cell in cells)
            {
                _gridWriter.SetCellState(_targetLayer, cell, CLEAR_STATE);
            }
        }

        public Vector3 GetObjectPosition(PlacementObject placementObject, Vector3 positionWorld)
        {
            if (placementObject == null)
                return Vector3.zero;

            var objectCellCount = ObjectCells.GetObjectCellCount(placementObject, _gridReader);
            var result = _gridReader.CellToWorld(_gridReader.WorldToCell(positionWorld));
            result.x += objectCellCount.x % 2 == 0 ? _gridReader.CellSize.x * 0.5f : 0.0f;
            result.z += objectCellCount.y % 2 == 0 ? _gridReader.CellSize.y * 0.5f : 0.0f;
            return result;
        }

        public bool IsPlacable(PlacementObject placementObject, Vector3 worldPosition)
        {
            var cells = new ObjectCells(placementObject, _gridReader);
            foreach (Vector2Int cell in cells)
            {
                if (!_gridReader.IsExistCell(cell))
                    return false;
                if(_gridReader.GetCellState(_targetLayer, cell) != CLEAR_STATE)
                    return false;
                if (!placementObject.IsPlacableState(_gridReader.GetCellState(_baseLayer, cell)))
                    return false;
            }
            return true;
        }

        public bool IsOverlap(PlacementObject a, PlacementObject b)
        {
            ObjectCells aCells = new ObjectCells(a, _gridReader);
            ObjectCells bCells = new ObjectCells(b, _gridReader);

            if (aCells.MinCell.x > bCells.MinCell.x)
                return true;
            if (aCells.MinCell.y > bCells.MinCell.y)
                return true;
            if (aCells.MaxCell.x < bCells.MaxCell.x)
                return true;
            if (aCells.MaxCell.y < bCells.MaxCell.y)
                return true;
            return false;
        }

        public Vector3 GetObjectArea(PlacementObject placement)
        {
            if (placement == null)
                return Vector3.zero;
            var count = ObjectCells.GetObjectCellCount(placement, _gridReader);
            switch (_gridReader.Axis)
            {
                case GridAxis.XZ: return new Vector3(count.x * _gridReader.CellSize.x, 1.0f, count.y * _gridReader.CellSize.y);
                case GridAxis.XY: return new Vector3(count.x * _gridReader.CellSize.x, count.y * _gridReader.CellSize.y, 1.0f);
            }
            return Vector3.zero;
        }

        // -----------------------------------------------------
        private class ObjectCells : IEnumerable<Vector2Int>
        {
            private Vector2Int _centerCell;
            private Vector2Int _cellCount;

            public Vector2Int CellCount => _cellCount;

            public Vector2Int MinCell
            {
                get
                {
                    return _centerCell - CellCount / 2;
                }
            }
            public Vector2Int MaxCell
            {
                get
                {
                    return _cellCount + CellCount / 2;
                }
            }

            public ObjectCells(PlacementObject placementObject, GridReader gridViwer)
            {
                _cellCount = GetObjectCellCount(placementObject, gridViwer);
                var position = placementObject.Transform.position;
                position.x -= _cellCount.x % 2 == 0 ? gridViwer.CellSize.x * 0.5f : 0.0f;
                position.z -= _cellCount.y % 2 == 0 ? gridViwer.CellSize.y * 0.5f : 0.0f;
                _centerCell = gridViwer.WorldToCell(position);
            }

            public IEnumerator<Vector2Int> GetEnumerator()
            {
                for (int i = 0; i <= (int)(_cellCount.y / 2); i++)
                {
                    for (int j = 0; j <= (int)(_cellCount.x / 2); j++)
                    {
                        yield return _centerCell + new Vector2Int(j, i);
                    }
                    for (int j = 0; j < (int)((_cellCount.x + 1) / 2) - 1; j++)
                    {
                        yield return _centerCell + new Vector2Int(-j - 1, i);
                    }
                }
                for (int i = 0; i < (int)((_cellCount.y + 1) / 2) - 1; i++)
                {
                    for (int j = 0; j <= (int)(_cellCount.x / 2); j++)
                    {
                        yield return _centerCell + new Vector2Int(j, -i - 1);
                    }
                    for (int j = 0; j < (int)((_cellCount.x + 1) / 2) - 1; j++)
                    {
                        yield return _centerCell + new Vector2Int(-j - 1, -i - 1);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public static Vector2Int GetObjectCellCount(PlacementObject placementObject, GridReader gridViwer)
            {
                Vector3 objectSize = placementObject.ObjectSize;
                switch (gridViwer.Axis)
                {
                    default:
                        var angle = placementObject.Transform.rotation.eulerAngles.y % 360;
                        if (90.0f <= angle && angle < 180.0f ||
                            angle >= 270.0f ||
                           -180.0f < angle && angle <= -90.0f ||
                           angle <= -270.0f)
                        {
                            var tmp = objectSize.x;
                            objectSize.x = objectSize.z;
                            objectSize.z = tmp;
                        }
                        break;
                }
                return GetObjectCellCount(objectSize, gridViwer.CellSize);
            }

            private static Vector2Int GetObjectCellCount(Vector3 objectSize, Vector2 cellSize)
            {
                var cellCount = Vector2Int.zero;
                cellCount.x = Mathf.CeilToInt(objectSize.x / cellSize.x);
                cellCount.y = Mathf.CeilToInt(objectSize.z / cellSize.y);
                return cellCount;
            }
        }
    }
}
