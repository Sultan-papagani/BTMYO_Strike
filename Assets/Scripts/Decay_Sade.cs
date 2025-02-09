using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay_Sade : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }
}
