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

    [Header("Seguridad Anti-Caída al Vacío")]
    [Tooltip("Si un jugador baja de esta Y, la cámara lo ignora hasta que reaparezca. Pone el mismo valor que el límite de caída del spawner.")]
    public float pisoSeguroY = -20f;

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

        // 2. Encontrar cuál es el jugador que va ganando, ignorando a los que cayeron al vacío
        GameObject jugadorMasAdelantado = null;
        float mejorPuntaje = float.MinValue;

        foreach (GameObject j in jugadores)
        {
            // Si el jugador está por debajo del piso seguro, lo ignoramos (está cayendo al vacío)
            if (j.transform.position.y < pisoSeguroY) continue;

            float puntaje = ObtenerPuntajeProgreso(j);
            if (puntaje > mejorPuntaje)
            {
                mejorPuntaje = puntaje;
                jugadorMasAdelantado = j;
            }
        }

        // Si TODOS cayeron al vacío a la vez, no movemos la cámara
        if (jugadorMasAdelantado == null) return;

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
        // Solo cambiamos a la nueva zona si su número de orden es mayor o igual al actual
        if (zonaActiva == null || nuevaZona.ordenDeZona >= zonaActiva.ordenDeZona)
        {
            zonaActiva = nuevaZona;
            Debug.Log("<color=cyan>[CAMARA] Entrando a nueva zona: " + nuevaZona.gameObject.name + " | SeguirEnY: " + nuevaZona.seguirEnY + " | Orden: " + nuevaZona.ordenDeZona + "</color>");
        }
        else
        {
            Debug.Log("<color=grey>[CAMARA] Se ignoró la zona " + nuevaZona.gameObject.name + " porque su orden (" + nuevaZona.ordenDeZona + ") es menor a la actual (" + zonaActiva.ordenDeZona + ").</color>");
        }
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
