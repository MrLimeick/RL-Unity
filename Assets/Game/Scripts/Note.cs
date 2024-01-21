using UnityEngine;

public enum NoteDirection
{
	Up,
	Down,
	Right,
	Left
}

public class Note : MonoBehaviour
{
	public NoteDirection direction = NoteDirection.Up;
	public float time;

    public NotesTextures Textures;

	[Header("Renderers")]
	[SerializeField] private SpriteRenderer Layer1;
	[SerializeField] private SpriteRenderer Layer2;
	[SerializeField] private SpriteRenderer Key;
    [SerializeField] private SpriteRenderer Approach;

	private bool isHitted = false;

    void Update()
	{
		float t = Game.Time - time;

		switch(t)
		{
			case < 0:
				t = Mathf.Min(t, 0);
				t = Mathf.Abs(t);
                t = 1 - Mathf.Clamp(t, 0, 1);

                Approach.color = new(1, 1, 1, t);
                Approach.transform.localScale = Vector2.Lerp(Vector2.one, Vector2.one * 0.2f, t);
                break;
			case < 1 / 2f:
				if(isHitted)
				{
					t = Easing.OutQuint(t * 2f);
                    transform.localScale = new(1 + 2 * t, 1 + 2 * t);

					Color color = Layer1.color;
					Color color1 = Color.white;
					color.a = 1 - t;
					color1.a = 1 - t;
					Layer1.color = color;
					Layer2.color = color1;
					Key.color = color1;
                }
                break;
			case > 1: Destroy(gameObject); break;
		}
    }

	public void Hit()
	{
		isHitted = true;
		Approach.gameObject.SetActive(false);
    }

	public void Miss()
	{
		Layer1.color = Color.gray;
        //Circle.material.SetFloat("_missed", 1);
	}
}

