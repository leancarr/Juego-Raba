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

    private ZonaCamara zonaActiva = null;

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

        // 2. Encontrar cuál es el jugador que va ganando (el que tiene el X más alto o mejor puntaje de progreso)
        GameObject jugadorMasAdelantado = jugadores[0];
        float mejorPuntaje = ObtenerPuntajeProgreso(jugadores[0]);

        for (int i = 1; i < jugadores.Length; i++)
        {
            float puntaje = ObtenerPuntajeProgreso(jugadores[i]);
            if (puntaje > mejorPuntaje)
            {
                mejorPuntaje = puntaje;
                jugadorMasAdelantado = jugadores[i];
            }
        }

        // 3. Calcular la posición de destino
        float desvioActualX = (zonaActiva != null) ? zonaActiva.desvioX : desvíoX;
        float destinoX = jugadorMasAdelantado.transform.position.x + desvioActualX;

        // Si activaron los límites, restringimos la X para que no se salga del mapa
        if (zonaActiva != null && zonaActiva.usarLimitesX)
        {
            destinoX = Mathf.Clamp(destinoX, zonaActiva.limiteIzquierdo, zonaActiva.limiteDerecho);
        }
        else if (usarLimites)
        {
            destinoX = Mathf.Clamp(destinoX, limiteIzquierdo, limiteDerecho);
        }

        float destinoY = alturaFijaY;
        if (zonaActiva != null && zonaActiva.seguirEnY)
        {
            destinoY = jugadorMasAdelantado.transform.position.y + zonaActiva.desvioY;
            if (zonaActiva.usarLimitesY)
            {
                destinoY = Mathf.Clamp(destinoY, zonaActiva.limiteInferior, zonaActiva.limiteSuperior);
            }
        }

        // 4. Crear el vector de posición final respetando la altura y profundidad inicial de la cámara o siguiendo en Y
        Vector3 posicionObjetivo = new Vector3(
            (zonaActiva != null && !zonaActiva.seguirEnX) ? transform.position.x : destinoX,
            destinoY,
            profundidadFijaZ
        );

        // 5. Mover la cámara de forma fluida usando Lerp
        float suavizadoActual = (zonaActiva != null && zonaActiva.suavizadoOverride > 0f) ? zonaActiva.suavizadoOverride : suavizado;
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, suavizadoActual * Time.deltaTime);
    }

    // El GameManager seguirá llamando a esta función, pero ahora solo congelará el movimiento
    public void EnfocarGanador(GameObject ganador)
    {
        partidaTerminada = true;
    }

    public void SetZonaActiva(ZonaCamara nuevaZona)
    {
        zonaActiva = nuevaZona;
        Debug.Log("<color=cyan>[CAMARA] Entrando a nueva zona: " + nuevaZona.gameObject.name + " | SeguirEnY: " + nuevaZona.seguirEnY + "</color>");
    }

    public void LimpiarZona(ZonaCamara zonaAEliminar)
    {
        if (zonaActiva == zonaAEliminar)
        {
            zonaActiva = null;
        }
    }

    private float ObtenerPuntajeProgreso(GameObject jugador)
    {
        if (zonaActiva != null && zonaActiva.seguirEnY)
        {
            // En descenso diagonal (desvioY < 0), el progreso es x - y
            if (zonaActiva.desvioY < 0f)
            {
                return jugador.transform.position.x - jugador.transform.position.y;
            }
        }
        return jugador.transform.position.x;
    }
}