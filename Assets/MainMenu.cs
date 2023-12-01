using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenCardEditor()
    {
        SceneManager.LoadScene("Path maker");
    }

    public async void Close()
    {
        bool ans = await Dialog.ShowDialog("Quit?", "Are you sure to quit?");
        if(ans) Application.Quit();
    }
}