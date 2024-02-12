using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    
    public string playerName;

    public int m_Points;
    public List<int> highScores = new List<int>();
    private static int highScoresMaxLength = 9;
    private int highScorePosition;
    public List<string> highScoreNames = new List<string>();
    
    // Make this DataManager an Instance
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadHighScores();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckHighScores()
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
    }

    [System.Serializable]
    class SaveData 
    {
        public List<int> highScores = new List<int>();
        public List<string> highScoreNames = new List<string>();  
    }

    public void SaveHighScores()
    {
        SaveData data = new SaveData();

        for(int i = 0; i < highScores.Count; i++)
        {
            data.highScores.Add(highScores[i]);
            data.highScoreNames.Add(highScoreNames[i]);
        }

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadHighScores() {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            for(int i = 0; i < data.highScores.Count; i++)
            {
                highScores.Add(data.highScores[i]);
                highScoreNames.Add(data.highScoreNames[i]);
            }
        }
    }
}
