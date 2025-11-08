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

    private void Update()
    {
        // Instanciamos al player una vez estamos en la sala
        if (PhotonNetwork.InRoom)
        {
            StaticSpawnPlayer.SpawnEverything();
        }
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("Saliendo de la sala...");

        // Reseteamos en spawn una vez salimos de sala
        StaticSpawnPlayer.Reset(); 
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

    // -----  CALLBACKS DE PHOTON  -----
    // se ejecutan automaticamente dependiendo lo que ocurra 

    // Cuando salimos de la sala
    public override void OnLeftRoom()
    {
        Debug.Log("Jugador local abandonó la sala. Cargando escena de seleccion de nivel...");
        SceneManager.LoadScene("SelectLevel");

        // Reseteamos en spawn una vez salimos de sala
        StaticSpawnPlayer.Reset(); 
    }


    // Cuando un jugador entra a la sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Jugador entró a la sala: {newPlayer.NickName}");
        ShowPlayerCount();
    }


    // Cuando un jugador sale de la sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Jugador salió de la sala: {otherPlayer.NickName}");
        ShowPlayerCount();
    }
}
