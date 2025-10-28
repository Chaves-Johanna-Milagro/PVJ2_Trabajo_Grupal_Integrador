using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("Cuarto", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // Detecta el orden de entrada
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        Vector2 spawnPos;

        if (playerCount == 1)
        {
            // Primer jugador
            spawnPos = new Vector2(-5f, 0f);
        }
        else
        {
            // Segundo jugador
            spawnPos = new Vector2(5f, 0f);
        }

        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
        Debug.Log($"Jugador {playerCount} instanciado en {spawnPos}");
    }
}
