using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            RunnerMovement player = other.GetComponent<RunnerMovement>();
            if (player != null)
            {
                GameManager.Instance.GameOver(); // Llamar al game over
                Debug.Log("¡Colisión con obstáculo!");
            }
        }
    }
}