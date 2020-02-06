using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    private static Text txt;

    void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = "";
    }
    public static void ShowRes(string res)
    {
        txt.text = res;
    }
}
