using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour, IInventoryObserver, IPlayerUI // Clase observadora
{
    private Image[] _slots;

    private Button[] _buttons;

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

            // Permite que los slots se liberen al clikearlos
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


    // Se encarga de saber y obtener el componente ScoreSubject del jugador LOCAL
    private InventorySubject FindLocalSubject()
    {
        // Buscar subjects en la escena
        InventorySubject[] subjects = FindObjectsOfType<InventorySubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            // Solo registrar al subject LOCAL
            if (pv != null && pv.IsMine)
            {
                return s;
            }
        }

        return null;
    }

    // Metodos implementados de la interfaz IPlayerUI
    public void ActiveUI()
    {
        // Buscar el subject local
        _currentSubject = FindLocalSubject();

        if (_currentSubject == null)
        {
            Debug.LogWarning("[PlayerInventoryUI] No se encontró Subject local al activar UI...");
            return;
        }

        // Registrar el observer una sola vez
        _currentSubject.AddObserver(this);

        Debug.Log("[PlayerInventoryUI] UI activada y observer registrado...");
    }

    public void DesactiveUI()
    {
        if (_currentSubject == null) return;

        _currentSubject.RemoveObserver(this);
        _currentSubject = null;

        Debug.Log("[PlayerInventoryUI] UI desactivada y observer removido...");
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
