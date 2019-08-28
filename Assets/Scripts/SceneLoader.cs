using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public bool gameOver = false;
    [SerializeField] GameStatus gameStatus = null;

    void Start() {
        if (gameStatus == null) {
            gameStatus = FindObjectOfType<GameStatus>();
        }
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
        gameOver = false;
        gameStatus.score = 0;
        gameStatus.NewLevel();
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void GameOver() {
        gameOver = true;
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
}
