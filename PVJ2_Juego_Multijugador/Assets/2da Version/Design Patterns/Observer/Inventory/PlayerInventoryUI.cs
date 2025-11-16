using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour, IInventoryObserver // Clase observadora
{
    private Image[] _slots;

    // Bandera para evitar bucles
    private bool _isRegistered = false;

    // Guardamos referencia al subject local
    private InventorySubject _currentSubject;

    void Start()
    {
        // Obtenemos los componentes Image de los hijos 
        // De este modo cambiarlos por los nuevos sprites cuando el jugador obtenga los powerUp
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

            // Solo registrar al subject LOCAL
            if (pv != null && pv.IsMine)
            {
                Debug.Log("[PlayerInventoryUI] Registrado al Subject LOCAL");

                s.AddObserver(this);

                _currentSubject = s;     // guardamos referencia
                _isRegistered = true;    // no repetir

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
