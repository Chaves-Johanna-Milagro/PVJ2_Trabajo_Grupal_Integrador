using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class MoveBall : MonoBehaviourPunCallbacks
{
    private float _speed = 8f;
    private float _limitX = 9f;      // Límite horizontal
    private float _limitY = 4.5f;    // Límite vertical
    private float _goalHeight = 3f;  // Altura del área de gol (zona central)

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        /*if (!PhotonNetwork.IsMasterClient)
        {
            _rb.simulated = false; // evitar la sinulacion de física en clientes
            return;
        }*/

        Launch();
    }

    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Normalizamos la velosidad para que sea constante
        _rb.linearVelocity = _rb.linearVelocity.normalized * _speed;

        Vector2 pos = _rb.position;

        // Rebote con el techo y suelo
        if (pos.y > _limitY || pos.y < -_limitY)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -_rb.linearVelocity.y);
        }

        // Si se pasa del límite derecho
        if (pos.x > _limitX)
        {
            if (Mathf.Abs(pos.y) < _goalHeight / 2f)
            {
                // Gol derecho
                Debug.Log("¡Gol derecha!");

                photonView.RPC("ResetBall", RpcTarget.All);

                return;
            }
            else
            {
                // Rebote lateral superior/inferior (no gol)
                _rb.linearVelocity = new Vector2(-_rb.linearVelocity.x, _rb.linearVelocity.y);
            }
        }

        // Si se pasa del límite izquierdo
        if (pos.x < -_limitX)
        {
            if (Mathf.Abs(pos.y) < _goalHeight / 2f)
            {
                // Gol izquierdo
                Debug.Log("¡Gol izquierda!");

                photonView.RPC("ResetBall", RpcTarget.All);

                return;
            }
            else
            {
                // Rebote lateral superior/inferior (no gol)
                _rb.linearVelocity = new Vector2(-_rb.linearVelocity.x, _rb.linearVelocity.y);
            }
        }
    }

    private void Launch()
    {
        Vector2 dir = new Vector2(Random.value < 0.5f ? -1f : 1f, Random.Range(-0.5f, 0.5f)).normalized;
        _rb.linearVelocity = dir * _speed;// * Time.fixedDeltaTime;
    }

    // RPC para sincronizar posición y relanzar la pelota
    [PunRPC]
    private void ResetBall()
    {
        transform.position = Vector2.zero;
        Launch();
    }

    // Este callback se dispara cuando cambia el MasterClient
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Nuevo MasterClient: " + newMasterClient.NickName);

        // Solo el nuevo MasterClient relanza la pelota
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ResetBall", RpcTarget.All); // método que relanza la pelota en todos
        }
    }


}
