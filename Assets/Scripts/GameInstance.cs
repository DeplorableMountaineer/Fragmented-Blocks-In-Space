using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstance : MonoBehaviour
{
    [Range(0.1f, 10f)] [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private ControlType controlType = ControlType.Mouse;

    private float score = 0f;
    private bool gameOver = false;

    private static GameInstance singleton = null;

    public static GameInstance GetInstance(GameObject prefab = null) {
        if (singleton == null) {
            if (FindObjectsOfType<GameInstance>().Length < 1) {
                if (prefab) {
                    singleton = ((GameObject)Object.Instantiate(prefab, Vector2.zero, Quaternion.identity))
                        .GetComponent<GameInstance>();
                } else {
                    singleton = ((GameObject)Object.Instantiate(Resources.Load("Prefabs/GameInstance"), Vector2.zero,
                        Quaternion.identity)).GetComponent<GameInstance>();
                }
            } else {
                singleton = FindObjectOfType<GameInstance>();
            }
        }
        return singleton;
    }

    void Awake() {
        if (FindObjectsOfType<GameInstance>().Length > 1) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        Time.timeScale = gameSpeed;
        Statistics.GetInstance();
    }

    public void ResetGame() {
        score = 0;
        Time.timeScale = gameSpeed;
        gameOver = false;
    }

    public float GetScore() {
        return score;
    }

    public void SetScore(float newScore) {
        score = newScore;
    }

    public ControlType GetControlType() {
        return controlType;
    }

    public void SetControlType(ControlType ct) {
        controlType = ct;
    }

    public void SetGameSpeed(float speed) {
        gameSpeed = speed;
        Time.timeScale = gameSpeed;
    }

    public void LoadNextScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex >= SceneManager.sceneCountInBuildSettings - 2) {
            SceneManager.LoadScene(1);
            //TODO shuffle
        } else {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    public void LoadStartScene() {
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void GameOver() {
        LevelInstance.GetInstance().SaveStats(false);
        gameOver = true;
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    public bool IsGameOver() {
        return gameOver;
    }
}

public enum ControlType
{
    Mouse,
    Keyboard,
    Autoplay
}
