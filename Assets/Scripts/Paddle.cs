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

    void Start() {
        ball = FindObjectOfType<Ball>();
        controlType = GameInstance.GetInstance().GetControlType();
        if (controlType == ControlType.Autoplay) {
            accuracy = Random.Range(Accuracy_Min, Accuracy_Max);
            Invoke("AutoLaunch", 1f);
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
        if (ball.transform.position.y < 5) {
            float x = Mathf.Clamp(ball.transform.position.x, X_Min, X_Max);
            x = transform.position.x * (1 - accuracy) + x * accuracy;
            Vector2 pos = new Vector2(x, transform.position.y);
            transform.position = pos;
        }
    }


}