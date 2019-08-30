using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Block : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] float volume = .3f;
    [SerializeField] Level level = null;
    [SerializeField] SceneLoader sceneLoader = null;
    [SerializeField] GameStatus gameStatus = null;
    [SerializeField] private float pointValue = 1f;

    [SerializeField] private GameObject hitEffect;
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ball") {
            PlayHitEffect();
            DestroyBlock();
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

    void DestroyBlock() {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.1f);
        GetComponent<BoxCollider2D>().isTrigger = true;
        gameStatus.AddScore(pointValue);
    }

    void PlayHitEffect() {
        GameObject smoke = Object.Instantiate(hitEffect, transform.position, transform.rotation);
        ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
        Gradient gr = new Gradient();
        ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = true;
        Color c = GetComponent<SpriteRenderer>().color;
        gr.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 0.1f), new GradientColorKey(c, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(0.1f, 0.2f), new GradientAlphaKey(0.0f, 1.0f) });
        col.color = gr;
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }
}
