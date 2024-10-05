using GridSystem;
using UnityEngine;

namespace PlacementSystem
{
    [CreateAssetMenu(menuName = "PlacementSystem/Settings/Map", fileName ="PlacementMapSettings",order = 10)]
    public class PlacementMapSettings : ScriptableObject
    {
        public bool _useObjectPlacer;

        [SerializeField] GridData _gridData;
        public GridDrawerSettings gridDrawerSettings;

        public bool GetGridData(out GridData data)
        {
            data = _gridData;
            return _gridData != null;
        }

#if UNITY_EDITOR
        public void SetGridData(GridData data)
        {
            _gridData = data;
        }
#endif
    }
}

