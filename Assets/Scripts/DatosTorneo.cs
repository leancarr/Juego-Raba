using UnityEngine;

public class DatosTorneo : MonoBehaviour
{
    public static DatosTorneo instancia;
    public int rondasTotales;
    public int victoriasP1 = 0;
    public int victoriasP2 = 0;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
            CargarDatos(); // Cargamos la configuración inicial

            // --- SOLUCIÓN AL CACHÉ ---
            // Esto solo se ejecuta una vez al darle "Play" en el editor de Unity.
            // Borra las victorias del Play anterior pero mantiene las rondas elegidas.
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

    // Llama a esto desde el GameManager después de sumar un punto
    public void GuardarProgreso()
    {
        PlayerPrefs.SetInt("VictoriasP1", victoriasP1);
        PlayerPrefs.SetInt("VictoriasP2", victoriasP2);
        PlayerPrefs.Save(); // Forzamos escritura en disco
    }

    private void CargarDatos()
    {
        rondasTotales = PlayerPrefs.GetInt("RondasElegidas", 3);
        victoriasP1 = PlayerPrefs.GetInt("VictoriasP1", 0);
        victoriasP2 = PlayerPrefs.GetInt("VictoriasP2", 0);
    }

    public int ObtenerRondasParaGanar()
    {
        // Esto calcula cuántas rondas necesita alguien para ganar el torneo (Mayoría absoluta)
        return (rondasTotales / 2) + 1;
    }

    public void ResetearTorneo()
    {
        victoriasP1 = 0;
        victoriasP2 = 0;
        PlayerPrefs.DeleteKey("VictoriasP1"); // Borramos del disco duro
        PlayerPrefs.DeleteKey("VictoriasP2");
        rondasTotales = PlayerPrefs.GetInt("RondasElegidas", 3);
    }
}