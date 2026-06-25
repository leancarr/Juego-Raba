using UnityEngine;

public class DatosTorneo : MonoBehaviour
{
    public static DatosTorneo instancia;
    public int rondasTotales;
    public int victoriasP1 = 0;
    public int victoriasP2 = 0;

    // --- NUEVO: Variables para saber qué personaje eligió cada uno ---
    // (0 podría ser Marcus, 1 el Personaje 2, 2 el Personaje 3, etc.)
    public int idPersonajeP1 = 0;
    public int idPersonajeP2 = 0;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
            CargarDatos(); // Cargamos la configuración inicial

            // --- SOLUCIÓN AL CACHÉ ---
#if UNITY_EDITOR
            ResetearTorneo();
            Debug.Log("<color=yellow>[DatosTorneo] Modo Editor detectado: Registro de victorias previas limpiado con éxito.</color>");
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GuardarProgreso()
    {
        PlayerPrefs.SetInt("VictoriasP1", victoriasP1);
        PlayerPrefs.SetInt("VictoriasP2", victoriasP2);
        // --- NUEVO: Guardamos qué personaje están usando ---
        PlayerPrefs.SetInt("IdPersonajeP1", idPersonajeP1);
        PlayerPrefs.SetInt("IdPersonajeP2", idPersonajeP2);

        PlayerPrefs.Save();
    }

    private void CargarDatos()
    {
        rondasTotales = PlayerPrefs.GetInt("RondasElegidas", 3);
        victoriasP1 = PlayerPrefs.GetInt("VictoriasP1", 0);
        victoriasP2 = PlayerPrefs.GetInt("VictoriasP2", 0);
        // --- NUEVO: Cargamos el personaje ---
        idPersonajeP1 = PlayerPrefs.GetInt("IdPersonajeP1", 0);
        idPersonajeP2 = PlayerPrefs.GetInt("IdPersonajeP2", 0);
    }

    public int ObtenerRondasParaGanar()
    {
        return (rondasTotales / 2) + 1;
    }

    public void ResetearTorneo()
    {
        victoriasP1 = 0;
        victoriasP2 = 0;
        PlayerPrefs.DeleteKey("VictoriasP1");
        PlayerPrefs.DeleteKey("VictoriasP2");
        rondasTotales = PlayerPrefs.GetInt("RondasElegidas", 3);
        // Nota: NO reseteamos los IDs de personaje aquí, para que si le dan a "Reiniciar Torneo", 
        // sigan jugando con los mismos personajes que habían elegido.
    }
}