/* 
 * ==============================================================================
 * SCRIPT: MenuPrincipalManager.cs
 * CATEGORIA: 4. Menues y UI (Interfaz)
 * DESCRIPCION: Controlador de navegacion puro (apretar boton -> cargar la otra escena).
 * ==============================================================================
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    // Ahora el botón de jugar te manda a elegir las rondas
    public void IrA_SeleccionRondas()
    {
        SceneManager.LoadScene("SeleccionRondas");
    }

    // El botón de salir cierra el juego por completo
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego de forma segura...");
        Application.Quit(); // Cierra el .exe compilado
    }
}
