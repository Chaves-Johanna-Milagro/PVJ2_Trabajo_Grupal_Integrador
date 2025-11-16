using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySubject : MonoBehaviourPun
{
    private List<IInventoryObserver> _observers = new List<IInventoryObserver>();

    private List<Sprite> _items = new List<Sprite>();

    public void AddObserver(IInventoryObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
            Debug.Log("[InventorySubject] Observer agregado: " + observer.GetType().Name);

            // Enviar puntaje inicial apenas se registra
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
        if (_items.Count >= 3) return;

        _items.Add(sprite);

        foreach (var o in _observers)
            o.OnInventoryChanged(_items);
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= _items.Count) return;

        _items.RemoveAt(index);

        foreach (var o in _observers)
            o.OnInventoryChanged(_items);
    }
}
