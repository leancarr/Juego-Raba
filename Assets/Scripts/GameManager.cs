using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ¡Obligatorio para usar Corrutinas!

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Paneles de UI")]
    public GameObject panelVictoria;
    public GameObject panelDerrota;

    [Header("Configuración de Tiempos")]
    [Tooltip("Tiempo en segundos antes de reiniciar tras ganar o perder")]
    public float tiempoEspera = 4f;

    private bool juegoTerminado = false;

    void Awake()
    {
        instancia = this;
    }

    public void GanarPartida()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (panelVictoria != null) panelVictoria.SetActive(true);

        // Iniciamos la cuenta regresiva para reiniciar
        StartCoroutine(EsperarYReiniciar());
    }

    public void PerderPartida()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (panelDerrota != null) panelDerrota.SetActive(true);

        // Iniciamos la cuenta regresiva para reiniciar
        StartCoroutine(EsperarYReiniciar());
    }

    // Esta es la corrutina que hace la magia de la espera
    private IEnumerator EsperarYReiniciar()
    {
        // Espera la cantidad de segundos que configuraste en el Inspector
        yield return new WaitForSeconds(tiempoEspera);

        // Como no tenés más escenas, volvemos a cargar la escena actual (reinicio automático)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}