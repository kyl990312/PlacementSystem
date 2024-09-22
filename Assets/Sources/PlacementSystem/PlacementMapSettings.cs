using GridSystem;
using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    [CreateAssetMenu(menuName = "PlacementSystem/Settings/Map", fileName ="PlacementMapSettings",order = 10)]
    public class PlacementMapSettings : ScriptableObject
    {
        public int layerCount;
        public List<int> _unplacableStates;

        [SerializeField] GridData _gridData;
        public GridDrawerSettings gridDrawerSettings;

        public bool GetGridData(out GridData data)
        {
            data = _gridData;
            return _gridData != null;
        }

        public bool IsUnplacableState(int state)
        {
            if (_unplacableStates == null)
                return false;
            return _unplacableStates.Contains(state);
        }

#if UNITY_EDITOR
        public void SetGridData(GridData data)
        {
            _gridData = data;
        }
#endif
    }
}

