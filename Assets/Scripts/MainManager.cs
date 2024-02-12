using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using TMPro;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public static string playerName;
    public Text NameText;

    public Text ScoreText;
    public GameObject GameOverText;

    public TextMeshProUGUI MenuHighScoresText;
    public static List<int> highScores = new List<int>();
    private static int highScoresMaxLength = 9;
    private int highScorePosition;
    public static List<string> highScoreNames = new List<string>();
    
    private bool m_Started = false;
    public int m_Points;
    
    private bool m_GameOver = false;

    // Make this MainManager an Instance
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        MenuHighScores();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void GetObjects()
    {
        Ball = GameObject.FindWithTag("Ball").GetComponent<Rigidbody>();
        NameText = GameObject.FindWithTag("Name Text").GetComponent<Text>();
        ScoreText = GameObject.FindWithTag("Score Text").GetComponent<Text>();
        GameOverText = GameObject.FindWithTag("Game Over Text");
    }

    void CreateBricks()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        CheckHighScores();
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1).completed += (x) => {
            GetObjects();
            CreateBricks();
        };
        
    }

    void CheckHighScores()
    {
        if(highScores.Count == 0)
        {
            AdjustHighScores();
        }
        else
        {
            highScorePosition = highScores.Count;
            foreach(int score in highScores)
            {
                if( score < m_Points)
                {
                highScorePosition--;
                }
            }
            AdjustHighScores();
        }
    }

    void AdjustHighScores()
    {
        if(highScores.Count == 0 || highScorePosition > highScores.Count)
        {
            highScores.Add(m_Points);
            highScoreNames.Add(playerName);
        }
        // Wenn der Score höher als ein HighScore is und die Maximale Länge von HighScores noch nicht erreicht ist
        // der niedrigste HighScore wird hinzugefügt und alle anderen unter der neuen highscoreposition verschoben
        else if(highScorePosition < highScores.Count)
        {
            // dupliziert den letzten wert der Liste wenn noch Platz in der Liste ist
            if(highScores.Count < highScoresMaxLength)
            {
                highScores.Add(highScores[highScores.Count-1]);
                highScoreNames.Add(highScoreNames[highScoreNames.Count-1]);
            }
            for(int position = highScores.Count - 1; position > highScorePosition; position--)
            {
                highScores[position] = highScores[position - 1];
                highScoreNames[position] = highScoreNames[position - 1];
            }
            highScores[highScorePosition] = m_Points;
            highScoreNames[highScorePosition] = playerName;
        }
        // Wenn der Score niedriger als alle HighScores sind und noich platz im Array ist wird er geaddet
        else if(highScorePosition >= highScores.Count && highScores.Count < highScoresMaxLength)
        {
            highScores.Add(m_Points);
            highScoreNames.Add(playerName);
        }
        MenuHighScores();
    }

    void MenuHighScores()
    {
        MenuHighScoresText = GameObject.FindWithTag("Menu High Score Text").GetComponent<TextMeshProUGUI>();
        MenuHighScoresText.text = "";
        for(int i = 0; i < highScoreNames.Count; i++) 
        {
            MenuHighScoresText.text += highScoreNames[i] + ": " + highScores[i] + "\n";
        }
    }

    void MainHighScore()
    {
        NameText.text = "Best Score: " + highScoreNames[1] + ": " + highScores[1];
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void GetPlayerName(string input)
    {
        playerName = input;
    }
}
