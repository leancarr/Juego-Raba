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

    public void MuerteDefinitiva()
    {
        if (!puedeMorir) return;

        Debug.Log("¡" + gameObject.name + " murio! Haciendo ganar al rival...");

        if (GameManager.instancia != null)
        {
            puedeMorir = false;

            // Buscamos al rival en la escena.
            // Si este objeto se llama "Personaje", el rival podria llamarse "Personaje2" (o el nombre que tenga el tuyo)
            string nombreRival = (gameObject.name == "Personaje") ? "Player2" : "Personaje";
            GameObject rival = GameObject.Find(nombreRival);

            if (rival != null)
            {
                // Le avisamos al GameManager que gane el rival que quedo vivo
                GameManager.instancia.GanarPartida(rival);
            }
            else
            {
                // Plan B: Si por alguna razon no encuentra al rival con ese nombre, 
                // ejecuta la victoria comun sobre el que este para que no se trabe el juego
                GameManager.instancia.GanarPartida(rival);
            }
        }
    }
}