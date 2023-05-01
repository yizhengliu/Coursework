using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryEventArgs : EventArgs
{
    public IInventoryItem item;
    public int index;
    public InventoryEventArgs(IInventoryItem item, int index) {
        this.item = item;
        this.index = index;
    }
}
