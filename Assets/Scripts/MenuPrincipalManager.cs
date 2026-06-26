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
    [Header("Referencias UI")]
    public GameObject panelOpciones;

    public void IrA_SeleccionRondas()
    {
        SceneManager.LoadScene("SeleccionRondas");
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego de forma segura...");
        Application.Quit();
    }

    public void AbrirOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(true);
            panelOpciones.transform.SetAsLastSibling();
        }
    }

    public void CerrarOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(false);
        }
    }
}
