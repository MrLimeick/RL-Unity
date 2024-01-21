using RL.CardEditor;
using UnityEngine;
using System.Collections.Generic;
using RL.Math;

namespace RL.UI
{
    public enum GridType
    {
        /// <summary>
        /// Сетка из квадратов.
        /// <para>Стандартная сетка.</para>
        /// </summary>
        Square,
        /// <summary>
        /// Сетка из кругов и напрвлений.
        /// </summary>
        Circle
    }

    /// <summary>
    /// Скрипт позволяющей отрисовать 2 вида сетки поверх объектов.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class GridCamera : MonoBehaviour
    {
        /// <summary>
        /// Цвет сетки.
        /// </summary>
        public Color GridColor = Color.black;

        
        private Vector2 resolution = new(.5f, .5f);

        /// <summary>
        /// Разрешение квадратной сетки.
        /// </summary>
        public Vector2 Resolution
        {
            get => resolution;
            set => resolution = new(Mathf.Clamp(value.x, 0.001f, float.MaxValue), Mathf.Clamp(value.y, 0.001f, float.MaxValue));
        }

        /// <summary>
        /// Данная камера.
        /// </summary>
        private Camera Cam;

        /// <summary>
        /// Камера за которой будет следить сетка.
        /// </summary>
        public Camera SeekToCamera = null;

        /// <summary>
        /// Радиус круговой сетки.
        /// </summary>
        private float radius = 1f;
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Clamp(value, 0.001f, float.MaxValue);
        }

        //public float Step = Mathf.PI / 36; // TODO: Шаг угла круговой сетки.

        /// <summary>
        /// Тип сетки.
        /// </summary>
        public GridType GridType = GridType.Square;

        /// <summary>
        /// Материал сетки.
        /// </summary>
        private static Material lineMat;

        #region Grid 

        private void Awake()
        {
            Cam = GetComponent<Camera>();
        }

        private static void CreateLineMaterial()
        {
            if (!lineMat)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMat = new(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                // Turn on alpha blending
                lineMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMat.SetInt("_ZWrite", 0);
            }
        }

        public void OnPostRender()
        {
            CreateLineMaterial();
            lineMat.SetPass(0);

            if (SeekToCamera != null)
            {
                Cam.transform.position = SeekToCamera.transform.position;
                Cam.orthographicSize = SeekToCamera.orthographicSize;
            }

            Vector2
                pos = transform.position,
                center = PathMaker.GlobalGrid ? Vector2.zero : PathMaker.LastPointPosition, // TODO: Отвязать сетку от PathMaker
                zero = (Vector2)Cam.ViewportToWorldPoint(new Vector2(0f, 0f)) - center,
                one = (Vector2)Cam.ViewportToWorldPoint(new Vector2(1f, 1f)) - center;

            GL.PushMatrix();
            GL.MultMatrix(Cam.transform.localToWorldMatrix);

            switch (GridType)
            {
                case GridType.Circle:
                    DrawCircleGrid(zero, one, center, pos);
                    break;
                case GridType.Square:
                    DrawGrid(zero, one, center, pos);
                    break;
                default:
                    throw new System.NotImplementedException("Похоже, данный тип сетки пока не поддерживаеться.");
            }

            GL.PopMatrix();
        }

        void DrawCircleGrid(Vector2 zero, Vector2 one, Vector2 center, Vector2 pos)
        {
            float count = Vector2.Distance(zero, one);

            float x1 = Mathf.Max(0, zero.x);
            float y1 = Mathf.Max(0, zero.y);

            float x2 = Mathf.Min(0, one.x);
            float y2 = Mathf.Min(0, one.y);

            float dis = Vector2.Distance(new Vector2(
                (x1 > 0) ? x1 : (x2 < 0) ? x2 : 0.5f,
                (y1 > 0) ? y1 : (y2 < 0) ? y2 : 0.5f), Vector2.zero);

            for (float i = 0; i < count; i += radius)
                DrawCircle(Maths.Round(i + dis, radius), 72, -pos + center); 

            DrawLines(Maths.Round(count + dis, radius), 72, -pos + center);
        }

        void DrawGrid(Vector2 zero, Vector2 one, Vector2 center, Vector2 pos)
        {
            center += zero - pos;
            Vector2 camSize = one - zero;

            float
                horizontalNum = camSize.x / resolution.x,
                offsetX = center.x - (zero.x % resolution.x),
                verticalNum = camSize.y / resolution.y,
                offsetY = center.y - (zero.y % resolution.y);

            GL.Begin(GL.LINES);

            for (int i = 0; i < horizontalNum; i++) // Draw horizontal lines
            {
                float x = offsetX + resolution.x * i;

                GL.Color(GridColor);
                GL.Vertex3(x, center.y, 0f);

                GL.Color(GridColor);
                GL.Vertex3(x, center.y + verticalNum * resolution.y, 0f);
            }

            for (int i = 0; i < verticalNum; i++) // Draw vertical lines
            {
                float y = offsetY + resolution.y * i;

                GL.Color(GridColor);
                GL.Vertex3(center.x, y, 0f);

                GL.Color(GridColor);
                GL.Vertex3(center.x + horizontalNum * resolution.x, y, 0f);
            }

            GL.End();
        }

        void DrawCircle(float radius, int numOfVertices, Vector2 pos)
        {
            GL.Begin(GL.LINE_STRIP);

            float step = Mathf.PI * 2 / numOfVertices;
            for (int j = 0; j < numOfVertices + 1; j++)
            {
                float radian = j * step;
                float x = Mathf.Cos(radian) * radius + pos.x;
                float y = Mathf.Sin(radian) * radius + pos.y;

                GL.Color(GridColor);
                GL.Vertex3(x, y, 0f);
            }

            GL.End();
        }

        void DrawLines(float radius, int numOfVertices, Vector2 pos)
        {
            GL.Begin(GL.LINES);

            float step = Mathf.PI * 2 / numOfVertices;
            for (int j = 0; j < numOfVertices; j++)
            {
                float radian = j * step;

                float cos = Mathf.Cos(radian);
                float sin = Mathf.Sin(radian);

                GL.Color(GridColor);
                GL.Vertex3(pos.x + cos * this.radius, pos.y + sin * this.radius, 0);

                GL.Color(GridColor);
                GL.Vertex3(cos * radius + pos.x, sin * radius + pos.y, 0f);
            }

            GL.End();
        }
        #endregion
    }
}