using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Ciudades"); // Cambia "Juego" por el nombre de tu escena
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("El juego se cerraría (solo funciona fuera del editor)");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits"); // Cambia por el nombre de tu escena de créditos
    }
}
