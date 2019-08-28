using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatus : MonoBehaviour
{
    [Range(0.1f, 10f)] [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private TextMeshProUGUI scoreText;

    public float score = 0f;

    // Start is called before the first frame update
    void Start() {
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update() {
        Time.timeScale = gameSpeed;
    }

    public void AddScore(float pointValue) {
        score += pointValue;
        scoreText.text = score.ToString();
    }
}
