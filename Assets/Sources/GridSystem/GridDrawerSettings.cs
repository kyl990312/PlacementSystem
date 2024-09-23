using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GridSystem/Settings/GridDrawer", fileName = "GridDrawerSettings", order = 10)]
public class GridDrawerSettings : ScriptableObject
{
    public List<int> exceptStates;
    public Color lineColor;
    public float lineThickness;
}
