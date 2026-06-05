using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoBasico25D : MonoBehaviour
{
    [Header("Configuración de Teclas (Custom)")]
    public KeyCode teclaIzquierda = KeyCode.A;
    public KeyCode teclaDerecha = KeyCode.D;
    public KeyCode teclaSalto = KeyCode.W;
    public KeyCode teclaCaida = KeyCode.S;

    [Header("Configuración de Movimiento")]
    public float velocidad = 7f;
    public float fuerzaSalto = 10f;
    public float fuerzaCaidaRapida = 8f;
    private float inputHorizontal;
    private Rigidbody rb;

    // VARIABLES PARA ANIMACIÓN Y CONTENEDOR INTERMEDIO
    private Animator anim;
    private Transform centroVisual; // Apunta al objeto vacío intermedio centrado

    [Header("Físicas de Salto (Raycast Obligatorio)")]
    public LayerMask capaSuelo;
    public float distanciaRaycast = 1.1f;
    public float distanciaRaycastPared = 0.6f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Buscamos el Animator automáticamente en los hijos
        anim = GetComponentInChildren<Animator>();

        // Guardamos el primer hijo directo, que ahora es el contenedor 'Centro_Visual'
        if (transform.childCount > 0)
        {
            centroVisual = transform.GetChild(0);
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionZ;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Player"), true);
    }

    void Update()
    {
        // 1. Procesar el input horizontal según las teclas asignadas
        inputHorizontal = 0f;
        if (Input.GetKey(teclaIzquierda)) inputHorizontal = -1f;
        if (Input.GetKey(teclaDerecha)) inputHorizontal = 1f;

        // CONTROL DE ORIENTACIÓN DINÁMICA (Rotamos el contenedor intermedio centrado)
        if (centroVisual != null)
        {
            Vector3 rotacionLocal = centroVisual.localEulerAngles;

            if (inputHorizontal == 0f)
            {
                // IDLE: Mira de frente a la cámara
                rotacionLocal.y = 90f;
            }
            else if (inputHorizontal > 0f)
            {
                // CAMINANDO DERECHA: Perfil a la derecha (90 grados)
                rotacionLocal.y = 0f;
            }
            else if (inputHorizontal < 0f)
            {
                // CAMINANDO IZQUIERDA: Perfil a la izquierda (-90 grados)
                rotacionLocal.y = -180f;
            }

            centroVisual.localEulerAngles = rotacionLocal;
        }

        // Mandar los datos al Animator
        if (anim != null)
        {
            anim.SetFloat("Velocidad", Mathf.Abs(inputHorizontal * velocidad));
            anim.SetBool("EstaEnSuelo", EsSuelo());
        }

        // 2. Salto con la tecla asignada
        if (Input.GetKeyDown(teclaSalto) && EsSuelo())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, 0);
            if (anim != null) anim.SetTrigger("SaltoTrigger");
        }

        // 3. Caída rápida (solo en el aire)
        if (Input.GetKeyDown(teclaCaida) && !EsSuelo())
        {
            rb.AddForce(Vector3.down * fuerzaCaidaRapida, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Si estamos tocando una tecla de movimiento Y además tenemos una pared enfrente
        if (inputHorizontal != 0f && EsPared())
        {
            // Frenamos en seco el empuje horizontal para no trepar la pared
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else
        {
            // Movimiento normal
            rb.linearVelocity = new Vector3(inputHorizontal * velocidad, rb.linearVelocity.y, 0);
        }
    }

    bool EsSuelo()
    {
        return Physics.Raycast(transform.position, Vector3.down, distanciaRaycast, capaSuelo);
    }

    bool EsPared()
    {
        // Calculamos hacia dónde estamos yendo (1 para derecha, -1 para izquierda)
        float direccionX = Mathf.Sign(inputHorizontal);
        Vector3 direccion = new Vector3(direccionX, 0, 0);

        // Levantamos el origen del rayo un poco desde los pies hacia la cintura
        // para que no choque contra el piso por accidente
        Vector3 origen = transform.position + (Vector3.down * 0.8f);

        return Physics.Raycast(origen, direccion, distanciaRaycastPared, capaSuelo);
    }
    void OnDrawGizmos()
    {
        // Gizmo del suelo (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distanciaRaycast);

        // Gizmo de la pared (Azul) - Se dibuja a la altura de la cintura
        Gizmos.color = Color.blue;
        Vector3 origenPared = transform.position + (Vector3.down * 0.2f);
        // Por defecto en el editor mirará a la derecha para dibujarlo
        Gizmos.DrawLine(origenPared, origenPared + Vector3.right * 0.6f);
    }
}