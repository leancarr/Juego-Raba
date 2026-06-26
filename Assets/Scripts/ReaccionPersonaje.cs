/* 
 * ==============================================================================
 * SCRIPT: ReaccionPersonaje.cs
 * CATEGORIA: 1. Control del Jugador (Personajes)
 * DESCRIPCION: Probablemente usado para manejar animaciones o efectos visuales cuando un personaje es golpeado (o como un sistema auxiliar de hitboxes).
 * ==============================================================================
 */
using UnityEngine;

public class ReaccionPersonaje : MonoBehaviour
{
    [Header("ConfiguraciÃ³n")]
    public float velocidadRotacion = 45f;
    public float suavizado = 10f; // QuÃ© tan rÃ¡pido frena y se da vuelta

    private Quaternion rotacionOriginal;
    private bool estaIluminado = false;
    private Animator miAnimator;

    void Start()
    {
        // Guardamos su posiciÃ³n frontal exacta al arrancar el juego
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
            // MODO VITRINA: Gira constantemente sobre sÃ­ mismo
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        }
    }

    // El Manager usarÃ¡ esta funciÃ³n para avisarle que la luz lo estÃ¡ tocando
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
