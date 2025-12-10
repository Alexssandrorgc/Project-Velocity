using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public Transform[] tiles;
    
    [Header("Sistema de Obstáculos")]
    public ObstacleSpawner obstacleSpawner; // Referencia al spawner

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Spawner obstáculos iniciales en todos los tiles al inicio
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
        // Última pista actual
        Transform lastTile = tiles[tiles.Length - 1];

        // Obtener EndTrigger de la última pista y StartPoint de la pista a mover
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

        // DESACTIVAR la pista visualmente antes de moverla
        MeshRenderer[] renderers = tileToMove.GetComponentsInChildren<MeshRenderer>();
        foreach(var renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Calcular SOLO el offset en Z
        float offsetZ = tileToMove.position.z - moveStart.position.z;

        // Nueva posición
        Vector3 newPos = tileToMove.position;
        newPos.z = lastEnd.position.z + offsetZ;
        newPos.x = tileToMove.position.x;
        newPos.y = tileToMove.position.y;
        
        tileToMove.position = newPos;

        // REACTIVAR la pista en su nueva posición
        foreach(var renderer in renderers)
        {
            renderer.enabled = true;
        }

        Debug.Log($"Pista {tileToMove.name} movida a Z: {newPos.z}");

        // SPAWNER NUEVOS OBSTÁCULOS en el tile movido
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