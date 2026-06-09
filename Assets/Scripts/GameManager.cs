using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Componentes de UI")]
    public RectTransform imagenIris;
    public GameObject panelTextoVictoria;
    public GameObject panelEstadisticasFinales;
    
    [Header("Textos de Estadísticas Finales")]
    public TextMeshProUGUI textoGanadorFinal;     
    public TextMeshProUGUI textoContadorVictorias; 

    [Header("Configuracion del Efecto")]
    public float duracionCierre = 1.5f;
    public float tiempoEsperaPostCierre = 2.0f;

    private bool juegoTerminado = false;
    private GameObject personajeGanador;

    void Awake()
    {
        if (instancia == null) instancia = this;
    }

    public void GanarPartida(GameObject ganador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        personajeGanador = ganador;
        Debug.LogWarning($"<color=cyan>=== [GM] GanarPartida() invocado por: {ganador.name} ===</color>");

        if (DatosTorneo.instancia != null)
        {
            if (ganador.name == "Player 1")
            {
                DatosTorneo.instancia.victoriasP1++;
                Debug.Log($"<color=green>[GM] Punto para Player 1. Total: {DatosTorneo.instancia.victoriasP1}</color>");
            }
            else if (ganador.name == "Player 2")
            {
                DatosTorneo.instancia.victoriasP2++;
                Debug.Log($"<color=green>[GM] Punto para Player 2. Total: {DatosTorneo.instancia.victoriasP2}</color>");
            }

            // Guardamos el progreso en disco inmediatamente
            DatosTorneo.instancia.GuardarProgreso();
        }
        else
        {
            Debug.LogError("<color=red>[GM] ¡ALERTA! DatosTorneo.instancia es NULL al intentar sumar puntos.</color>");
        }

        if (personajeGanador != null)
        {
            Animator anim = personajeGanador.GetComponentInChildren<Animator>();
            if (anim != null) anim.SetFloat("Velocidad", 0f);

            MonoBehaviour[] scripts = personajeGanador.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) script.enabled = false;
            }

            Rigidbody rb = personajeGanador.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            float gradosDeRotacionY = 90f;
            personajeGanador.transform.rotation = Quaternion.Euler(0f, gradosDeRotacionY, 0f);
        }

        if (Camera.main != null)
        {
            CamaraScroll25D scriptCamara = Camera.main.GetComponent<CamaraScroll25D>();
            if (scriptCamara != null)
            {
                scriptCamara.EnfocarGanador(personajeGanador);
            }
        }

        GameObject[] todosLosJugadores = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject jugador in todosLosJugadores)
        {
            if (jugador != personajeGanador)
            {
                MonoBehaviour[] scriptsPerdedor = jugador.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scriptsPerdedor)
                {
                    script.enabled = false;
                }

                Rigidbody rbPerdedor = jugador.GetComponent<Rigidbody>();
                if (rbPerdedor != null)
                {
                    rbPerdedor.linearVelocity = Vector3.zero;
                    rbPerdedor.angularVelocity = Vector3.zero;
                    rbPerdedor.isKinematic = true;
                }

                Animator animPerdedor = jugador.GetComponentInChildren<Animator>();
                if (animPerdedor != null) animPerdedor.SetFloat("Velocidad", 0f);
            }
        }

        if (panelTextoVictoria != null)
        {
            panelTextoVictoria.SetActive(true);
            Debug.Log("[GM] Panel Texto Victoria ACTIVADO.");
        }

        if (imagenIris != null)
        {
            imagenIris.gameObject.SetActive(true);
            Debug.Log("[GM] Iniciando Corrutina de Iris...");
            StartCoroutine(AnimarCierreIris());
        }
        else
        {
            Debug.LogError("<color=red>[GM] Imagen Iris es NULL. No se puede ejecutar el cierre.</color>");
        }
    }

    private IEnumerator AnimarCierreIris()
    {
        Transform target = null;
        if (personajeGanador != null)
        {
            target = personajeGanador.transform.Find("HeadTarget");
            if(target == null) Debug.LogWarning("[GM] No se encontró 'HeadTarget' en el ganador, el Iris cerrará en el centro del objeto.");
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
        Debug.Log("[GM] Efecto Iris Terminado. Esperando tiempo post-cierre...");

        yield return new WaitForSeconds(tiempoEsperaPostCierre);

        Debug.LogWarning("<color=orange>=== [GM] Pasó el tiempo de espera. Evaluando condiciones de fin de torneo ===</color>");

        if (DatosTorneo.instancia != null)
        {
            int rondasParaGanar = DatosTorneo.instancia.ObtenerRondasParaGanar();
            Debug.Log($"[GM] Datos Torneo -> Rondas para ganar: {rondasParaGanar} | P1: {DatosTorneo.instancia.victoriasP1} - P2: {DatosTorneo.instancia.victoriasP2}");

            if (DatosTorneo.instancia.victoriasP1 >= rondasParaGanar || DatosTorneo.instancia.victoriasP2 >= rondasParaGanar)
            {
                Debug.LogWarning("<color=green>[GM] ¡CUMPLIDO! Un jugador alcanzó las victorias necesarias. Saltando a TerminarTorneoCompleto()</color>");
                TerminarTorneoCompleto();
            }
            else
            {
                Debug.Log("<color=yellow>[GM] Nadie llegó al límite todavía. Recargando escena para la siguiente ronda...</color>");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            Debug.LogError("<color=red>[GM] DatosTorneo.instancia es NULL en la corrutina. Plan B: Reiniciando escena suelta.</color>");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void TerminarTorneoCompleto()
    {
        Debug.LogWarning("<color=magenta>=== [GM] ENTRANDO A TerminarTorneoCompleto() ===</color>");

        if (panelEstadisticasFinales != null)
        {
            if (imagenIris != null) imagenIris.gameObject.SetActive(false);
            if (panelTextoVictoria != null) panelTextoVictoria.SetActive(false);

            Debug.Log("[GM] Forzando encendido del Panel Estadisticas Finales...");
            panelEstadisticasFinales.SetActive(true);

            if (DatosTorneo.instancia != null)
            {
                string nombreCampeon = "";
                if (DatosTorneo.instancia.victoriasP1 > DatosTorneo.instancia.victoriasP2)
                {
                    nombreCampeon = "¡PLAYER 1 ES EL CAMPEÓN!";
                }
                else
                {
                    nombreCampeon = "¡PLAYER 2 ES EL CAMPEÓN!";
                }

                Debug.Log($"[GM] Ganador calculated: {nombreCampeon}");

                if (textoGanadorFinal != null)
                {
                    textoGanadorFinal.text = nombreCampeon;
                    Debug.Log("[GM] Texto del Ganador Final inyectado con éxito.");
                }
                else
                {
                    Debug.LogError("<color=red>[GM] ¡Ojo! El campo 'textoGanadorFinal' está vacío en el Inspector.</color>");
                }

                if (textoContadorVictorias != null)
                {
                    textoContadorVictorias.text = $"Player 1: {DatosTorneo.instancia.victoriasP1}  -  Player 2: {DatosTorneo.instancia.victoriasP2}";
                    Debug.Log("[GM] Texto del Contador de Victorias inyectado con éxito.");
                }
                else
                {
                    Debug.LogError("<color=red>[GM] ¡Ojo! El campo 'textoContadorVictorias' está vacío en el Inspector.</color>");
                }
            }
        }
        else
        {
            Debug.LogWarning("[GM] 'panelEstadisticasFinales' es NULL en el inspector, cargando 'Escena_Podio' como Plan B...");
            SceneManager.LoadScene("Escena_Podio");
        }
    }

    public void BotonReiniciarTorneo()
    {
        if (DatosTorneo.instancia != null)
        {
            DatosTorneo.instancia.ResetearTorneo();
        }

        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    public void BotonVolverAlMenu()
    {
        if (DatosTorneo.instancia != null)
        {
            Destroy(DatosTorneo.instancia.gameObject);
        }

        SceneManager.LoadScene("MenuPrincipal");
    }
}