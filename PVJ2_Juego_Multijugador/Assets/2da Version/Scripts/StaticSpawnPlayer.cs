using UnityEngine;
using Photon.Pun;

public static class StaticSpawnPlayer //Clase encargada de la instanciacion de los jugadores
{
    // Nombre del prefab
    private static readonly string _player = "Jugador"; 

    // Posiciones de los players
    private static readonly Vector2 _posPlayer1 = new Vector2(-5f, 0f);
    private static readonly Vector2 _posPlayer2 = new Vector2(5f, 0f);

    // Bandera para evitar duplicaciones
    private static bool _playersSpawned = false;

    public static void SpawnEverything()
    {
        if (_playersSpawned) return;
        _playersSpawned = true;

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("Intentaste spawnear pero no estas en una sala...");
            return;
        }

        SpawnPlayers();
    }

    private static void SpawnPlayers()
    {
        int actor = PhotonNetwork.LocalPlayer.ActorNumber; // numero de jugador local

        Vector2 pos = (actor == 1) ? _posPlayer1 : _posPlayer2; // si es el primero se intancia a la izquierda

        PhotonNetwork.Instantiate(_player, pos, Quaternion.identity);
    }

    // Reseteo para cuando salga de la sala y al re-ingresar permitir el spawn
    public static void Reset() 
    {
        _playersSpawned = false;
    }
}
