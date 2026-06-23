using UnityEngine;

public class AccionEmpuje : MonoBehaviour
{
    [Header("Configuración del Input")]
    public KeyCode teclaEmpuje = KeyCode.E;

    [Header("Físicas del Empuje")]
    public float fuerzaEmpujeBase = 12f;
    public float factorVelocidadEmpuje = 2.0f;
    public float radioDeGolpe = 1.8f;

    [Header("Control de Cooldown")]
    public float cooldownEmpuje = 3f;
    private float tiempoSiguienteEmpuje = 0f;

    private Rigidbody rb;
    private Transform pelvis;
    private Animator anim; // <-- NUEVO: Para la animación del atacante (opcional)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); // <-- NUEVO: Buscamos el Animator en los hijos

        pelvis = transform.Find("Centro_Visual/Modelo/Circle006/Circle008/Bip002");

        if (pelvis == null)
        {
            GameObject objetoPelvis = GameObject.Find("Bip002");
            if (objetoPelvis != null) pelvis = objetoPelvis.transform;
        }
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
        Debug.Log("¡Habilidad Ejecutada: EMPUJAR desde la Pelvis!");

        if (anim != null)
        {
            anim.SetTrigger("EjecutarAtaqueEmpuje");
        }

        Vector3 origenGolpe = (pelvis != null) ? pelvis.position : transform.position;
        Collider[] objetosGolpeados = Physics.OverlapSphere(origenGolpe, radioDeGolpe);

        foreach (Collider col in objetosGolpeados)
        {
            if (col.gameObject != gameObject && col.CompareTag("Player"))
            {
                MovimientoBasico25D scriptRival = col.GetComponent<MovimientoBasico25D>();

                if (scriptRival != null)
                {
                    // --- FIX DE DIRECCIÓN SEGURO (2.5D) ---
                    float signoDireccion = Mathf.Sign(col.transform.position.x - transform.position.x);

                    if (Mathf.Abs(col.transform.position.x - transform.position.x) < 0.1f)
                    {
                        if (transform.GetChild(0).localEulerAngles.y > 100f || transform.GetChild(0).localEulerAngles.y < -100f)
                        {
                            signoDireccion = -1f;
                        }
                        else
                        {
                            signoDireccion = 1f;
                        }
                    }

                    // Creamos el vector de dirección limpio
                    Vector3 direccionFinal = new Vector3(signoDireccion, 0.15f, 0f).normalized;

                    // Calculamos fuerzas
                    float velocidadNuestra = (rb != null) ? rb.linearVelocity.magnitude : 0f;
                    float fuerzaTotal = fuerzaEmpujeBase + (velocidadNuestra * factorVelocidadEmpuje);

                    Vector3 fuerzaFinal = direccionFinal * fuerzaTotal;

                    // --- [NUEVO] DISPARAR ANIMACIÓN DE IMPACTO EN EL RIVAL ---
                    // Buscamos el Animator en el rival que acabamos de golpear
                    Animator animRival = col.GetComponentInChildren<Animator>();
                    if (animRival != null)
                    {
                        // Activamos el Trigger que creaste en el Animator de Unity
                        animRival.SetTrigger("RecibirGolpe");
                        Debug.Log($"[ANIM] Trigger 'RecibirGolpe' enviado a {col.gameObject.name}");
                    }
                    else
                    {
                        Debug.LogError($"[ANIM] No se encontró Animator en los hijos de {col.gameObject.name} para aplicar el golpe.");
                    }

                    // Aplicamos el empuje físico puro
                    scriptRival.RecibirEmpujePuro(fuerzaFinal, 0.5f);

                    Debug.Log($"¡Empuje exitoso a {col.gameObject.name}! Dirección X: {signoDireccion} | Fuerza: {fuerzaTotal}");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Transform pelvisGizmo = transform.Find("Centro_Visual/Modelo/Circle006/Circle008/Bip002");
        Vector3 origenGizmo = (pelvisGizmo != null) ? pelvisGizmo.position : transform.position;

        Gizmos.DrawWireSphere(origenGizmo, radioDeGolpe);
    }
}