using UnityEngine;

public class AccionEmpuje : MonoBehaviour
{
    // --- CONTROLES DIN�MICOS (Asignados por el Generador) ---
    private KeyCode teclaEmpuje;

    [Header("F�sicas del Empuje")]
    public float fuerzaEmpujeBase = 12f;
    public float factorVelocidadEmpuje = 2.0f;
    public float radioDeGolpe = 1.8f;

    [Header("Control de Cooldown")]
    public float cooldownEmpuje = 3f;
    private float tiempoSiguienteEmpuje = 0f;

    private Rigidbody rb;
    public Transform pelvis;

    // --- NUEVO: ESTA ES LA FUNCI�N QUE LLAMA EL GENERADOR ---
    public void ConfigurarControlesEmpuje(int numeroDeJugador)
    {
        if (numeroDeJugador == 1)
        {
            teclaEmpuje = KeyCode.E;
        }
        else if (numeroDeJugador == 2)
        {
            teclaEmpuje = KeyCode.Space;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        pelvis = transform.Find("Centro_Visual/Modelo/Circle006/Circle008/Bip002");

        if (pelvis == null)
        {
            // GameObject objetoPelvis = GameObject.Find("Bip002");
            // if (objetoPelvis != null) pelvis = objetoPelvis.transform;
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
                Debug.Log("Empuje en Cooldown. Esper�: " + tiempoRestante.ToString("F1") + "s");
            }
        }
    }

    void EjecutarEmpuje()
    {
        Debug.Log("�Habilidad Ejecutada: EMPUJAR desde la Pelvis!");

        // El origen visual/detecci�n sigue siendo la pelvis (est� perfecto para el OverlapSphere)
        Vector3 origenGolpe = (pelvis != null) ? pelvis.position : transform.position;
        Collider[] objetosGolpeados = Physics.OverlapSphere(origenGolpe, radioDeGolpe);

        foreach (Collider col in objetosGolpeados)
        {
            if (col.gameObject != gameObject && col.CompareTag("Player"))
            {
                MovimientoBasico25D scriptRival = col.GetComponent<MovimientoBasico25D>();

                if (scriptRival != null)
                {
                    // --- FIX DE DIRECCI�N SEGURO (2.5D) ---
                    // En lugar de usar la pelvis que deforma el �ngulo vertical, usamos la posici�n del objeto padre.
                    // Calculamos la direcci�n pura en X bas�ndonos en qui�n est� a la izquierda y qui�n a la derecha.
                    float signoDireccion = Mathf.Sign(col.transform.position.x - transform.position.x);

                    // Si por alguna raz�n est�n perfectamente superpuestos (distancia casi 0),
                    // usamos la direcci�n hacia donde est� mirando tu personaje actual en su jerarqu�a.
                    if (Mathf.Abs(col.transform.position.x - transform.position.x) < 0.1f)
                    {
                        // Si tu objeto visual est� rotado a la izquierda (Y aproximado a 180 o -180), empuja a la izquierda
                        if (transform.GetChild(0).localEulerAngles.y > 100f || transform.GetChild(0).localEulerAngles.y < -100f)
                        {
                            signoDireccion = -1f;
                        }
                        else
                        {
                            signoDireccion = 1f;
                        }
                    }

                    // Creamos el vector de direcci�n limpio: solo en X, y le sumamos un *m�nimo* toque en Y (0.15f)
                    // para que despegue apenas los pies del suelo y la fricci�n no lo frene, pero que NO sea un gancho hacia arriba.
                    Vector3 direccionFinal = new Vector3(signoDireccion, 0.15f, 0f).normalized;

                    // Calculamos fuerzas normales
                    float velocidadNuestra = (rb != null) ? rb.linearVelocity.magnitude : 0f;
                    float fuerzaTotal = fuerzaEmpujeBase + (velocidadNuestra * factorVelocidadEmpuje);

                    Vector3 fuerzaFinal = direccionFinal * fuerzaTotal;

                    // Aplicamos el empuje puro sin stun por 0.35 segundos
                    scriptRival.RecibirEmpujePuro(fuerzaFinal, 0.35f);

                    Debug.Log($"�Empuje exitoso a {col.gameObject.name}! Direcci�n X: {signoDireccion} | Fuerza: {fuerzaTotal}");
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