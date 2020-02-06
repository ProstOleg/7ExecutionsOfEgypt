using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void GoStartGame()
    {
        SceneManager.LoadScene("_7ExEg");
    }
    public void GoExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
    public void GoMenu()
    {
        SceneManager.LoadScene("_Menu");
    }
}
