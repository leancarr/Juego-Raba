/* 
 * ==============================================================================
 * SCRIPT: PlataformaMovil.cs
 * CATEGORIA: 3. Camara y Entorno
 * DESCRIPCION: Hace que los pisos floten de lado a lado o arriba a abajo para los parkours.
 * ==============================================================================
 */
using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    [Header("ConfiguraciÃ³n de Movimiento")]
    public float velocidad = 3f;
    public float distancia = 4f;
    public bool moverHorizontal = true; // Si marcas la casilla en Unity, va de lado a lado. Si la desmarcas, va de arriba a abajo.

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos el punto de partida exacto de la plataforma al iniciar el juego
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Mathf.Sin crea un movimiento de vaivÃ©n suave
        float desfase = Mathf.Sin(Time.time * velocidad) * distancia;

        // Aplicamos el movimiento dependiendo del eje elegido
        if (moverHorizontal)
        {
            transform.position = posicionInicial + new Vector3(desfase, 0, 0);
        }
        else
        {
            transform.position = posicionInicial + new Vector3(0, desfase, 0);
        }
    }

    // --- EL TRUCO PARA QUE EL JUGADOR NO SE RESBALE ---
    // --- EL SENSOR INVISIBLE PARA EL PARENTESCO ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // Lo suelta cuando sale de la zona
        }
    }
}
