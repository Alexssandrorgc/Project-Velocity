using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    public float velocidadCorrer = 10f; // Velocidad hacia adelante
    public float velocidadCarril = 10f; // Qué tan rápido cambia de carril
    public float fuerzaSalto = 7f;      // Fuerza del Rigidbody

    [Header("Configuración de Carriles")]
    public float distanciaCarril = 3f; // Distancia entre carriles (ajústalo a tu mapa)
    
    // 0 = Izquierda, 1 = Centro, 2 = Derecha
    private int carrilActual = 1; 
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. CORRER HACIA ADELANTE (Automático)
        transform.Translate(Vector3.forward * velocidadCorrer * Time.deltaTime);

        // 2. DETECTAR INPUT (Teclas A/D o Flechas)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (carrilActual < 2) carrilActual++; // Mover derecha
        }
        
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (carrilActual > 0) carrilActual--; // Mover izquierda
        }

        // CALCULAR POSICIÓN LATERAL
        float targetX = (carrilActual - 1) * distanciaCarril;

        // MOVER AL CARRIL SUAVEMENTE
        Vector3 nuevaPosicion = transform.position;
        nuevaPosicion.x = Mathf.Lerp(nuevaPosicion.x, targetX, velocidadCarril * Time.deltaTime);
        transform.position = nuevaPosicion;

        // 3. SALTO (Barra Espaciadora)
        // Nota: Si usas Unity 6, usa 'linearVelocity'. Si usas una versión anterior y te da error, cambia 'linearVelocity' por 'velocity'
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }
}