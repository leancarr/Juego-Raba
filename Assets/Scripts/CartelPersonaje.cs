using UnityEngine;

public class CartelPersonaje : MonoBehaviour
{
    // Esta función la llamará el Manager
    public void MostrarCartel(bool mostrar)
    {
        // Enciende o apaga todo el objeto Canvas y sus hijos (Panel y Texto)
        gameObject.SetActive(mostrar);
    }
}