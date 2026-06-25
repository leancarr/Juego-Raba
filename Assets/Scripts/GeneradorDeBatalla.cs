/* 
 * ==============================================================================
 * SCRIPT: GeneradorDeBatalla.cs
 * CATEGORIA: 2. Core y Managers (Gestores Invisibles)
 * DESCRIPCION: Se ejecuta al principio de cada Nivel. Lee la memoria (PlayerPrefs) para saber que eligieron los jugadores en el menu y los hace aparecer (Instantiate) magicamente en sus puntos de aparicion, asignandoles sus controles respectivos.
 * ==============================================================================
 */
using UnityEngine;

public class GeneradorDeBatalla : MonoBehaviour
{
    [Header("Roster de Personajes Jugables")]
    [Tooltip("Â¡IMPORTANTE! Arrastra aquÃ­ los PREFABS REALES (con fÃ­sicas y controles) en el mismo orden que el menÃº.")]
    public GameObject[] personajesPrefabs;

    [Header("Puntos de ApariciÃ³n (Spawn Points)")]
    public Transform puntoInicioP1;
    public Transform puntoInicioP2;

    void Start()
    {
        // Al arrancar la escena, llamamos a la funciÃ³n que crea a los luchadores
        GenerarJugadores();
    }

    void GenerarJugadores()
    {
        // 1. Leemos la memoria. Si por algÃºn error no hay datos, por defecto elegirÃ¡ el 0
        int eleccionP1 = PlayerPrefs.GetInt("EleccionP1", 0);
        int eleccionP2 = PlayerPrefs.GetInt("EleccionP2", 0);

        // Seguridad: Evitar que el juego explote si olvidaste arrastrar los prefabs
        if (personajesPrefabs.Length == 0)
        {
            Debug.LogError("Â¡No pusiste personajes en la lista del GeneradorDeBatalla!");
            return;
        }

        // 2. Hacer aparecer al Player 1
        if (eleccionP1 >= 0 && eleccionP1 < personajesPrefabs.Length)
        {
            GameObject jugador1 = Instantiate(personajesPrefabs[eleccionP1], puntoInicioP1.position, puntoInicioP1.rotation);
            jugador1.name = "Player 1";
            jugador1.SetActive(true);

            // [NUEVO] Le avisamos que es el P1
            jugador1.GetComponent<MovimientoBasico25D>().ConfigurarControles(1);
            jugador1.GetComponent<HabilidadesJugador>().ConfigurarHabilidades(1);
            jugador1.GetComponent<AccionEmpuje>().ConfigurarControlesEmpuje(1);
        }

        // 3. Hacer aparecer al Player 2
        if (eleccionP2 >= 0 && eleccionP2 < personajesPrefabs.Length)
        {
            GameObject jugador2 = Instantiate(personajesPrefabs[eleccionP2], puntoInicioP2.position, puntoInicioP2.rotation);
            jugador2.name = "Player 2";
            jugador2.SetActive(true);

            // [NUEVO] Le avisamos que es el P2
            jugador2.GetComponent<MovimientoBasico25D>().ConfigurarControles(2);
            jugador2.GetComponent<HabilidadesJugador>().ConfigurarHabilidades(2);
            jugador2.GetComponent<AccionEmpuje>().ConfigurarControlesEmpuje(2);
        }
    }
}
