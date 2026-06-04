using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Obligatorio si usan el Slider

public class CargaAsincronica : MonoBehaviour
{
    [Header("Nombre de la escena del juego")]
    public string escenaA_Cargar = "Escena_tooni_Test"; // Poné acá el nombre exacto de tu mapa

    [Header("UI Componentes (Opcional)")]
    public Slider barraDeProgreso;

    void Start()
    {
        // Iniciamos una Corrutina (un proceso de fondo) para cargar el mapa
        StartCoroutine(CargarMapaProgreso());
    }

    IEnumerator CargarMapaProgreso()
    {
        // Empezamos a cargar la escena del juego de fondo
        AsyncOperation operacionCarga = SceneManager.LoadSceneAsync(escenaA_Cargar);

        // Mientras la carga no haya terminado...
        while (!operacionCarga.isDone)
        {
            // El progreso va de 0 a 0.9 en Unity. Lo normalizamos a 0-1.
            float progresoReal = Mathf.Clamp01(operacionCarga.progress / 0.9f);

            // Si le pusieron barra, se va a ir llenando sola
            if (barraDeProgreso != null)
            {
                barraDeProgreso.value = progresoReal;
            }

            Debug.Log("Progreso de carga: " + (progresoReal * 100) + "%");

            // Espera al siguiente frame antes de seguir el bucle (así no se congela el juego)
            yield return null;
        }
    }
}