using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [Header("Punto de control donde revive este jugador")]
    public Transform respawn;

    [Header("Límite de caída en el vacío")]
    public int posicionEjeY = -20;

    private Rigidbody rb;

    void Start()
    {
        // Guardamos el Rigidbody al arrancar para no sobrecargar el Update
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. Si el jugador cae por debajo del límite de la Y
        if (transform.position.y < posicionEjeY)
        {
            EjecutarRespawn();
        }

        // 2. Si el jugador aprieta la B para volver manualmente (Testing)
        if (Input.GetKeyUp(KeyCode.B))
        {
            EjecutarRespawn();
        }
    }

    void EjecutarRespawn()
    {
        // Si hay un punto de respawn asignado, lo movemos ahí
        if (respawn != null)
        {
            transform.position = respawn.position;
        }
        else
        {
            Debug.LogWarning("¡Ojo! No le asignaste un objeto Respawn al script de: " + gameObject.name);
        }

        // Frenamos las físicas por completo para que no aparezca con impulso de la caída
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // --- LÓGICA DE VIDAS ---
        // Descomentá la línea de abajo SOLO si tenés el script 'control' con 'vidasJugador' creado:
        // if (GetComponent<control>() != null) GetComponent<control>().vidasJugador -= 1;
    }
}