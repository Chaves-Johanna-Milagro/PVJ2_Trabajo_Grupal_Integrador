using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayerManager : MonoBehaviour
{
    private bool _playerSpawned = false;


    private void Update()
    {
        // Solo spawnea una vez cuando estemos dentro de una sala
        if (!_playerSpawned && PhotonNetwork.InRoom)
        {
            _playerSpawned = true;
            SpawnearPlayer();
        }
    }

    private void SpawnearPlayer()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Vector2 spawnPos = playerCount == 1 ? new Vector2(-5f, 0f) : new Vector2(5f, 0f);

        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
        Debug.Log($"Jugador {playerCount} instanciado en {spawnPos} (Escena: {SceneManager.GetActiveScene().name})");
    }
}
