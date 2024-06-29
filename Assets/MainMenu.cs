using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static void LoadScene(string scene)
    { 
        SceneManager.LoadScene(scene);
    }
    public static void ExitGame()
    {
        Application.Quit();
    }

    public static void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public static void OpenMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeInHierarchy);
    }
}
