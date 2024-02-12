using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class UIHandler : MonoBehaviour
{
    public TextMeshProUGUI MenuHighScoresText;

    
    // Start is called before the first frame update
    void Start()
    {
        MenuHighScores();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    void MenuHighScores()
    {
        for(int i = 0; i < DataManager.Instance.highScoreNames.Count; i++) 
        {
            MenuHighScoresText.text += DataManager.Instance.highScoreNames[i] + ": " + DataManager.Instance.highScores[i] + "\n";
        }
    }

    public void GetPlayerName(string input)
    {
        if(input != "")
        {
            DataManager.Instance.playerName = input;
        }
    }

    public void Exit()
    {
        DataManager.Instance.SaveHighScores();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
