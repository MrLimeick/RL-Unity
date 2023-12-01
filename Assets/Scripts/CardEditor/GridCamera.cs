using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GridCamera : MonoBehaviour
{
    public Vector2 Resolution = new(.5f,.5f);
    public Color GridColor = Color.black;

    #region Grid 
    private Camera Cam;
    public Camera SeekToCamera = null;
    private static Material lineMat;

    private void Awake()
    {
        CreateLineMaterial();
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

        if(SeekToCamera != null)
        {
            Cam.transform.position = SeekToCamera.transform.position;
            Cam.orthographicSize = SeekToCamera.orthographicSize;
        }

        Vector2
            zero = Cam.ViewportToWorldPoint(new Vector2(0f, 0f)),
            one = Cam.ViewportToWorldPoint(new Vector2(1f, 1f)),
            fixedZero = zero - (Vector2)Cam.transform.position,
            camSize = one - zero;

        float
            horizontalNum = camSize.x / Resolution.x,
            offsetX = fixedZero.x - (zero.x % Resolution.x),
            verticalNum = camSize.y / Resolution.y,
            offsetY = fixedZero.y - (zero.y % Resolution.y);

        GL.PushMatrix();
        GL.MultMatrix(Cam.transform.localToWorldMatrix);
        GL.Begin(GL.LINES);

        for (int i = 0; i < horizontalNum; i++) // Draw horizontal lines
        {
            float x = offsetX + Resolution.x * i;

            GL.Color(GridColor);
            GL.Vertex3(x, fixedZero.y, 0f);

            GL.Color(GridColor);
            GL.Vertex3(x, fixedZero.y + verticalNum * Resolution.y, 0f);
        }

        for (int i = 0; i < verticalNum; i++) // Draw vertical lines
        {
            float y = offsetY + Resolution.y * i;

            GL.Color(GridColor);
            GL.Vertex3(fixedZero.x, y, 0f);

            GL.Color(GridColor);
            GL.Vertex3(fixedZero.x + horizontalNum * Resolution.x, y, 0f);
        }

        GL.End();
        GL.PopMatrix();
    }
    #endregion
}
