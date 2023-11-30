using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

namespace RL.GameEditor
{
    [AddComponentMenu("RL/Card Editor/Camera")]
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Camera Cam;

        void Awake()
        {
            Cam = GetComponent<Camera>();

            m_Size = Cam.orthographicSize;
        }

        Vector3 previousMousePosition;
        Vector3 MouseDelta
        {
            get
            {
                var mousePosDelta = Input.mousePosition - previousMousePosition;
                return new Vector3(mousePosDelta.x / Screen.height, mousePosDelta.y / Screen.height) * (Cam.orthographicSize * 2);
            }
        }

        [Min(0.1f)] public float MinSize = 1;
        [Min(0.1f)] public float MaxSize = 20;
        [Min(0.1f)] public float ScrollSensivity = 0.5f;

        public bool CanMove = true;
        public bool CanScroll = true;

        [SerializeField] private float m_Size = 5f;

        public bool IsTrackpad = false;

        void Update()
        {
            if (IsTrackpad)
            {
                if(Input.GetKey(KeyCode.LeftAlt))
                {
                    if (CanScroll) m_Size = Mathf.Clamp(m_Size + Input.mouseScrollDelta.y * ScrollSensivity, MinSize, MaxSize);
                }
                else if (CanMove) transform.position -= (Vector3)(Input.mouseScrollDelta * new Vector2(0.2f * m_Size, -0.2f * m_Size));
            }
            else
            {
                if (Input.GetMouseButton(2) && CanMove)
                    transform.position -= MouseDelta;
                if (CanScroll)
                    m_Size = Mathf.Clamp(m_Size + Input.mouseScrollDelta.y * ScrollSensivity, MinSize, MaxSize);
            }

            previousMousePosition = Input.mousePosition; // Save mouse position for mouseDelta
        }

        private void FixedUpdate()
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, m_Size, 0.5f);
        }
    }
}
/*
/// <summary>
/// ���������� ��� ������� ������ �������� ���������
/// </summary>
[AddComponentMenu("RL/Camera controller")]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera Cam;
    [HideInInspector]
    public float CamSize = 5;

    /// <summary>
    /// ����������� ������ ������
    /// </summary>
    [Header("Settings")]
    public float CamSizeMin = 0.1f;
    /// <summary>
    /// ������������ ������ ������
    /// </summary>
    public float CamSizeMax = 5;
    private float OldCamSize;
    /// <summary>
    /// ���������������� ��������� ������ �������� �����
    /// </summary>
    public float CamSizeSens = 0.1f;

    private Coroutine Move, Scroll;
    private Vector3 Asspect;

    /// <summary>
    /// ����� �� �������� ������
    /// </summary>
    public bool CanMove = true;
    /// <summary>
    /// ����� �� �������� ������ ������
    /// </summary>
    public bool CanScroll = true;
    void Start()
    {
        Cam = GetComponent<Camera>();
    }
    void Update()
    {

        #region ScrollWheel
        float Scroll = Input.GetAxis("Mouse ScrollWheel");
        if (CanScroll)
        {
            if (Scroll < 0) CamSize += CamSizeSens;
            if (Scroll > 0) CamSize -= CamSizeSens;
        }
        if (CamSize > CamSizeMax) CamSize = CamSizeMax;
        if (CamSize < CamSizeMin) CamSize = CamSizeMin;
        CamSize = Mathf.Round(CamSize / 0.1f) * 0.1f;
        if (CamSize != OldCamSize)
        {
            if (this.Scroll != null) StopCoroutine(this.Scroll);
            this.Scroll = StartCoroutine(ScroolTo(CamSize));
            OldCamSize = CamSize;
        }
        #endregion
        #region Move
        Asspect = (Cam.ViewportToWorldPoint(new Vector2(1, 1)) - Cam.transform.position) * 2;
        if (Input.GetMouseButtonDown(2) && CanMove)
        {
            if (EasingCamMoveCoroutine != null) StopCoroutine(EasingCamMoveCoroutine);
            Move = StartCoroutine(CamMove());
        }
        if (Input.GetMouseButtonUp(2) && CanMove && Move != null)
        {
            StopCoroutine(Move);
        }
        #endregion
    }
    IEnumerator CamMove()
    {
        var oldMousePos = Cam.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f, 0);
        var OldPos = Cam.transform.position;
        while (CanMove)
        {
            Vector2 MousePos = Cam.ScreenToViewportPoint(Input.mousePosition) - (new Vector3(0.5f, 0.5f, 0));
            Cam.transform.position = new Vector3(OldPos.x + -(MousePos.x - oldMousePos.x) * Asspect.x, OldPos.y + -(MousePos.y - oldMousePos.y) * Asspect.y, 0);
            yield return 1;
        }
    }
    Coroutine EasingCamMoveCoroutine = null;
    public void EasingCamMove(Vector2 To, bool CanMove,float Lenght = 1)
    {
        if (EasingCamMoveCoroutine != null) StopCoroutine(EasingCamMoveCoroutine);
        EasingCamMoveCoroutine = StartCoroutine(EasingCamMoveAnim(To,CanMove,Lenght));
    }
    IEnumerator EasingCamMoveAnim(Vector2 To, bool CanMove,float Lenght = 1)
    {
        bool CouldCamMove = this.CanMove;
        this.CanMove = CanMove;
        float OldTime = Time.unscaledTime;
        Vector2 From = Cam.transform.position;

        float localTime = 0;
        while((localTime = (Time.unscaledTime - OldTime) / Lenght) < 1)
        {
            yield return new WaitForEndOfFrame();
            Cam.transform.position = Vector2.Lerp(From,To,localTime);
        }

        Cam.transform.position = To;
        this.CanMove = CouldCamMove;
    }
    bool CouldCamMove;
    private CancellationTokenSource FixToken;
    /// <summary>
    /// ������������� ������ �� �������
    /// </summary>
    public void UnFix()
    {
        if(FixToken != null) FixToken.Cancel();
    }
    /// <summary>
    /// ������������� ������ �� �������
    /// </summary>
    /// <param name="objectToFix">������ �� ������� ����� ������������ ����������</param>
    public async void Fix(GameObject objectToFix)
    {
        FixToken = new CancellationTokenSource();
        CouldCamMove = CanMove;
        CanMove = false;
        float OldTime = Time.unscaledTime;
        Vector2 From = Cam.transform.position;

        float localTime = 0;
        while ((localTime = (Time.unscaledTime - OldTime) / 0.5f) < 1)
        {
            if (FixToken.IsCancellationRequested) return;
            Cam.transform.position = Vector2.Lerp(From, objectToFix.transform.position, localTime);
            await Task.Yield();
        }
        while (true)
        {
            if (FixToken.IsCancellationRequested) return;
            Cam.transform.position = objectToFix.transform.position;
            await Task.Yield();
        }
    }
    IEnumerator ScroolTo(float To)
    {
        //var OldSize = CamSize;
        var OldTime = Time.time;
        while ((Time.time - OldTime) / 0.2f <= 1)
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, To, (Time.time - OldTime) / 0.2f);
            yield return 1;
        }
        Asspect = (Cam.ViewportToWorldPoint(new Vector2(1, 1)) - Cam.transform.position) * 2;
        Cam.orthographicSize = To;
    }
    public void Reset()
    {
        CamSize = 5;
        Cam = Camera.main;
        Cam.transform.position = Vector2.zero;
        Cam.orthographicSize = CamSize;
    }

}
}
*/
