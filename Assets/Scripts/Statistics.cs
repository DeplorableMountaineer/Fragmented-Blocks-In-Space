using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

public class Statistics : MonoBehaviour
{
    private string statsFileName = "GameStats.csv";
    private string[] statsFields = { "Date", "Level", "NumBlocks", "IsWin", "ElapsedTime", "Score", "Accuracy" };

    private static Statistics singleton = null;
    private string statsfile;
    public int numStatsAdded = 0;

    public static Statistics GetInstance(GameObject prefab = null) {
        if (singleton == null) {
            if (FindObjectsOfType<Statistics>().Length < 1) {
                if (prefab) {
                    singleton = ((GameObject)Object.Instantiate(prefab, Vector2.zero, Quaternion.identity))
                        .GetComponent<Statistics>();
                } else {
                    singleton = ((GameObject)Object.Instantiate(Resources.Load("Prefabs/Statistics"), Vector2.zero,
                        Quaternion.identity)).GetComponent<Statistics>();
                }
            } else {
                singleton = FindObjectOfType<Statistics>();
            }
        }
        return singleton;
    }

    void Awake() {
        if (FindObjectsOfType<Statistics>().Length > 1) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (statsFileName.EndsWith(".csv")) {
            statsfile = Application.persistentDataPath + Path.DirectorySeparatorChar + statsFileName;
        } else {
            statsfile = Application.persistentDataPath + Path.DirectorySeparatorChar + statsFileName + ".csv";
        }
        Debug.Log("Storing statistics in " + statsfile);
        if (!File.Exists(statsfile)) {
            File.Create(statsfile).Close();
            AppendRow(statsFields);
        }
    }

    public void AddStat(int numBlocks, bool isWin, float elapsedTime, float score, float accuracy) {
        numStatsAdded++;
        AppendRow(DateTime.Now.ToString(), SceneManager.GetActiveScene().name,
            numBlocks.ToString(), isWin.ToString(), elapsedTime.ToString(), score.ToString(), accuracy.ToString());
    }

    void AppendRow(params String[] fields) {
        StreamWriter writer = File.AppendText(statsfile);
        String result = "";
        String formatted_result = "";
        int i = 0;
        foreach (String field in fields) {
            if (result.Length > 0) {
                result += ",";
            }
            result += "\"" + field + "\"";
            if (i != 3 && i!=0) {
                formatted_result += "<color=blue>" + statsFields[i] + "</color>: " + field + ";   ";
            }

            i++;
        }
        writer.WriteLine(result);
        if (fields[3] == "True") {
            Debug.Log(numStatsAdded.ToString() + "  <color=green>Win</color>:" + formatted_result + " to " + statsfile);
        } else {
            Debug.Log(numStatsAdded.ToString() + " <color=red>Lose</color>: " + formatted_result + " to " + statsfile);
        }

        writer.Close();
    }
}
