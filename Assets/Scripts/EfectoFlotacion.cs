using UnityEngine;

public class EfectoFlotacion : MonoBehaviour
{
    public float velocidadVeces = 3f;      // Qué tan rápido oscila
    public float amplitudDistancia = 15f;  // Cuántos píxeles sube y baja
    public float desfaseInicio = 0f;       // El truco: cuándo arranca a moverse

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        // Sumamos el desfase dentro del tiempo para romper la sincronía perfecta
        float tiempoModificado = (Time.time * velocidadVeces) + desfaseInicio;
        float nuevoY = posicionInicial.y + Mathf.Sin(tiempoModificado) * amplitudDistancia;

        transform.localPosition = new Vector3(posicionInicial.x, nuevoY, posicionInicial.z);
    }
}