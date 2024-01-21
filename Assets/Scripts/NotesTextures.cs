using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New notes textures", menuName = "RL/Notes Textures")]
public class NotesTextures : ScriptableObject
{
    public Sprite Up, Down, Right, Left;

    public void Apply(SpriteRenderer renderer, NoteDirection direction)
    {
        renderer.sprite = direction switch
        {
            NoteDirection.Up => Up,
            NoteDirection.Down => Down,
            NoteDirection.Right => Right,
            _ => Left
        };
    }
}
