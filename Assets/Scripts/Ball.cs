using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] GameObject paddle = null;
    [SerializeField] Vector2 launchSpeed = new Vector2(2f, 15f);
    [SerializeField] float preferredVelocity = 10f;
    [SerializeField] float randomBounce = 0.5f;
    [SerializeField] float velocityCorrectAmount = 0.5f;


    private Vector2 paddleToBallVector;
    private Rigidbody2D rb;
    private bool isBallInPlay = false;

    // Start is called before the first frame update
    void Start() {
        if (paddle == null) {
            paddle = GameObject.FindGameObjectsWithTag("Paddle")[0];
        }
        paddleToBallVector = transform.position - paddle.transform.position;
        rb = GetComponent<Rigidbody2D>();
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 v = rb.velocity;
        float m = v.magnitude;
        rb.AddForce(v.normalized * (preferredVelocity - m) * velocityCorrectAmount + Random.insideUnitCircle * randomBounce, ForceMode2D.Impulse);
    }
}
