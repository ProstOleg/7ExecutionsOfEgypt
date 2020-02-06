using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    private Text txt;
    string txt_select = "Выберите карту";
    string txt_setup = "Положите на правильное место, если пусто берите колодец";
    string txt_well = "";

    void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = "";
    }
}
