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
    
    [Header("Icono de Volumen (Mute/Desmute)")]
    public Image iconoMute;
    public Sprite spriteVolumenActivado;
    public Sprite spriteVolumenMutado;
    
    private bool isMuted = false;
    private float volumenAntesDeMute = 1f;

    void Start()
    {
        // Cargar el volumen guardado (si no hay dato, por defecto es 1 = 100%)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMaestro", 1f);
        
        // Cargar el volumen previo al mute, si existe
        volumenAntesDeMute = PlayerPrefs.GetFloat("VolumenAntesDeMute", 1f);

        if (volumenGuardado <= 0.001f)
        {
            isMuted = true;
        }
        else
        {
            isMuted = false;
            volumenAntesDeMute = volumenGuardado;
            PlayerPrefs.SetFloat("VolumenAntesDeMute", volumenAntesDeMute);
        }

        AudioListener.volume = volumenGuardado;

        Debug.Log("<color=cyan>[UI] MenuOpciones START. Volumen cargado desde PlayerPrefs: " + volumenGuardado + "</color>");

        if (sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
            
            // Asignamos el evento por código para asegurarnos de que el Slider funcione
            sliderVolumen.onValueChanged.RemoveAllListeners();
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
        
        ActualizarIconoMute();
    }

    // Esta función la llama el Slider directamente desde su evento On Value Changed
    public void CambiarVolumen(float valor)
    {
        Debug.Log("<color=yellow>[UI] MenuOpciones CambiarVolumen. Nuevo valor: " + valor + "</color>");
        
        if (isMuted && valor > 0.001f)
        {
            isMuted = false;
        }
        else if (!isMuted && valor <= 0.001f)
        {
            isMuted = true;
            if (AudioListener.volume > 0.001f)
            {
                volumenAntesDeMute = AudioListener.volume;
                PlayerPrefs.SetFloat("VolumenAntesDeMute", volumenAntesDeMute);
            }
        }
        else if (!isMuted)
        {
            volumenAntesDeMute = valor;
            PlayerPrefs.SetFloat("VolumenAntesDeMute", volumenAntesDeMute);
        }

        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolumenMaestro", valor);
        PlayerPrefs.Save();
        
        ActualizarIconoMute();
    }
    
    // Función que se llamará al presionar el botón del icono
    public void ToggleMute()
    {
        isMuted = !isMuted;
        
        if (isMuted)
        {
            // Guardamos el volumen actual antes de mutear si es mayor a cero
            if (sliderVolumen != null && sliderVolumen.value > 0.001f)
            {
                volumenAntesDeMute = sliderVolumen.value;
                PlayerPrefs.SetFloat("VolumenAntesDeMute", volumenAntesDeMute);
            }
            
            // Aplicar el mute
            if (sliderVolumen != null) sliderVolumen.value = 0f;
            AudioListener.volume = 0f;
            PlayerPrefs.SetFloat("VolumenMaestro", 0f);
        }
        else
        {
            // Restaurar volumen previo (por defecto 1f si estaba mal guardado)
            float volumenRestaurado = volumenAntesDeMute > 0.001f ? volumenAntesDeMute : 1f;
            
            if (sliderVolumen != null) sliderVolumen.value = volumenRestaurado;
            AudioListener.volume = volumenRestaurado;
            PlayerPrefs.SetFloat("VolumenMaestro", volumenRestaurado);
        }
        
        PlayerPrefs.Save();
        ActualizarIconoMute();
    }
    
    // Cambia el sprite del icono según el estado
    private void ActualizarIconoMute()
    {
        if (iconoMute != null)
        {
            if (isMuted || (sliderVolumen != null && sliderVolumen.value <= 0.001f))
            {
                if (spriteVolumenMutado != null)
                    iconoMute.sprite = spriteVolumenMutado;
            }
            else
            {
                if (spriteVolumenActivado != null)
                    iconoMute.sprite = spriteVolumenActivado;
            }
        }
    }
}
