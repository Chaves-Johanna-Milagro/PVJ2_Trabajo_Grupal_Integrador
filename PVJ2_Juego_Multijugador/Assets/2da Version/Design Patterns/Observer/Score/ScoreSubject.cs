using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSubject : MonoBehaviourPun // Componente del jugador
{
    private int _score;

    private List<IScoreObserver> observers = new List<IScoreObserver>();

    public void AddObserver(IScoreObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
            Debug.Log("[ScoreSubject] Observer agregado: " + observer.GetType().Name);

            // Enviar puntaje inicial apenas se registra
            observer.OnScoreChanged(_score);
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

        if (observers.Count == 0)
        {
            Debug.Log("[ScoreSubject] No hay observers → NO se notificá...");
            return;
        }

        foreach (var o in observers)
        {
            o.OnScoreChanged(_score);
        }
    }

    public void AddScore(int amount)
    {
        if (!photonView.IsMine) return;

        if (observers.Count == 0)
        {
            Debug.Log("[ScoreSubject] No hay observers → NO se suma puntaje...");
            return;
        }

        Debug.Log("[ScoreSubject] Mandando RPC_AddScore con amount: " + amount);

        photonView.RPC("RPC_AddScore", RpcTarget.AllBuffered, amount);
    }

    public void RemoveScore(int amount)
    {
        if (!photonView.IsMine) return;

        if (observers.Count == 0)
        {
            Debug.Log("[ScoreSubject] No hay observers → NO se resta puntaje...");
            return;
        }

        Debug.Log("[ScoreSubject] Mandando RPC_RemoveScore con amount: " + amount);

        photonView.RPC("RPC_RemoveScore", RpcTarget.AllBuffered, amount);
    }

    public void ResetScore()
    {
        if (!photonView.IsMine) return;

        if (observers.Count == 0)
        {
            Debug.Log("[ScoreSubject] No hay observers → NO se resetea puntaje...");
            return;
        }

        Debug.Log("[ScoreSubject] Mandando RPC_ResetScore con puntaje:" + _score);

        photonView.RPC("RPC_ResetScore", RpcTarget.AllBuffered);
    }

    // Enviar el nuevo puntaje a todos los jugadores que ingresen a la sala
    [PunRPC]
    private void RPC_AddScore(int amount)
    {
        if (observers.Count == 0)
        {
            Debug.Log("[ScoreSubject] RPC_AddScore cancelado → no hay observers...");
            return;
        }

        _score += amount;

        Debug.Log("[ScoreSubject] Puntaje actualizado: " + _score);

        NotifyObservers();
    }

    [PunRPC]
    private void RPC_RemoveScore(int amount)
    {
        if (observers.Count == 0)
        {
            Debug.Log("[ScoreSubject] RPC_RemoveScore cancelado → no hay observers...");
            return;
        }

        _score -= amount;

        Debug.Log("[ScoreSubject] Puntaje actualizado: " + _score);

        NotifyObservers();
    }

    [PunRPC]
    private void RPC_ResetScore()
    {
        if (observers.Count == 0)
        {
            Debug.Log("[ScoreSubject] RPC_ResetScore cancelado → no hay observers...");
            return;
        }

        _score = 0;

        Debug.Log("[ScoreSubject] Puntaje actualizado: " + _score);

        NotifyObservers();
    }
}
