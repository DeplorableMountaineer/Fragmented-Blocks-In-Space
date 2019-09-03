﻿using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] GameObject paddle = null;
    [SerializeField] Vector2 launchSpeed = new Vector2(2f, 15f);
    [SerializeField] float preferredVelocity = 10f;
    [SerializeField] float randomBounce = 0.5f;
    [SerializeField] float velocityCorrectAmount = 0.5f;
    [SerializeField] public float damage = 25f;

    private Vector2 paddleToBallVector;
    private Rigidbody2D rb;
    public bool isBallInPlay = false;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        if (paddle == null) {
            paddle = GameObject.FindGameObjectsWithTag("Paddle")[0];
        }
        paddleToBallVector = transform.position - paddle.transform.position;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (!isBallInPlay) {
            LockBallToPaddle();
            LaunchBallOnMouseClick();
        }
    }

    private void LockBallToPaddle() {
        Vector2 paddlePosition = paddle.transform.position;
        transform.position = paddlePosition + paddleToBallVector;
        rb.angularVelocity = 0;
    }

    private void LaunchBallOnMouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            isBallInPlay = true;
            rb.AddForce(launchSpeed, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isBallInPlay) {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 v = rb.velocity;
            float m = v.magnitude;
            Vector2 force = v.normalized * (preferredVelocity - m) * velocityCorrectAmount + Random.insideUnitCircle * randomBounce;
            if (Mathf.Abs(v.x * v.y) <= .25f) {
                force += randomBounce * Random.Range(0f, 1f) * new Vector2(v.y, -v.x);
            }
            rb.AddForce(force, ForceMode2D.Impulse);
            if (audioSource.isActiveAndEnabled) {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }
}
