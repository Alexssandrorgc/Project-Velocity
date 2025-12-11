using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public Transform[] tiles;
    
    [Header("Sistema de Obstáculos")]
    public ObstacleSpawner obstacleSpawner;
    
    [Header("Configuración de Teletransporte")]
    [SerializeField] private float minDistanceFromCamera = 100f; // Distancia mínima de la cámara para mover
    [SerializeField] private float checkInterval = 0.1f; // Cada cuánto verificar la distancia
    
    private Camera mainCamera;
    private bool isMovingTile = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (obstacleSpawner != null)
        {
            foreach (Transform tile in tiles)
            {
                obstacleSpawner.SpawnObstaclesOnTile(tile);
            }
        }
    }

    public void MoveTile(Transform tileToMove)
    {
        if (isMovingTile) return;
        
        // Esperar hasta que el chunk esté fuera de vista
        StartCoroutine(MoveTileWhenOutOfSight(tileToMove));
    }
    
    private IEnumerator MoveTileWhenOutOfSight(Transform tileToMove)
    {
        isMovingTile = true;
        
        // Esperar hasta que el chunk esté lo suficientemente lejos de la cámara
        while (true)
        {
            float distance = Mathf.Abs(tileToMove.position.z - mainCamera.transform.position.z);
            
            if (distance > minDistanceFromCamera)
            {
                Debug.Log($"Chunk {tileToMove.name} está a {distance} unidades, procediendo a mover");
                break;
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
        
        // Ahora mover el chunk
        PerformTileMove(tileToMove);
        
        isMovingTile = false;
    }
    
    private void PerformTileMove(Transform tileToMove)
    {
        Transform lastTile = tiles[tiles.Length - 1];
        Transform lastEnd = lastTile.Find("EndTrigger");
        Transform moveStart = tileToMove.Find("StartPoint");

        if(lastEnd == null)
        {
            Debug.LogError("Falta EndTrigger en la pista: " + lastTile.name);
            return;
        }

        if(moveStart == null)
        {
            Debug.LogError("Falta StartPoint en la pista: " + tileToMove.name);
            return;
        }

        float offsetZ = tileToMove.position.z - moveStart.position.z;
        Vector3 newPos = tileToMove.position;
        newPos.z = lastEnd.position.z + offsetZ;
        newPos.x = tileToMove.position.x;
        newPos.y = tileToMove.position.y;
        
        tileToMove.position = newPos;

        Debug.Log($"Pista {tileToMove.name} movida a Z: {newPos.z}");

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SpawnObstaclesOnTile(tileToMove);
        }

        ReorderTiles(tileToMove);
    }

    void ReorderTiles(Transform movedTile)
    {
        for (int i = 0; i < tiles.Length - 1; i++)
        {
            if (tiles[i] == movedTile)
            {
                Transform temp = tiles[i];

                for (int j = i; j < tiles.Length - 1; j++)
                {
                    tiles[j] = tiles[j + 1];
                }

                tiles[tiles.Length - 1] = temp;
                break;
            }
        }
    }
}