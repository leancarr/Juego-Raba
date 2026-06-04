using UnityEngine;
using TMPro; // Obligatorio para manejar el TextMeshPro de la UI
using UnityEngine.SceneManagement; // <-- IMPORTANTE: Añadimos esto para poder cambiar de pantalla

public class PodioPersonaje : MonoBehaviour
{
    private Animator miAnimator;
    private string nombreParametro = "Velocidad";
    private Quaternion rotacionOriginal;
    private bool mouseEncima = false;

    // Variables de control de estado
    private bool estaPreseleccionado = false;
    private bool estaConfirmado = false;

    [Header("Configuración de Velocidad de Animación")]
    [Range(0f, 1f)][SerializeField] private float velocidadAlCaminar = 1f;

    [Header("Configuración de Rotación")]
    [SerializeField] private float velocidadRotacion = 30f;
    [SerializeField] private float suavizadoFrente = 10f;

    [Header("UI - Componentes Básicos")]
    [SerializeField] private TextMeshProUGUI componenteTextoUI; // Arrastramos el TextoInfo acá
    [SerializeField] private GameObject panelResumenUI;         // Arrastramos el PanelResumen acá

    [Header("UI - Controles de Selección (Objetos Completos)")]
    [SerializeField] private GameObject botonPreseleccionUI;   // Arrastramos BotonSelceccio entero acá
    [SerializeField] private GameObject botonConfirmarUI;       // Arrastramos BotonConfirmar_P1 entero acá
    [SerializeField] private GameObject textoListoUI;           // Arrastramos el TextoListo ("¡LISTO!") acá

    [Header("Textos del Personaje")]
    [TextArea(2, 4)][SerializeField] private string lineaNombre = "PROFESOR";
    [TextArea(2, 4)][SerializeField] private string lineaDescripcion = "Docente desquiciado entre ecuaciones.";
    [Space(10)]
    [TextArea(2, 4)][SerializeField] private string lineaHabilidadTitulo = "SABOTAJE DE FASE";
    [TextArea(2, 4)][SerializeField] private string lineaHabilidadDesc = "Dispara un pulso que cambia el color de una plataforma lejana.";

    void Start()
    {
        rotacionOriginal = transform.rotation;

        miAnimator = GetComponentInChildren<Animator>();
        if (miAnimator != null)
        {
            miAnimator.SetFloat(nombreParametro, velocidadAlCaminar);
        }

        // Reseteamos el estado visual de la UI al arrancar la escena
        if (panelResumenUI != null) panelResumenUI.SetActive(false);
        if (botonConfirmarUI != null) botonConfirmarUI.SetActive(false);
        if (textoListoUI != null) textoListoUI.SetActive(false);

        // Seteamos el texto inicial del botón principal de forma segura
        CambiarTextoBotonPrincipal("Preseleccionar");
    }

    void Update()
    {
        // Si ya está preseleccionado o confirmado, se clava mirando al frente pase lo que pase
        if (estaPreseleccionado || estaConfirmado)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionOriginal, Time.deltaTime * suavizadoFrente);
            return;
        }

        // Lógica de rotación normal por mouse
        if (!mouseEncima)
        {
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionOriginal, Time.deltaTime * suavizadoFrente);
        }
    }

    // FUNCIÓN DEL BOTÓN PRINCIPAL (Preseleccionar / Cancelar)
    // Vinculá esta función al OnClick() de tu botón "Preseleccionar"
    public void PresionarBotonPrincipal()
    {
        if (estaConfirmado) return; // Seguridad: si ya confirmó, bloqueamos interacciones

        if (!estaPreseleccionado)
        {
            // --- ENTRANDO A MODO: PRESELECCIONAR ---
            estaPreseleccionado = true;

            if (panelResumenUI != null) panelResumenUI.SetActive(true);
            if (botonConfirmarUI != null) botonConfirmarUI.SetActive(true); // Muestra botón Confirmar
            CambiarTextoBotonPrincipal("Cancelar");                         // Cambia texto a Cancelar

            if (miAnimator != null) miAnimator.SetFloat(nombreParametro, 0f); // Pausa la animación

            // Escribimos los textos formateados en el cuadro
            if (componenteTextoUI != null)
            {
                componenteTextoUI.text = lineaNombre + "\n" + lineaDescripcion + "\n\n" +
                                         lineaHabilidadTitulo + "\n" + lineaHabilidadDesc;
            }
        }
        else
        {
            // --- ENTRANDO A MODO: CANCELAR ---
            estaPreseleccionado = false;

            if (panelResumenUI != null) panelResumenUI.SetActive(false);
            if (botonConfirmarUI != null) botonConfirmarUI.SetActive(false); // Oculta Confirmar
            CambiarTextoBotonPrincipal("Preseleccionar");                     // Vuelve al texto original

            if (miAnimator != null) miAnimator.SetFloat(nombreParametro, velocidadAlCaminar); // Vuelve a caminar
        }
    }

    // FUNCIÓN PARA EL BOTÓN CONFIRMAR
    // Vinculá esta función al OnClick() de tu botón "Confirmar"
    public void ConfirmarEleccion()
    {
        estaConfirmado = true;
        estaPreseleccionado = false;

        if (botonConfirmarUI != null) botonConfirmarUI.SetActive(false); // Se apaga a sí mismo
        if (panelResumenUI != null) panelResumenUI.SetActive(false);     // Apaga el cartel gris (opcional)

        if (textoListoUI != null) textoListoUI.SetActive(true);          // Muestra el "¡LISTO!" verde en pantalla
        if (botonPreseleccionUI != null) botonPreseleccionUI.SetActive(false); // Apaga por completo el botón de Preseleccionar/Cancelar

        // --- EL CAMBIO CLAVE: Mandamos al juego a la pantalla de carga ---
        Debug.Log("Personaje confirmado, cargando juego...");
        SceneManager.LoadScene("PantallaCarga");
    }

    // Función auxiliar para cambiar el texto del botón principal sin renegar con la asignación
    private void CambiarTextoBotonPrincipal(string nuevoTexto)
    {
        if (botonPreseleccionUI != null)
        {
            TextMeshProUGUI textoHijo = botonPreseleccionUI.GetComponentInChildren<TextMeshProUGUI>();
            if (textoHijo != null)
            {
                textoHijo.text = nuevoTexto;
            }
        }
    }

    void OnMouseEnter()
    {
        if (estaPreseleccionado || estaConfirmado) return;
        mouseEncima = true;
        if (miAnimator != null) miAnimator.SetFloat(nombreParametro, 0f);
    }

    void OnMouseExit()
    {
        if (estaPreseleccionado || estaConfirmado) return;
        mouseEncima = false;
        if (miAnimator != null) miAnimator.SetFloat(nombreParametro, velocidadAlCaminar);
    }
}