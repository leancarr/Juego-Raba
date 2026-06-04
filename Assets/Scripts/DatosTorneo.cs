using UnityEngine;

public class DatosTorneo : MonoBehaviour
{
    public static DatosTorneo instancia;

    [Header("Estado del Torneo")]
    public int rondasTotales;
    public int victoriasP1 = 0;
    public int victoriasP2 = 0;

    void Awake()
    {
        // --- SINGLETON PERSISTENTE (DontDestroyOnLoad) ---
        // Esto asegura que el objeto sobreviva a los reinicios de escena
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            // Leemos las rondas que guardó tu "SeleccionRondasManager" vía PlayerPrefs.
            // Si testeás la escena suelta desde el editor, por defecto jugará a 3 rondas.
            rondasTotales = PlayerPrefs.GetInt("RondasElegidas", 3);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int ObtenerRondasParaGanar()
    {
        // La matemática exacta para "mayoría absoluta" sin empates es:
        // División entera de rondas totales entre 2, más 1.
        // - Para 3 rondas: (3 / 2) + 1 = 1 + 1 = 2 victorias.
        // - Para 5 rondas: (5 / 2) + 1 = 2 + 1 = 3 victorias.
        // - Para 10 rondas: (10 / 2) + 1 = 5 + 1 = 6 victorias.
        return (rondasTotales / 2) + 1;
    }

    /// <summary>
    /// Llámame desde los botones de "Revancha" o "Volver al Menú" 
    /// para limpiar el marcador antes de un nuevo torneo.
    /// </summary>
    public void ResetearTorneo()
    {
        victoriasP1 = 0;
        victoriasP2 = 0;
        // Volvemos a leer por las dudas si cambiaron la opción en el menú
        rondasTotales = PlayerPrefs.GetInt("RondasElegidas", 3);
        Debug.Log("Torneo reseteado. Listos para una nueva partida.");
    }
}