using UnityEngine;
using Photon.Pun;
public class PlayerColor : MonoBehaviourPun // Script para diferenciar y probar la sincro entre jugadores
{
    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        // Asigna un color random al jugador dueño del script sin afectar a los demas
        if (photonView.IsMine)
        {
            Color myColor = GetRandomColor();

            // Se aplica al jugador
            _sr.color = myColor;

            // Todos los jugadores lo persiven 
            photonView.RPC("RPC_SetColor", RpcTarget.OthersBuffered, myColor.r, myColor.g, myColor.b);
            Debug.Log("[PlayerColor] Color asignado...");
        }

    }

    [PunRPC]
    private void RPC_SetColor(float r, float g, float b)
    {
        _sr.color = new Color(r, g, b);
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
