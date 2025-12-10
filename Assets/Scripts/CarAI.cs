using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Configuración de Carriles")]
    [SerializeField] private float leftLaneX = 165f;
    [SerializeField] private float centerLaneX = 146f;
    [SerializeField] private float rightLaneX = 125f;
    
    [Header("Comportamiento IA")]
    [SerializeField] private float moveSpeed = 5f; // Velocidad de cambio de carril
    [SerializeField] private float decisionTime = 2f; // Cada cuánto decide cambiar carril
    [SerializeField] private bool followPlayer = true; // ¿Persigue al jugador?
    [SerializeField] private float detectionRange = 100f; // Distancia para detectar player
    
    [Header("Movimiento Hacia Adelante")]
    [SerializeField] private float forwardSpeed = 10f; // Velocidad hacia -Z
    [SerializeField] private bool moveForward = true;
    
    private float[] lanePositions;
    private int currentTargetLane = 1; // Carril objetivo (0=izq, 1=centro, 2=der)
    private float nextDecisionTime;
    private Transform player;
    
    void Start()
    {
        lanePositions = new float[] { leftLaneX, centerLaneX, rightLaneX };
        nextDecisionTime = Time.time + decisionTime;
        
        // Buscar al player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // Empezar en un carril aleatorio
        currentTargetLane = Random.Range(0, 3);
    }
    
    void Update()
    {
        // Tomar decisión de cambio de carril
        if (Time.time >= nextDecisionTime)
        {
            MakeDecision();
            nextDecisionTime = Time.time + decisionTime;
        }
        
        // Moverse hacia el carril objetivo
        MoveToTargetLane();
        
        // Moverse hacia adelante (hacia +Z, contrario al player que va a -Z)
        if (moveForward)
        {
            transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
        }
    }
    
    void MakeDecision()
    {
        if (followPlayer && player != null)
        {
            // IA: Intentar seguir al jugador si está cerca
            float distanceToPlayer = Mathf.Abs(player.position.z - transform.position.z);
            
            if (distanceToPlayer < detectionRange)
            {
                // Detectar en qué carril está el jugador
                int playerLane = GetPlayerLane();
                
                // 70% probabilidad de ir al carril del jugador
                if (Random.value < 0.7f)
                {
                    currentTargetLane = playerLane;
                }
                else
                {
                    // 30% de ir a un carril aleatorio (para no ser tan predecible)
                    currentTargetLane = Random.Range(0, 3);
                }
                
                Debug.Log($"CarAI: Player detectado en carril {playerLane}, moviendo a carril {currentTargetLane}");
            }
            else
            {
                // Jugador lejos, movimiento aleatorio
                RandomLaneChange();
            }
        }
        else
        {
            // Sin seguir jugador, movimiento aleatorio
            RandomLaneChange();
        }
    }
    
    void RandomLaneChange()
    {
        // Cambiar a un carril adyacente o quedarse
        int decision = Random.Range(-1, 2); // -1, 0, 1
        currentTargetLane = Mathf.Clamp(currentTargetLane + decision, 0, 2);
    }
    
    int GetPlayerLane()
    {
        if (player == null) return 1;
        
        float playerX = player.position.x;
        
        // Determinar cuál carril está más cerca
        float distLeft = Mathf.Abs(playerX - lanePositions[0]);
        float distCenter = Mathf.Abs(playerX - lanePositions[1]);
        float distRight = Mathf.Abs(playerX - lanePositions[2]);
        
        if (distLeft < distCenter && distLeft < distRight)
            return 0; // Izquierda
        else if (distRight < distCenter)
            return 2; // Derecha
        else
            return 1; // Centro
    }
    
    void MoveToTargetLane()
    {
        // Obtener posición X objetivo
        float targetX = lanePositions[currentTargetLane];
        
        // Interpolar suavemente hacia el carril objetivo
        float newX = Mathf.Lerp(transform.position.x, targetX, moveSpeed * Time.deltaTime);
        
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
    
    // Visualización en el editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.yellow;
        // Línea hacia el carril objetivo
        Vector3 targetPos = new Vector3(lanePositions[currentTargetLane], transform.position.y, transform.position.z);
        Gizmos.DrawLine(transform.position, targetPos);
        
        // Rango de detección
        if (followPlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}