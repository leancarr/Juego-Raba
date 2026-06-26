/* 
 * ==============================================================================
 * SCRIPT: PlataformasColores.cs
 * CATEGORIA: 3. Camara y Entorno
 * DESCRIPCION: Gestiona mecanicas especificas de pisos que cambian de color (quizas se caen o se activan).
 * ==============================================================================
 */
using UnityEngine;

public class PlataformasColores : MonoBehaviour
{
    // Elegimos el color de ESTA plataforma desde el inspector del Unity
    public ColorDeFondo.ColorEstado colorDeEstaPlataforma;

    // Manejo de Colliders y renderers originales
    private Collider[] misColliders;
    private MeshRenderer miRenderer;
    private Color colorOriginal;
    private bool yaEstabaApagada = false;

    void Start()
    {
        misColliders = GetComponents<Collider>();
        miRenderer = GetComponent<MeshRenderer>();

        if (misColliders.Length == 0)
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

        // REGLA INVERSA ACTUAL: Si el fondo es de su mismo color -> SE APAGA
        if (colorDeEstaPlataforma == colorFondoActual)
        {
            if (!yaEstabaApagada)
            {
                DesactivarPlataforma();
            }
        }
        else
        {
            if (yaEstabaApagada)
            {
                ActivarPlataforma();
            }
        }
    }

    void DesactivarPlataforma()
    {
        yaEstabaApagada = true;
        // Debug.Log("APAGANDO: " + gameObject.name);

        transform.DetachChildren();

        foreach (Collider col in misColliders)
        {
            col.enabled = false;
        }

        if (miRenderer != null) miRenderer.enabled = false;
    }

    void ActivarPlataforma()
    {
        yaEstabaApagada = false;
        // Debug.Log("PRENDIENDO: " + gameObject.name);

        foreach (Collider col in misColliders)
        {
            col.enabled = true;
        }

        if (miRenderer != null) miRenderer.enabled = true;
    }

    // --- FUNCIÓN CORREGIDA CON TUS COLORES OFICIALES ---
    public void SabotearBando()
    {
        // Rotación cíclica entre tus 3 colores oficiales
        if (colorDeEstaPlataforma == ColorDeFondo.ColorEstado.Azul)
        {
            colorDeEstaPlataforma = ColorDeFondo.ColorEstado.Amarillo;
            if (miRenderer != null) miRenderer.material.color = Color.yellow; // Amarillo
        }
        else if (colorDeEstaPlataforma == ColorDeFondo.ColorEstado.Amarillo)
        {
            colorDeEstaPlataforma = ColorDeFondo.ColorEstado.Rojo;
            if (miRenderer != null) miRenderer.material.color = Color.red; // Rojo
        }
        else if (colorDeEstaPlataforma == ColorDeFondo.ColorEstado.Rojo)
        {
            colorDeEstaPlataforma = ColorDeFondo.ColorEstado.Azul;
            if (miRenderer != null) miRenderer.material.color = Color.blue; // Azul
        }

        Debug.Log("¡Plataforma " + gameObject.name + " saboteada! Nuevo color: " + colorDeEstaPlataforma);
    }
}
