using UnityEngine;
using UnityEngine.Events;

public class Vector2Field : MonoBehaviour
{
    [SerializeField]
    private Field xField, yField;

    private float _x = 1;
    public float X
    {
        get => _x;
        set
        {
            _x = value;
            xField.SetTextWithoutNotify(value.ToString());
            onValueChanged.Invoke(Vector);
        }
    }

    private float _y = 1;
    public float Y
    {
        get => _y;
        set
        {
            _y = value;
            yField.SetTextWithoutNotify(value.ToString());
            onValueChanged.Invoke(Vector);
        }
    }

    public Vector2 Vector => new(X, Y);

    [Space]

    public UnityEvent<Vector2> onValueChanged;

    private void Awake()
    {
        float.TryParse(xField.text, out _x);
        float.TryParse(yField.text, out _y);

        xField.onValueChanged.AddListener((t) =>
        {
            if (!float.TryParse(t, out float res)) return;

            _x = res;
            onValueChanged.Invoke(Vector);
        });
        yField.onValueChanged.AddListener((t) =>
        {
            if (!float.TryParse(t, out float res)) return;

            _y = res;
            onValueChanged.Invoke(Vector);
        });
    }
}
