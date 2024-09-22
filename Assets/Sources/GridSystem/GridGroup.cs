using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem.Internal;

namespace GridSystem
{
    public class GridGroup
    {
        private StateGrid _grid;
        private GridController _controller;
        private GridViwer _viewer;

        public void Initialize(GridData data, Transform parent, int layerCount = 1)
        {
            _grid = new StateGrid();
            _grid.Initialize(data, parent, layerCount);
        }

        public bool GetGridController(out GridController controller)
        {
            controller = null;
            if (_grid == null)
            {
                UnityEngine.Debug.LogWarning("Please initialize first.");
                return false;
            }

            if(_controller == null)
            {
                _controller = new GridController(_grid);
            }
            controller = _controller;
            return controller != null;
        }

        public bool GetGridViewer(out GridViwer viewer)
        {
            viewer = null;
            if (_grid == null)
            {
                UnityEngine.Debug.LogWarning("Please initialize first.");
                return false;
            }

            if (_viewer == null)
            {
                _viewer = new GridViwer(_grid);
            }
            viewer = _viewer;
            return viewer != null;
        }
    }
}

