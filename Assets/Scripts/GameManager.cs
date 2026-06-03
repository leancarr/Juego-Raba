using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Componentes de UI")]
    [Tooltip("Arrastrá acá el RectTransform de la Image negra con el agujero transparente")]
    public RectTransform imagenIris;

    [Tooltip("Tu panel viejo de texto de victoria (Opcional, sin fondo negro)")]
    public GameObject panelTextoVictoria;

    [Tooltip("Tu panel de derrota")]
    public GameObject panelDerrota;

    [Header("Configuración del Efecto")]
    [Tooltip("Cuánto tarda en cerrarse el círculo")]
    public float duracionCierre = 1.5f;
    [Tooltip("Cuánto tiempo se queda la pantalla en negro antes de reiniciar")]
    public float tiempoEsperaPostCierre = 2.0f;
    [Tooltip("Tiempo de espera cuando perdés antes de reiniciar")]
    public float tiempoEsperaMuerte = 3.0f;

    private bool juegoTerminado = false;

    void Awake()
    {
        if (instancia == null) instancia = this;
    }

    public void GanarPartida()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        GameObject personaje = GameObject.Find("Personaje");
        if (personaje != null)
        {
            // 1. CONGELAR EL ANIMATOR 
            // Buscamos el Animator (puede estar en el objeto principal o en un hijo)
            Animator anim = personaje.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                // Forzamos la transición a la animación quieta:
                anim.SetFloat("Velocidad", 0f);
            }

            // 2. APAGAR SCRIPTS DE MOVIMIENTO
            // Este truco busca todos los scripts (MonoBehaviour) pegados al personaje y los apaga
            // para que tu script de movimiento deje de mandar inputs o fuerzas.
            MonoBehaviour[] scripts = personaje.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) // Apagamos todos menos este script, claro
                {
                    script.enabled = false;
                }
            }

            // 3. FRENAR FÍSICAS
            Rigidbody rb = personaje.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero; // Evita que siga girando sobre sí mismo
                rb.isKinematic = true;            // Lo saca del sistema de físicas
            }

            // 4. FORZAR ROTACIÓN HACIA LA CÁMARA
            // Modificá este número hasta que mire a la cámara. 
            // Valores comunes para probar: 0, 90, 180, 270 (o negativos como -90)
            float gradosDeRotacionY = 90f;

            // Quaternion.Euler convierte los grados (X, Y, Z) al sistema de rotación de Unity
            personaje.transform.rotation = Quaternion.Euler(0f, gradosDeRotacionY, 0f);
        }

        // Activamos la UI y el Iris
        if (panelTextoVictoria != null) panelTextoVictoria.SetActive(true);

        if (imagenIris != null)
        {
            imagenIris.gameObject.SetActive(true);
            StartCoroutine(AnimarCierreIris());
        }
    }
    // ¡Acá volvió la función que te faltaba para el spawner!
    public void PerderPartida()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (panelDerrota != null) panelDerrota.SetActive(true);

        StartCoroutine(EsperarYReiniciarDerrota());
    }

    private IEnumerator AnimarCierreIris()
    {
        // Buscamos el target una sola vez al principio
        GameObject target = GameObject.Find("Personaje/HeadTarget");

        float tiempoPasado = 0f;
        Vector3 escalaInicial = new Vector3(150f, 150f, 1f);
        Vector3 escalaFinal = new Vector3(1f, 1f, 1f);

        while (tiempoPasado < duracionCierre)
        {
            tiempoPasado += Time.deltaTime;

            // NUEVO: Actualizamos la posición en CADA FRAME.
            // Si el personaje rota, se mueve o el péndulo lo desplaza,
            // la imagen de la UI lo va a perseguir en tiempo real.
            if (target != null && Camera.main != null)
            {
                Vector2 posicionPantalla = Camera.main.WorldToScreenPoint(target.transform.position);
                imagenIris.position = posicionPantalla;
            }

            // Achicamos el círculo progresivamente
            imagenIris.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tiempoPasado / duracionCierre);

            yield return null; // Espera al siguiente frame
        }

        // Aseguramos que quede en el tamaño final
        imagenIris.localScale = escalaFinal;

        // Esperamos antes de reiniciar la escena
        yield return new WaitForSeconds(tiempoEsperaPostCierre);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator EsperarYReiniciarDerrota()
    {
        yield return new WaitForSeconds(tiempoEsperaMuerte);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}