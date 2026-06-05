using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Componentes de UI")]
    public RectTransform imagenIris;
    public GameObject panelTextoVictoria;
    public GameObject panelEstadisticasFinales;

    [Header("Configuracion del Efecto")]
    public float duracionCierre = 1.5f;
    public float tiempoEsperaPostCierre = 2.0f;

    private bool juegoTerminado = false;

    // Almacenamos al ganador para que la corrutina de Iris sepa a quien seguir
    private GameObject personajeGanador;

    void Awake()
    {
        if (instancia == null) instancia = this;
    }

    // AHORA RECIBE AL JUGADOR QUE GANÓ POR PARÁMETRO
    public void GanarPartida(GameObject ganador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        personajeGanador = ganador;

        // --- LÓGICA DE PUNTUACIÓN DE TORNEO ---
        if (DatosTorneo.instancia != null)
        {
            // Sumamos el punto al jugador correcto según su nombre exacto
            if (ganador.name == "Player 1")
            {
                DatosTorneo.instancia.victoriasP1++;
            }
            else if (ganador.name == "Player 2")
            {
                DatosTorneo.instancia.victoriasP2++;
            }
        }

        if (personajeGanador != null)
        {
            // 1. CONGELAR EL ANIMATOR
            Animator anim = personajeGanador.GetComponentInChildren<Animator>();
            if (anim != null) anim.SetFloat("Velocidad", 0f);

            // 2. APAGAR SCRIPTS DE MOVIMIENTO DEL GANADOR
            MonoBehaviour[] scripts = personajeGanador.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) script.enabled = false;
            }

            // 3. FRENAR FISICAS DEL GANADOR
            Rigidbody rb = personajeGanador.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // 4. FORZAR ROTACION HACIA LA CAMARA
            float gradosDeRotacionY = 90f;
            personajeGanador.transform.rotation = Quaternion.Euler(0f, gradosDeRotacionY, 0f);
        }

        // --- NUEVO: CORTE DEL DIRECTOR (Cámara y Perdedores) ---

        // Enfocar la cámara solo en el ganador
        if (Camera.main != null)
        {
            CamaraScroll25D scriptCamara = Camera.main.GetComponent<CamaraScroll25D>();
            if (scriptCamara != null)
            {
                scriptCamara.EnfocarGanador(personajeGanador);
            }
        }

        // Desactivar a todos los demas jugadores (los perdedores)
        GameObject[] todosLosJugadores = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject jugador in todosLosJugadores)
        {
            if (jugador != personajeGanador) // Solo actuamos sobre los que NO ganaron
            {
                // Apagamos sus scripts (MovimientoBasico25D, etc.)
                MonoBehaviour[] scriptsPerdedor = jugador.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scriptsPerdedor)
                {
                    script.enabled = false;
                }

                // Congelamos sus físicas en el aire
                Rigidbody rbPerdedor = jugador.GetComponent<Rigidbody>();
                if (rbPerdedor != null)
                {
                    rbPerdedor.linearVelocity = Vector3.zero;
                    rbPerdedor.angularVelocity = Vector3.zero;
                    rbPerdedor.isKinematic = true;
                }

                // Opcional: Frenar la animación de los perdedores
                Animator animPerdedor = jugador.GetComponentInChildren<Animator>();
                if (animPerdedor != null) animPerdedor.SetFloat("Velocidad", 0f);
            }
        }
        // --- FIN DEL CORTE DEL DIRECTOR ---

        if (panelTextoVictoria != null) panelTextoVictoria.SetActive(true);

        if (imagenIris != null)
        {
            imagenIris.gameObject.SetActive(true);
            StartCoroutine(AnimarCierreIris());
        }
    }

    private IEnumerator AnimarCierreIris()
    {
        // BUSQUEDA DINÁMICA: Busca el HeadTarget adentro del personaje que ganó
        Transform target = null;
        if (personajeGanador != null)
        {
            target = personajeGanador.transform.Find("HeadTarget");
        }

        float tiempoPasado = 0f;
        Vector3 escalaInicial = new Vector3(150f, 150f, 1f);
        Vector3 escalaFinal = new Vector3(1f, 1f, 1f);

        while (tiempoPasado < duracionCierre)
        {
            tiempoPasado += Time.deltaTime;

            if (target != null && Camera.main != null)
            {
                Vector2 posicionPantalla = Camera.main.WorldToScreenPoint(target.position);
                imagenIris.position = posicionPantalla;
            }

            imagenIris.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tiempoPasado / duracionCierre);

            yield return null;
        }

        imagenIris.localScale = escalaFinal;

        yield return new WaitForSeconds(tiempoEsperaPostCierre);

        // DETECTOR DE MENTIRAS (Pegá esto acá):
        Debug.LogWarning($"=== [DEBUG TORNEO] ===");
        Debug.LogWarning($"¿DatosTorneo existe en esta escena?: {(DatosTorneo.instancia != null ? "SÍ" : "NO, ES NULL")}");
        if (DatosTorneo.instancia != null)
        {
            Debug.LogWarning($"Rondas Totales en memoria: {DatosTorneo.instancia.rondasTotales}");
            Debug.LogWarning($"Marcador Actual -> P1: {DatosTorneo.instancia.victoriasP1} | P2: {DatosTorneo.instancia.victoriasP2}");
            Debug.LogWarning($"Rondas necesarias para cortar: {DatosTorneo.instancia.ObtenerRondasParaGanar()}");
        }
        Debug.LogWarning($"=======================");

        // --- CHEQUEO MATEMÁTICO DE FIN DE TORNEO ---
        if (DatosTorneo.instancia != null)
        {
            int rondasParaGanar = DatosTorneo.instancia.ObtenerRondasParaGanar();

            if (DatosTorneo.instancia.victoriasP1 >= rondasParaGanar || DatosTorneo.instancia.victoriasP2 >= rondasParaGanar)
            {
                // ¡TENEMOS UN GANADOR DEFINITIVO!
                TerminarTorneoCompleto();
            }
            else
            {
                // El torneo sigue, reiniciamos la ronda comun
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            // Plan B: Si jugas desde el editor sin pasar por el menu, resetea normal
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void TerminarTorneoCompleto()
    {
        // Opción elegida: Activamos el panel de estadisticas aqui mismo en la escena
        if (panelEstadisticasFinales != null)
        {
            //Apagamos el Iris para que deje de tapar la pantalla
            if (imagenIris != null) imagenIris.gameObject.SetActive(false);
            if (panelTextoVictoria != null) panelTextoVictoria.SetActive(false); // <--- ACÁ APAGÁS EL "GANASTE"

            panelEstadisticasFinales.SetActive(true);

            // Aca podes rellenar los textos de tu panel usando los datos guardados:
            // DatosTorneo.instancia.victoriasP1 y DatosTorneo.instancia.victoriasP2
            Debug.Log("Torneo terminado. P1: " + DatosTorneo.instancia.victoriasP1 + " vs P2: " + DatosTorneo.instancia.victoriasP2);
        }
        else
        {
            // Esto sirve si queremos que se vayan a otra escena (Escena separada del podio):
            SceneManager.LoadScene("Escena_Podio");
        }
    }

    // Función para el botón de "Reiniciar" / "Revancha"
    public void BotonReiniciarTorneo()
    {
        // 1. Reseteamos los puntos a 0 en el objeto que no se destruye
        if (DatosTorneo.instancia != null)
        {
            DatosTorneo.instancia.victoriasP1 = 0;
            DatosTorneo.instancia.victoriasP2 = 0;
        }

        // 2. Recargamos la escena de juego actual para empezar de cero
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    // Función para el botón de "Volver al Menú"
    public void BotonVolverAlMenu()
    {
        // 1. Rompemos el DatosTorneo para que la próxima vez que entremos al juego
        // desde el menú, se cree de cero con la nueva cantidad de rondas elegidas.
        if (DatosTorneo.instancia != null)
        {
            Destroy(DatosTorneo.instancia.gameObject);
        }

        // 2. Cargamos la escena del Menú Principal 
        SceneManager.LoadScene("MenuPrincipal");
    }
}