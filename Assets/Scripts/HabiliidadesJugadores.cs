using UnityEngine;

public class HabilidadesJugador : MonoBehaviour
{
    public enum TipoClase { Profesor, Mecanico, Rapero }

    [Header("Identificación del Jugador")]
    public int numeroJugador = 1;

    [Header("Configuración de Clase")]
    public TipoClase clase;
    public KeyCode teclaHabilidad = KeyCode.E;
    public float cooldown = 4f;
    private float tiempoSiguienteUso = 0f;

    [Header("Habilidad Mecánico")]
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
        if (Input.GetKeyDown(teclaHabilidad))
        {
            // FIX: Saqué la variable que no existía. Esto va a avisar limpio en consola.
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

    void UsarHabilidad()
    {
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
        float direccionX = 1f;
        if (transform.childCount > 0)
        {
            Transform centroVisual = transform.GetChild(0);
            if (centroVisual.localEulerAngles.y > 100f || centroVisual.localEulerAngles.y < -100f)
            {
                direccionX = -1f;
            }
        }

        Vector3 direccionDisparo = new Vector3(direccionX, 0, 0);
        // Forzamos el carril Z a 0 para que impacte de lleno en las plataformas
        Vector3 origenLaser = new Vector3(transform.position.x, transform.position.y + 1f, 0f);
        Vector3 puntoFinalLaser = origenLaser + direccionDisparo * radioMaximoEscaneo;

        Debug.DrawRay(origenLaser, direccionDisparo * radioMaximoEscaneo, Color.green, 2f);

        RaycastHit hit;
        if (Physics.Raycast(origenLaser, direccionDisparo, out hit, radioMaximoEscaneo))
        {
            if (hit.collider != null)
            {
                puntoFinalLaser = hit.point;
                puntoFinalLaser.z = 0f; // Mantener el láser visual en el plano 2.5D

                // 1. Intentamos obtener tu script específico de la plataforma
                PlataformasColores scriptPlataforma = hit.collider.GetComponent<PlataformasColores>();

                // 2. Si el objeto tiene el script y no es un jugador, ejecutamos tu función lógica
                if (scriptPlataforma != null && hit.collider.GetComponent<MovimientoBasico25D>() == null)
                {
                    // EJECUTAMOS TU FUNCIÓN OFICIAL: cambia el enum y el material al mismo tiempo
                    scriptPlataforma.SabotearBando();

                    Debug.LogWarning($"[PROFESOR] ¡Láser interactuó con {hit.collider.name} y ejecutó SabotearBando()!");
                }
            }
        }

        if (efectoVisualLaser != null)
        {
            StartCoroutine(MostrarLaserVisual(origenLaser, puntoFinalLaser));
        }
    }
    void HabilidadRapero()
    {
        Debug.Log($"Jugador {numeroJugador} ejecutando habilidad de Rapero.");

        // 1. RESPUESTA VISUAL
        if (visualOndaRapero != null)
        {
            Vector3 posicionOnda = new Vector3(transform.position.x, transform.position.y, 0f);
            GameObject onda = Instantiate(visualOndaRapero, posicionOnda, Quaternion.identity);
            Destroy(onda, 0.5f);
        }

        // 2. DETECCIÓN EN ÁREA
        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioEmpuje);

        foreach (Collider col in objetosCercanos)
        {
            if (col.gameObject == this.gameObject) continue;

            // 3. EFECTO SOBRE EL RIVAL
            MovimientoBasico25D scriptRival = col.GetComponent<MovimientoBasico25D>();
            if (scriptRival != null)
            {
                Vector3 direccionEmpuje = col.transform.position - transform.position;

                // --- FIX: Declaramos direccionX como float ---
                float direccionX = Mathf.Sign(direccionEmpuje.x);

                // Armamos el vector de fuerza final
                Vector3 fuerzaFinal = new Vector3(direccionX * fuerzaEmpuje, fuerzaEmpuje * 0.4f, 0f);

                // Aplicamos el empuje y congelamos al rival
                scriptRival.AplicarEmpujeYStun(fuerzaFinal, duracionStunRival);

                Debug.LogWarning($"[RAPERO] ¡Onda expansiva golpeó a {col.name}! Mandado a volar.");
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