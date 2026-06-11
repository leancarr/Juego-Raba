using UnityEngine;

public class ZonaCamara : MonoBehaviour
{
    [Header("Configuracion de Seguimiento")]
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Camera.main != null)
            {
                CamaraScroll25D camara = Camera.main.GetComponent<CamaraScroll25D>();
                if (camara != null)
                {
                    camara.LimpiarZona(this);
                }
            }
        }
    }
}
