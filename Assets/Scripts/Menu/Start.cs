using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SevenExEg.Menu
{
    public class Start : MonoBehaviour
    {
        public void GoStartGame()
        {
            // TODO: (Oleg) проверить
            SceneManager.LoadScene(1);
        }
       
    }
}
