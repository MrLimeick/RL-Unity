using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RL.Editor;
using UnityEngine.UI;
using RL.GameEditor;

namespace RL.GameEditor
{
    public partial class Hierarchy : Window
    {
        public GameObject PointPrefab;
        public RectTransform Content;
        [SerializeField]
        private List<HierarchyPoint> m_Points = new();

        public float Size = 1f;
        public void Sync()
        {
            foreach (HierarchyPoint point in m_Points)
            {
                point.RectT.anchoredPosition = new Vector2(40f + point.OriginalPoint.Time * (40 * Size), 0);
            }
            Content.sizeDelta = new Vector2(m_Points[^1].RectT.anchoredPosition.x + 80f, 0);
        }

        public void CreatePoint(CardEditorPoint point)
        {
            HierarchyPoint HPoint = Instantiate(PointPrefab, Content.transform).GetComponent<HierarchyPoint>();
            HPoint.RectT = HPoint.GetComponent<RectTransform>();
            HPoint.OriginalPoint = point;

            point.OnRemove.AddListener(() =>
            {
                m_Points.Remove(HPoint);
                Destroy(HPoint.gameObject);
            });
            point.OnSelect.AddListener(() =>
            {
                HPoint.gameObject.GetComponent<Image>().color = new Color(0f, 0.5f, 1f);
            });
            point.OnDeSelect.AddListener(() =>
            {
                HPoint.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            });
            HPoint.OnClick.AddListener(point.SelectToggle);

            m_Points.Add(HPoint);
            point.HierarchyPoint = HPoint;
            Sync();
        }
        public void ChangeSize(float size)
        {
            Size = size;
            Sync();
        }
    }
}