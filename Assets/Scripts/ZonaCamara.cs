/* 
 * ==============================================================================
 * SCRIPT: ZonaCamara.cs
 * CATEGORIA: 3. Camara y Entorno
 * DESCRIPCION: Triggers invisibles en el mapa. Cuando un jugador los toca, le dicen a la camara 'Ey, frena aca' o 'Cambia a esta nueva vista', para lograr ese estilo Crash Bandicoot.
 * ==============================================================================
 */
using UnityEngine;

public class ZonaCamara : MonoBehaviour
{
    [Header("Configuracion de Seguimiento")]
    [Tooltip("La camara solo cambiara si el orden de la nueva zona es mayor o igual a la actual.")]
    public int ordenDeZona = 1;
    public bool seguirEnX = true;
    public bool seguirEnY = false;

    [Header("Offsets / Desvios")]
    public float desvioX = 2f;
    public float desvioY = 0f;

    [Header("Suavizado Override")]
    public float suavizadoOverride = 0f; // 0 significa usar el por defecto de la camara

    [Header("Limites de la Zona (X)")]
    public bool usarLimitesX = false;
    public float limiteIzquierdo = 0f;
    public float limiteDerecho = 100f;

    [Header("Limites de la Zona (Y)")]
    public bool usarLimitesY = false;
    public float limiteInferior = -50f;
    public float limiteSuperior = 50f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Camera.main != null)
            {
                CamaraScroll25D camara = Camera.main.GetComponent<CamaraScroll25D>();
                if (camara != null)
                {
                    camara.SetZonaActiva(this);
                }
            }
        }
    }

}

