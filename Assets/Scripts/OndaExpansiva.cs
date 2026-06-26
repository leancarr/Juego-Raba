/* 
 * ==============================================================================
 * SCRIPT: OndaExpansiva.cs
 * CATEGORIA: 5. Elementos del Nivel (Efectos y Triggers)
 * DESCRIPCION: El efecto visual del Rapero. Hace que una esfera crezca rapidisimo y desaparezca para simular un sonido fisico.
 * ==============================================================================
 */
using UnityEngine;

public class OndaExpansiva : MonoBehaviour
{
    public float velocidadExpansion = 15f;
    private Material mat;
    private Color colorOriginal;

    void Start()
    {
        // Empieza a la mitad de su tamaÃ±o original para que el crecimiento sea mÃ¡s notorio
        transform.localScale = Vector3.one * 0.5f;

        MeshRenderer render = GetComponent<MeshRenderer>();
        if (render != null)
        {
            mat = render.material;
            // Chequeamos quÃ© tipo de shader tiene para agarrar el color base
            if (mat.HasProperty("_BaseColor")) // Usualmente en Universal Render Pipeline
            {
                colorOriginal = mat.GetColor("_BaseColor");
            }
            else if (mat.HasProperty("_Color")) // Usualmente en el Standard Shader
            {
                colorOriginal = mat.GetColor("_Color");
            }
        }
    }

    void Update()
    {
        // Crece uniformemente en todos los ejes
        transform.localScale += Vector3.one * velocidadExpansion * Time.deltaTime;

        // Se va desvaneciendo a medida que se hace mÃ¡s grande
        if (mat != null)
        {
            Color c = colorOriginal;
            // Calcula la transparencia. Si la escala llega a 6, se vuelve 100% invisible.
            float alpha = Mathf.Clamp01(1f - (transform.localScale.x / 6f)); 
            c.a = alpha;

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", c);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", c);
        }
    }
}

