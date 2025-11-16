using System.Collections.Generic;
using UnityEngine;

public interface IInventoryObserver
{
    void OnInventoryChanged(List<Sprite> items);
}

