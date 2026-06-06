using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionMultijugador : MonoBehaviour
{
    [Header("Reflectores (Tus Spotlights frontales)")]
    public Light reflectorP1; // Arrastra la luz Azul
    public Light reflectorP2; // Arrastra la luz Roja

    [Header("Los Pedestales")]
    public Transform[] posiciones; // Arrastra aquí los cilindros en orden (Izquierda, Medio, Derecha)

    [Header("Controles Player 1")]
    public KeyCode p1Izquierda = KeyCode.A;
    public KeyCode p1Derecha = KeyCode.D;
    public KeyCode p1Confirmar = KeyCode.Space;

    [Header("Controles Player 2")]
    public KeyCode p2Izquierda = KeyCode.LeftArrow;
    public KeyCode p2Derecha = KeyCode.RightArrow;
    public KeyCode p2Confirmar = KeyCode.Return; // Enter

    private int indiceP1 = 0;
    private int indiceP2 = 0;

    private bool p1Listo = false;
    private bool p2Listo = false;

    void Start()
    {
        // Hacemos que el Player 2 arranque en el último personaje para que no se superpongan
        if (posiciones.Length > 1)
        {
            indiceP2 = posiciones.Length - 1;
        }

        ActualizarReflector(reflectorP1, indiceP1);
        ActualizarReflector(reflectorP2, indiceP2);
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
                Debug.Log("P1 Confirmado en opción: " + indiceP1);
                ChequearArranque();
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
                Debug.Log("P2 Confirmado en opción: " + indiceP2);
                ChequearArranque();
            }
        }
    }

    void MoverP1(int direccion)
    {
        indiceP1 += direccion;
        // Si se pasa de los bordes, da la vuelta (como un menú de Arcade)
        if (indiceP1 >= posiciones.Length) indiceP1 = 0;
        if (indiceP1 < 0) indiceP1 = posiciones.Length - 1;

        ActualizarReflector(reflectorP1, indiceP1);
    }

    void MoverP2(int direccion)
    {
        indiceP2 += direccion;
        if (indiceP2 >= posiciones.Length) indiceP2 = 0;
        if (indiceP2 < 0) indiceP2 = posiciones.Length - 1;

        ActualizarReflector(reflectorP2, indiceP2);
    }

    void ActualizarReflector(Light reflector, int indice)
    {
        if (reflector != null && posiciones.Length > 0)
        {
            // Tomamos la posición de donde dejaste armada la luz manualmente
            Vector3 posicionLuz = reflector.transform.position;

            // SOLO cambiamos su eje X para alinearla con el nuevo pedestal.
            // Así respetamos la altura y distancia frontal que le diste en Unity.
            posicionLuz.x = posiciones[indice].position.x;

            reflector.transform.position = posicionLuz;
        }
    }

    void ChequearArranque()
    {
        if (p1Listo && p2Listo)
        {
            Debug.Log("¡Ambos jugadores están listos! Cargando el nivel...");

            // Reemplaza "PantallaCarga" por el nombre exacto de la escena que sigue
            SceneManager.LoadScene("PantallaCarga");
        }
    }
}