using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem.Internal;

namespace GridSystem
{
    public class GridGroup
    {
        private StateGrid _grid;
        private GridWriter _controller;
        private GridReader _viewer;

        public void Initialize(GridData data, Transform parent, int layerCount = 1)
        {
            _grid = new StateGrid();
            _grid.Initialize(data, parent, layerCount);
        }

        public bool GetGridWriter(out GridWriter controller)
        {
            controller = null;
            if (_grid == null)
            {
                UnityEngine.Debug.LogWarning("Please initialize first.");
                return false;
            }

            if(_controller == null)
            {
                _controller = new GridWriter(_grid);
            }
            controller = _controller;
            return controller != null;
        }

        public bool GetGridReader(out GridReader viewer)
        {
            viewer = null;
            if (_grid == null)
            {
                UnityEngine.Debug.LogWarning("Please initialize first.");
                return false;
            }

            if (_viewer == null)
            {
                _viewer = new GridReader(_grid);
            }
            viewer = _viewer;
            return viewer != null;
        }
    }
}

