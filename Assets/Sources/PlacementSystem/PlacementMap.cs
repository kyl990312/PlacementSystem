using UnityEngine;
using GridSystem;
using GridSystem.Component;
using GridSystem.Debug;

namespace PlacementSystem
{
    public class PlacementMap : MonoBehaviour
    {
        public static int TILE_LAYER = 1;
        public static int PLACEMENT_LAYER = 0;

        public PlacementMapSettings settings;
        
        private GridGroup _gridGroup;
        private GridWriter _gridWriter;
        private GridReader _gridReader;
        private GridDrawer _gridDrawer;
        private ObjectPlacer _objectPlacer;
        private bool _inited;

        public float RotationUnit
        {
            get
            {
                if (_gridReader == null)
                    return 0.0f;
                return 360.0f / _gridReader.CellVertexCount;
            }
        }

        public ObjectPlacer ObjectPlacer
        {
            get
            {
                return _objectPlacer;
            }
        }

        public void Initialize()
        {
            if (!settings.GetGridData(out var gridData))
                return;
            _gridGroup = new GridGroup();
            _gridGroup.Initialize(gridData, transform);
            if (!_gridGroup.GetGridReader(out _gridReader))
                return;
            if (!_gridGroup.GetGridWriter(out _gridWriter))
                return;
            if(_gridReader.CellCount.x + _gridReader.CellCount.y == 0)
            {
                Debug.LogError("cell count is zero");
            }

            if (_gridDrawer == null)
            {
                _gridDrawer = gameObject.AddComponent<GridDrawer>();
            }
            _gridDrawer.settings = settings.gridDrawerSettings;
            _gridDrawer.Initialize();
            _gridDrawer.SetGrid(_gridReader);
            _gridDrawer.DrawGrid();

#if UNITY_EDITOR
            if (TryGetComponent(out GridDebugger gridDebugger))
            {
                gridDebugger.SetGrid(_gridReader);
            }
#endif
            if (settings._useObjectPlacer)
            {
                _objectPlacer = new ObjectPlacer();
                _objectPlacer.Initialize(_gridGroup, TILE_LAYER);
            }

            _inited = true;
        }

        public bool IsContains(Vector3 worldPosition)
        {
            return _gridReader.IsContains(worldPosition);
        }

        #region object placer
        /// <summary>
        /// 현재 오브젝트의 위치를 기준으로 맵의 정보를 변경합니다.
        /// </summary>
        public void UpdateMap(PlacementObject placementObject)
        {
            if(_objectPlacer == null)
            {
                Debug.Log(LogFormatter.OBJECT_PLACER_NOT_USED);
                return;
            }

            _objectPlacer.UpdateMap(placementObject);
        }

        /// <summary>
        /// 현재 오브젝트의 위치를 기준으로 맵의 정보를 처음 상태로 변경합니다.
        /// </summary>
        public void ClearMap(PlacementObject placementObject)
        {
            if (_objectPlacer == null)
            {
                Debug.Log(LogFormatter.OBJECT_PLACER_NOT_USED);
                return;
            }

            _objectPlacer.ClearMap(placementObject);
        }
        /// <summary>
        /// 오브젝트를 셀의 크기에 맞춰 스냅한 위치를 반환합니다.
        /// </summary>
        /// <param name="placementObject">배치 오브젝트</param>
        /// <param name="positionWorld">오브젝트를 배치할 위치</param>
        /// <returns>스탭된 위치</returns>
        public Vector3 GetObjectPosition(PlacementObject placementObject, Vector3 positionWorld)
        {
            if (_objectPlacer == null)
            {
                Debug.Log(LogFormatter.OBJECT_PLACER_NOT_USED);
                return Vector3.zero;
            }

            return _objectPlacer.GetObjectPosition(placementObject, positionWorld);
        }

        /// <summary>
        /// 해당 위치에 오브젝트가 배치 가능한지를 반환합니다.
        /// </summary>
        /// <returns>오브젝트 배치 가능 여부</returns>
        public bool IsPlacable(PlacementObject placementObject, Vector3 worldPosition)
        {
            if (_objectPlacer == null)
            {
                Debug.Log(LogFormatter.OBJECT_PLACER_NOT_USED);
                return false;
            }

            return _objectPlacer.IsPlacable(placementObject,worldPosition);
        }

        /// <summary>
        /// 그리드 상 두 오브젝트가 겹치는지를 검사합니다.
        /// </summary>
        /// <returns>오브젝트 overlap 여부</returns>
        public bool IsOverlap(PlacementObject a, PlacementObject b)
        {
            if (_objectPlacer == null)
            {
                Debug.Log(LogFormatter.OBJECT_PLACER_NOT_USED);
                return false;
            }

            return _objectPlacer.IsOverlap(a, b);
        }

        /// <summary>
        /// 그리드에서 오브젝트가 차지하는 크기를 계산합니다.
        /// </summary>
        /// <returns>그리드상 오브젝트 크기</returns>
        public Vector3 GetObjectArea(PlacementObject placement)
        {
            if (_objectPlacer == null)
            {
                Debug.Log(LogFormatter.OBJECT_PLACER_NOT_USED);
                return Vector3.zero;
            }

            return _objectPlacer.GetObjectArea(placement);
        }
        #endregion


        public void DrawTile(PlacementTileData tile, Vector3 worldPosition)
        {
            var cell = _gridReader.WorldToCell(worldPosition);
            if (_gridReader.IsExistCell(cell))
            {
                if (_gridReader.GetCellState(TILE_LAYER, cell) != tile.id)
                {
                    _gridWriter.SetCellState(TILE_LAYER, cell, tile.id);
                }
            }
        }

        // -----------------------------------------------------

        private class LogFormatter
        {
            public static readonly string OBJECT_PLACER_NOT_USED = "Object Placer isn't used.";
        }
    }
}

