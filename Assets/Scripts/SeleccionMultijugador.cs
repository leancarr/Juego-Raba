using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionMultijugador : MonoBehaviour
{
    [Header("Reflectores Frontales")]
    public Light reflectorP1;
    public Light reflectorP2;

    [Header("Los Pedestales")]
    public Transform[] posiciones;

    [Header("Los Carteles (World Space)")]
    public CartelPersonaje[] carteles;

    [Header("Controles Player 1")]
    public KeyCode p1Izquierda = KeyCode.A;
    public KeyCode p1Derecha = KeyCode.D;
    public KeyCode p1Confirmar = KeyCode.Space;

    [Header("Controles Player 2")]
    public KeyCode p2Izquierda = KeyCode.LeftArrow;
    public KeyCode p2Derecha = KeyCode.RightArrow;
    public KeyCode p2Confirmar = KeyCode.Return;

    private int indiceP1 = 0;
    private int indiceP2 = 0;

    private bool p1Listo = false;
    private bool p2Listo = false;

    void Start()
    {
        if (posiciones.Length > 1) indiceP2 = posiciones.Length - 1;

        ActualizarReflector(reflectorP1, indiceP1);
        ActualizarReflector(reflectorP2, indiceP2);

        // ¡NUEVO! Actualizamos luces, carteles y giros al arrancar
        ActualizarEntorno();
    }

    void Update()
    {
        if (!p1Listo)
        {
            if (Input.GetKeyDown(p1Derecha)) MoverP1(1);
            if (Input.GetKeyDown(p1Izquierda)) MoverP1(-1);

            if (Input.GetKeyDown(p1Confirmar))
            {
                p1Listo = true;
                PlayerPrefs.SetInt("EleccionP1", indiceP1);
                ChequearArranque();
            }
        }

        if (!p2Listo)
        {
            if (Input.GetKeyDown(p2Derecha)) MoverP2(1);
            if (Input.GetKeyDown(p2Izquierda)) MoverP2(-1);

            if (Input.GetKeyDown(p2Confirmar))
            {
                p2Listo = true;
                PlayerPrefs.SetInt("EleccionP2", indiceP2);
                ChequearArranque();
            }
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

    // ¡La función maestra que controla TODO!
    void ActualizarEntorno()
    {
        // 1. Apagamos TODOS los carteles y hacemos girar a TODOS los personajes
        for (int i = 0; i < posiciones.Length; i++)
        {
            if (i < carteles.Length && carteles[i] != null)
                carteles[i].MostrarCartel(false);

            if (posiciones[i] != null)
            {
                // Busca automáticamente el script en el modelo 3D adentro del cilindro
                ReaccionPersonaje reaccion = posiciones[i].GetComponentInChildren<ReaccionPersonaje>();
                if (reaccion != null) reaccion.CambiarEstado(false);
            }
        }

        // 2. Encendemos el cartel y frenamos al personaje del P1
        if (indiceP1 < carteles.Length && carteles[indiceP1] != null) carteles[indiceP1].MostrarCartel(true);
        if (indiceP1 < posiciones.Length && posiciones[indiceP1] != null)
        {
            ReaccionPersonaje reaccion = posiciones[indiceP1].GetComponentInChildren<ReaccionPersonaje>();
            if (reaccion != null) reaccion.CambiarEstado(true);
        }

        // 3. Encendemos el cartel y frenamos al personaje del P2 
        if (indiceP2 < carteles.Length && carteles[indiceP2] != null) carteles[indiceP2].MostrarCartel(true);
        if (indiceP2 < posiciones.Length && posiciones[indiceP2] != null)
        {
            ReaccionPersonaje reaccion = posiciones[indiceP2].GetComponentInChildren<ReaccionPersonaje>();
            if (reaccion != null) reaccion.CambiarEstado(true);
        }
    }

    void ChequearArranque()
    {
        if (p1Listo && p2Listo)
        {
            SceneManager.LoadScene("PantallaCarga");
        }
    }
}