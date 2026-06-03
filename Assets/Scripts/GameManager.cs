using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Componentes de UI")]
    public RectTransform imagenIris;
    public GameObject panelTextoVictoria;

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

    //AHORA RECIBE AL JUGADOR QUE GANÓ POR PARÁMETRO
    public void GanarPartida(GameObject ganador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        personajeGanador = ganador;

        if (personajeGanador != null)
        {
            // 1. CONGELAR EL ANIMATOR
            Animator anim = personajeGanador.GetComponentInChildren<Animator>();
            if (anim != null) anim.SetFloat("Velocidad", 0f);

            // 2. APAGAR SCRIPTS DE MOVIMIENTO
            MonoBehaviour[] scripts = personajeGanador.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) script.enabled = false;
            }

            // 3. FRENAR FISICAS
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}