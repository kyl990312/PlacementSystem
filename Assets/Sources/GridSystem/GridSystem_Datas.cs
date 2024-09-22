using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    [Serializable]
    public class CellData
    {
        public Vector2Int Coordinate;
        public int State;
    }

    [Serializable]
    public class GridData
    {
        public List<CellData> Cells;
        public Vector2Int CellCount;
        public Vector2 CellSize;
        public Vector2 CenterOffset;
        public GridAxis Axis = GridAxis.XZ;
        public CellStyle CellStyle = CellStyle.Square;
    }
}
