using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySubject : MonoBehaviourPun // Componente del jugador
{
    private List<IInventoryObserver> _observers = new List<IInventoryObserver>();

    private List<Sprite> _items = new List<Sprite>();

    public void AddObserver(IInventoryObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
            Debug.Log("[InventorySubject] Observer agregado: " + observer.GetType().Name);

            // Enviar inventario inicial apenas se registra
            observer.OnInventoryChanged(_items);
        }
        else
        {
            Debug.Log("[InventorySubject] Ya estaba registrado este observer.");
        }
    }

    public void RemoveObserver(IInventoryObserver observer)
    {
        if (_observers.Contains(observer))
        {
            _observers.Remove(observer);
            Debug.Log("[InventorySubject] Observer removido: " + observer.GetType().Name);
        }
    }
    private void NotifyObservers()
    {
        Debug.Log("[InventorySubject] Notificando observers... total: " + _observers.Count);

        foreach (var o in _observers)
        {
            o.OnInventoryChanged(_items);
        }
    }
    public void AddItem(Sprite sprite)
    {
        if (!photonView.IsMine) return;

        int id = StaticSpritePowerUps.GetId(sprite);

        photonView.RPC("RPC_AddItem", RpcTarget.AllBuffered, id);
    }

    // Enviar el nuevo inventario a todos los jugadores que ingresen a la sala
    [PunRPC]
    private void RPC_AddItem(int id)
    {
        Sprite sprite = StaticSpritePowerUps.GetSprite(id);

        _items.Add(sprite);

        NotifyObservers();
    }
}
