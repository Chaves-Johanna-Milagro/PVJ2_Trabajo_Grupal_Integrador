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

        // Mostramos la cantidad de jugadores apenas ingresamos al nivel
        ShowPlayerCount();
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("[GameManager] Saliendo de la sala...");
    }

    // Muestra la cantidad de jugadores en la sala
    private void ShowPlayerCount()
    {
        if (PhotonNetwork.InRoom)
        {
            int count = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log($"[GameManager] Jugadores en la sala '{PhotonNetwork.CurrentRoom.Name}': {count}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        }
        else
        {
            Debug.Log("[GameManager] El jugador aún no está en ninguna sala.");
        }
    }

    // -----  CALLBACKS DE PHOTON  -----
    // se ejecutan automaticamente dependiendo lo que ocurra 

    // Cuando salimos de la sala
    public override void OnLeftRoom()
    {
        Debug.Log("[GameManager] Jugador local abandonó la sala. Cargando escena de seleccion de nivel...");
        SceneManager.LoadScene("SelectLevel");
    }


    // Cuando un jugador entra a la sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[GameManager] Jugador entró a la sala: {newPlayer.NickName}");
        ShowPlayerCount();
    }


    // Cuando un jugador sale de la sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[GameManager] Jugador salió de la sala: {otherPlayer.NickName}");
        ShowPlayerCount();
    }
}
