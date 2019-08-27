using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseCollider : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader = null;

    void Start() {
        if (sceneLoader == null) {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>()) {
                sceneLoader = go.GetComponent<SceneLoader>();
                if (sceneLoader != null) {
                    break;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ball") {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            if (balls.Length < 1 || (balls.Length == 1 && balls[0] == collision.gameObject)) {
                sceneLoader.GameOver();
            }
        }
        Destroy(collision.gameObject);
    }
}

