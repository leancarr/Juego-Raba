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

    private Animator anim;
    private Transform centroVisual;

    [Header("Físicas de Salto (Raycast Obligatorio)")]
    public LayerMask capaSuelo;
    public float distanciaRaycast = 1.1f;
    public float distanciaRaycastPared = 0.6f;
    public float offsetRaycastSuelo = 0.4f;

    private bool estaAturdido = false;
    private float tiempoFinStun = 0f;

    // --- NUEVA VARIABLE PARA EMPUJE COMÚN (SIN STUN) ---
    private float tiempoFinInerciaEmpuje = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        if (transform.childCount > 0)
        {
            centroVisual = transform.GetChild(0);
        }

        // --- CONGELAR LA POSICIÓN EN Z (Plano 2.5D) ---
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionZ;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Player"), true);
    }

    void Update()
    {
        // --- CONTROL DEL TIEMPO DE STUN (Habilidades Fuertes) ---
        if (estaAturdido)
        {
            if (Time.time >= tiempoFinStun)
            {
                estaAturdido = false;
            }
        }

        // --- LEER INPUTS ---
        inputHorizontal = 0f;
        // Si está bajo un Stun pesado no lee nada, si es un empuje común SÍ lee las teclas
        if (!estaAturdido)
        {
            if (Input.GetKey(teclaIzquierda)) inputHorizontal = -1f;
            if (Input.GetKey(teclaDerecha)) inputHorizontal = 1f;
        }

        // --- GIRO VISUAL DEL PERSONAJE (Mantenido intacto) ---
        if (centroVisual != null && inputHorizontal != 0f)
        {
            Vector3 rotacionLocal = centroVisual.localEulerAngles;
            if (inputHorizontal > 0f) rotacionLocal.y = 0f;
            else if (inputHorizontal < 0f) rotacionLocal.y = -180f;
            centroVisual.localEulerAngles = rotacionLocal;
        }

        // --- ACTUALIZACIÓN DE ANIMACIONES (Sincronizado con tu Animator) ---
        if (anim != null)
        {
            anim.SetFloat("VelocidadX", Mathf.Abs(inputHorizontal * velocidad)); // Corregido para que coincida con tu Blend Tree
            anim.SetBool("EstaEnSuelo", EsSuelo());
            anim.SetFloat("VelocidadY", rb.linearVelocity.y); // Enviando la velocidad física vertical para la transición a Salto_Caida
        }

        // --- ACCIÓN: SALTO (Solo si no está aturdido) ---
        if (Input.GetKeyDown(teclaSalto) && EsSuelo() && !estaAturdido)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, 0);
            if (anim != null) anim.SetTrigger("SaltoTrigger");
        }

        // --- ACCIÓN: CAÍDA RÁPIDA (Solo si no está aturdido) ---
        if (Input.GetKeyDown(teclaCaida) && !EsSuelo() && !estaAturdido)
        {
            rb.AddForce(Vector3.down * fuerzaCaidaRapida, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // CASO 1: Si está bajo STUN completo (ej. Rapero), congelamos sus físicas de control horizontal
        if (estaAturdido) return;

        // CASO 2: Si recibió un empuje común (SIN STUN), no tocamos rb.linearVelocity en X
        // para dar una ventana donde la fuerza física del envión actúe sin ser reseteada a 0.
        if (Time.time < tiempoFinInerciaEmpuje) return;

        // --- MOVIMIENTO NORMAL CONTROLADO ---
        if (inputHorizontal != 0f && EsPared())
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else
        {
            rb.linearVelocity = new Vector3(inputHorizontal * velocidad, rb.linearVelocity.y, 0);
        }
    }

    // --- FUNCIÓN PARA EMPUJE CON STUN (Mecánicas tipo Rapero) ---
    public void AplicarEmpujeYStun(Vector3 fuerzaEmpuje, float duracionStun)
    {
        estaAturdido = true;
        tiempoFinStun = Time.time + duracionStun;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(fuerzaEmpuje, ForceMode.Impulse);
    }

    // --- NUEVA FUNCIÓN PARA EMPUJE PURO (AccionEmpuje Normal sin Stun) ---
    public void RecibirEmpujePuro(Vector3 fuerzaEmpuje, float duracionInercia)
    {
        // Le damos una pequeña ventana de tiempo para que vuele por físicas
        tiempoFinInerciaEmpuje = Time.time + duracionInercia;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(fuerzaEmpuje, ForceMode.Impulse);
    }

    bool EsSuelo()
    {
        Vector3 puntoCentro = transform.position;
        Vector3 puntoIzquierda = transform.position + (Vector3.left * offsetRaycastSuelo);
        Vector3 puntoDerecha = transform.position + (Vector3.right * offsetRaycastSuelo);

        int layerColorMascara = LayerMask.GetMask("PlataformaColor");
        LayerMask mascaraCombinada = capaSuelo | layerColorMascara;

        bool tocaCentro = Physics.Raycast(puntoCentro, Vector3.down, distanciaRaycast, mascaraCombinada);
        bool tocaIzquierda = Physics.Raycast(puntoIzquierda, Vector3.down, distanciaRaycast, mascaraCombinada);
        bool tocaDerecha = Physics.Raycast(puntoDerecha, Vector3.down, distanciaRaycast, mascaraCombinada);

        return tocaCentro || tocaIzquierda || tocaDerecha;
    }

    bool EsPared()
    {
        float direccionX = Mathf.Sign(inputHorizontal);
        Vector3 direccion = new Vector3(direccionX, 0, 0);
        Vector3 origenPared = transform.position;

        return Physics.Raycast(origenPared, direccion, distanciaRaycastPared, capaSuelo);
    }

    void OnDrawGizmos()
    {
        if (transform == null) return;

        Vector3 puntoCentro = transform.position;
        Vector3 puntoIzquierda = transform.position + (Vector3.left * offsetRaycastSuelo);
        Vector3 puntoDerecha = transform.position + (Vector3.right * offsetRaycastSuelo);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(puntoCentro, puntoCentro + Vector3.down * distanciaRaycast);
        Gizmos.DrawLine(puntoIzquierda, puntoIzquierda + Vector3.down * distanciaRaycast);
        Gizmos.DrawLine(puntoDerecha, puntoDerecha + Vector3.down * distanciaRaycast);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * Mathf.Sign(inputHorizontal == 0 ? 1 : inputHorizontal) * distanciaRaycastPared));
    }
}