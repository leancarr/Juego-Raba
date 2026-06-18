using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoBasico25D : MonoBehaviour
{
    [Header("Controles Dinámicos (Modificables para pruebas)")]
    public KeyCode teclaIzquierda = KeyCode.A;
    public KeyCode teclaDerecha = KeyCode.D;
    public KeyCode teclaSalto = KeyCode.W;
    public KeyCode teclaCaida = KeyCode.S;

    // --- [NUEVO] ESTA ES LA FUNCIÓN QUE LLAMA EL GENERADOR ---
    public void ConfigurarControles(int numeroDeJugador)
    {
        if (numeroDeJugador == 1)
        {
            // Controles estándar Jugador 1
            teclaIzquierda = KeyCode.A;
            teclaDerecha = KeyCode.D;
            teclaSalto = KeyCode.W;
            teclaCaida = KeyCode.S;
        }
        else if (numeroDeJugador == 2)
        {
            // Controles estándar Jugador 2
            teclaIzquierda = KeyCode.LeftArrow;
            teclaDerecha = KeyCode.RightArrow;
            teclaSalto = KeyCode.UpArrow;
            teclaCaida = KeyCode.DownArrow;
        }
    }

    [Header("Configuración de Movimiento")]
    public float velocidad = 7f;
    public float fuerzaSalto = 10f;
    public float fuerzaCaidaRapida = 8f;
    private float inputHorizontal;
    private Rigidbody rb;

    private Animator anim;
    [Header("Referencias Visuales")]
    public Transform centroVisual;

    [Header("Físicas de Salto (Raycast Obligatorio)")]
    public LayerMask capaSuelo;
    public float distanciaRaycast = 1.1f;
    public float distanciaRaycastPared = 0.6f;
    public float offsetRaycastSuelo = 0.4f;

    private bool estaAturdido = false;
    private float tiempoFinStun = 0f;
    private float tiempoFinInerciaEmpuje = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

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
        if (!estaAturdido)
        {
            // Ahora leemos las variables dinámicas en lugar de las estáticas
            if (Input.GetKey(teclaIzquierda)) inputHorizontal = -1f;
            if (Input.GetKey(teclaDerecha)) inputHorizontal = 1f;
        }

        // --- ACTUALIZACIÓN DE ANIMACIONES ---
        if (anim != null)
        {
            anim.SetFloat("VelocidadX", Mathf.Abs(inputHorizontal * velocidad));
            anim.SetBool("EstaEnSuelo", EsSuelo());
            anim.SetFloat("VelocidadY", rb.linearVelocity.y);
        }

        // --- ACCIÓN: SALTO (Solo si no está aturdido) ---
        // Ahora usamos teclaSalto
        if (Input.GetKeyDown(teclaSalto) && EsSuelo() && !estaAturdido)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, 0);
            if (anim != null) anim.SetTrigger("SaltoTrigger");
        }

        // --- ACCIÓN: CAÍDA RÁPIDA (Solo si no está aturdido) ---
        // Ahora usamos teclaCaida
        if (Input.GetKeyDown(teclaCaida) && !EsSuelo() && !estaAturdido)
        {
            rb.AddForce(Vector3.down * fuerzaCaidaRapida, ForceMode.Impulse);
        }
    }

    void LateUpdate()
    {
        // --- GIRO VISUAL DEL PERSONAJE (flip en el objeto del Animator) ---
        // Usamos anim.transform directamente porque es el padre de todos los huesos.
        // Cambiar la escala X del padre los voltea a todos sin que el Animator lo pise.
        if (anim != null && inputHorizontal != 0f)
        {
            Vector3 escala = anim.transform.localScale;
            escala.x = (inputHorizontal > 0f) ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
            anim.transform.localScale = escala;
        }
    }

    void FixedUpdate()
    {
        if (estaAturdido) return;
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

    public void AplicarEmpujeYStun(Vector3 fuerzaEmpuje, float duracionStun)
    {
        estaAturdido = true;
        tiempoFinStun = Time.time + duracionStun;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(fuerzaEmpuje, ForceMode.Impulse);
    }

    public void RecibirEmpujePuro(Vector3 fuerzaEmpuje, float duracionInercia)
    {
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