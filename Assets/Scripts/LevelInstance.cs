using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelInstance : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private int num_breakable_blocks = 0;
    private static LevelInstance instance = null;

    public static LevelInstance GetInstance(GameObject prefab = null) {
        if (!instance) {
            if (FindObjectsOfType<LevelInstance>().Length < 1) {
                if (prefab) {
                    instance = ((GameObject)Object.Instantiate(prefab, Vector2.zero, Quaternion.identity))
                        .GetComponent<LevelInstance>();
                } else {
                    instance = ((GameObject)Object.Instantiate(Resources.Load("Prefabs/LevelInstance"), Vector2.zero,
                        Quaternion.identity)).GetComponent<LevelInstance>();
                }
            } else {
                instance = FindObjectOfType<LevelInstance>();
            }
        }

        return instance;
    }

    // Start is called before the first frame update
    void Start() {
        if (!scoreText) {
            scoreText = FindObjectOfType<TextMeshProUGUI>();
        }
        scoreText.text = GameInstance.GetInstance().GetScore().ToString();
    }

    // Update is called once per frame
    void Update() {

    }

    public void AddScore(float pointValue) {
        GameInstance.GetInstance().SetScore(GameInstance.GetInstance().GetScore() + pointValue);
        scoreText.text = GameInstance.GetInstance().GetScore().ToString();
    }

    public int BreakBlock() {
        num_breakable_blocks--;
        return num_breakable_blocks;
    }

    public void AddBlock() {
        num_breakable_blocks++;
    }
}
