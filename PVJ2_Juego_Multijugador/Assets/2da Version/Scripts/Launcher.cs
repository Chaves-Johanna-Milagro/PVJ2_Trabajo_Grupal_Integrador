using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
    private bool _isConnecting;
    private string _targetRoomName;
    private const byte _maxPlayersPerRoom = 2;

    private Button _bMenu;
    private Button _bLevel1;
    private Button _bLevel2;

    private GameObject _tConnected;

    private void Awake()
    {
        // Permite que el MasterClient sincronice las escenas con los demás jugadores en su misma sala
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // Buscar los botones en la escena por el nombre 
        _bMenu = GameObject.Find("BMenu")?.GetComponent<Button>();
        _bLevel1 = GameObject.Find("BLevel1")?.GetComponent<Button>();
        _bLevel2 = GameObject.Find("BLevel2")?.GetComponent<Button>();

        if (_bMenu != null)
        {
            _bMenu.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");

                Debug.Log("Volviendo al menu...");
            });
        }

        if (_bLevel1 != null)
        {
            _bLevel1.onClick.AddListener(() => Connect(1));
        }
           
        if (_bLevel2 != null)
        {
            _bLevel2.onClick.AddListener(() => Connect(2));
        }

        _tConnected = GameObject.Find("TConnect");
        _tConnected.SetActive(false);
    }

    private void Connect(int levelIndex)
    {
        _targetRoomName = (levelIndex == 1) ? "Sala1" : "Sala2";

        _bMenu.gameObject.SetActive(false);
        _bLevel1.gameObject.SetActive(false);
        _bLevel2.gameObject.SetActive(false);
        _tConnected.gameObject.SetActive(true);

        if (PhotonNetwork.IsConnected)
        {
            // Si estamos conectados nos unimos a la sala
            JoinOrCreateTargetRoom();
        }
        else
        {
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
        }


    }

    private void JoinOrCreateTargetRoom()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = _maxPlayersPerRoom }; 

        PhotonNetwork.JoinOrCreateRoom(_targetRoomName, options, TypedLobby.Default);

        Debug.Log($"Intentando unirse o crear la sala: {_targetRoomName}");
    }

    
    
    // Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor maestro.");


        if (_isConnecting && !string.IsNullOrEmpty(_targetRoomName))
        {
            JoinOrCreateTargetRoom();
            _isConnecting = false;
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Entraste a la sala: {PhotonNetwork.CurrentRoom.Name}");

        // Solo el primer jugador carga el nivel correspondiente
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if (PhotonNetwork.CurrentRoom.Name == "Sala1")
            {
                PhotonNetwork.LoadLevel("Level_1");
            }
            else if (PhotonNetwork.CurrentRoom.Name == "Sala2")
            {
                PhotonNetwork.LoadLevel("Level_2");
            }

        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"No se pudo unir a la sala: {_targetRoomName}. Motivo: {message}");

        RoomOptions options = new RoomOptions { MaxPlayers = _maxPlayersPerRoom };

        PhotonNetwork.CreateRoom(_targetRoomName, options);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Desconectado del servidor. Motivo: {cause}");
    }
}
