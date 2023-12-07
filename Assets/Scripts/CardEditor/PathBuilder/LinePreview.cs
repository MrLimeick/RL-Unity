using UnityEngine;

namespace RL.CardEditor
{
    public partial class PathMaker
    {
        public class LinePreview
        {
            private bool _enabled;
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    bool val = InViewport && Mode == Modes.BuildsPath && value;

                    _enabled = val;

                    _line.gameObject.SetActive(val);
                    _point.gameObject.SetActive(val);
                }
            }

            public Vector2 Position => Vector2.Lerp(StartPoint, EndPoint, 0.5f);
            public const float s_Thickness = 0.25f;
            public float Angle => Mathf.Atan2(EndPoint.y - StartPoint.y, EndPoint.x - StartPoint.x) * Mathf.Rad2Deg;
            public float Lenght => Vector2.Distance(StartPoint, EndPoint);
            public Vector2 StartPoint => Paths.Current[^1].Position;
            public Vector2 EndPoint => MousePos;

            private readonly Transform _line;
            private readonly Transform _point;

            public LinePreview(Transform line, Transform point)
            {
                _line = line;
                _point = point;
            }

            public void Update()
            {
                if (!Enabled)
                    return;

                _line.position = Position;
                _line.localScale = new Vector3(Lenght, s_Thickness, 0);
                _line.rotation = Quaternion.Euler(0, 0, Angle);
                _point.position = EndPoint;
            }
        }
    }
}
