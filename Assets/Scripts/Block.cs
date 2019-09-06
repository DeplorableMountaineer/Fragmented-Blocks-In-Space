using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Block : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] float volume = .3f;
    [SerializeField] private float pointValue = 1f;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float startingHealth = 25f;
    [SerializeField] private Sprite[] hitSprites;
    private float health;
    private string breakableBlockTag = "Breakable Block";
    private LevelInstance levelInstance = null;
    private GameInstance gameInstance = null;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ball") {
            PlayHitEffect();
            if (gameObject.tag == breakableBlockTag) {
                PlayDamageEffect();
                health -= collision.gameObject.GetComponent<Ball>().damage;
                if (health <= 0) {
                    DestroyBlock();
                } else {
                    int hit = Mathf.RoundToInt(health / (startingHealth / (1f + hitSprites.Length)));
                    if (hit < hitSprites.Length) {
                        gameObject.GetComponent<SpriteRenderer>().sprite = hitSprites[hitSprites.Length - hit - 1];
                    }
                }
            }
        }
    }

    private void OnDestroy() {
        if (gameInstance && levelInstance && !gameInstance.IsGameOver() && levelInstance.BreakBlock() <= 0 && !levelInstance.IsGameWon()) {
            levelInstance.SetGameWon();
            LevelInstance.GetInstance().SaveStats(true);
            gameInstance.LoadNextScene();
        }
    }

    void Start() {
        health = startingHealth;
        levelInstance = LevelInstance.GetInstance();
        gameInstance = GameInstance.GetInstance();
        if (gameObject.tag == breakableBlockTag) {
            LevelInstance.GetInstance().AddBlock();
        }
    }

    void DestroyBlock() {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.1f);
        GetComponent<BoxCollider2D>().isTrigger = true;
        LevelInstance.GetInstance().AddScore(pointValue);
    }

    void PlayDamageEffect() {
        GameObject smoke = Object.Instantiate(hitEffect, transform.position, transform.rotation);
        ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
        Gradient gr = new Gradient();
        ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = true;
        Color c = GetComponent<SpriteRenderer>().color;
        gr.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 0.1f),
            new GradientColorKey(c, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f),
            new GradientAlphaKey(0.1f, 0.2f), new GradientAlphaKey(0.0f, 1.0f) });
        col.color = gr;
    }

    void PlayHitEffect() {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }
}
