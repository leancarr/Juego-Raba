using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelPausa; // El panel semitransparente que contiene todo el menú
    public GameObject panelOpciones; // El recuadro flotante de configuraciones

    private bool juegoPausado = false;

    void Start()
    {
        // Nos aseguramos de que los menúes arranquen escondidos y el tiempo corra normal
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

    public void ReiniciarNivel()
    {
        // ES VITAL devolver el tiempo a 1 antes de recargar, sino el nivel arranca congelado
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CargarMenuPrincipal()
    {
        // Descongelamos y volvemos al menú
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
