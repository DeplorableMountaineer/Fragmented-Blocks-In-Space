using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Ball : MonoBehaviour
{
    [SerializeField] GameObject paddle = null;
    [SerializeField] Vector2 launchSpeed = new Vector2(2f, 15f);
    [SerializeField] float preferredVelocity = 10f;
    [SerializeField] float randomBounce = 0.5f;
    [SerializeField] float velocityCorrectAmount = 0.5f;
    [SerializeField] public float damage = 25f;
    [SerializeField] public bool isBallTrapped = false;
    [SerializeField] public float nudge_interval = 0.4f;
    [SerializeField] public float nudge_amount = 2.5f;
    [SerializeField] private GameObject nudgeEffect;


    private Vector2 paddleToBallVector;
    private Rigidbody2D rb;
    public bool isBallInPlay = false;
    private AudioSource audioSource;
    private float direction = 1f;
    private int stuck_count = 0;
    private int unstuck_failed = 0;

    private float paddle_base_y = 0;

    private float nudge_seconds = -1000f;

    private float paddle_hit_seconds = -1000f;

    private Block targetBlock = null;
    private Vector2? targetLocation = null;

    private GameObject[] waypoints;

    private bool seeking = true;

    private bool down = true;
    // Start is called before the first frame update
    void Start() {
        if (paddle == null) {
            paddle = GameObject.FindGameObjectsWithTag("Paddle")[0];
        }
        paddleToBallVector = transform.position - paddle.transform.position;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
    }

    // Update is called once per frame
    void Update() {
        if (!isBallInPlay) {
            LockBallToPaddle();
            LaunchBallOnMouseClick();
        } else if (!isBallTrapped && rb.velocity.magnitude < preferredVelocity * 0.001f) {//check for stuck ball
            stuck_count++;
            if (stuck_count > 100) {
                stuck_count = 0;
                unstuck_failed++;
                rb.AddForce(Random.insideUnitCircle * preferredVelocity, ForceMode2D.Impulse);
                Debug.Log("Stuck!");
                if (unstuck_failed > 20) {
                    transform.position = (Vector2)transform.position + Random.insideUnitCircle;
                }
            }
        } else {
            stuck_count = 0;
            unstuck_failed = 0;
            if (!down && transform.position.y < 6 && rb.velocity.y < 0) {
                down = true;
                Vector2 impulse = new Vector2(rb.velocity.x, rb.velocity.y).normalized * (preferredVelocity / 2 * velocityCorrectAmount + rb.velocity.magnitude * (1 - velocityCorrectAmount)) - rb.velocity;
                rb.AddForce(impulse, ForceMode2D.Impulse);
            } else if (down && (transform.position.y >= 6 || rb.velocity.y > 0)) {
                down = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            if (Time.time - paddle_hit_seconds < nudge_interval) {
                Nudge(paddle.GetComponent<Paddle>());
                nudge_seconds = -1000;
            } else {
                nudge_seconds = Time.time;
            }
        }

        if (Vector2.Dot((paddle.transform.position - transform.position).normalized, rb.velocity.normalized) >=
            0.95f) {
            rb.AddForce((paddle.transform.position - transform.position).normalized * Time.deltaTime,
                ForceMode2D.Impulse);
        } else if (seeking) {
            if (targetBlock == null || targetBlock.GetHealth() <= 0) {
                GameObject go = GameObject.FindGameObjectWithTag("Breakable Block");
                if (go) {
                    targetBlock = go.GetComponent<Block>();
                } else {
                    targetBlock = null;
                }
            }

            if (targetLocation != null && Vector2.Dot(((Vector2)targetLocation - (Vector2)transform.position).normalized, rb.velocity.normalized) >=
                0.95f) {
                rb.AddForce(((Vector3)targetLocation - transform.position).normalized * Time.deltaTime,
                    ForceMode2D.Impulse);
            } else if (targetBlock && Vector2.Dot((targetBlock.transform.position - transform.position).normalized, rb.velocity.normalized) >=
                0.95f) {
                rb.AddForce((targetBlock.transform.position - transform.position).normalized * Time.deltaTime,
                    ForceMode2D.Impulse);
            }
        }

        if (Random.Range(0, 100) < 20 * Time.deltaTime) {
            seeking = !seeking;
            if (seeking && waypoints.Length > 0) {
                targetLocation = waypoints[Random.Range(0, waypoints.Length)].transform.position;
            }
        }
    }

    private void LockBallToPaddle() {
        Vector2 paddlePosition = paddle.transform.position;
        transform.position = paddlePosition + paddleToBallVector;
        rb.angularVelocity = 0;
    }

    private void LaunchBallOnMouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            LaunchBall();
        }
    }

    public void LaunchBall() {
        isBallInPlay = true;
        if (Random.Range(0.0f, 9.0f) < 5.0f) {
            launchSpeed *= new Vector2(-1f, 1f);
        }
        rb.AddForce(launchSpeed, ForceMode2D.Impulse);
    }

    private void Nudge(Paddle p) {
        rb.AddForce(Vector2.up * preferredVelocity * nudge_amount, ForceMode2D.Impulse);
        PlayNudgeEffect(p);
    }

    void PlayNudgeEffect(Paddle p) {
        GameObject smoke = Object.Instantiate(nudgeEffect, transform.position, transform.rotation);
        ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
        Gradient gr = new Gradient();
        ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = true;
        Color c = GetComponent<SpriteRenderer>().color;
        gr.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 0.1f),
            new GradientColorKey(c, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f),
            new GradientAlphaKey(0.4f, 0.2f), new GradientAlphaKey(0.0f, 1.0f) });
        col.color = gr;
        paddle_base_y = p.transform.position.y;
        p.transform.position = new Vector2(p.transform.position.x, paddle_base_y + 0.1f);
        Invoke("RestorePaddle", 0.1f);
    }

    void RestorePaddle() {
        paddle.transform.position = new Vector2(paddle.transform.position.x, paddle_base_y);
    }

    private void AddRandomNudge(Rigidbody2D rb) {
        Vector2 v = rb.velocity;

        float vx = Mathf.Sign(v.x) * (Mathf.Abs(v.x) * .8f + Mathf.Abs(v.y) * .2f);
        float vy = Mathf.Sign(v.y) * (Mathf.Abs(v.y) * .8f + Mathf.Abs(v.x) * .2f);
        if (Random.Range(0, 100) < 20) {
            vy *= 4;
        }

        Vector2 b = Random.insideUnitCircle;
        float factor = 1;
        if (transform.position.y < 5) {
            factor = 0.5f;
        }
        Vector2 impulse = new Vector2(vx, vy).normalized * (preferredVelocity * factor * velocityCorrectAmount + v.magnitude * (1 - velocityCorrectAmount)) - v;

        if (Random.Range(0f, 100f) < 10f) {
            direction = -direction;
        }
        rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isBallInPlay && !isBallTrapped) {
            AddRandomNudge(rb);
            if (audioSource.isActiveAndEnabled) {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (collision.gameObject.tag == "Paddle") {
                Paddle p = collision.gameObject.GetComponent<Paddle>();
                rb.AddForce(p.pseudovel * Vector2.right, ForceMode2D.Impulse);
                if (Time.time - nudge_seconds < nudge_interval && p.GetControlType() != ControlType.Autoplay) {
                    rb.AddForce(rb.velocity.normalized * preferredVelocity * nudge_amount, ForceMode2D.Impulse);
                    paddle_hit_seconds = -1000f;
                } else if (collision.gameObject.GetComponent<Paddle>().GetControlType() == ControlType.Autoplay) {
                    if (Random.Range(0, 100) < 10) {
                        paddle_hit_seconds = -1000f;
                        Nudge(p);
                    }
                } else {
                    paddle_hit_seconds = Time.time;
                }
            }
        }
    }
}
