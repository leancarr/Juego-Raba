/* 
 * ==============================================================================
 * SCRIPT: PlataformaMovilDiagonal.cs
 * CATEGORIA: 3. Camara y Entorno
 * DESCRIPCION: Hace que los pisos floten en diagonal para los parkours.
 * ==============================================================================
 */
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlataformaMovilDiagonal : MonoBehaviour
{
    public enum TipoMovimiento { PingPong, Loop, OneWay }

    [Header("Configuracion de Puntos")]
    [Tooltip("La plataforma arrancarÃ¡ donde la pusiste en la escena.")]
    public bool usarPosicionActualComoA = true;
    
    [Tooltip("Coordenada exacta del mundo desde donde arranca (si no usas la posiciÃ³n actual).")]
    public Vector3 puntoA;

    [Tooltip("Coordenada exacta del mundo a la que viajarÃ¡ la plataforma.")]
    public Vector3 puntoB;

    [Header("Parametros de Movimiento")]
    public float velocidad = 3f;
    [Tooltip("Â¡ElegÃ­ 'Loop' para que vaya a B y aparezca mÃ¡gicamente en A de nuevo!")]
    public TipoMovimiento tipoMovimiento = TipoMovimiento.Loop;
    public float pausaEnExtremos = 0f;
    public bool empezarEnB = false;

    private Vector3 target;
    private bool yendoAB = true;
    private float tiempoEsperaTerminado = 0f;
    private bool esperando = false;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Fundamental para que no se caiga por gravedad
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Movimiento suave

        if (usarPosicionActualComoA)
        {
            puntoA = transform.position;
        }

        if (empezarEnB)
        {
            transform.position = puntoB;
            target = puntoA;
            yendoAB = false;
        }
        else
        {
            // NO teletransportar al iniciar. Se queda en su posiciÃ³n visual actual (en el medio de la cascada).
            target = puntoB;
            yendoAB = true;
        }
    }

    void FixedUpdate() // Usamos FixedUpdate para fÃ­sicas perfectas
    {
        if (esperando)
        {
            if (Time.time >= tiempoEsperaTerminado)
            {
                esperando = false;
            }
            else
            {
                return;
            }
        }

        float paso = velocidad * Time.fixedDeltaTime;
        Vector3 nuevaPos = Vector3.MoveTowards(rb.position, target, paso);
        
        // MovePosition mueve la plataforma y arrastra fÃ­sicamente al jugador sin buguearlo
        rb.MovePosition(nuevaPos);

        if (Vector3.Distance(rb.position, target) < 0.001f)
        {
            if (pausaEnExtremos > 0f)
            {
                esperando = true;
                tiempoEsperaTerminado = Time.time + pausaEnExtremos;
            }

            CambiarTarget();
        }
    }

    void CambiarTarget()
    {
        switch (tipoMovimiento)
        {
            case TipoMovimiento.PingPong:
                if (yendoAB)
                {
                    target = puntoA;
                    yendoAB = false;
                }
                else
                {
                    target = puntoB;
                    yendoAB = true;
                }
                break;

            case TipoMovimiento.Loop:
                // Transporta la plataforma al inicio (Punto A) instantÃ¡neamente
                rb.position = puntoA;
                transform.position = puntoA; 
                target = puntoB;
                yendoAB = true;
                break;

            case TipoMovimiento.OneWay:
                // Se queda quieta
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 posA = usarPosicionActualComoA && !Application.isPlaying ? transform.position : puntoA;
        Vector3 posB = puntoB;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(posA, posB);
        Gizmos.DrawWireSphere(posA, 0.25f);
        Gizmos.DrawWireSphere(posB, 0.25f);
    }
}

