using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Debug
{
    [CreateAssetMenu(menuName = "GridSystem/Debug/Settings", fileName = "GridDebuggerSettings", order = 10)]
    public class GridDebuggerSettings : ScriptableObject
    {
        public List<ColorGroup> colorGroups;
    }

    [Serializable]
    public struct ColorGroup
    {
        public int layer;
        public List<Color> colors;
        public float alpha;
    }
}

