using UnityEngine;

public class ReaccionPersonaje : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadRotacion = 45f;
    public float suavizado = 10f; // Qué tan rápido frena y se da vuelta

    private Quaternion rotacionOriginal;
    private bool estaIluminado = false;
    private Animator miAnimator;

    void Start()
    {
        // Guardamos su posición frontal exacta al arrancar el juego
        rotacionOriginal = transform.rotation;

        // Si tienes animaciones, esto las controla
        miAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (estaIluminado)
        {
            // MODO SELECCIONADO: Frena y se gira suavemente hacia el frente
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionOriginal, Time.deltaTime * suavizado);
        }
        else
        {
            // MODO VITRINA: Gira constantemente sobre sí mismo
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        }
    }

    // El Manager usará esta función para avisarle que la luz lo está tocando
    public void CambiarEstado(bool iluminado)
    {
        estaIluminado = iluminado;

        // Si el personaje camina, lo frenamos al iluminarlo
        if (miAnimator != null)
        {
            miAnimator.SetFloat("Velocidad", iluminado ? 0f : 1f);
        }
    }
}