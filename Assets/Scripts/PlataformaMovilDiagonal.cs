using UnityEngine;

public class PlataformaMovilDiagonal : MonoBehaviour
{
    public enum TipoMovimiento { PingPong, Loop, OneWay }

    [Header("Configuracion de Puntos")]
    public Vector3 puntoA;
    public Vector3 puntoB;
    public bool usarPosicionActualComoA = true;

    [Header("Parametros de Movimiento")]
    public float velocidad = 3f;
    public TipoMovimiento tipoMovimiento = TipoMovimiento.PingPong;
    public float pausaEnExtremos = 0f;
    public bool empezarEnB = false;

    private Vector3 target;
    private bool yendoAB = true;
    private float tiempoEsperaTerminado = 0f;
    private bool esperando = false;

    void Start()
    {
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
            transform.position = puntoA;
            target = puntoB;
            yendoAB = true;
        }
    }

    void Update()
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

        float paso = velocidad * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, paso);

        if (Vector3.Distance(transform.position, target) < 0.001f)
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
                transform.position = puntoA;
                target = puntoB;
                yendoAB = true;
                break;

            case TipoMovimiento.OneWay:
                // Se queda quieta
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
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
