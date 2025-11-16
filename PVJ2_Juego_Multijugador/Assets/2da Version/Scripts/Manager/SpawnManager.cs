using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviour // Componente del GameManger de la escena
{
    // Nombre del prefab
    private string _player = "Jugador";

    // Posiciones de los players
    private Vector2 _posIzq = new Vector2(-5f, 0f);
    private Vector2 _posDer = new Vector2(5f, 0f);

    // Bandera para evitar duplicaciones
    private bool _spawned = false;


    private void Update()
    {
        if (_spawned) return;
        if (!PhotonNetwork.InRoom) return;

        _spawned = true;

        // Se asigna cada vez que se entra a la sala
        // El primero en entrar tiene el 1 el segundo el 2
        // Si el primero sale y vuelve a entrar tiene el actor en 3 y asi
        int actor = PhotonNetwork.LocalPlayer.ActorNumber;

        // Si el actor es par o impar se asigna la pos correspondiente 
        // IMPAR = PosIzq     PAR = PosDer
        Vector3 spawnPos = (actor % 2 == 1) ? _posIzq : _posDer;

        PhotonNetwork.Instantiate(_player, spawnPos, Quaternion.identity);
    }

}
