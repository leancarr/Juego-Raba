using UnityEngine;

public class AccionEmpuje : MonoBehaviour
{
    [Header("Configuración del Input")]
    public KeyCode teclaEmpuje = KeyCode.E;

    [Header("Físicas del Empuje")]
    public float fuerzaEmpujeBase = 12f;       // Subí la fuerza base para que el golpe sea bien evidente
    public float factorVelocidadEmpuje = 2.0f; // Multiplicador de tu velocidad actual
    public float radioDeGolpe = 1.8f;          // Rango del manotazo a tu alrededor

    [Header("Control de Cooldown")]
    public float cooldownEmpuje = 3f;          // Los 3 segundos de cooldown exigidos
    private float tiempoSiguienteEmpuje = 0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaEmpuje))
        {
            if (Time.time >= tiempoSiguienteEmpuje)
            {
                EjecutarEmpuje();
                tiempoSiguienteEmpuje = Time.time + cooldownEmpuje;
            }
            else
            {
                float tiempoRestante = tiempoSiguienteEmpuje - Time.time;
                Debug.Log("Empuje en Cooldown. Esperá: " + tiempoRestante.ToString("F1") + "s");
            }
        }
    }

    void EjecutarEmpuje()
    {
        Debug.Log("¡Habilidad Ejecutada: EMPUJAR!");

        // Creamos una esfera invisible de detección
        Collider[] objetosGolpeados = Physics.OverlapSphere(transform.position, radioDeGolpe);

        foreach (Collider col in objetosGolpeados)
        {
            // DELIMITACIÓN CRÍTICA: 
            // 1. Que NO sea el propio jugador que ejecuta el script
            // 2. Que tenga la etiqueta obligatoria "Player"
            if (col.gameObject != gameObject && col.CompareTag("Player"))
            {
                Rigidbody rbRival = col.GetComponent<Rigidbody>();
                if (rbRival != null)
                {
                    // Calculamos la dirección desde nuestra posición hacia el rival
                    Vector3 direccion = (col.transform.position - transform.position).normalized;
                    direccion.z = 0; // Forzamos plano 2.5D

                    // Si por la cercanía el cálculo da casi cero, empujamos hacia el frente del objeto
                    if (direccion.magnitude < 0.1f)
                    {
                        direccion = transform.right;
                    }

                    // Medimos la velocidad que traíamos nosotros en el script de movimiento
                    float velocidadNuestra = (rb != null) ? rb.linearVelocity.magnitude : 0f;

                    // Cálculo final de la fuerza de impulso
                    float fuerzaTotal = fuerzaEmpujeBase + (velocidadNuestra * factorVelocidadEmpuje);

                    // Aplicamos el golpe directo ignorando la masa del rival temporalmente (Impulse)
                    rbRival.linearVelocity = Vector3.zero; // Reseteamos su velocidad un milisegundo para que el golpe sea limpio
                    rbRival.AddForce(direccion * fuerzaTotal, ForceMode.Impulse);

                    Debug.Log("¡Empujaste con éxito al jugador: " + col.gameObject.name + " con fuerza: " + fuerzaTotal + "!");
                }
            }
        }
    }

    // Esto te permite ver la esfera de alcance en la pestaña Scene para saber qué tan cerca tenés que estar
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeGolpe);
    }
}