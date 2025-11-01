using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager _instance;

    private void Awake()
    {
        // Evita duplicados al recargar la escena
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Configuración automática de sincronización
        PhotonNetwork.AutomaticallySyncScene = true;

        // Conecta al servidor de Photon
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("En lobby, creando o uniéndose a sala...");
        PhotonNetwork.JoinOrCreateRoom("Cuarto", new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, Photon.Realtime.TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Conectado a la sala 'Cuarto'");
    }

}
