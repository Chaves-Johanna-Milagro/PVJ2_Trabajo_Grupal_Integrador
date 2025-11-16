using Photon.Pun;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun // Componente del jugador
{
    private InventorySubject _inventorySubject;

    private Sprite _sprite;

    void Start()
    {
        _inventorySubject = GetComponent<InventorySubject>();

        _sprite = transform.Find("Circle").GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
        // Solo el jugador dueño puede sumar puntos
        if (!photonView.IsMine) return;

        // Prueba: añadir al inventario al apretar G
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (_inventorySubject != null)
            {
                Debug.Log("[PlayerInventory] Tecla G presionada → añadiendo item...");
                _inventorySubject.AddItem(_sprite);
            }
            else
            {
                Debug.LogWarning("[PlayerInventory] No hay InventorySubject para añadir items...");
            }
        }
    }
}
