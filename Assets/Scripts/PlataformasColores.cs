using UnityEngine;

public class PlataformasColores : MonoBehaviour
{
    // Elegimos el color de ESTA plataforma desde el inspector del Unity
    public ColorDeFondo.ColorEstado colorDeEstaPlataforma;

    private Collider miCollider;
    private MeshRenderer miRenderer;
    private Color colorOriginal;
    private bool yaEstabaApagada = false;

    void Start()
    {
        miCollider = GetComponent<Collider>();
        miRenderer = GetComponent<MeshRenderer>();

        // Test de diagnóstico para el grupo:
        if (miCollider == null)
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

        if (miCollider != null) miCollider.enabled = false;

        // APAGAMOS por completo el componente visual. El cubo se vuelve 100% invisible.
        if (miRenderer != null) miRenderer.enabled = false;
    }

    void ActivarPlataforma()
    {
        yaEstabaApagada = false;
        Debug.Log("PRENDIENDO: " + gameObject.name);

        if (miCollider != null) miCollider.enabled = true;

        // PRENDEMOS el componente visual para que vuelva a renderizarse con su color original.
        if (miRenderer != null) miRenderer.enabled = true;
    }
}