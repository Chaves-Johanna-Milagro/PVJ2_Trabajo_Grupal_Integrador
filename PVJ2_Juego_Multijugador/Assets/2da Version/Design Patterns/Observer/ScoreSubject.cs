using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSubject : MonoBehaviourPun // Componente del jugador
{
    private int score;

    private List<IScoreObserver> observers = new List<IScoreObserver>();

    public void AddObserver(IScoreObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
            Debug.Log("[ScoreSubject] Observer agregado: " + observer.GetType().Name);
        }
        else
        {
            Debug.Log("[ScoreSubject] Ya estaba registrado este observer.");
        }
    }

    public void RemoveObserver(IScoreObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
            Debug.Log("[ScoreSubject] Observer removido: " + observer.GetType().Name);
        }
    }

    private void NotifyObservers()
    {
        Debug.Log("[ScoreSubject] Notificando observers... total: " + observers.Count);

        foreach (var o in observers)
        {
            o.OnScoreChanged(score);
        }
    }

    public void AddScore(int amount)
    {
        if (!photonView.IsMine) return;

        Debug.Log("[ScoreSubject] Mandando RPC_AddScore con amount: " + amount);

        photonView.RPC("RPC_AddScore", RpcTarget.AllBuffered, amount);
    }

    [PunRPC]
    private void RPC_AddScore(int amount)
    {
        score += amount;

        Debug.Log("[ScoreSubject] Puntaje actualizado: " + score);

        NotifyObservers();
    }
}
