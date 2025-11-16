using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    private string _readyText = "isReady";

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(_readyText))
        {
            bool isReady = (bool)changedProps[_readyText];
            Debug.Log($"[GameManager] Player {targetPlayer.ActorNumber} ({targetPlayer.NickName}) cambió su estado → isReady = {isReady}");
        }

        CheckIfBothReady();
    }

    private void CheckIfBothReady()
    {
        // no iniciar si aún no hay dos jugadores
        if (PhotonNetwork.PlayerList.Length < 2) return;

        bool p1Ready = false;
        bool p2Ready = false;

        var players = PhotonNetwork.PlayerList;

        if (players[0].CustomProperties.ContainsKey(_readyText))
            p1Ready = (bool)players[0].CustomProperties[_readyText];

        if (players[1].CustomProperties.ContainsKey(_readyText))
            p2Ready = (bool)players[1].CustomProperties[_readyText];

        // Ambos listos?
        if (p1Ready && p2Ready)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        Debug.Log("AMBOS jugadores listos → iniciando partida!");
    }
}
