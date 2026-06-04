using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Clave para poder controlar los botones

public class SeleccionPersonajeManager : MonoBehaviour
{
    private bool personajeSeleccionado = false;

    // Estas cajitas van a aparecer en el Inspector para que arrastres tus botones
    public Button botonConfirmar;
    public Button botonCancelar;

    void Start()
    {
        // Al empezar la pantalla, el botón Confirmar arranca desactivado
        // y el botón Cancelar arranca oculto o desactivado
        if (botonConfirmar != null) botonConfirmar.interactable = false;
        if (botonCancelar != null) botonCancelar.gameObject.SetActive(false);
    }

    // Se ejecuta al tocar el botón "PRESELECCIONAR"
    public void SeleccionarMarcus()
    {
        personajeSeleccionado = true;
        Debug.Log("Marcus Preseleccionado");

        // Activamos Confirmar y mostramos Cancelar
        if (botonConfirmar != null) botonConfirmar.interactable = true;
        if (botonCancelar != null) botonCancelar.gameObject.SetActive(true);
    }

    // Se ejecuta al tocar el botón "CANCELAR"
    public void CancelarSeleccion()
    {
        personajeSeleccionado = false;
        Debug.Log("Selección cancelada");

        // Desactivamos Confirmar y ocultamos Cancelar
        if (botonConfirmar != null) botonConfirmar.interactable = false;
        if (botonCancelar != null) botonCancelar.gameObject.SetActive(false);
    }

    // Se ejecuta al tocar el botón "CONFIRMAR"
    public void BotonListo()
    {
        if (personajeSeleccionado)
        {
            SceneManager.LoadScene("PantallaCarga");
        }
    }
}