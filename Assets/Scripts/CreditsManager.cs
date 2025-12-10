using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public void CloseCredits()
    {
        SceneManager.LoadScene("MainMenu"); // Cambia "Menu" por el nombre exacto de tu escena de menú
    }
}
