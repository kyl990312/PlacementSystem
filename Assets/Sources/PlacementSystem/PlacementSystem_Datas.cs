using GridSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    [Serializable]
    public struct PlacementObjectData
    {
        public int cellState;
        public List<int> unplacableCellState;
    }
}

