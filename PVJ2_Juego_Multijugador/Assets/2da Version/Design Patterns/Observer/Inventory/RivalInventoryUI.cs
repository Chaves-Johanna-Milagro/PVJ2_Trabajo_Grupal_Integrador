using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RivalInventoryUI : MonoBehaviour, IInventoryObserver // Clase observadora
{
    private Image[] _slots;

    // Bandera para evitar bucles
    private bool _isRegistered = false;

    // Guardamos referencia al subject del rival
    private InventorySubject _currentSubject;

    void Start()
    {
        _slots = new Image[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _slots[i] = transform.GetChild(i).GetComponent<Image>();
        }

    }

    void Update()
    {
        // Si estaba registrado pero el subject YA NO EXISTE → volver a buscar
        if (_isRegistered && _currentSubject == null)
        {
            _isRegistered = false;
        }

        // Si ya está registrado → no hacer nada
        if (_isRegistered) return;

        // Buscar subjects en la escena
        InventorySubject[] subjects = FindObjectsOfType<InventorySubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            // Solo registrar al subject RIVAL
            if (pv != null && !pv.IsMine)
            {
                Debug.Log("[RivalInventoryUI] Registrado al nuevo Subject del RIVAL");

                s.AddObserver(this);

                _currentSubject = s;    // Guardamos la referencia
                _isRegistered = true;   // Para no repetir

                break;
            }
        }
    }

    public void OnInventoryChanged(List<Sprite> items)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < items.Count)
            {
                _slots[i].sprite = items[i];
                _slots[i].enabled = true;
                Debug.Log("Imagen cambiada...");
            }
            else
            {
                _slots[i].sprite = null;
                _slots[i].enabled = false;
            }
        }
    }
}
