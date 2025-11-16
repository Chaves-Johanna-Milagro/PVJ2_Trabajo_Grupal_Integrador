using Photon.Pun;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun // Componente del jugador
{
    private InventorySubject _inventorySubject;

    // Guardamos una lista de sprites obtenidos de los hijos
    private Sprite[] _childSprites;

    void Start()
    {
        _inventorySubject = GetComponent<InventorySubject>();

        // Obtener todos los sprites de los hijos para comprobar que se muestre el mismo en la UI
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();

        _childSprites = new Sprite[childRenderers.Length];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            _childSprites[i] = childRenderers[i].sprite;
        }

        Debug.Log("[PlayerInventory] Sprites encontrados en hijos: " + _childSprites.Length);
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

                // Elegir sprite random
                Sprite randomSprite = _childSprites[Random.Range(0, _childSprites.Length)];

                Debug.Log("[PlayerInventory] Añadiendo item RANDOM → " + randomSprite.name);

                _inventorySubject.AddItem(randomSprite);
            }
            else
            {
                Debug.LogWarning("[PlayerInventory] No hay InventorySubject para añadir items...");
            }
        }
    }
}
