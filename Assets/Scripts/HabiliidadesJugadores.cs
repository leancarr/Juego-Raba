using UnityEngine;

public class HabilidadesJugador : MonoBehaviour
{
    public enum TipoClase { Profesor, Mecanico, Rapero }

    [Header("Animación")]
    public Animator anim; // Arrastrá el Animator de tu personaje acá en el Inspector
                          // [NUEVO] Arrastrá el GameObject "visualCenter" (o ModeloVisual) de Marcus acá en el Inspector
    public Transform visualCenter;

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
        // 1. Averiguar hacia dónde mira el jugador ACTUALMENTE (derecha 1f, izquierda -1f)
        float facingX = 1f;
        bool isFacingRight = true;
        if (visualCenter != null)
        {
            // En Unity, localY 0 es Derecha, localY 180 es Izquierda
            if (visualCenter.localEulerAngles.y > 100f || visualCenter.localEulerAngles.y < -100f)
            {
                facingX = -1f;
                isFacingRight = false;
            }
        }

        // 2. Detectar la plataforma sobre la que está parado el jugador (para ignorarla)
        Transform plataformaActual = null;
        RaycastHit hitSuelo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitSuelo, 2f))
        {
            if (hitSuelo.collider.GetComponent<PlataformasColores>() != null)
            {
                plataformaActual = hitSuelo.collider.transform;
            }
        }

        // 3. Buscar la plataforma MÁS CERCANA (Pura distancia en 360°)
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

        // 4. Si encontramos objetivo, procesamos el giro automático y el disparo
        if (plataformaObjetivo != null && visualCenter != null)
        {
            Collider coliderPlataforma = plataformaObjetivo.GetComponent<Collider>();
            Vector3 targetPoint = coliderPlataforma != null ? coliderPlataforma.ClosestPoint(centroJugador) : plataformaObjetivo.transform.position;
            targetPoint.z = 0f;

            // [NUEVO LOGIC START] --- GIRO AUTOMÁTICO ---

            // Calculamos la dirección del mundo hacia el objetivo (X positivo es Derecha, X negativo es Izquierda)
            float directionToTargetWorldX = Mathf.Sign(targetPoint.x - transform.position.x);

            // Si hay una diferencia horizontal clara (más de 0.2m)
            if (Mathf.Abs(targetPoint.x - transform.position.x) > 0.2f)
            {
                // Matriz de decisión de giro:
                // Si el objetivo está a la DERECHA (+X) y miramos a la IZQUIERDA (localY 180) -> OBLIGAR GIRO DERECHA (localY 0)
                // Si el objetivo está a la IZQUIERDA (-X) y miramos a la DERECHA (localY 0) -> OBLIGAR GIRO IZQUIERDA (localY 180)

                if (directionToTargetWorldX > 0 && !isFacingRight)
                {
                    visualCenter.localEulerAngles = new Vector3(0f, 0f, 0f); // Girar a Derecha
                    facingX = 1f; // Actualizar variable local
                }
                else if (directionToTargetWorldX < 0 && isFacingRight)
                {
                    visualCenter.localEulerAngles = new Vector3(0f, 180f, 0f); // Girar a Izquierda
                    facingX = -1f; // Actualizar variable local
                }
            }

            // [NUEVO LOGIC END] 

            // 5. Configurar el disparo y animación (Ahora Marcus SIEMPRE mira al objetivo)
            // El origen de la mano se proyecta simétricamente según la nueva orientación de Marcus
            Vector3 origenLaser = new Vector3(transform.position.x + (0.4f * facingX), transform.position.y + 1f, 0f);
            Vector3 puntoFinalLaser = coliderPlataforma != null ? coliderPlataforma.ClosestPoint(origenLaser) : plataformaObjetivo.transform.position;
            puntoFinalLaser.z = 0f;

            if (anim != null)
            {
                // Como forzamos al personaje a mirar al frente del objetivo, 
                // la distancia horizontal pura "adelante" SIEMPRE es positiva
                float diffX = Mathf.Abs(puntoFinalLaser.x - origenLaser.x);
                float diffY = puntoFinalLaser.y - origenLaser.y;

                // Atan2 ahora siempre devuelve datos coherentes (hemisferio frontal de -90 a 90)
                float anguloFinal = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;

                // Protegemos los límites de tu Blend Tree frontal
                anguloFinal = Mathf.Clamp(anguloFinal, -90f, 90f);

                anim.SetFloat("AnguloApuntado", anguloFinal);
                anim.SetTrigger("PulsoTrigger");
            }

            Debug.DrawLine(origenLaser, puntoFinalLaser, Color.magenta, 1f);
            plataformaObjetivo.SabotearBando();
            Debug.LogWarning($"[PROFESOR] Sabotaje simétrico y giro automático exitosos. Ángulo Local enviado: {anim.GetFloat("AnguloApuntado")}");

            if (efectoVisualLaser != null)
            {
                StartCoroutine(MostrarLaserVisual(origenLaser, puntoFinalLaser));
            }
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