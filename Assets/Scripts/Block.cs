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
    [SerializeField] private GameObject ballPrefab = null;
    [SerializeField] private bool exploding = false;
    [SerializeField] private float explosionRadius = 3f;

    private float health;
    private string breakableBlockTag = "Breakable Block";
    private LevelInstance levelInstance = null;
    private GameInstance gameInstance = null;
    private GameObject trappedBall = null;

    private float next_damage = 0;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ball") {
            if (collision.gameObject.GetComponent<Ball>().isBallTrapped) {
                return;
            }
            PlayHitEffect();
            if (gameObject.tag == breakableBlockTag) {
                DamageBlock(collision.gameObject.GetComponent<Ball>().damage);
            }
        }
    }

    public void DamageBlock(float damage, float time = 0) {
        next_damage = damage;
        if (time == 0) {
            DamageBlockNow();
        } else {
            Invoke("DamageBlockNow", time);
        }
    }

    public void DamageBlockNow() {
        if (health > 0) {
            PlayDamageEffect();
            health -= next_damage;
            if (health <= 0) {
                DestroyBlock();
            } else {
                int hit = Mathf.FloorToInt((health / startingHealth) * hitSprites.Length);
                //Debug.Log("Health: " + health.ToString() + "   Starting Health: " + startingHealth.ToString() + "   Hit: " + hit.ToString());
                if (hit < hitSprites.Length) {
                    gameObject.GetComponent<SpriteRenderer>().sprite = hitSprites[hit];
                }
            }
        }
    }

    public float GetHealth() {
        return health;
    }

    private void OnDestroy() {
        if (gameInstance && levelInstance && !gameInstance.IsGameOver() && levelInstance.BreakBlock() <= 0 && !levelInstance.IsGameWon()) {
            levelInstance.SetGameWon();
            if (gameInstance.GetControlType() == ControlType.Autoplay) {
                LevelInstance.GetInstance().SaveStats(true);
            }
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

        if (transform.childCount > 0 && transform.GetChild(0).gameObject.tag == "Ball") {
            trappedBall = transform.GetChild(0).gameObject;
        }
    }

    void DestroyBlock() {
        GameObject newBall;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.1f);
        GetComponent<BoxCollider2D>().isTrigger = true;
        rb.AddForce(Vector2.down * 20f, ForceMode2D.Impulse);
        LevelInstance.GetInstance().AddScore(pointValue);
        if (trappedBall) {
            newBall = Instantiate(ballPrefab);
            newBall.GetComponent<Ball>().isBallInPlay = true;
            newBall.transform.position = trappedBall.transform.position;
            Destroy(trappedBall);
            trappedBall = null;
        }

        if (exploding) {
            PlayDamageEffect(true);
            foreach (Block b in FindObjectsOfType<Block>()) {
                if (b.gameObject != gameObject) {
                    float r = (b.transform.position - transform.position).magnitude;
                    if (r < explosionRadius) {
                        b.DamageBlock(explosionRadius / (r + .01f) * 100, r);
                    }
                }
            }
            Destroy(gameObject);
        }
    }


    void PlayDamageEffect(bool superdamage = false) {
        GameObject smoke = Object.Instantiate(hitEffect, transform.position, transform.rotation);
        ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
        Gradient gr = new Gradient();
        ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = true;
        Color c = GetComponent<SpriteRenderer>().color;
        if (superdamage) {
            smoke.transform.localScale *= 2;
            gr.SetKeys(new GradientColorKey[] {
                new GradientColorKey(c, 0.0f), new GradientColorKey(c, 0.1f),
                new GradientColorKey(c, 1.0f)
            }, new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f, 0.0f),
                new GradientAlphaKey(0.5f, 0.2f), new GradientAlphaKey(0.0f, 1.0f)
            });
        } else {
            gr.SetKeys(new GradientColorKey[] {
                new GradientColorKey(c, 0.0f), new GradientColorKey(c, 0.1f),
                new GradientColorKey(c, 1.0f)
            }, new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f, 0.0f),
                new GradientAlphaKey(0.1f, 0.2f), new GradientAlphaKey(0.0f, 1.0f)
            });
        }

        col.color = gr;
    }

    void PlayHitEffect() {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }
}
