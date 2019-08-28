using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] float volume = .3f;
    [SerializeField] Level level = null;
    [SerializeField] SceneLoader sceneLoader = null;
    [SerializeField] GameStatus gameStatus = null;
    [SerializeField] private float pointValue = 1f;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ball") {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.1f);
            GetComponent<BoxCollider2D>().isTrigger = true;
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
            gameStatus.AddScore(pointValue);
        }

    }

    private void OnDestroy() {
        if (sceneLoader && level) {
            if (!sceneLoader.gameOver && level.BreakBlock() <= 0) {
                sceneLoader.LoadNextScene();
            }
        }
    }

    void Start() {
        if (level == null) {
            level = FindObjectOfType<Level>();
        }
        if (sceneLoader == null) {
            sceneLoader = FindObjectOfType<SceneLoader>();
        }
        if (gameStatus == null) {
            gameStatus = FindObjectOfType<GameStatus>();
        }
        level.AddBlock();
    }

}
