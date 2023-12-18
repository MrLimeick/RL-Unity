using System;
using System.Collections;
using UnityEngine;

namespace RL.CardEditor
{
    public partial class PathMaker
    {
        public class LinePreview : MonoBehaviour
        {
            private bool _enabled;
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    bool val = InViewport && Mode == Modes.BuildsPath && value;

                    _enabled = val;

                    Line.gameObject.SetActive(val);
                    Point.gameObject.SetActive(val);
                }
            }

            public Vector2 Position => Vector2.Lerp(Start, End, 0.5f);
            public const float s_Thickness = 0.25f;
            public float Angle => Mathf.Atan2(End.y - Start.y, End.x - Start.x) * Mathf.Rad2Deg;
            public float Lenght => Vector2.Distance(Start, End);

            public Vector2 Start => Paths.Current[^1].Position;
            public Vector2 End => MousePos;

            private Transform _line;
            public Transform Line
            {
                get => _line;
                set
                {
                    _line = value;
                }
            }

            private Transform _point;
            public Transform Point
            {
                get => _point;
                set
                {
                    _point = value;
                }
            }
            
            public void Update()
            {
                if (!Enabled)
                    return;

                Line.position = Position;
                Line.localScale = new Vector3(Lenght, s_Thickness, 0);
                Line.rotation = Quaternion.Euler(0, 0, Angle);
                Point.position = End;
            }

            public void Dispose()
            {

            }
        }
    }
}
