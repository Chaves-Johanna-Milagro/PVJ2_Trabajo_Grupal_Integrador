using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour, IInventoryObserver // Clase observadora
{
    private Image[] _slots;

    private Button[] _buttons;

    // Bandera para evitar bucles
    private bool _isRegistered = false;

    // Guardamos referencia al subject local
    private InventorySubject _currentSubject;

    void Start()
    {       
        // Obtenemos los componentes Image de los hijos 
        // De este modo cambiarlos por los nuevos sprites cuando el jugador obtenga los powerUp
        int count = transform.childCount;

        _slots = new Image[count];
        _buttons = new Button[count];

        for (int i = 0; i < count; i++)
        {
            // Imagen del slot
            _slots[i] = transform.GetChild(i).GetComponent<Image>();

            // Botón del slot
            _buttons[i] = transform.GetChild(i).GetComponent<Button>();

            int index = i;
            _buttons[i].onClick.AddListener(() =>
            {
                Debug.Log($"[PlayerInventoryUI] Liberando slot {index}");

                if (_currentSubject == null) return;

                int max = _currentSubject.GetItemCount();
                if (index >= max) return; // ← evita tocarlos vacíos

                _currentSubject.RemoveItemAt(index);
            });
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
                _buttons[i].interactable = true;
            }
            else
            {
                _slots[i].sprite = null;
                _slots[i].enabled = false;
                _buttons[i].interactable = false;
            }
        }
    }
}
