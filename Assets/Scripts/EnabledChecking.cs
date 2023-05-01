using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnabledChecking : MonoBehaviour
{
    [SerializeField]
    private bool isNext;
    private Button pageChanger;
    // Start is called before the first frame update
    private void Start()
    {
        pageChanger = GetComponent<Button>();
        checkAvailability();
    }

    public void checkAvailability() 
    {
        if (isNext)
        {
            if (Inventory.Instance.CurrentPage == Inventory.Instance.PageLimit - 1)
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

}
