using UnityEngine;

public class paredmortal : MonoBehaviour
{
    void OnTriggerEnter(Collider otro)
    {
        // Si lo que tocó el borde es el jugador
        if (otro.CompareTag("Player"))
        {
            // Buscamos el script spawner que ahora maneja la muerte directa
            spawner scriptSpawner = otro.GetComponent<spawner>();

            if (scriptSpawner != null)
            {
                // En vez de un Invoke diferido, lo matamos al instante
                scriptSpawner.MuerteDefinitiva();
            }
            else
            {
                Debug.LogWarning("Ojo: " + otro.gameObject.name + " tocó la pared mortal pero no tiene el script spawner.cs");
            }
        }
    }
}