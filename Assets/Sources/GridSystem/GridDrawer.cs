using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Component
{
    public class GridDrawer : MonoBehaviour
    {
        public GridDrawerSettings settings;

        private GridViwer _gridViewer;
        private List<LineRenderer> _lines;
        private Vector3 _prevCenter;
        private bool _inited = false;

        private void Update()
        {
            if (_gridViewer == null)
                return;
            if (!_inited)
            {
                Initialize();
            }
            if (!_inited)
                return;

            if (Vector3.Distance(_gridViewer.Center, _prevCenter) > 0.1f)
            {
                DrawGrid();
                _prevCenter = _gridViewer.Center;
            }
        }

        public void SetGrid(GridViwer gridViewer)
        {
            _gridViewer = gridViewer; 
        }

        public virtual void Initialize()
        {
            SetLineRenderer();
            _inited = true;
        }

        public virtual void DrawGrid()
        {
            if (_gridViewer == null)
                return;

            var positions = new List<Vector3>();

            // draw horizontal
            if (_lines.Count > 0)
            {
                var line = _lines[0];
                for (int i = 0; i <= _gridViewer.CellCount.y; i++)
                {
                    if (i % 2 == 0)
                    {
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x, _gridViewer.MinCell.y + i)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MaxCell.x, _gridViewer.MinCell.y + i)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MaxCell.x, _gridViewer.MinCell.y + i + 1)));
                    }
                    else
                    {
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MaxCell.x, _gridViewer.MinCell.y + i)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x, _gridViewer.MinCell.y + i)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x, _gridViewer.MinCell.y + i + 1)));
                    }
                }

                for (int i = 0; i < positions.Count; i++)
                {
                    positions[i] -= new Vector3(_gridViewer.CellSize.x, 0.0f, _gridViewer.CellSize.y) * 0.5f;
                }

                line.positionCount = positions.Count-1;
                line.SetPositions(positions.ToArray());
            }
            positions.Clear();

            // draw vertical
            if (_lines.Count > 0)
            {
                var line = _lines[1];
                for (int i = 0; i <= _gridViewer.CellCount.x; i++)
                {
                    if (i % 2 == 0)
                    {
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x + i, _gridViewer.MinCell.y)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x + i, _gridViewer.MaxCell.y)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x + i + 1, _gridViewer.MaxCell.y)));
                    }
                    else
                    {
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x + i, _gridViewer.MaxCell.y)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x + i, _gridViewer.MinCell.y)));
                        positions.Add(_gridViewer.CellToWorld(new Vector2Int(_gridViewer.MinCell.x + i + 1, _gridViewer.MinCell.y)));
                    }
                }

                for (int i = 0; i < positions.Count; i++)
                {
                    positions[i] -= new Vector3(_gridViewer.CellSize.x, 0.0f, _gridViewer.CellSize.y) * 0.5f;
                }

                line.positionCount = positions.Count-1;
                line.SetPositions(positions.ToArray());
            }

        }

        private void SetLineRenderer()
        {
            if(_lines == null)
                _lines = new List<LineRenderer>();

            for(int i = 0; i<2; i++)
            {
                var line = new GameObject("line");
                line.transform.SetParent(gameObject.transform, false);

                LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                if (lineRenderer == null)
                    return;
                if (settings != null)
                {
                    if (settings.lineMaterial != null)
                        lineRenderer.material = settings.lineMaterial;
                    lineRenderer.colorGradient = settings.lineColor;
                    lineRenderer.startWidth = settings.lineThickness;
                    lineRenderer.endWidth = settings.lineThickness;
                }

                _lines.Add(lineRenderer);
            }

        }
    }
}

