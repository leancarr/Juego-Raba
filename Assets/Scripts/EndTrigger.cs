/* 
 * ==============================================================================
 * SCRIPT: EndTrigger.cs
 * CATEGORIA: 5. Elementos del Nivel (Efectos y Triggers)
 * DESCRIPCION: La linea de meta del nivel lineal. Quien la toque gana la ronda.
 * ==============================================================================
 */
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    // Al usar OnTriggerEnter, Unity detecta la superposición sin frenar el paso del jugador
    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el objeto que entró al cubo tiene el Tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El jugador cruzó la meta!");

            // Le avisamos al GameManager que ejecute la lógica de victoria
            if (GameManager.instancia != null)
            {
                GameManager.instancia.GanarPartida(other.gameObject);
            }
            else
            {
                Debug.LogWarning("Ojo: No se encontró ningún GameManager en la escena.");
            }
        }
    }
}
