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