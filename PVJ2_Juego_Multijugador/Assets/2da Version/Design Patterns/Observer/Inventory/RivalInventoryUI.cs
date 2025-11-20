using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RivalInventoryUI : MonoBehaviour, IInventoryObserver, IPlayerUI // Clase observadora
{
    private Image[] _slots;

    // Guardamos referencia al subject del rival
    private InventorySubject _currentSubject;

    void Start()
    {
        int count = transform.childCount;

        _slots = new Image[count];

        for (int i = 0; i < count; i++)
        {
            // Imagen del slot
            _slots[i] = transform.GetChild(i).GetComponent<Image>();

            // Desactiva la interaccion con los botones rivales
            var button = _slots[i].GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }

        }

    }

 
    // Se encarga de saber y obtener el componente InventorySubject del jugador Rival
    private InventorySubject FindRivalSubject()
    {
        // Buscar subjects en la escena
        InventorySubject[] subjects = FindObjectsOfType<InventorySubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            // Solo registrar al subject RIVAL
            if (pv != null && !pv.IsMine)
            {
                return s;
            }
        }

        return null;
    }


    // Metodos implementados de la interfaz IPlayerUI
    public void ActiveUI()
    {
        // Buscar el subject rival
        _currentSubject = FindRivalSubject();

        if (_currentSubject == null)
        {
            Debug.LogWarning("[RivalInventoryUI] No se encontró Subject rival al activar UI...");
            return;
        }

        // Registrar el observer una sola vez
        _currentSubject.AddObserver(this);

        Debug.Log("[RivalInventoryUI] UI activada y observer registrado...");
    }

    public void DesactiveUI()
    {
        if (_currentSubject == null) return;

        _currentSubject.RemoveObserver(this);
        _currentSubject = null;

        Debug.Log("[RivalInventoryUI] UI desactivada y observer removido...");
    }


    // Metodo que implementa la interfaz IInventoryObserver
    public void OnInventoryChanged(List<Sprite> items)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < items.Count)
            {
                _slots[i].sprite = items[i];
                _slots[i].enabled = true;
                Debug.Log("Imagen rival cambiada...");
            }
            else
            {
                _slots[i].sprite = null;
                _slots[i].enabled = false;
            }
        }
    }
}
