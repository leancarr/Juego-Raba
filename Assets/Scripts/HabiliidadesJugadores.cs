/* 
 * ==============================================================================
 * SCRIPT: HabiliidadesJugadores.cs
 * CATEGORIA: 1. Control del Jugador (Personajes)
 * DESCRIPCION: El nucleo de las clases/poderes. Aca esta programado el Laser del Profesor, el Muro de Cajas del Mecanico y la Onda Expansiva del Rapero.
 * ==============================================================================
 */
using UnityEngine;

public class HabilidadesJugador : MonoBehaviour
{
    public enum TipoClase { Profesor, Mecanico, Rapero }

    [Header("AnimaciÃ³n")]
    public Animator anim;
    public Transform visualCenter;

    [Header("Controles")]
    public KeyCode teclaHabilidad = KeyCode.F; // CambiÃ¡ esta tecla desde el Inspector

    // --- ESTAS VARIABLES SON PRIVADAS Y DINÃMICAS ---
    private int numeroJugador;

    // --- LA FUNCIÃ“N QUE CONFIGURA EL NÃšMERO DE JUGADOR ---
    public void ConfigurarHabilidades(int numJugador)
    {
        numeroJugador = numJugador;
        
        // Asignamos teclas distintas para que no choquen
        if (numJugador == 1)
        {
            teclaHabilidad = KeyCode.F; // Jugador 1
        }
        else if (numJugador == 2)
        {
            teclaHabilidad = KeyCode.RightShift; // Jugador 2
        }
    }

    [Header("ConfiguraciÃ³n de Clase")]
    public TipoClase clase;
    public float cooldown = 4f;
    private float tiempoSiguienteUso = 0f;

    [Header("AnimaciÃ³n del Poder")]
    public float duracionAnimEspecial = 3f; // DuraciÃ³n de la animaciÃ³n de tu poder en segundos
    private bool estaCantando = false;

    [Header("Habilidad MecÃ¡nico")]
    public GameObject prefabBloqueMecanico;

    [Header("Habilidad Profesor (Raycast Inteligente)")]
    public float radioMaximoEscaneo = 7f;
    public LayerMask capaObstaculosYPlataformas;
    public LineRenderer efectoVisualLaser;

    [Header("Habilidad Rapero")]
    public float radioEmpuje = 5f;
    public float fuerzaEmpuje = 12f;
    public float duracionStunRival = 0.5f;
    public GameObject visualOndaRapero;

    void Update()
    {
        if (Time.timeScale == 0f) return; // Juego pausado

        if (Input.GetKeyDown(teclaHabilidad))
        {
            Debug.Log($"[RASTREO] Tecla {teclaHabilidad} apretada por Player {numeroJugador}. Clase: {clase}");

            if (Time.time >= tiempoSiguienteUso)
            {
                UsarHabilidad();
                tiempoSiguienteUso = Time.time + cooldown;
            }
            else
            {
                Debug.Log($"Jugador {numeroJugador}: Habilidad en Cooldown. Faltan {tiempoSiguienteUso - Time.time:F1}s");
            }
        }
    }

    void TerminarAnimEspecial()
    {
        estaCantando = false;
    }

    void UsarHabilidad()
    {
        Debug.Log($"Usando hab. {clase}. anim asignado? {anim != null}. ¿Qué objeto lo tiene? {(anim != null ? anim.gameObject.name : "Nadie")}");

        // --- Activa la animación del poder al mismo tiempo que la habilidad ---
        if (anim != null && !estaCantando)
        {
            estaCantando = true; // Actúa como bloqueo para no spamear la habilidad
            
            // Disparar el Trigger correcto según la clase
            if (clase == TipoClase.Rapero) 
            {
                anim.SetTrigger("CantarTrigger");
            }
            else if (clase == TipoClase.Mecanico) 
            {
                Debug.Log("¡Enviando Trigger MecanicoPushTrigger al Animator!");
                anim.SetTrigger("MecanicoPushTrigger"); // <--- Nuevo Trigger para el Mecánico
            }
            // (El Profesor ya maneja su propio "PulsoTrigger" dentro de HabilidadProfesor)

            Invoke("TerminarAnimEspecial", duracionAnimEspecial);
        }
        else if (anim == null)
        {
            Debug.LogError("El Animator (anim) no está asignado en el script, aunque la pared sí aparece.");
        }

        switch (clase)
        {
            case TipoClase.Mecanico:
                HabilidadMecanico();
                break;
            case TipoClase.Profesor:
                HabilidadProfesor();
                break;
            case TipoClase.Rapero:
                HabilidadRapero();
                break;
        }
    }

    void HabilidadMecanico()
    {
        if (prefabBloqueMecanico != null)
        {
            Vector3 direccionSpawn = Vector3.right;
            if (transform.childCount > 0)
            {
                Transform centroVisual = transform.GetChild(0);
                if (centroVisual.localEulerAngles.y > 100f || centroVisual.localEulerAngles.y < -100f)
                {
                    direccionSpawn = Vector3.left;
                }
            }
            Vector3 posicionSpawn = transform.position + direccionSpawn * 1.5f;
            Instantiate(prefabBloqueMecanico, posicionSpawn, Quaternion.identity);
        }
    }

    void HabilidadProfesor()
    {
        float facingX = 1f;
        bool isFacingRight = true;
        if (visualCenter != null)
        {
            if (visualCenter.localEulerAngles.y > 100f || visualCenter.localEulerAngles.y < -100f)
            {
                facingX = -1f;
                isFacingRight = false;
            }
        }

        Transform plataformaActual = null;
        RaycastHit hitSuelo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitSuelo, 2f))
        {
            if (hitSuelo.collider.GetComponent<PlataformasColores>() != null)
            {
                plataformaActual = hitSuelo.collider.transform;
            }
        }

        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioMaximoEscaneo);
        PlataformasColores plataformaObjetivo = null;
        float distanciaMinima = Mathf.Infinity;
        Vector3 centroJugador = new Vector3(transform.position.x, transform.position.y + 0.5f, 0f);

        foreach (Collider col in objetosCercanos)
        {
            PlataformasColores scriptPlataforma = col.GetComponent<PlataformasColores>();
            if (scriptPlataforma != null && col.transform != plataformaActual)
            {
                Vector3 puntoMasCercano = col.ClosestPoint(centroJugador);
                float distancia = Vector3.Distance(centroJugador, puntoMasCercano);

                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    plataformaObjetivo = scriptPlataforma;
                }
            }
        }

        if (plataformaObjetivo != null && visualCenter != null)
        {
            Collider coliderPlataforma = plataformaObjetivo.GetComponent<Collider>();
            Vector3 targetPoint = coliderPlataforma != null ? coliderPlataforma.ClosestPoint(centroJugador) : plataformaObjetivo.transform.position;
            targetPoint.z = 0f;

            float directionToTargetWorldX = Mathf.Sign(targetPoint.x - transform.position.x);

            if (Mathf.Abs(targetPoint.x - transform.position.x) > 0.2f)
            {
                if (directionToTargetWorldX > 0 && !isFacingRight)
                {
                    visualCenter.localEulerAngles = new Vector3(0f, 0f, 0f);
                    facingX = 1f;
                }
                else if (directionToTargetWorldX < 0 && isFacingRight)
                {
                    visualCenter.localEulerAngles = new Vector3(0f, 180f, 0f);
                    facingX = -1f;
                }
            }

            Vector3 origenLaser = new Vector3(transform.position.x + (0.4f * facingX), transform.position.y + 1f, 0f);
            Vector3 puntoFinalLaser = coliderPlataforma != null ? coliderPlataforma.ClosestPoint(origenLaser) : plataformaObjetivo.transform.position;
            puntoFinalLaser.z = 0f;

            if (anim != null)
            {
                float diffX = Mathf.Abs(puntoFinalLaser.x - origenLaser.x);
                float diffY = puntoFinalLaser.y - origenLaser.y;

                float anguloFinal = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
                anguloFinal = Mathf.Clamp(anguloFinal, -90f, 90f);

                anim.SetFloat("AnguloApuntado", anguloFinal);
                anim.SetTrigger("PulsoTrigger");
            }

            Debug.DrawLine(origenLaser, puntoFinalLaser, Color.magenta, 1f);
            plataformaObjetivo.SabotearBando();
            Debug.LogWarning($"[PROFESOR] Sabotaje simÃ©trico y giro automÃ¡tico exitosos. Ãngulo Local enviado: {anim.GetFloat("AnguloApuntado")}");

            if (efectoVisualLaser != null)
            {
                StartCoroutine(MostrarLaserVisual(origenLaser, puntoFinalLaser));
            }
        }
    }

    void HabilidadRapero()
    {
        Debug.Log($"Jugador {numeroJugador} ejecutando habilidad de Rapero.");

        if (visualOndaRapero != null)
        {
            Vector3 posicionOnda = new Vector3(transform.position.x, transform.position.y, 0f);
            GameObject onda = Instantiate(visualOndaRapero, posicionOnda, Quaternion.identity);
            Destroy(onda, 0.5f);
        }

        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioEmpuje);

        foreach (Collider col in objetosCercanos)
        {
            if (col.gameObject == this.gameObject) continue;

            MovimientoBasico25D scriptRival = col.GetComponent<MovimientoBasico25D>();
            if (scriptRival != null)
            {
                Vector3 direccionEmpuje = col.transform.position - transform.position;
                float direccionX = Mathf.Sign(direccionEmpuje.x);

                Vector3 fuerzaFinal = new Vector3(direccionX * fuerzaEmpuje, fuerzaEmpuje * 0.4f, 0f);
                scriptRival.AplicarEmpujeYStun(fuerzaFinal, duracionStunRival);

                Debug.LogWarning($"[RAPERO] Â¡Onda expansiva golpeÃ³ a {col.name}! Mandado a volar.");
            }
        }
    }

    System.Collections.IEnumerator MostrarLaserVisual(Vector3 inicio, Vector3 fin)
    {
        efectoVisualLaser.enabled = true;
        efectoVisualLaser.SetPosition(0, inicio);
        efectoVisualLaser.SetPosition(1, fin);
        yield return new WaitForSeconds(0.15f);
        efectoVisualLaser.enabled = false;
    }
}
