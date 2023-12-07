using UnityEngine;

namespace RL.CardEditor
{
    public class GridSettings
    {
        private readonly GridCamera _gridCamera;
        private Vector2 _resolution = new(1f, 1f);
        private Vector2 _buildPath = new(1f, 1f);
        private Vector2 _editingCurves = new(.25f, .25f);

        public Vector2 Resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                _gridCamera.Resolution = value;
            }
        }

        public Vector2 BuildPath
        {
            get => _buildPath;
            set
            {
                _buildPath = value;
                Resolution = value;
            }
        }

        public Vector2 EditingCurves
        {
            get => _editingCurves;
            set
            {
                _editingCurves = value;
                Resolution = value;
            }
        }

        public GridSettings(GridCamera gridCamera)
        {
            _gridCamera = gridCamera;
            _gridCamera.Resolution = _resolution;
        }
    }
}
