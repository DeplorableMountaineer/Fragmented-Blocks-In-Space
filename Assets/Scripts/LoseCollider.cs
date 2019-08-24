using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseCollider : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ball") {
            Destroy(collision.gameObject);
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
             
            if (balls.Length < 1 || (balls.Length==1 && balls[0] == collision.gameObject))
            {
                sceneLoader.GameOver();
            }
        }
    }

}

