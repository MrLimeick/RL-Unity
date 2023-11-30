using System.Collections;
using System.Collections.Generic;
using RL.GameEditor;
using UnityEngine;
using static RL.GameEditor.PathMaker;

public class NoteCreationController : MonoBehaviour
{
    public Camera Camera;
    public CardEditorPath Path;
    public string Code = "";

    public void Update()
    {
        if (Code != "" && Input.GetKeyDown(Code))
        {
            print("Lol this worked");
        }
        if (Code == "" && Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
        {
            print(Input.inputString);
            Code = Input.inputString;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) Code = "";
    }
}
