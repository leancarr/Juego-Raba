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

        // Ahora sí, pasamos a la pantalla de carga asincrónica que armamos antes
        SceneManager.LoadScene("PantallaCarga");
    }
}