using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ball") {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            if (balls.Length < 1 || (balls.Length == 1 && balls[0] == collision.gameObject)) {
                GameInstance.GetInstance().GameOver();
            }
        }
        Destroy(collision.gameObject);
    }
}

