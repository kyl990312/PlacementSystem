using UnityEngine;
using GridSystem;
using GridSystem.Component;
using GridSystem.Debug;
using System.Collections.Generic;
using System.Collections;

namespace PlacementSystem
{
    public class PlacementMap : MonoBehaviour
    {
        public static int TILE_LAYER = 1;
        public static int PLACEMENT_LAYER = 0;

        public PlacementMapSettings settings;
        private GridGroup _gridGroup;
        private GridController _gridController;
        private GridViwer _gridViewer;
        private GridDrawer _gridDrawer;
        private Dictionary<Vector2Int, int> _originalGrid;

        public float RotationUnit
        {
            get
            {
                if (_gridDrawer == null)
                    return 0.0f;
                return 360.0f / _gridViewer.CellVertexCount;
            }
        }


        public void Initialize()
        {
            if (!settings.GetGridData(out var gridData))
                return;
            _gridGroup = new GridGroup();
            _gridGroup.Initialize(gridData, transform);
            if (!_gridGroup.GetGridViewer(out _gridViewer))
                return;
            if (!_gridGroup.GetGridController(out _gridController))
                return;

            if (_gridDrawer == null)
            {
                _gridDrawer = gameObject.AddComponent<GridDrawer>();
            }
            _gridDrawer.settings = settings.gridDrawerSettings;
            _gridDrawer.Initialize();
            _gridDrawer.SetGrid(_gridViewer);
            _gridDrawer.DrawGrid();

            if(_originalGrid == null)
                _originalGrid = new Dictionary<Vector2Int, int>();
            else
                _originalGrid.Clear();
            foreach(Vector2Int cell in _gridViewer.CellCoordinates)
            {
                _originalGrid.Add(cell, _gridViewer.GetCellState(PLACEMENT_LAYER, cell));
            }

#if UNITY_EDITOR
            if (TryGetComponent(out GridDebugger gridDebugger))
            {
                gridDebugger.SetGrid(_gridViewer);
            }
#endif
        }

        /// <summary>
        /// 현재 오브젝트의 위치를 기준으로 맵의 정보를 변경합니다.
        /// </summary>
        public void UpdateMap(PlacementObject placementObject)
        {
            var cells = new ObjectCells(placementObject, _gridViewer);
            foreach(Vector2Int cell in cells)
            {
                _gridController.SetCellState(PLACEMENT_LAYER, cell, placementObject.CellState);
            }
        }

        /// <summary>
        /// 현재 오브젝트의 위치를 기준으로 맵의 정보를 처음 상태로 변경합니다.
        /// </summary>
        public void ClearMap(PlacementObject placementObject)
        {
            var cells = new ObjectCells(placementObject, _gridViewer);
            foreach (Vector2Int cell in cells)
            {
                if (_originalGrid.ContainsKey(cell))
                {
                    _gridController.SetCellState(PLACEMENT_LAYER, cell, _originalGrid[cell]);
                }
                else
                {
                    _gridController.SetCellState(PLACEMENT_LAYER, cell, 0);
                }
            }
        }

        public Vector3 GetObjectPosition(PlacementObject placementObject, Vector3 positionWorld)
        {
            if (placementObject == null)
                return Vector3.zero;
            
            var objectCellCount = ObjectCells.GetObjectCellCount(placementObject, _gridViewer);
            var result = _gridViewer.CellToWorld(_gridViewer.WorldToCell(positionWorld));
            result.x += objectCellCount.x % 2 == 0 ? _gridViewer.CellSize.x * 0.5f : 0.0f;
            result.z += objectCellCount.y % 2 == 0 ? _gridViewer.CellSize.y * 0.5f : 0.0f;
            return result;
        }

        public bool IsContains(Vector3 worldPosition)
        {
            return _gridViewer.IsContains(worldPosition);
        }

        public bool IsPlacable(PlacementObject placementObject, Vector3 worldPosition)
        {
            var cells = new ObjectCells(placementObject, _gridViewer);
            foreach(Vector2Int cell in cells)
            {
                if (!_gridViewer.IsExistCell(cell))
                    return false;
                if (settings == null)
                    return false;
                if (settings.IsUnplacableState(_gridViewer.GetCellState(PLACEMENT_LAYER, cell)))
                    return false;
            }
            return true;
        }

        public void DrawTile(PlacementTileData tile, Vector3 worldPosition)
        {
            var cell = _gridViewer.WorldToCell(worldPosition);
            if (_gridViewer.IsExistCell(cell))
            {
                if(_gridViewer.GetCellState(TILE_LAYER,cell) != tile.id)
                {
                    _gridController.SetCellState(TILE_LAYER, cell, tile.id);
                }
            }
        }

        public bool IsOverlap(PlacementObject a, PlacementObject b)
        {
            ObjectCells aCells = new ObjectCells(a, _gridViewer);
            ObjectCells bCells = new ObjectCells(b, _gridViewer);

            if (aCells.MinCell.x > bCells.MinCell.x)
                return true;
            if (aCells.MinCell.y > bCells.MinCell.y)
                return true;
            if (aCells.MaxCell.x < bCells.MaxCell.x)
                return true;
            if(aCells.MaxCell.y < bCells.MaxCell.y)
                return true;
            return false;
        }

        public Vector3 GetObjectArea(PlacementObject placement)
        {
            if (placement == null)
                return Vector3.zero;
            var count = ObjectCells.GetObjectCellCount(placement, _gridViewer);
            switch (_gridViewer.Axis)
            {
                case GridAxis.XZ: return new Vector3(count.x * _gridViewer.CellSize.x, 1.0f, count.y * _gridViewer.CellSize.y);
                case GridAxis.XY: return new Vector3(count.x * _gridViewer.CellSize.x,count.y * _gridViewer.CellSize.y,1.0f);
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
                    return _centerCell - CellCount/2;
                }
            }
            public Vector2Int MaxCell
            {
                get
                {
                    return _cellCount + CellCount/2;
                }
            }

            public ObjectCells(PlacementObject placementObject, GridViwer gridViwer)
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

            public static Vector2Int GetObjectCellCount(PlacementObject placementObject, GridViwer gridViwer)
            {
                Vector3 objectSize = placementObject.ObjectSize;
                switch (gridViwer.Axis)
                {
                    default:
                        var angle = placementObject.Transform.rotation.eulerAngles.y % 360;
                        Debug.Log(angle);
                        if(90.0f <= angle && angle < 180.0f ||
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
                Debug.Log(objectSize);
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

