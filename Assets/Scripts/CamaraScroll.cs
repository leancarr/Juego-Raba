using UnityEngine;

public class CamaraScroll25D : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public float suavizado = 5f;       // Qué tan suave acompaña la cámara al jugador (Lerp)
    public float desvíoX = 2f;         // Para tirar la cámara un poquito hacia adelante y ver lo que viene

    [Header("Límites del Escenario")]
    public bool usarLimites = false;   // Por si quieren que la cámara no pase de cierto punto
    public float limiteIzquierdo = 0f;
    public float limiteDerecho = 100f;

    private float alturaFijaY;
    private float profundidadFijaZ;

    // Variable para congelar por completo la posición de la cámara
    private bool partidaTerminada = false;

    void Start()
    {
        // Guardamos la posición inicial de la cámara en Y y Z para mantenerlas fijas congeladas
        alturaFijaY = transform.position.y;
        profundidadFijaZ = transform.position.z;
    }

    void LateUpdate()
    {
        // --- EL CAMBIO CLAVE ---
        // Si la partida terminó, la cámara se queda COMPLETAMENTE CONGELADA donde está.
        // No se mueve ni un milímetro, ignorando a todos los jugadores.
        if (partidaTerminada) return;

        // 1. Buscar a todos los objetos con el Tag "Player" en la escena
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");

        // Si no hay jugadores en la escena, no hace nada
        if (jugadores.Length == 0) return;

        // 2. Encontrar cuál es el jugador que va ganando (el que tiene el X más alto)
        GameObject jugadorMasAdelantado = jugadores[0];
        float mayorX = jugadores[0].transform.position.x;

        for (int i = 1; i < jugadores.Length; i++)
        {
            if (jugadores[i].transform.position.x > mayorX)
            {
                mayorX = jugadores[i].transform.position.x;
                jugadorMasAdelantado = jugadores[i];
            }
        }

        // 3. Calcular la posición de destino (Solo modificamos la X)
        float destinoX = jugadorMasAdelantado.transform.position.x + desvíoX;

        // Si activaron los límites, restringimos la X para que no se salga del mapa
        if (usarLimites)
        {
            destinoX = Mathf.Clamp(destinoX, limiteIzquierdo, limiteDerecho);
        }

        // 4. Crear el vector de posición final respetando la altura y profundidad inicial de la cámara
        Vector3 posicionObjetivo = new Vector3(destinoX, alturaFijaY, profundidadFijaZ);

        // 5. Mover la cámara de forma fluida usando Lerp
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, suavizado * Time.deltaTime);
    }

    // El GameManager seguirá llamando a esta función, pero ahora solo congelará el movimiento
    public void EnfocarGanador(GameObject ganador)
    {
        partidaTerminada = true;
    }
}