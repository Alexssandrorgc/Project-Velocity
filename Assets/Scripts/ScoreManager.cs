using UnityEngine;

using TMPro;



public class ScoreManager : MonoBehaviour

{

    public static ScoreManager Instance;



    private const string HIGH_SCORE_KEY = "HighScore";   // Guarda el récord histórico

    private const string LAST_SCORE_KEY = "LastGameScore"; // Guarda el score de la partida en curso (pausa)



    [Header("UI")]

    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI highScoreText; // Opcional: para mostrar high score en juego

   

    [Header("Configuración")]

    public float pointsPerSecond = 10f;

    public bool isGameActive = true;



    private float currentScore = 0f;

    private int highScore = 0;



    private void Awake()

    {

        if (Instance == null)

            Instance = this;

        else

            Destroy(gameObject);

    }



    private void Start()

    {

        // Cargar el high score guardado

        LoadHighScore();

        LoadLastScoreForContinuation();

        UpdateScoreUI();

        UpdateHighScoreUI();

    }



    private void Update()

    {

        if (isGameActive)

        {

            currentScore += pointsPerSecond * Time.deltaTime;

            UpdateScoreUI();

        }

    }



    // =================================================================

    // 🛑 FUNCIÓN DE GAME OVER (El jugador pierde)

    // =================================================================

    // ESTA FUNCIÓN DEBE SER LLAMADA CUANDO EL JUGADOR PIERDE EN EL JUEGO.

    public void GameOver()

    {

        isGameActive = false; // Detiene la acumulación de puntos

        CheckAndSaveHighScore(); // Solo verifica el récord máximo

       

        // Opcional: Si el jugador pierde, queremos que el próximo juego inicie en 0

        ResetLastScoreKey();

       

        Debug.Log("FIN DEL JUEGO: Score guardado solo si es un nuevo récord.");

    }

   

    // =================================================================

    // ⏸️ FUNCIÓN DE PAUSA Y GUARDADO (El jugador sale voluntariamente)

    // =================================================================

    // ESTA FUNCIÓN SERÁ LLAMADA POR PauseManager.SaveAndExit().

    public void SaveCurrentGame()

    {

        isGameActive = false; // Detener el conteo mientras está en el menú

       

        // Guardar el score actual para poder reanudar

        PlayerPrefs.SetInt(LAST_SCORE_KEY, GetScore());

        PlayerPrefs.Save();

       

        Debug.Log($"Guardado de Partida en Curso: {GetScore()}");

    }



    // =================================================================

    // 🔄 LÓGICA DE CARGA Y RESET

    // =================================================================

   

    // Carga el score guardado de la última partida (LAST_SCORE_KEY) y establece currentScore

    void LoadLastScoreForContinuation()

    {

        int lastScore = PlayerPrefs.GetInt(LAST_SCORE_KEY, 0);

        currentScore = lastScore;

       

        if (lastScore > 0)

        {

            Debug.Log($"Reanudando partida con Score: {currentScore}");

        }

    }

   

    // Borra la clave de Last Score (útil al iniciar un nuevo juego o al perder)

    public void ResetLastScoreKey()

    {

        PlayerPrefs.DeleteKey(LAST_SCORE_KEY);

        PlayerPrefs.Save();

        Debug.Log("Clave de Continuación de Partida borrada.");

    }





    void UpdateScoreUI()

    {

        if (scoreText != null)

        {

            scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();

        }

    }



    void UpdateHighScoreUI()

    {

        if (highScoreText != null)

        {

            highScoreText.text = "Best: " + highScore.ToString();

        }

    }



    public void AddPoints(float points)

    {

        currentScore += points;

    }



    public void StopGame()

    {

        isGameActive = false;

        CheckAndSaveHighScore(); // Guardar high score cuando termina el juego

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



    public int GetHighScore()

    {

        return highScore;

    }



    // Verificar si es nuevo récord y guardar

    void CheckAndSaveHighScore()

    {

        int finalScore = GetScore();

       

        if (finalScore > highScore)

        {

            highScore = finalScore;

            SaveHighScore();

            Debug.Log($"¡NUEVO RÉCORD! High Score: {highScore}");

        }

        else

        {

            Debug.Log($"Score: {finalScore} | High Score: {highScore}");

        }

    }



    // Guardar high score en PlayerPrefs

    void SaveHighScore()

    {

        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);

        PlayerPrefs.Save(); // Asegurar que se guarde inmediatamente

    }



    // Cargar high score de PlayerPrefs

    void LoadHighScore()

    {

        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0); // 0 es el valor por defecto

        Debug.Log($"High Score cargado: {highScore}");

    }



    // Método para resetear el high score (útil para testing)

    public void ResetHighScore()

    {

        highScore = 0;

        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);

        PlayerPrefs.Save();

        UpdateHighScoreUI();

        Debug.Log("High Score reseteado");

    }



    // Verificar si el score actual es un nuevo récord

    public bool IsNewHighScore()

    {

        return GetScore() > highScore;

    }

}