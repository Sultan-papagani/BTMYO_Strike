using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class killFeed : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI yazi;
    [SerializeField] TextMeshProUGUI yazi2;


    private void Start() {
        
    }

    public void _start(string _yazi, string _yazi2, float sure = 3f, bool s = true)
    {
        yazi.text = _yazi;
        if (s){
        yazi2.text = _yazi2;}
        Destroy(gameObject, sure);
    }
}
