/* 
 * ==============================================================================
 * SCRIPT: MenuPausa.cs
 * CATEGORIA: 4. Menues y UI (Interfaz)
 * DESCRIPCION: Congela el tiempo (Time.timeScale = 0) en medio de la partida y te deja volver al menu.
 * ==============================================================================
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelPausa; // El panel semitransparente que contiene todo el menÃº
    public GameObject panelOpciones; // El recuadro flotante de configuraciones

    private bool juegoPausado = false;

    void Start()
    {
        // Nos aseguramos de que los menÃºes arranquen escondidos y el tiempo corra normal
        panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Detectamos si el jugador presiona Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f; // Congela el tiempo
        juegoPausado = true;
    }

    public void Reanudar()
    {
        panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        Time.timeScale = 1f; // Descongela el tiempo
        juegoPausado = false;
    }

    public void ReiniciarPartida()
    {
        // Resetea el contador de victorias a 0 y recarga el nivel desde cero
        Time.timeScale = 1f;
        if (DatosTorneo.instancia != null)
        {
            DatosTorneo.instancia.ResetearTorneo();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CargarMenuPrincipal()
    {
        // Descongelamos y volvemos al menÃº
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void AbrirOpciones()
    {
        panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        panelPausa.SetActive(true);
    }
}

