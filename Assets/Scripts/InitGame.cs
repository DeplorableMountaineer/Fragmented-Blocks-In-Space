using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InitGame : MonoBehaviour
{
    [SerializeField] private LevelType levelType = LevelType.Playable;
    [SerializeField] private GameObject gameInstancePrefab = null;
    [SerializeField] private GameObject levelInstancePrefab = null;
    [SerializeField] private GameObject gameCanvasPrefab = null;
    [SerializeField] private ControlType controlType = ControlType.Mouse;
    [Range(0.1f, 10f)] [SerializeField] private float gameSpeed = 1f;


    void Awake() {
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
            GameInstance.GetInstance().SetControlType(controlType);
            GameInstance.GetInstance().SetGameSpeed(gameSpeed);
        }
    }
}

public enum LevelType
{
    Playable,
    StartScreen,
    GameOver
}