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
        if (sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
        }
    }

    // Esta función la llama el Slider directamente desde su evento On Value Changed
    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolumenMaestro", valor);
        PlayerPrefs.Save();
    }
}

