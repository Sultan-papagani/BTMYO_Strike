using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariables : MonoBehaviour
{
    public static StaticVariables singleton;

    public bool actual_host = false;
    public string OYUNCU_ADI = ""; // sadece yerel oyuncu için.

    private void Start() {
        singleton = this;
    }

    public bool AdHazir()
    {
        return OYUNCU_ADI == "";
    }
}
