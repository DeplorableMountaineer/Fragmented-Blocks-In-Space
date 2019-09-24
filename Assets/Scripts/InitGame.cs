using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;



public class InitGame : MonoBehaviour
{
    [SerializeField] private LevelType levelType = LevelType.Playable;
    [SerializeField] private GameObject gameInstancePrefab = null;
    [SerializeField] private GameObject levelInstancePrefab = null;
    [SerializeField] private GameObject gameCanvasPrefab = null;
    [SerializeField] private ControlType controlType = ControlType.Mouse;
    [Range(0.1f, 10f)] [SerializeField] private float gameSpeed = 1f;


    void Awake() {
        if (levelType == LevelType.StartScreen) {
            GameInstance.GetInstance().SetControlType(controlType);
            if (controlType == ControlType.Mouse){
                Cursor.lockState = CursorLockMode.Confined;
            }
        } else {
            controlType = GameInstance.GetInstance().GetControlType();
        }
        GameInstance.GetInstance(gameInstancePrefab);
        if (levelType == LevelType.Playable) {
            LevelInstance.GetInstance(levelInstancePrefab);
            if (!FindObjectOfType<TextMeshProUGUI>()) {
                if (gameCanvasPrefab == null) {
                    Object.Instantiate(Resources.Load("Prefabs/Game Canvas"), Vector2.zero,
                        Quaternion.identity);
                } else {
                    Object.Instantiate(gameCanvasPrefab, Vector2.zero, Quaternion.identity);
                }
            }
        } else if (levelType == LevelType.StartScreen) {
            GameInstance.GetInstance().ResetGame();
            GameInstance.GetInstance().SetGameSpeed(gameSpeed);
            if (controlType == ControlType.Autoplay) {
                Invoke("StartRandomLevel", 1f);
            }
        } else if (levelType == LevelType.GameOver) {
            if (controlType == ControlType.Autoplay) {
                Invoke("RestartGame", 1f);
            }
        }

        if (controlType == ControlType.Autoplay) {
            AudioListener.volume = 0;
            GameInstance.GetInstance().SetScore(0);
        }
    }

    void StartRandomLevel() {
        int next_level = GameInstance.GetInstance().lastLevelPlayed + 1;
        if (next_level >= SceneManager.sceneCountInBuildSettings - 1) {
            next_level = 1;
        }

        SceneManager.LoadScene(next_level);
        //SceneManager.LoadScene(Random.Range(1, SceneManager.sceneCountInBuildSettings - 1));
    }

    void RestartGame() {
        SceneManager.LoadScene(0);
    }
}

public enum LevelType
{
    Playable,
    StartScreen,
    GameOver
}