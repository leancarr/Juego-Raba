using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour
{
    [Header("Límite de caída en el vacío")]
    public int posicionEjeY = -50;

    private bool puedeMorir = false;

    void Start()
    {
        Debug.Log("La posición inicial en Y de " + gameObject.name + " es: " + transform.position.y);
        StartCoroutine(ActivarProteccionInicial());
    }

    IEnumerator ActivarProteccionInicial()
    {
        yield return new WaitForSeconds(0.25f);
        puedeMorir = true;
    }

    void Update()
    {
        if (!puedeMorir) return;

        // 1. Si cae al vacío
        if (transform.position.y < posicionEjeY)
        {
            MuerteDefinitiva();
        }

        // 2. Tecla de suicidio para testear
        if (Input.GetKeyDown(KeyCode.B))
        {
            MuerteDefinitiva();
        }
    }

    // ¡Acá está el truco! Ahora el trigger externo de la pared tampoco puede matarte si no pasó el tiempo
    public void MuerteDefinitiva()
    {
        if (!puedeMorir) return; // <--- ESCUDO DE FUERZA ACTIVADO

        Debug.Log("¡" + gameObject.name + " murió! Activando pantalla de derrota...");

        if (GameManager.instancia != null)
        {
            GameManager.instancia.PerderPartida();
            puedeMorir = false;
        }
    }
}