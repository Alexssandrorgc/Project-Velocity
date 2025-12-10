using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstáculos")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    
    [Header("Configuración de Carriles")]
    [SerializeField] private float leftLaneX = 165f;
    [SerializeField] private float centerLaneX = 146f;
    [SerializeField] private float rightLaneX = 125f;
    [SerializeField] private float obstacleHeight = 0f;
    
    [Header("Distancias de Spawn (importante para dificultad)")]
    [SerializeField] private float minObstacleDistance = 50f;  // Distancia mínima entre obstáculos
    [SerializeField] private float maxObstacleDistance = 120f; // Distancia máxima entre obstáculos
    [SerializeField] private float safeZoneAtStart = 150f;     // Zona segura al inicio del tile
    
    [Header("Patrones de Dificultad")]
    [SerializeField] [Range(0f, 1f)] private float spawnChance = 0.7f;
    [SerializeField] private bool alwaysLeaveOneLaneFree = true; // Siempre deja 1 carril libre
    [SerializeField] private int minObstaclesPerTile = 3;
    [SerializeField] private int maxObstaclesPerTile = 8;
    
    private float[] lanePositions;
    private List<int> availableLanes = new List<int> { 0, 1, 2 };
    
    void Start()
    {
        lanePositions = new float[] { leftLaneX, centerLaneX, rightLaneX };
    }
    
    public void SpawnObstaclesOnTile(Transform tile)
    {
        Transform startPoint = tile.Find("StartPoint");
        Transform endTrigger = tile.Find("EndTrigger");
        
        if (startPoint == null || endTrigger == null)
        {
            Debug.LogWarning($"Tile {tile.name} no tiene StartPoint o EndTrigger");
            return;
        }
        
        // Limpiar obstáculos anteriores
        ClearObstaclesOnTile(tile);
        
        float startZ = startPoint.position.z;
        float endZ = endTrigger.position.z;
        
        // Como avanzas hacia -Z, startZ > endZ
        float tileLength = Mathf.Abs(startZ - endZ);
        
        Debug.Log($"Tile {tile.name}: StartZ={startZ}, EndZ={endZ}, Length={tileLength}");
        
        // Número aleatorio de obstáculos para este tile
        int obstacleCount = Random.Range(minObstaclesPerTile, maxObstaclesPerTile + 1);
        
        // Posición inicial (dejamos zona segura)
        float currentZ = startZ - safeZoneAtStart;
        
        for (int i = 0; i < obstacleCount; i++)
        {
            // Avanzar hacia -Z
            currentZ -= Random.Range(minObstacleDistance, maxObstacleDistance);
            
            // Si llegamos al final del tile, terminamos
            if (currentZ <= endZ + 50f) break; // 50 unidades antes del EndTrigger
            
            // Decidir cuántos carriles bloquear (1 o 2, nunca los 3)
            int lanesBlocked = alwaysLeaveOneLaneFree ? Random.Range(1, 3) : Random.Range(1, 4);
            
            // Seleccionar carriles aleatorios
            List<int> selectedLanes = GetRandomLanes(lanesBlocked);
            
            // Spawner obstáculos en los carriles seleccionados
            foreach (int laneIndex in selectedLanes)
            {
                if (Random.value <= spawnChance)
                {
                    SpawnSingleObstacle(tile, laneIndex, currentZ, i);
                }
            }
        }
    }
    
    private void SpawnSingleObstacle(Transform parent, int laneIndex, float zPos, int index)
    {
        float xPos = lanePositions[laneIndex];
        Vector3 spawnPos = new Vector3(xPos, obstacleHeight, zPos);
        
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        
        // Usar la rotación original del prefab
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, obstaclePrefab.transform.rotation, parent);
        obstacle.name = $"Obstacle_{index}_Lane{laneIndex}";
        
        // Asegurar que tiene collider trigger
        Collider col = obstacle.GetComponent<Collider>();
        if (col == null)
        {
            col = obstacle.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;
        
        // Asegurar que tiene el script de colisión
        if (obstacle.GetComponent<ObstacleCollision>() == null)
        {
            obstacle.AddComponent<ObstacleCollision>();
        }
    }
    
    private List<int> GetRandomLanes(int count)
    {
        // Crear lista de carriles disponibles
        List<int> lanes = new List<int> { 0, 1, 2 };
        List<int> selected = new List<int>();
        
        // Seleccionar 'count' carriles aleatorios
        for (int i = 0; i < count && lanes.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, lanes.Count);
            selected.Add(lanes[randomIndex]);
            lanes.RemoveAt(randomIndex);
        }
        
        return selected;
    }
    
    private void ClearObstaclesOnTile(Transform tile)
    {
        ObstacleCollision[] obstacles = tile.GetComponentsInChildren<ObstacleCollision>();
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }
    }
    
    // Método para ajustar dificultad dinámicamente según velocidad del player
    public void AdjustDifficulty(float playerSpeed, float maxSpeed)
    {
        // A mayor velocidad, más obstáculos y más cerca entre sí
        float speedPercent = playerSpeed / maxSpeed;
        
        // Reducir distancia entre obstáculos (más difícil)
        minObstacleDistance = Mathf.Lerp(60f, 35f, speedPercent);
        maxObstacleDistance = Mathf.Lerp(130f, 80f, speedPercent);
        
        // Aumentar cantidad de obstáculos
        minObstaclesPerTile = Mathf.RoundToInt(Mathf.Lerp(3f, 5f, speedPercent));
        maxObstaclesPerTile = Mathf.RoundToInt(Mathf.Lerp(7f, 12f, speedPercent));
    }
}