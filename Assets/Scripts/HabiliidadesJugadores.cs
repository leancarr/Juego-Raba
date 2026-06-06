using UnityEngine;

public enum ClasePersonaje { Mecanico, Profesor, Rapero }

public class HabilidadesJugador : MonoBehaviour
{
    [Header("Configuración de Clase")]
    public ClasePersonaje clase;
    public KeyCode teclaHabilidad = KeyCode.E;
    public float cooldown = 4f;
    private float tiempoSiguienteHabilidad = 0f;

    [Header("Habilidad Mecánico")]
    public GameObject prefabBloqueMecanico;

    [Header("Habilidad Profesor")]
    public GameObject prefabPulsoProfesor;

    [Header("Habilidad Rapero (Configurable)")]
    public float radioEmpuje = 5f;
    public float fuerzaEmpuje = 12f;
    public float duracionStunRival = 0.5f; // Modificable desde el Inspector
    public GameObject visualOndaRapero; // Efecto visual (ej: una esfera que spawnea y se agranda)

    void Update()
    {
        if (Input.GetKeyDown(teclaHabilidad) && Time.time >= tiempoSiguienteHabilidad)
        {
            UsarHabilidad();
        }
    }

    void UsarHabilidad()
    {
        tiempoSiguienteHabilidad = Time.time + cooldown;

        switch (clase)
        {
            case ClasePersonaje.Mecanico:
                // Spawnea el bloque gris justo adelante del jugador
                Vector3 posBloque = transform.position + (transform.childCount > 0 ? transform.GetChild(0).forward * 1.2f : Vector3.right);
                Instantiate(prefabBloqueMecanico, posBloque, Quaternion.identity);
                break;

            case ClasePersonaje.Profesor:
                // Spawnea el proyectil que viaja en el eje X
                GameObject pulso = Instantiate(prefabPulsoProfesor, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                PulsoProfesor scriptPulso = pulso.GetComponent<PulsoProfesor>();
                
                // Detectamos hacia dónde está mirando el Centro_Visual para darle dirección al disparo
                if (transform.childCount > 0)
                {
                    // Si la rotación Y local es 0 está mirando a la derecha, si es -180 o 180 a la izquierda
                    float rotY = transform.GetChild(0).localEulerAngles.y;
                    scriptPulso.direccionX = (rotY > 45s && rotY < 135f) ? 0f : (rotY == 0f ? 1f : -1f); 
                    // Nota simplificada: Si tu script rota en base a 90f (frente), 0f (der), -180f (izq):
                    if (Mathf.Approximately(rotY, 0f)) scriptPulso.direccionX = 1f;
                    else if (Mathf.Approximately(rotY, 180f) || Mathf.Approximately(rotY, -180f)) scriptPulso.direccionX = -1f;
                }
                break;

            case ClasePersonaje.Rapero:
                CantarOndaExpansiva();
                break;
        }
    }

    void CantarOndaExpansiva()
    {
        Debug.Log("¡El Rapero cantó tirando onda expansiva!");
        
        // Instanciamos el efecto visual si existe
        if (visualOndaRapero != null)
        {
            GameObject onda = Instantiate(visualOndaRapero, transform.position, Quaternion.identity);
            Destroy(onda, 0.4f); // Se destruye rápido
        }

        // Buscamos todos los colisionadores en el radio de explosión 360°
        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioEmpuje);

        foreach (Collider col in objetosCercanos)
        {
            // Evitamos empujarnos a nosotros mismos
            if (col.gameObject == this.gameObject) continue;

            // Si encuentra otra cápsula de jugador con el script de movimiento
            MovimientoBasico25D rivalMov = col.gameObject.GetComponent<MovimientoBasico25D>();
            if (rivalMov != null)
            {
                // Calculamos la dirección exacta en 360 grados desde el Rapero hacia el rival
                Vector3 direccionEmpuje = (col.transform.position - transform.position).normalized;
                
                // Forzamos que se mantenga en el plano 2.5D (Eje Z en cero, empuje en X e Y de salto)
                direccionEmpuje.z = 0f;
                // Le damos un pequeño ángulo hacia arriba para que los despegue del suelo al empujarlos
                direccionEmpuje.y += 0.3f; 
                direccionEmpuje = direccionEmpuje.normalized;

                // Aplicamos el impacto y el stun configurable
                rivalMov.AplicarEmpujeYStun(direccionEmpuje * fuerzaEmpuje, duracionStunRival);
            }
        }
    }

    // Para dibujar el rango de la onda en el editor de Unity
    void OnDrawGizmosSelected()
    {
        if (clase == ClasePersonaje.Rapero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, radioEmpuje);
        }
    }
}