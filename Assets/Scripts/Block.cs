using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] float volume = .3f;
    [SerializeField] GameObject level = null;
    [SerializeField] SceneLoader sceneLoader = null;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ball") {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.1f);
            GetComponent<BoxCollider2D>().isTrigger = true;
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
        }

    }

    private void OnDestroy() {
        if (level.GetComponent<Level>().BreakBlock() <= 0) {
            sceneLoader.LoadNextScene();
        }
    }

    void Start() {
        if (level == null) {
            level = GameObject.FindGameObjectsWithTag("Level")[0];
        }
        if (sceneLoader == null) {
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>()) {
                sceneLoader = go.GetComponent<SceneLoader>();
                if (sceneLoader != null) {
                    break;
                }
            }
        }
        level.GetComponent<Level>().AddBlock();
    }

}
