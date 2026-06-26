/* 
 * ==============================================================================
 * SCRIPT: ColorDeFondo.cs
 * CATEGORIA: 3. Camara y Entorno
 * DESCRIPCION: Script estetico que le da tintes al fondo de tu escena para que no se vea estatico.
 * ==============================================================================
 */
using UnityEngine;

public class ColorDeFondo : MonoBehaviour
{
    // Definimos los colores posibles del ciclo
    public enum ColorEstado { Rojo, Azul, Amarillo }

    [Header("Configuración del Ciclo")]
    public ColorEstado colorActual = ColorEstado.Rojo;
    public float tiempoPorColor = 3.0f; // Cuánto dura cada color (en segundos)
    public float tiempoMinimoCambiante = 1.0f; // El límite de velocidad en la escalada de dificultad

    [Header("Colores Visuales (Asignar en el Inspector)")]
    public Color colorRojoVisual = Color.red;
    public Color colorAzulVisual = Color.blue;
    public Color colorAmarilloVisual = Color.yellow;

    private Camera camaraPrincipal;
    private float cronometro;

    // Instancia estática para que cualquier script (como las plataformas) pueda leer el color actual fácilmente
    public static ColorDeFondo Instancia;

    void Awake()
    {
        // Configuración del Singleton
        if (Instancia == null) { Instancia = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        camaraPrincipal = Camera.main;
        cronometro = tiempoPorColor;
        ActualizarColorCamara();
    }

    void Update()
    {
        // El cronómetro corre en reversa
        cronometro -= Time.deltaTime;

        if (cronometro <= 0)
        {
            CambiarSiguienteColor();
            cronometro = tiempoPorColor; // Reinicia el tiempo
        }
    }

    void CambiarSiguienteColor()
    {
        // Ciclo: Rojo -> Azul -> Amarillo -> Rojo...
        if (colorActual == ColorEstado.Rojo) { colorActual = ColorEstado.Azul; }
        else if (colorActual == ColorEstado.Azul) { colorActual = ColorEstado.Amarillo; }
        else if (colorActual == ColorEstado.Amarillo) { colorActual = ColorEstado.Rojo; }

        ActualizarColorCamara();
    }

    void ActualizarColorCamara()
    {
        if (camaraPrincipal == null) return;

        // Cambiamos el color de fondo "Solid Color" de la cámara de Unity
        if (colorActual == ColorEstado.Rojo) camaraPrincipal.backgroundColor = colorRojoVisual;
        else if (colorActual == ColorEstado.Azul) camaraPrincipal.backgroundColor = colorAzulVisual;
        else if (colorActual == ColorEstado.Amarillo) camaraPrincipal.backgroundColor = colorAmarilloVisual;
    }

    // Mecánica extra: Función para que la dificultad aumente (achica el tiempo del ciclo)
    public void AcelerarCiclo(float cantidad)
    {
        tiempoPorColor = Mathf.Max(tiempoMinimoCambiante, tiempoPorColor - cantidad);
    }
}
