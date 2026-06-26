using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SeleccionMultijugador : MonoBehaviour
{
    [Header("Reflectores Frontales")]
    public Light reflectorP1;
    public Light reflectorP2;

    [Header("Los Pedestales")]
    public Transform[] posiciones;

    [Header("Los Carteles de INFO (World Space)")]
    public CartelPersonaje[] carteles;

    [Header("UI - Carteles de LISTO (World Space)")]
    // Ahora pedimos Transforms para moverlos en el espacio 3D
    public Transform transformListoP1; // Arrastrar "Cartel_Listo_P1"
    public Transform transformListoP2; // Arrastrar "Cartel_Listo_P2"

    [Header("Configuración de Posición LISTO")]
    // Nos permite ajustar qué tan arriba/adelante sale el cartel "Listo" del personaje
    public Vector3 offsetListo = new Vector3(0, 2.5f, -0.5f);

    // Cuántos metros se moverá el cartel hacia el centro si eligen los costados
    [Tooltip("Empuja los carteles de los costados hacia el centro para que no se corten con la pantalla")]
    public float empujeHaciaElCentro = 1.2f;

    [Header("Controles Player 1")]
    public KeyCode p1Izquierda = KeyCode.A;
    public KeyCode p1Derecha = KeyCode.D;
    public KeyCode p1Confirmar = KeyCode.Space;
    public KeyCode p1Cancelar = KeyCode.LeftShift;

    [Header("Controles Player 2")]
    public KeyCode p2Izquierda = KeyCode.LeftArrow;
    public KeyCode p2Derecha = KeyCode.RightArrow;
    public KeyCode p2Confirmar = KeyCode.Return;
    public KeyCode p2Cancelar = KeyCode.RightShift;

    private int indiceP1 = 0;
    private int indiceP2 = 0;

    private bool p1Listo = false;
    private bool p2Listo = false;
    private bool cargandoEscena = false;

    void Start()
    {
        if (posiciones.Length > 1) indiceP2 = posiciones.Length - 1;

        // Apagamos los carteles de Listo al iniciar
        if (transformListoP1 != null) transformListoP1.gameObject.SetActive(false);
        if (transformListoP2 != null) transformListoP2.gameObject.SetActive(false);

        ActualizarReflector(reflectorP1, indiceP1);
        ActualizarReflector(reflectorP2, indiceP2);
        ActualizarEntorno();
    }

    void Update()
    {
        // --- PLAYER 1 ---
        if (!p1Listo)
        {
            if (Input.GetKeyDown(p1Derecha)) MoverP1(1);
            if (Input.GetKeyDown(p1Izquierda)) MoverP1(-1);

            if (Input.GetKeyDown(p1Confirmar))
            {
                p1Listo = true;
                ConfirmarP1(); // ¡NUEVO FNC!
                ChequearArranque();
            }
        }
        else
        {
            if (Input.GetKeyDown(p1Cancelar))
            {
                p1Listo = false;
                if (transformListoP1 != null) transformListoP1.gameObject.SetActive(false);
                cargandoEscena = false;
            }
        }

        // --- PLAYER 2 ---
        if (!p2Listo)
        {
            if (Input.GetKeyDown(p2Derecha)) MoverP2(1);
            if (Input.GetKeyDown(p2Izquierda)) MoverP2(-1);

            if (Input.GetKeyDown(p2Confirmar))
            {
                p2Listo = true;
                ConfirmarP2(); // ¡NUEVO FNC!
                ChequearArranque();
            }
        }
        else
        {
            if (Input.GetKeyDown(p2Cancelar))
            {
                p2Listo = false;
                if (transformListoP2 != null) transformListoP2.gameObject.SetActive(false);
                cargandoEscena = false;
            }
        }
    }

    void ConfirmarP1()
    {
        PlayerPrefs.SetInt("EleccionP1", indiceP1);
        PlayerPrefs.Save();

        if (transformListoP1 != null && posiciones.Length > 0)
        {
            Vector3 posicionFinal = posiciones[indiceP1].position + offsetListo;

            // Si es IZQUIERDA (0), restamos para empujar hacia el centro
            if (indiceP1 == 0) posicionFinal.x -= empujeHaciaElCentro;

            // Si es DERECHA (2), sumamos para empujar hacia el centro
            else if (indiceP1 == posiciones.Length - 1) posicionFinal.x += empujeHaciaElCentro;

            transformListoP1.position = posicionFinal;
            transformListoP1.gameObject.SetActive(true);
        }
    }

    void ConfirmarP2()
    {
        PlayerPrefs.SetInt("EleccionP2", indiceP2);
        PlayerPrefs.Save();

        if (transformListoP2 != null && posiciones.Length > 0)
        {
            Vector3 posicionFinal = posiciones[indiceP2].position + offsetListo;

            // ¡CORREGIDO! Ahora usa el indiceP2
            if (indiceP2 == 0) posicionFinal.x -= empujeHaciaElCentro;

            else if (indiceP2 == posiciones.Length - 1) posicionFinal.x += empujeHaciaElCentro;

            transformListoP2.position = posicionFinal;
            transformListoP2.gameObject.SetActive(true);
        }
    }

    void MoverP1(int direccion)
    {
        indiceP1 += direccion;
        if (indiceP1 >= posiciones.Length) indiceP1 = 0;
        if (indiceP1 < 0) indiceP1 = posiciones.Length - 1;
        ActualizarReflector(reflectorP1, indiceP1);
        ActualizarEntorno();
    }

    void MoverP2(int direccion)
    {
        indiceP2 += direccion;
        if (indiceP2 >= posiciones.Length) indiceP2 = 0;
        if (indiceP2 < 0) indiceP2 = posiciones.Length - 1;
        ActualizarReflector(reflectorP2, indiceP2);
        ActualizarEntorno();
    }

    void ActualizarReflector(Light reflector, int indice)
    {
        if (reflector != null && posiciones.Length > 0)
        {
            Vector3 posicionLuz = reflector.transform.position;
            posicionLuz.x = posiciones[indice].position.x;
            reflector.transform.position = posicionLuz;
        }
    }

    void ActualizarEntorno()
    {
        for (int i = 0; i < posiciones.Length; i++)
        {
            if (i < carteles.Length && carteles[i] != null)
                carteles[i].MostrarCartel(false);

            if (posiciones[i] != null)
            {
                ReaccionPersonaje reaccion = posiciones[i].GetComponentInChildren<ReaccionPersonaje>();
                if (reaccion != null) reaccion.CambiarEstado(false);
            }
        }

        if (indiceP1 < carteles.Length && carteles[indiceP1] != null) carteles[indiceP1].MostrarCartel(true);
        if (indiceP1 < posiciones.Length && posiciones[indiceP1] != null)
        {
            ReaccionPersonaje reaccion = posiciones[indiceP1].GetComponentInChildren<ReaccionPersonaje>();
            if (reaccion != null) reaccion.CambiarEstado(true);
        }

        if (indiceP2 < carteles.Length && carteles[indiceP2] != null) carteles[indiceP2].MostrarCartel(true);
        if (indiceP2 < posiciones.Length && posiciones[indiceP2] != null)
        {
            ReaccionPersonaje reaccion = posiciones[indiceP2].GetComponentInChildren<ReaccionPersonaje>();
            if (reaccion != null) reaccion.CambiarEstado(true);
        }
    }

    void ChequearArranque()
    {
        if (p1Listo && p2Listo && !cargandoEscena)
        {
            StartCoroutine(CargarNivelConRetraso());
        }
    }

    IEnumerator CargarNivelConRetraso()
    {
        cargandoEscena = true;
        yield return new WaitForSeconds(1.5f);

        if (p1Listo && p2Listo)
        {
            SceneManager.LoadScene("PantallaCarga");
        }
        else
        {
            cargandoEscena = false;
        }
    }
}