using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private Button _bLeaveRoom;

    private void Start()
    {
        // Busca el botón por el nombre 
        _bLeaveRoom = GameObject.Find("BLeaveRoom")?.GetComponent<Button>();

        if (_bLeaveRoom != null)
        {
            _bLeaveRoom.onClick.AddListener(LeaveRoom);
        }

        ShowPlayerCount();
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("Saliendo de la sala...");
    }

    // Muestra la cantidad de jugadores en la sala
    private void ShowPlayerCount()
    {
        if (PhotonNetwork.InRoom)
        {
            int count = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log($"Jugadores en la sala '{PhotonNetwork.CurrentRoom.Name}': {count}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        }
        else
        {
            Debug.Log("El jugador aún no está en ninguna sala.");
        }
    }


    // Photon Callbacks
    public override void OnLeftRoom()
    {
        Debug.Log("Jugador local abandonó la sala. Cargando escena de seleccion de nivel...");
        SceneManager.LoadScene("SelectLevel");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Jugador entró a la sala: {newPlayer.NickName}");
        ShowPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Jugador salió de la sala: {otherPlayer.NickName}");
        ShowPlayerCount();
    }
}
