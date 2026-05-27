using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paredmortal : MonoBehaviour
{
    // Cambiamos a OnTriggerEnter para que la muerte sea fluida y no rebote
    void OnTriggerEnter(Collider otro)
    {
        // 1. Chequeamos si lo que tocó el borde de la pantalla tiene el Tag de jugador
        if (otro.CompareTag("Player"))
        {
            // 2. Buscamos el componente 'spawner' que tiene pegado ese jugador en particular
            spawner scriptSpawner = otro.GetComponent<spawner>();

            if (scriptSpawner != null)
            {
                Debug.Log("¡" + otro.gameObject.name + " fue devorado por el borde de la pantalla! Respawneando...");

                // 3. Le ordenamos al spawner de ESTE jugador que lo mande a su base y le reste vida
                scriptSpawner.Invoke("EjecutarRespawn", 0f);
            }
            else
            {
                // Plan B por si te olvidaste de pegarle el script spawner al jugador
                Debug.LogWarning("Ojo: " + otro.gameObject.name + " tocó la pared pero no tiene el script spawner.cs");
            }
        }
    }
}