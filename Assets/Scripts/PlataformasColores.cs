using UnityEngine;

public class PlataformasColores : MonoBehaviour
{
    // Elegimos el color de ESTA plataforma desde el inspector del Unity
    public ColorDeFondo.ColorEstado colorDeEstaPlataforma;

    // CAMBIO 1: Cambiamos de una sola variable a un Array (lista) para manejar varios Colliders
    private Collider[] misColliders;
    private MeshRenderer miRenderer;
    private Color colorOriginal;
    private bool yaEstabaApagada = false;

    void Start()
    {
        // CAMBIO 2: Usamos GetComponents (en plural) para atrapar al sólido y al Trigger
        misColliders = GetComponents<Collider>();
        miRenderer = GetComponent<MeshRenderer>();

        // Test de diagnóstico actualizado:
        if (misColliders.Length == 0) // Verificamos si la lista está vacía
        {
            Debug.LogError("¡ALERTA! El script en " + gameObject.name + " NO encontró ningún Collider. Revisá dónde está pegado el script.");
        }

        if (miRenderer == null)
        {
            Debug.LogError("¡ALERTA! El script en " + gameObject.name + " NO encontró ningún MeshRenderer.");
        }

        if (miRenderer != null)
        {
            colorOriginal = miRenderer.material.color;
        }
    }

    void Update()
    {
        if (ColorDeFondo.Instancia == null) return;

        ColorDeFondo.ColorEstado colorFondoActual = ColorDeFondo.Instancia.colorActual;

        if (colorDeEstaPlataforma == colorFondoActual)
        {
            // Solo ejecuta si la plataforma NO estaba apagada todavía
            if (!yaEstabaApagada)
            {
                DesactivarPlataforma();
            }
        }
        else
        {
            // Solo ejecuta si la plataforma estaba apagada y ahora debe volver
            if (yaEstabaApagada)
            {
                ActivarPlataforma();
            }
        }
    }

    void DesactivarPlataforma()
    {
        yaEstabaApagada = true;
        Debug.Log("APAGANDO: " + gameObject.name);

        // Le decimos a la plataforma que suelte a cualquier jugador que tenga "pegado" como hijo
        transform.DetachChildren();

        // CAMBIO 3: Un bucle que apaga todos los colisionadores de la lista al mismo tiempo
        foreach (Collider col in misColliders)
        {
            col.enabled = false;
        }

        // APAGAMOS por completo el componente visual. El cubo se vuelve 100% invisible.
        if (miRenderer != null) miRenderer.enabled = false;
    }

    void ActivarPlataforma()
    {
        yaEstabaApagada = false;
        Debug.Log("PRENDIENDO: " + gameObject.name);

        // CAMBIO 4: Un bucle que vuelve a prender todos los colisionadores
        foreach (Collider col in misColliders)
        {
            col.enabled = true;
        }

        // PRENDEMOS el componente visual para que vuelva a renderizarse con su color original.
        if (miRenderer != null) miRenderer.enabled = true;
    }
}