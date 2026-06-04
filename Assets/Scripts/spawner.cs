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

        if (GameManager.instancia != null)
        {
            puedeMorir = false;

            // 🎯 NOMBRES EXACTOS: Si muere "Player 1", el rival es "Player 2". Si no, es "Player 1".
            string nombreRival = (gameObject.name == "Player 1") ? "Player 2" : "Player 1";

            GameObject rival = GameObject.Find(nombreRival);

            if (rival != null)
            {
                GameManager.instancia.GanarPartida(rival);
            }
            else
            {
                // Plan B por seguridad
                Debug.LogError("No se encontró en la escena al rival: " + nombreRival);
                GameManager.instancia.GanarPartida(gameObject);
            }
        }
    }
}