using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnabledChecking : MonoBehaviour
{
    [SerializeField]
    private bool isNext;
    private Button pageChanger;
    [SerializeField]
    private GameObject anotherInventory = null;
    // Start is called before the first frame update
    private void Awake()
    {
        pageChanger = GetComponent<Button>();
        checkAvailability();
    }

    public void checkAvailability() 
    {
        if (pageChanger == null)
            return;
        if (isNext)
        {
            if (Inventory.Instance.CurrentPage ==
                Mathf.CeilToInt((float)Inventory.Instance.ItemLimit / (float)Inventory.ITEMS_PER_PAGE) - 1)
                pageChanger.interactable = false;
            else
                pageChanger.interactable = true;
        }
        else
        {
            if (Inventory.Instance.CurrentPage > 0)
                pageChanger.interactable = true;
            else
                pageChanger.interactable = false;
        }
    }

    public void check() { checkAvailability(); }
}
