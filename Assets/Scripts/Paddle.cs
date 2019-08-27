using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float X_Min = 1.3f;
    [SerializeField] private float X_Max = 14.7f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        float x = (Input.mousePosition.x / Screen.width) * Camera.main.orthographicSize * Camera.main.aspect * 2;
        Vector2 pos = new Vector2(Mathf.Clamp(x, X_Min, X_Max), transform.position.y);
        transform.position = pos;
    }

   
}