/* 
 * ==============================================================================
 * SCRIPT: MenuOpciones.cs
 * CATEGORIA: 4. Menues y UI (Interfaz)
 * DESCRIPCION: Controla el Slider del volumen maestro y la Pantalla Completa, guardando los gustos del jugador.
 * ==============================================================================
 */
using UnityEngine;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider sliderVolumen;

    void Start()
    {
        // Cargar el volumen guardado (si no hay dato, por defecto es 1 = 100%)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMaestro", 1f);
        AudioListener.volume = volumenGuardado;

        Debug.Log("<color=cyan>[UI] MenuOpciones START. Volumen cargado desde PlayerPrefs: " + volumenGuardado + "</color>");

        if (sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
            
            // Asignamos el evento por código para asegurarnos de que el Slider funcione
            sliderVolumen.onValueChanged.RemoveAllListeners();
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    // Esta función la llama el Slider directamente desde su evento On Value Changed
    public void CambiarVolumen(float valor)
    {
        Debug.Log("<color=yellow>[UI] MenuOpciones CambiarVolumen. Nuevo valor: " + valor + "</color>");
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolumenMaestro", valor);
        PlayerPrefs.Save();
    }
}
