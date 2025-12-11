using UnityEngine;

using UnityEngine.SceneManagement;

using TMPro;



public class GameManager : MonoBehaviour

{

    public static GameManager Instance;

   

    [Header("Referencias")]

    public RunnerMovement player;

   

    [Header("UI Game Over")]

    public GameObject gameOverPanel; // Panel de Game Over

    public TextMeshProUGUI finalScoreText; // "Score: 1234"

    public TextMeshProUGUI highScoreText; // "Best: 5678"

    public TextMeshProUGUI newRecordText; // "¡NUEVO RÉCORD!" (opcional)

   

    private bool isGameOver = false;

   

    private void Awake()

    {

        if (Instance == null)

        {

            Instance = this;

        }

        else

        {

            Destroy(gameObject);

        }

    }



    private void Start()

    {

        // Asegurar que el panel esté oculto al inicio

        if (gameOverPanel != null)

        {

            gameOverPanel.SetActive(false);

        }

       

        if (newRecordText != null)

        {

            newRecordText.gameObject.SetActive(false);

        }

    }

   

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        
        // 1. Detener jugador (player.StopMovement()...
        
        // 2. Detener el score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.GameOver(); 
        }
        
        // 3. DETENER EL TIEMPO PARA CONGELAR EL JUEGO
        Time.timeScale = 0f; 
        
        Debug.Log("¡GAME OVER!");
        
        // 4. Mostrar panel de Game Over
        ShowGameOverUI();
        
        // 5. MOSTRAR CURSOR AQUI:
        Cursor.lockState = CursorLockMode.None; // Desbloquea el mouse
        Cursor.visible = true;                  // Hace visible el mouse
    }

   

    void ShowGameOverUI()

    {

        if (gameOverPanel != null)

        {

            gameOverPanel.SetActive(true);

        }

       

        if (ScoreManager.Instance != null)

        {

            int finalScore = ScoreManager.Instance.GetScore();

            int highScore = ScoreManager.Instance.GetHighScore();

            bool isNewRecord = ScoreManager.Instance.IsNewHighScore();

           

            // Mostrar score final

            if (finalScoreText != null)

            {

                finalScoreText.text = "Score: " + finalScore.ToString();

            }

           

            // Mostrar high score

            if (highScoreText != null)

            {

                highScoreText.text = "Best: " + highScore.ToString();

            }

           

            // Mostrar mensaje de nuevo récord

            if (newRecordText != null && isNewRecord)

            {

                newRecordText.gameObject.SetActive(true);

            }

        }

    }

   

    // Llamar desde botón de UI

    public void RestartGame()

    {

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

   

    // Llamar desde botón de UI (opcional)

    public void QuitGame()

    {

        Debug.Log("Saliendo del juego...");

        Application.Quit();

       

        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

        #endif

    }

}