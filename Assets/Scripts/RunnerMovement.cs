using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class RunnerMovement : MonoBehaviour
{
    [Header("Velocidades")]
    public float startForwardSpeed = 30f;    // Velocidad inicial
    public float maxForwardSpeed = 75f;      // Velocidad máxima
    public float accelerationRate = 1f;      // Aceleración por segundo
    [HideInInspector]
    public float forwardSpeed;               // Velocidad actual (se calcula automáticamente)
    
    public float laneDistance = 3f;
    public float laneLerpSpeed = 10f;

    [Header("Salto / gravedad")]
    public float jumpForce = 6f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 velocity;
    private int currentLane = 1;
    private Vector3 startPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        startPosition = transform.position;
        forwardSpeed = startForwardSpeed; // Iniciar con velocidad base
    }

    void Update()
    {
        // ACELERACIÓN PROGRESIVA
        if (forwardSpeed < maxForwardSpeed)
        {
            forwardSpeed += accelerationRate * Time.deltaTime;
            forwardSpeed = Mathf.Min(forwardSpeed, maxForwardSpeed);
        }

        // 1) Movimiento hacia delante según la orientación del personaje
        Vector3 move = transform.forward * forwardSpeed;

        // 2) Lectura de inputs
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
                currentLane = Mathf.Max(0, currentLane - 1);

            if (Keyboard.current.dKey.wasPressedThisFrame)
                currentLane = Mathf.Min(2, currentLane + 1);
        }

        // 3) Calculamos la posición objetivo
        Vector3 targetPosition = startPosition;

        if (currentLane == 0)
            targetPosition += -transform.right * laneDistance;
        else if (currentLane == 2)
            targetPosition += transform.right * laneDistance;

        // 4) Suavizamos el movimiento lateral
        Vector3 horizontalDiff = targetPosition - transform.position;
        Vector3 lateralMovement = new Vector3(horizontalDiff.x, 0f, 0f) * laneLerpSpeed;
        move += lateralMovement;

        // 5) Salto y gravedad
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -1f;

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                velocity.y = jumpForce;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        move.y = velocity.y;

        // 6) Aplicar movimiento
        controller.Move(move * Time.deltaTime);
    }

    // Métodos útiles para game over / reset
    public void StopMovement()
    {
        forwardSpeed = 0f;
    }

    public void ResetSpeed()
    {
        forwardSpeed = startForwardSpeed;
    }

    public float GetCurrentSpeed()
    {
        return forwardSpeed;
    }
}