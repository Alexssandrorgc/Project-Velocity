using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;
    public TextMeshProUGUI currentScoreText;

    private bool isPaused = false;

    void Update()
        {
            // 🛑 CAMBIO CLAVE: Usar el nuevo Input System para detectar P o Escape
            if (Keyboard.current != null)
            {
                if (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    if (isPaused)
                        ResumeGame();
                    else
                        PauseGame();
                }
            }
        }

    // -------------------------------
    // PAUSAR EL JUEGO
    // -------------------------------
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Detiene el tiempo
        pauseMenuUI.SetActive(true);

    // <--- NUEVA LÓGICA DE PUNTUACIÓN AQUÍ
    if (currentScoreText != null && ScoreManager.Instance != null)
    {
        int score = ScoreManager.Instance.GetScore();
        currentScoreText.text = $"Tu Puntuación Actual: {score}";
    }
    // FIN NUEVA LÓGICA DE PUNTUACIÓN --->

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // -------------------------------
    // REANUDAR EL JUEGO
    // -------------------------------
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Reactiva el tiempo
        pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // -------------------------------
    // SALIR Y GUARDAR
    // -------------------------------
    // Fragmento del PauseManager:
    public void SaveAndExit()
    {
        // Llamar a tu ScoreManager para guardar el score de la partida en curso
        if (ScoreManager.Instance != null)
        {
            // CAMBIAR: Llama a la nueva función de guardado de partida
            ScoreManager.Instance.SaveCurrentGame(); 
        }

        Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowCursor()
    {
        // Asegurarse de que el tiempo no esté detenido si la pausa no estaba activa
        if (Time.timeScale == 0f) 
        {
            // Si el tiempo está en cero, significa que la escena está pausada (ya sea por Game Over o Pausa)
        }

        Cursor.lockState = CursorLockMode.None; // Desbloquea el mouse
        Cursor.visible = true;                  // Hace visible el mouse
        Debug.Log("Cursor Visible y Desbloqueado.");
    }
}
