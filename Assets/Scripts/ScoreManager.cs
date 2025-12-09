using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    
    [Header("Configuración")]
    public float pointsPerSecond = 10f;
    public bool isGameActive = true;

    private float currentScore = 0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (isGameActive)
        {
            currentScore += pointsPerSecond * Time.deltaTime;
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();
        }
    }

    public void AddPoints(float points)
    {
        currentScore += points;
    }

    public void StopGame()
    {
        isGameActive = false;
    }

    public void ResetScore()
    {
        currentScore = 0f;
        UpdateScoreUI();
    }

    public int GetScore()
    {
        return Mathf.FloorToInt(currentScore);
    }
}