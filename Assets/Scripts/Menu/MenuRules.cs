using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuRules : MonoBehaviour
{
    private Text txt;
    private bool show = false;
    string txt_rules = "Выберит одну карту." + "Найдите ей место." +
            "Если место не занято другой картой, то возьмите карту из колодцов." +
            "Если колодцы опустеют и карты не на своих местах, то вы проиграли.";
    void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = "";
    }

    public void SwitchRules()   // Запускается из UIB_Rules, в методе On_click отправляется сообщение.
    {
        show = !show;
    }

    private void Update()
    {
        if (show == false)
            txt.text = "";
        else
            txt.text = txt_rules;
    }
}
