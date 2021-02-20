using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.HasSelectedBuilding = false;
        GameManager.instance.isMenuActive = false;
        Time.timeScale = 1.0f;
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("Atlantice");
    }

    public void onQuitButton()
    {
        Application.Quit();
    }
}
