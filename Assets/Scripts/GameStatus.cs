using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatus : MonoBehaviour
{
    [Range(0.1f, 10f)] [SerializeField] private float gameSpeed = 1f;
    private TextMeshProUGUI scoreText;

    public float score = 0f;

    void Awake() {
        int gameStatusCount = FindObjectsOfType<GameStatus>().Length;
        if (gameStatusCount > 1) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        NewLevel();
    }

    // Update is called once per frame
    void Update() {
        Time.timeScale = gameSpeed;
    }

    public void AddScore(float pointValue) {
        score += pointValue;
        scoreText.text = score.ToString();
    }

    public void NewLevel() {
        scoreText = FindObjectOfType<TextMeshProUGUI>();
        scoreText.text = score.ToString();
    }


}
