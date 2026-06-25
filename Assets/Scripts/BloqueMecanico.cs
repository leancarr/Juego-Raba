/* 
 * ==============================================================================
 * SCRIPT: BloqueMecanico.cs
 * CATEGORIA: 5. Elementos del Nivel (Efectos y Triggers)
 * DESCRIPCION: El comportamiento de la pared de cajas que invoca el Mecanico con su habilidad.
 * ==============================================================================
 */
using UnityEngine;

public class BloqueMecanico : MonoBehaviour
{
    public float tiempoVida = 3.5f;

    void Start()
    {
        // Desaparece solo tras el tiempo configurado
        Destroy(gameObject, tiempoVida);
    }
}
