using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AdministradorMusica : MonoBehaviour
{
    // --- PATRÓN SINGLETON ---
    public static AdministradorMusica instancia;

    [Header("Pistas de Audio")]
    public AudioClip musicaMenus;
    public AudioClip musicaNiveles;

    private AudioSource audioSource;

    void Awake()
    {
        // Si ya existe otro AdministradorMusica, destruimos este nuevo para no tener música doble
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        // Si somos el primero, nos guardamos como la instancia oficial
        instancia = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // La música siempre debe repetirse

        // Hacemos que este objeto NO se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
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
        // Revisamos el nombre de la escena para saber qué música poner
        string nombreEscena = escena.name.ToLower();

        if (nombreEscena.Contains("menu") || nombreEscena.Contains("seleccion") || nombreEscena.Contains("victoria"))
        {
            CambiarMusica(musicaMenus);
        }
        else if (nombreEscena.Contains("nivel"))
        {
            CambiarMusica(musicaNiveles);
        }
    }

    void CambiarMusica(AudioClip nuevaMusica)
    {
        // Si no hay música asignada, no hacemos nada
        if (nuevaMusica == null) return;

        // Si YA está sonando esta misma pista, no la reiniciamos (así es fluido entre niveles)
        if (audioSource.clip == nuevaMusica) return;

        // Cambiamos y reproducimos
        audioSource.clip = nuevaMusica;
        audioSource.Play();
    }
}
