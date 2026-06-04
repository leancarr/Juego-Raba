using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionRondasManager : MonoBehaviour
{
    // Esta función la van a compartir todos los botones de rondas
    public void SeleccionarCantidadRondas(int cantidad)
    {
        // Guardamos el número de rondas en la memoria con la clave "RondasElegidas"
        PlayerPrefs.SetInt("RondasElegidas", cantidad);
        PlayerPrefs.Save(); // Asegura que se escriba en el disco

        Debug.Log("Rondas guardadas en memoria: " + cantidad);

        // Cambiamos "PantallaCarga" por el nombre EXACTO de tu escena de personajes:
        SceneManager.LoadScene("SelecciondePersonajes"); // u el nombre exacto que le hayas puesto a tu escena
    }
}