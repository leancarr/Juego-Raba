/* 
 * ==============================================================================
 * SCRIPT: CamaraScroll.cs
 * CATEGORIA: 3. Camara y Entorno
 * DESCRIPCION: La camara principal del juego lineal. Sigue a los jugadores y los mantiene enfocados.
 * ==============================================================================
 */
using UnityEngine;

public class CamaraScroll25D : MonoBehaviour
{
    [Header("ConfiguraciÃ³n de Seguimiento")]
    public float suavizado = 5f;       // QuÃ© tan suave acompaÃ±a la cÃ¡mara al jugador (Lerp)
    public float desvÃ­oX = 2f;         // Para tirar la cÃ¡mara un poquito hacia adelante y ver lo que viene

    [Header("LÃ­mites del Escenario")]
    public bool usarLimites = false;   // Por si quieren que la cÃ¡mara no pase de cierto punto
    public float limiteIzquierdo = 0f;
    public float limiteDerecho = 100f;

    private float alturaFijaY;
    private float profundidadFijaZ;

    // Variable para congelar por completo la posiciÃ³n de la cÃ¡mara
    private bool partidaTerminada = false;

    private ZonaCamara zonaActiva = null;

    void Start()
    {
        // Guardamos la posiciÃ³n inicial de la cÃ¡mara en Y y Z para mantenerlas fijas congeladas
        alturaFijaY = transform.position.y;
        profundidadFijaZ = transform.position.z;
    }

    void LateUpdate()
    {
        // --- EL CAMBIO CLAVE ---
        // Si la partida terminÃ³, la cÃ¡mara se queda COMPLETAMENTE CONGELADA donde estÃ¡.
        // No se mueve ni un milÃ­metro, ignorando a todos los jugadores.
        if (partidaTerminada) return;

        // 1. Buscar a todos los objetos con el Tag "Player" en la escena
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");

        // Si no hay jugadores en la escena, no hace nada
        if (jugadores.Length == 0) return;

        // 2. Encontrar cuÃ¡l es el jugador que va ganando (el que tiene el X mÃ¡s alto o mejor puntaje de progreso)
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

        // 3. Calcular la posiciÃ³n de destino
        float desvioActualX = (zonaActiva != null) ? zonaActiva.desvioX : desvÃ­oX;
        float destinoX = jugadorMasAdelantado.transform.position.x + desvioActualX;

        // Si activaron los lÃ­mites, restringimos la X para que no se salga del mapa
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

        // 4. Crear el vector de posiciÃ³n final respetando la altura y profundidad inicial de la cÃ¡mara o siguiendo en Y
        Vector3 posicionObjetivo = new Vector3(
            (zonaActiva != null && !zonaActiva.seguirEnX) ? transform.position.x : destinoX,
            destinoY,
            profundidadFijaZ
        );

        // 5. Mover la cÃ¡mara de forma fluida usando Lerp
        float suavizadoActual = (zonaActiva != null && zonaActiva.suavizadoOverride > 0f) ? zonaActiva.suavizadoOverride : suavizado;
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, suavizadoActual * Time.deltaTime);
    }

    // El GameManager seguirÃ¡ llamando a esta funciÃ³n, pero ahora solo congelarÃ¡ el movimiento
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
