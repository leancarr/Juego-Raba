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
        // 1. Averiguar hacia dónde mira el jugador (derecha 1f, izquierda -1f)
        float direccionX = 1f;
        if (transform.childCount > 0)
        {
            Transform centroVisual = transform.GetChild(0);
            if (centroVisual.localEulerAngles.y > 100f || centroVisual.localEulerAngles.y < -100f)
            {
                direccionX = -1f;
            }
        }

        // 2. Detectar la plataforma sobre la que está parado el jugador (para ignorarla)
        Transform plataformaActual = null;
        RaycastHit hitSuelo;
        // Lanzamos un rayo corto hacia abajo para ver qué pisamos
        if (Physics.Raycast(transform.position, Vector3.down, out hitSuelo, 2f))
        {
            if (hitSuelo.collider.GetComponent<PlataformasColores>() != null)
            {
                plataformaActual = hitSuelo.collider.transform;
            }
        }

        // 3. Buscar todas las plataformas en el radio de escaneo
        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioMaximoEscaneo);
        
        PlataformasColores plataformaObjetivo = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider col in objetosCercanos)
        {
            PlataformasColores scriptPlataforma = col.GetComponent<PlataformasColores>();
            
            // Si tiene el script y NO es la plataforma que estamos pisando
            if (scriptPlataforma != null && col.transform != plataformaActual)
            {
                // Solo apuntar a plataformas que estén hacia adelante de donde mira el jugador
                float direccionHaciaPlataforma = Mathf.Sign(col.transform.position.x - transform.position.x);
                
                if (direccionHaciaPlataforma == direccionX) 
                {
                    float distancia = Vector3.Distance(transform.position, col.transform.position);
                    
                    // Nos quedamos con la que tenga la distancia más corta
                    if (distancia < distanciaMinima)
                    {
                        distanciaMinima = distancia;
                        plataformaObjetivo = scriptPlataforma;
                    }
                }
            }
        }

        // 4. Configurar el disparo visual y lógico
        Vector3 origenLaser = new Vector3(transform.position.x, transform.position.y + 1f, 0f);
        Vector3 puntoFinalLaser;

        if (plataformaObjetivo != null)
        {
            // Encontramos un objetivo válido: apuntamos al centro exacto de esa plataforma
            puntoFinalLaser = plataformaObjetivo.transform.position;
            puntoFinalLaser.z = 0f; // Mantenemos el láser en el plano 2.5D

            // Ejecutamos tu función de sabotaje
            plataformaObjetivo.SabotearBando();
            Debug.LogWarning($"[PROFESOR] ¡Láser auto-apuntó a {plataformaObjetivo.name} y ejecutó SabotearBando()!");
        }
        else
        {
            // Si no encontró ninguna plataforma cerca adelante, tira el rayo derecho al vacío (feedback visual de que falló)
            puntoFinalLaser = origenLaser + new Vector3(direccionX * radioMaximoEscaneo, 0, 0);
        }

        // 5. Mostrar el láser visual
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