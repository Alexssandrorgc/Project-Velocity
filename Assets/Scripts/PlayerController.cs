using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadCorrer = 10f; 
    public float velocidadCarril = 10f; 
    public float fuerzaSalto = 7f;
    public float distanciaCarril = 3f; 

    // 0 = Izquierda, 1 = Centro, 2 = Derecha
    private int carrilActual = 1; 
    private Rigidbody rb;
    
    // Variable para recordar dónde estaba la calle al principio
    private float zInicial; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 1. AUTO-CORRECCIÓN DE ROTACIÓN (Mirar al Este/X)
        transform.rotation = Quaternion.Euler(0, 90, 0);

        // 2. ¡AQUÍ ESTÁ EL TRUCO! 
        // Guardamos la Z donde tú colocaste al robot manualmente.
        // Así, el carril central será ESA posición, no el 0 del mundo.
        zInicial = transform.position.z;
    }

    void Update()
    {
        // --- 1. INPUT ---
        if (Keyboard.current != null)
        {
            // DERECHA (D)
            if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                if (carrilActual < 2) carrilActual++;
            }
            
            // IZQUIERDA (A)
            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                if (carrilActual > 0) carrilActual--; 
            }

            // SALTO
            if (Keyboard.current.spaceKey.wasPressedThisFrame && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            }
        }

        // --- 2. CÁLCULO DE POSICIÓN ---
        
        Vector3 posFinal = transform.position;

        // A) Avanzar en X (Correr)
        posFinal.x += velocidadCorrer * Time.deltaTime;

        // B) Calcular Carril en Z (RELATIVO A DONDE EMPEZÓ)
        // Usamos 'zInicial' como base.
        float targetZ = zInicial - (carrilActual - 1) * distanciaCarril; 
        
        // C) Mover suavemente hacia el carril
        posFinal.z = Mathf.Lerp(posFinal.z, targetZ, velocidadCarril * Time.deltaTime);

        transform.position = posFinal;
    }
}