using System.Collections.Generic;
using UnityEngine;

namespace RL.CardEditor
{
    public partial class PathMaker
    {
        public class SelectedPointsList
        {
            private readonly List<CardEditorPoint> Points = new();

            public CardEditorPoint this[int index] => Points[index];

            public void Select(CardEditorPoint point)
            {
                Points.Add(point);
                point.OnPointSelect();
            }

            public void UnSelect(CardEditorPoint point)
            {
                if (Points.Remove(point))
                    point.OnPointUnSelect();
            }

            public void RemoveAll()
            {
                for (int i = 0; i < Points.Count; i++)
                    Paths.Current.Remove(Points[i]);

                Points.Clear();
            }

            public void RemoveLast() // TODO: Remove last point method
            {
                throw new System.NotImplementedException();
            }

            public void UnSelectAll()
            {
                Points.ForEach((p) => p.OnPointUnSelect());
                Points.Clear();
            }

            public void MoveAll(Vector2 vector)
            {
                Points.ForEach((p) => p.Position += vector);
            }
        }
    }
}
