/* 
 * ==============================================================================
 * SCRIPT: CartelPersonaje.cs
 * CATEGORIA: 4. Menues y UI (Interfaz)
 * DESCRIPCION: Controla la interfaz visual de las cartas o nombres de los personajes en la pantalla de seleccion.
 * ==============================================================================
 */
using UnityEngine;

public class CartelPersonaje : MonoBehaviour
{
    // Esta funciÃ³n la llamarÃ¡ el Manager
    public void MostrarCartel(bool mostrar)
    {
        // Enciende o apaga todo el objeto Canvas y sus hijos (Panel y Texto)
        gameObject.SetActive(mostrar);
    }
}
