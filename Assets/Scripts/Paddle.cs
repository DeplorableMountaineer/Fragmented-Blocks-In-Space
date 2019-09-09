using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float X_Min = 1.3f;
    [SerializeField] private float X_Max = 14.7f;
    private float Accuracy_Min = 0.3f;
    private float Accuracy_Max = 0.4f;

    private ControlType controlType;
    public float accuracy;
    private Ball ball;
    private Ball ball2 = null;
    private bool isPossibleMultiball = false;

    void Start() {
        ball = FindObjectOfType<Ball>();
        controlType = GameInstance.GetInstance().GetControlType();
        if (controlType == ControlType.Autoplay) {
            accuracy = Random.Range(Accuracy_Min, Accuracy_Max);
            Invoke("AutoLaunch", 1f);
        }

        if (FindObjectsOfType<Ball>().Length > 1) {
            isPossibleMultiball = true;
        }
    }

    void AutoLaunch() {
        ball.LaunchBall();
    }

    // Update is called once per frame
    void Update() {
        float x;
        Vector2 pos;
        switch (controlType) {
            case ControlType.Keyboard:
                break;
            case ControlType.Autoplay:
                if (ball && ball.isActiveAndEnabled && ball.isBallInPlay) {
                    AutoPlay(accuracy);
                }
                break;
            default: //mouse
                x = (Input.mousePosition.x / Screen.width) * Camera.main.orthographicSize * Camera.main.aspect * 2;
                pos = new Vector2(Mathf.Clamp(x, X_Min, X_Max), transform.position.y);
                transform.position = pos;
                break;
        }

    }

    void AutoPlay(float accuracy) {
        if (Time.timeSinceLevelLoad >= 300 && Mathf.RoundToInt(Time.timeSinceLevelLoad % 300) == 0) {
            transform.position = new Vector2((X_Min + X_Max) * .5f, transform.position.y);
            Debug.Log("Timeout: paddle moved to center.");
            return;
        }
        if (ball2 && ball.tag != "Ball") {
            ball = ball2;
            ball2 = null;
            if (FindObjectsOfType<Ball>().Length <= 1) {
                isPossibleMultiball = false;
            }
        } else if (ball2 && ball2.tag != "Ball") {
            ball2 = null;
            if (FindObjectsOfType<Ball>().Length <= 1) {
                isPossibleMultiball = false;
            }
        }

        if (ball2 && Mathf.Abs(ball2.transform.position.x - transform.position.x) <
            Mathf.Abs(ball.transform.position.x - transform.position.x) && ball2.GetComponent<Rigidbody2D>().velocity.y < 0) {
            if (ball2.transform.position.y < 6) {
                float x = Mathf.Clamp(ball2.transform.position.x, X_Min, X_Max);
                x = transform.position.x * (1 - accuracy) + x * accuracy;
                Vector2 pos = new Vector2(x, transform.position.y);
                transform.position = pos;
            }
        } else if (ball.transform.position.y < 6 && ball.GetComponent<Rigidbody2D>().velocity.y < 0) {
            float x = Mathf.Clamp(ball.transform.position.x, X_Min, X_Max);
            x = transform.position.x * (1 - accuracy) + x * accuracy;
            Vector2 pos = new Vector2(x, transform.position.y);
            transform.position = pos;
        } else if (ball2) {
            if (ball2.transform.position.y < 6) {
                float x = Mathf.Clamp(ball2.transform.position.x, X_Min, X_Max);
                x = transform.position.x * (1 - accuracy) + x * accuracy;
                Vector2 pos = new Vector2(x, transform.position.y);
                transform.position = pos;
            }
            if (FindObjectsOfType<Ball>().Length <= 1) {
                isPossibleMultiball = false;
            }
        } else if (isPossibleMultiball) {
            foreach (Ball x in FindObjectsOfType<Ball>()) {
                if (x != ball && x.isBallInPlay && !x.isBallTrapped && x.tag == "Ball") {
                    ball2 = x;
                    break;
                }
            }
        }
    }


}