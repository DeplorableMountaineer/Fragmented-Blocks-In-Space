using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float X_Min = 1.3f;
    [SerializeField] private float X_Max = 14.7f;
    private float Accuracy_Min = 0.846f;
    private float Accuracy_Max = 0.851f;

    private ControlType controlType;
    public float accuracy;
    private Ball[] balls;
    private bool isPossibleMultiball = false;
    public float pseudovel = 0;
    private Rigidbody2D rb;

    void Start() {
        balls = FindObjectsOfType<Ball>();
        controlType = GameInstance.GetInstance().GetControlType();
        if (controlType == ControlType.Autoplay) {
            accuracy = Random.Range(Accuracy_Min, Accuracy_Max);
            Invoke("AutoLaunch", 1f);
        }

        if (balls.Length > 1) {
            isPossibleMultiball = true;
            for (int i = 0; i < balls.Length; i++) {
                if (!balls[i].isBallTrapped) {
                    balls = new[] { balls[i] };
                    break;
                }
            }
        }

        rb = GetComponent<Rigidbody2D>();
    }

    void AutoLaunch() {
        balls[0].LaunchBall();
    }

    // Update is called once per frame
    void Update() {
        float x;
        Vector2 pos;
        pseudovel = 0;
        float oldPos = transform.position.x;
        switch (controlType) {
            case ControlType.Keyboard:
                break;
            case ControlType.Autoplay:
                if (balls[0] && balls[0].isActiveAndEnabled && balls[0].isBallInPlay) {
                    AutoPlay(accuracy);
                }
                break;
            default: //mouse
                Ball b = FindBestBall();
                x = (Input.mousePosition.x / Screen.width) * Camera.main.orthographicSize * Camera.main.aspect * 2;
                if (b && b.transform.position.y < 6) {
                    x = x * 0.8f + b.transform.position.x * 0.2f;
                }

                x = Mathf.Clamp(x, X_Min, X_Max);
                pos = new Vector2(x, transform.position.y);
                transform.position = pos;
                break;
        }

        pseudovel = (transform.position.x - oldPos) / Time.deltaTime;
    }

    public ControlType GetControlType() {
        return controlType;
    }

    Ball FindBestBall() {
        if (isPossibleMultiball) {
            var current_balls = FindObjectsOfType<Ball>();
            foreach (Ball x in current_balls) {
                //Debug.Log("Ball: " + x + "; in balls:" + balls.Contains(x) + "; in play: " + x.isBallInPlay + "; is trapped: " + x.isBallTrapped + "; tag: " + x.tag);
                if (!balls.Contains(x) && x.isBallInPlay && !x.isBallTrapped && x.tag == "Ball") {
                    var b = balls.ToList();
                    b.Add(x);
                    balls = b.ToArray();
                    //Debug.Log("Num Balls: " + balls.Length);
                    break;
                }
            }

            if (balls.Length > 1) {
                for (int i = balls.Length - 1; i >= 0; i--) {
                    if (!balls[i] || balls[i].tag != "Ball") {
                        var b = balls.ToList();
                        b.RemoveAt(i);
                        balls = b.ToArray();
                    }
                }
            } else {
                if (current_balls.Length <= 1) {
                    isPossibleMultiball = false;
                }
            }
        }

        if (isPossibleMultiball && balls.Length > 1) {
            int best_index = -1;
            float best_time = 100000f;

            for (int i = 0; i < balls.Length; i++) {
                if (balls[i].transform.position.y < 6) {
                    Vector2 vel = balls[i].GetComponent<Rigidbody2D>().velocity;
                    if (vel.y < 0) {
                        float t = Mathf.Abs(balls[i].transform.position.y / vel.y);
                        if (t < best_time || best_index < 0) {
                            best_index = i;
                            best_time = t;
                        }
                    }
                }
            }

            if (best_index >= 0) {
                return balls[best_index];
            }
        }

        if (balls.Length > 0) {
            return balls[0];
        }

        return null;
    }

    void AutoPlay(float accuracy) {
        float effective_accuracy = accuracy / Time.deltaTime * Time.timeScale * 0.017f;
        //check for stuck ball
        if (Time.timeSinceLevelLoad >= 600 && Mathf.RoundToInt(Time.timeSinceLevelLoad % 600) == 0) {
            transform.position = new Vector2((X_Min + X_Max) * .5f, transform.position.y);
            Debug.Log("Timeout: paddle moved to center.");
            return;
        }

        Ball b = FindBestBall();
        if (b) {
            Vector2 vel = b.GetComponent<Rigidbody2D>().velocity;
            if (vel.y < 0) {
                float x = Mathf.Clamp(b.transform.position.x, X_Min, X_Max);
                x = transform.position.x * (1 - effective_accuracy) + x * effective_accuracy;
                Vector2 pos = new Vector2(x, transform.position.y);
                transform.position = pos;
            }
        }
    }


}