using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public Transform parentTile;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player cruzó el EndTrigger de {parentTile.name}");
            
            // Validación adicional (opcional)
            if (TileManager.Instance != null && parentTile != null)
            {
                TileManager.Instance.MoveTile(parentTile);
            }
            else
            {
                Debug.LogError("TileManager.Instance o parentTile es null!");
            }
        }
    }

    // Visualización en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        BoxCollider box = GetComponent<BoxCollider>();
        if(box != null)
        {
            Gizmos.DrawWireCube(transform.position, box.size);
        }
    }
}