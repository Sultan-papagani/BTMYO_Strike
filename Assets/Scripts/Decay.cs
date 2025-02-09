using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    Color _color;
    float fade = 0;
    Color target_color = new Color(0,0,0,0);
    void Start()
    {
        _color = line.startColor;
        Destroy(gameObject, 1f);
    }

    void FixedUpdate() 
    {
        line.startColor = Color.Lerp(_color, target_color, fade);
        transform.Translate(Vector3.up * Time.deltaTime * 0.05f);
        fade += Time.deltaTime;
    }
}
