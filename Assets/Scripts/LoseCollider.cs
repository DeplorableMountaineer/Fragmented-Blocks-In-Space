using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ball") {
            collision.gameObject.tag = "Dead";
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject ball in balls) {
                if (ball == collision.gameObject) {
                    continue;
                }
                if (ball.GetComponent<Ball>().isBallTrapped) {
                    continue;
                }
                return;
            }
            GameInstance.GetInstance().GameOver();
        }
        Destroy(collision.gameObject);
    }
}

