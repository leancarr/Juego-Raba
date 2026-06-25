using UnityEngine;
using UnityEngine.UI; // Necesario para usar Sliders y Toggles

public class MenuOpciones : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider sliderVolumen;
    public Toggle togglePantallaCompleta;

    void Start()
    {
        // 1. Cargar el volumen guardado (si no hay, el por defecto es 1, o sea 100%)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMaestro", 1f);
        AudioListener.volume = volumenGuardado;
        if (sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
        }

        // 2. Cargar pantalla completa guardada (por defecto 1 = true)
        bool pantallaCompletaGuardada = PlayerPrefs.GetInt("PantallaCompleta", 1) == 1;
        Screen.fullScreen = pantallaCompletaGuardada;
        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.isOn = pantallaCompletaGuardada;
        }
    }

    // --- ESTAS FUNCIONES SE LLAMAN DESDE LOS EVENTOS DE LA UI ---

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolumenMaestro", valor);
        PlayerPrefs.Save();
    }

    public void CambiarPantallaCompleta(bool esCompleta)
    {
        Screen.fullScreen = esCompleta;
        // Guardamos 1 si es true, 0 si es false (PlayerPrefs no soporta bool directo)
        PlayerPrefs.SetInt("PantallaCompleta", esCompleta ? 1 : 0);
        PlayerPrefs.Save();
    }
}
