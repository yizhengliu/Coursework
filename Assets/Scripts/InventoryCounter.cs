using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCounter : MonoBehaviour
{
    //private const int INITIAL_POS = -442;
    private const int STEP_DISTANCE = 420;
    // Start is called before the first frame update
    private void Start()
    {
        for (int i = Inventory.Instance.CurrentPage; i > 0; i--)
            toRight();
    }


    public void toRight() 
    {
        Inventory.Instance.toRight();
        Vector3 currentPos = transform.position;
        currentPos.x += STEP_DISTANCE;
        transform.position = currentPos;
    }
    public void toLeft()
    {
        Inventory.Instance.toLeft();
        Vector3 currentPos = transform.position;
        currentPos.x -= STEP_DISTANCE;
        transform.position = currentPos;
    }
}
