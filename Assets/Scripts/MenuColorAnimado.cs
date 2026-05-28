using UnityEngine;
using System.Collections;

public class MenuColorAnimado : MonoBehaviour
{
    // Usamos el mismo sistema de estados para el menú
    public enum ColorEstado { Rojo, Azul, Amarillo }
    public ColorEstado colorActual = ColorEstado.Rojo;

    [Header("Tiempos del Menú")]
    public float tiempoPorColor = 4.0f; // Un poquito más lento para que el menú sea relajado
    public float velocidadBarrido = 1.2f; // Qué tan rápido se expande la onda radial

    [Header("Material del Shader Graph")]
    public Material materialFondoMenu; // Acá arrastrás tu 'Mat_FondoDinamico'

    [Header("Paleta de Colores")]
    public Color colorRojoVisual = Color.red;
    public Color colorAzulVisual = Color.blue;
    public Color colorAmarilloVisual = Color.yellow;

    private Color colorColorActualVisual;
    private float cronometro;

    void Start()
    {
        // Hacemos que el cronómetro arranque al límite para que dispare el cambio YA
        cronometro = 0.5f;

        // El color inicial real que ve el jugador va a ser el Rojo
        colorColorActualVisual = colorRojoVisual;
        colorActual = ColorEstado.Rojo;

        if (materialFondoMenu != null)
        {
            // Forzamos al shader a arrancar pintado de ROJO completo, 
            // pero el Update va a llamar al Azul casi al instante.
            materialFondoMenu.SetColor("_ColorOriginal", colorRojoVisual);
            materialFondoMenu.SetColor("_ColorNuevo", colorRojoVisual);
            materialFondoMenu.SetFloat("_ProgresoTransicion", 1f);
        }
    }

    void Update()
    {
        cronometro -= Time.deltaTime;
        if (cronometro <= 0)
        {
            CambiarSiguienteColor();
            cronometro = tiempoPorColor;
        }
    }

    void CambiarSiguienteColor()
    {
        Color colorViejo = colorColorActualVisual;

        // Ciclo de colores idéntico
        if (colorActual == ColorEstado.Rojo) { colorActual = ColorEstado.Azul; colorColorActualVisual = colorAzulVisual; }
        else if (colorActual == ColorEstado.Azul) { colorActual = ColorEstado.Amarillo; colorColorActualVisual = colorAmarilloVisual; }
        else if (colorActual == ColorEstado.Amarillo) { colorActual = ColorEstado.Rojo; colorColorActualVisual = colorRojoVisual; }

        // Lanza la corrutina que mueve el "Step" del Shader Graph
        if (gameObject.activeInHierarchy && materialFondoMenu != null)
        {
            StartCoroutine(AnimarBarridoEsquina(colorViejo, colorColorActualVisual));
        }
    }

    IEnumerator AnimarBarridoEsquina(Color viejo, Color nuevo)
    {
        // Pasamos los colores al shader
        materialFondoMenu.SetColor("_ColorOriginal", viejo);
        materialFondoMenu.SetColor("_ColorNuevo", nuevo);

        float progreso = 0f;
        materialFondoMenu.SetFloat("_ProgresoTransicion", progreso);

        // Va expandiendo el círculo matemático del Shader desde 0 hasta 1
        while (progreso < 1f)
        {
            progreso += Time.deltaTime * velocidadBarrido;
            materialFondoMenu.SetFloat("_ProgresoTransicion", Mathf.Clamp01(progreso));
            yield return null; // Espera al próximo frame
        }
    }
}