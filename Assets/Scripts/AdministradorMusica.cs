/* 
 * ==============================================================================
 * SCRIPT: AdministradorMusica.cs
 * CATEGORIA: 2. Core y Managers (Gestores Invisibles)
 * DESCRIPCION: Script global inmortal (DontDestroyOnLoad). Viaja entre todas las escenas sin borrarse para que la musica sea fluida y decide que cancion poner segun el nombre del nivel.
 * ==============================================================================
 */
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AdministradorMusica : MonoBehaviour
{
    // --- PATRÃ“N SINGLETON ---
    public static AdministradorMusica instancia;

    [Header("Pistas de Audio")]
    public AudioClip musicaMenus;
    public AudioClip musicaNiveles;

    private AudioSource audioSource;

    void Awake()
    {
        // Si ya existe otro AdministradorMusica, destruimos este nuevo para no tener mÃºsica doble
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        // Si somos el primero, nos guardamos como la instancia oficial
        instancia = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // La música siempre debe repetirse
        Debug.Log("<color=yellow>[AUDIO] AdministradorMusica AWAKE. Instancia oficial creada. AudioSource encontrado: " + (audioSource != null) + "</color>");

        // Hacemos que este objeto NO se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("<color=yellow>[AUDIO] AdministradorMusica START. Escena actual: " + SceneManager.GetActiveScene().name + "</color>");
        // Forzamos la validación de música para la escena actual al darle Play
        // (porque sceneLoaded no se dispara para la primera escena que abrimos)
        if (instancia == this)
        {
            ValidarMusicaPorEscena(SceneManager.GetActiveScene().name);
        }
    }

    void OnEnable()
    {
        // Nos suscribimos al evento de cambio de escena
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        ValidarMusicaPorEscena(escena.name);
    }

    void ValidarMusicaPorEscena(string nombreOriginal)
    {
        string nombreEscena = nombreOriginal.ToLower();
        Debug.Log("<color=yellow>[AUDIO] Validando música para escena: " + nombreEscena + "</color>");

        if (nombreEscena.Contains("menu") || nombreEscena.Contains("seleccion") || nombreEscena.Contains("victoria"))
        {
            CambiarMusica(musicaMenus);
        }
        else if (nombreEscena.Contains("nivel"))
        {
            CambiarMusica(musicaNiveles);
        }
        else
        {
            Debug.LogWarning("<color=orange>[AUDIO] La escena '" + nombreEscena + "' no tiene 'menu' ni 'nivel' en el nombre. No se cambió la música.</color>");
        }
    }

    void CambiarMusica(AudioClip nuevaMusica)
    {
        // Si no hay música asignada, no hacemos nada
        if (nuevaMusica == null)
        {
            Debug.LogError("<color=red>[AUDIO] ¡ERROR! Se intentó cambiar de música pero el AudioClip estaba VACÍO (NULL) en el Inspector.</color>");
            return;
        }

        // Si YA está sonando esta misma pista, no la reiniciamos
        if (audioSource.clip == nuevaMusica)
        {
            Debug.Log("<color=yellow>[AUDIO] La pista " + nuevaMusica.name + " ya está sonando. No se reinicia.</color>");
            return;
        }

        // Cambiamos y reproducimos
        audioSource.clip = nuevaMusica;
        audioSource.Play();
        Debug.Log("<color=green>[AUDIO] Reproduciendo exitosamente nueva pista: " + nuevaMusica.name + ". Volumen actual del Listener: " + AudioListener.volume + "</color>");
    }
}

