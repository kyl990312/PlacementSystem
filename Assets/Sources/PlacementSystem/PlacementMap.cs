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
        /// ���� ������Ʈ�� ��ġ�� �������� ���� ������ �����մϴ�.
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
        /// ���� ������Ʈ�� ��ġ�� �������� ���� ������ ó�� ���·� �����մϴ�.
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
        /// ������Ʈ�� ���� ũ�⿡ ���� ������ ��ġ�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="placementObject">��ġ ������Ʈ</param>
        /// <param name="positionWorld">������Ʈ�� ��ġ�� ��ġ</param>
        /// <returns>���ǵ� ��ġ</returns>
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
        /// �ش� ��ġ�� ������Ʈ�� ��ġ ���������� ��ȯ�մϴ�.
        /// </summary>
        /// <returns>������Ʈ ��ġ ���� ����</returns>
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
        /// �׸��� �� �� ������Ʈ�� ��ġ������ �˻��մϴ�.
        /// </summary>
        /// <returns>������Ʈ overlap ����</returns>
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
        /// �׸��忡�� ������Ʈ�� �����ϴ� ũ�⸦ ����մϴ�.
        /// </summary>
        /// <returns>�׸���� ������Ʈ ũ��</returns>
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

