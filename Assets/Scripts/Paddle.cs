using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float X_Min = 1.3f;
    [SerializeField] private float X_Max = 14.7f;
    private ControlType controlType;

    private Ball ball;
    void Start() {
        ball = FindObjectOfType<Ball>();
        controlType = GameInstance.GetInstance().GetControlType();
        if (controlType == ControlType.Autoplay) {
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
                if (ball.isBallInPlay) {
                    x = ball.transform.position.x;
                    pos = new Vector2(Mathf.Clamp(x, X_Min, X_Max), transform.position.y);
                    transform.position = pos;
                }
                break;
            default: //mouse
                x = (Input.mousePosition.x / Screen.width) * Camera.main.orthographicSize * Camera.main.aspect * 2;
                pos = new Vector2(Mathf.Clamp(x, X_Min, X_Max), transform.position.y);
                transform.position = pos;
                break;
        }

    }


}