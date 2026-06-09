using UnityEngine;

public class GeneradorDeBatalla : MonoBehaviour
{
    [Header("Roster de Personajes Jugables")]
    [Tooltip("¡IMPORTANTE! Arrastra aquí los PREFABS REALES (con físicas y controles) en el mismo orden que el menú.")]
    public GameObject[] personajesPrefabs;

    [Header("Puntos de Aparición (Spawn Points)")]
    public Transform puntoInicioP1;
    public Transform puntoInicioP2;

    void Start()
    {
        // Al arrancar la escena, llamamos a la función que crea a los luchadores
        GenerarJugadores();
    }

    void GenerarJugadores()
    {
        // 1. Leemos la memoria. Si por algún error no hay datos, por defecto elegirá el 0
        int eleccionP1 = PlayerPrefs.GetInt("EleccionP1", 0);
        int eleccionP2 = PlayerPrefs.GetInt("EleccionP2", 0);

        // Seguridad: Evitar que el juego explote si olvidaste arrastrar los prefabs
        if (personajesPrefabs.Length == 0)
        {
            Debug.LogError("¡No pusiste personajes en la lista del GeneradorDeBatalla!");
            return;
        }

        // 2. Hacer aparecer (Instanciar) al Player 1
        if (eleccionP1 >= 0 && eleccionP1 < personajesPrefabs.Length)
        {
            // Crea una copia del Prefab en la posición y rotación del Punto 1
            GameObject jugador1 = Instantiate(personajesPrefabs[eleccionP1], puntoInicioP1.position, puntoInicioP1.rotation);
            jugador1.name = "Jugador_1"; // Le limpiamos el nombre para que quede prolijo
        }

        // 3. Hacer aparecer (Instanciar) al Player 2
        if (eleccionP2 >= 0 && eleccionP2 < personajesPrefabs.Length)
        {
            // Crea una copia del Prefab en la posición y rotación del Punto 2
            GameObject jugador2 = Instantiate(personajesPrefabs[eleccionP2], puntoInicioP2.position, puntoInicioP2.rotation);
            jugador2.name = "Jugador_2";
        }
    }
}