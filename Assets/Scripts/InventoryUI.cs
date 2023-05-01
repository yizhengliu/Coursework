using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : MonoBehaviour
{

    private const int TENT = 0;
    private const int DUNGEON = 1;
    private const int BATTLE = 2;

    [SerializeField]
    private Image[] chips;
    [SerializeField]
    private Image[] contents;
    [SerializeField]
    private TextMeshProUGUI[] cooldowns;
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private Image[] inventoryCounters;

    [SerializeField]
    private TextMeshProUGUI message;

    [SerializeField]
    private Button seeDetail;
    [SerializeField]
    private Button discard;
    private Color selectedColor = new Color(59f / 255f, 159f / 255f, 255f / 255f);
    [SerializeField]
    private int type;
    private int lastSelectedItem = -1;

    private void Awake()
    {
        Inventory.Instance.ItemAdded += refreshUI;
        Inventory.Instance.ItemRemoved += refreshUI;
        Inventory.Instance.ItemUsed += refreshUI;
    }
    private void Start()
    {
        refreshUI(null, null);
        if(type == DUNGEON)
            foreach (Button b in buttons)
                b.interactable = false;
    }

    private void refreshUI(object sender, InventoryEventArgs args) 
    {
        showPage();
        showIndicator();
    }

    public void showIndicator()
    {
        List<IInventoryItem> temp = Inventory.Instance.Items;
        for (int i = 0; i < inventoryCounters.Length; i++) {
            if (i < temp.Count)
            {
                inventoryCounters[i].color = Color.white;
                if (temp[i].CurrentCooldown == 0)
                    inventoryCounters[i].sprite =
                        SpriteHolder.Instance.getItemChipFrame(SpriteHolder.OCCUPIED_MULTI_TIME_ALREADY);
                else
                    inventoryCounters[i].sprite =
                       SpriteHolder.Instance.getItemChipFrame(SpriteHolder.OCCUPIED_MULTI_TIME_ON_COOLDOWN);
            }
            else 
            {
                if (i < Inventory.Instance.ItemLimit)
                {
                    inventoryCounters[i].color = Color.white;
                    inventoryCounters[i].sprite =
                     SpriteHolder.Instance.getItemChipFrame(SpriteHolder.NO_ITEM_ROUND);
                }
                //if it is unavailable
                else
                    inventoryCounters[i].color = Color.black;

            }
        }
    }

    public void showPage() {
        List<IInventoryItem> temp = Inventory.Instance.getCurrentPageItems();
        for (int i = 0; i < Inventory.ITEMS_PER_PAGE; i++)
        {
            if (i < temp.Count)
            {
                //if the item is current already
                if (temp[i].CurrentCooldown == 0)
                    setAttributesProperty(i, SpriteHolder.OCCUPIED_MULTI_TIME_ALREADY, false, true);
                //if the item is on cooldown
                else
                {
                    setAttributesProperty(i, SpriteHolder.OCCUPIED_MULTI_TIME_ALREADY, true, false);
                    cooldowns[i].text = "Turn Wait " + temp[i].CurrentCooldown;
                }
                //set the content of the chip (the weapon sprite)
                contents[i].sprite = SpriteHolder.Instance.getItemSprite(
                    temp[i].getProperty(IInventoryItem.INDEX));
            }
            else
            {
                contents[i].sprite = null;
                // if it is empty chip
                if (i + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)
                    < Inventory.Instance.ItemLimit)
                    setAttributesProperty(i, SpriteHolder.NO_ITEM_SQUARE, false, false);
                //if it is unavailable
                else
                    setAttributesProperty(i, SpriteHolder.UNAVAILABLE, false, false);
            }
        }
    }

    public void tentPageChanged() 
    {
        if (lastSelectedItem == -1)
            return;
        for (int i = Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage,
            end = i + Inventory.ITEMS_PER_PAGE; i < end; i++) 
                chips[i - Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage].color
                    = (i == lastSelectedItem ? selectedColor : Color.white);
    }

    public void tentItemSelected(int index) 
    {
        //if this is selected last time and user want to cancel it
        if (lastSelectedItem == (index + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)))
        {
            chips[index].color = Color.white;
            seeDetail.interactable = false;
            discard.interactable = false;
            lastSelectedItem = -1;
            return;
        }
        //if another item has been selected
        if (lastSelectedItem != -1)
            chips[lastSelectedItem - (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)].color = Color.white;
        IInventoryItem item = Inventory.Instance.getCurrentPageItems()[index];
        message.text = item.Name + "\n" + 
            (item.getProperty(IInventoryItem.ATK) == 0 ? "" : (
            "ATK = " + item.getProperty(IInventoryItem.ATK) + "\n"))
             +
             (item.getProperty(IInventoryItem.DEF) == 0 ? "" : (
            "DEF = " + item.getProperty(IInventoryItem.DEF) + "\n"))
             +
             (item.getProperty(IInventoryItem.COOLDOWN) == 0 ? "Cooldown = immediate\n" : (
            "Cooldown = " + item.getProperty(IInventoryItem.COOLDOWN) + " turns\n"))
            +
            "PROPERTY = " + 
            ((item.getProperty(IInventoryItem.PROPERTY) == IInventoryItem.PROPERTY_PHYSIC)
            ? "PHYSICAL":"MAGICAL");
        chips[index].color = selectedColor;
        seeDetail.interactable = true;
        discard.interactable = true;
        lastSelectedItem = index + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage);
    }
    //reset selection
    public void tentClosed() 
    {
        foreach (Image i in chips)
            i.color = Color.white;
        lastSelectedItem = -1;
        seeDetail.interactable = false;
        discard.interactable = false;
    }

    public void discardItem()
    {
        Inventory.Instance.removeItem(Inventory.Instance.getCurrentPageItems()[lastSelectedItem]);
        tentClosed();
    }

    private void setAttributesProperty(int index,int chipFrameIndex, bool isCooldown, bool isInteractable) 
    {
        chips[index].sprite = SpriteHolder.Instance.getItemChipFrame(chipFrameIndex);
        cooldowns[index].enabled = isCooldown;
        buttons[index].interactable = isInteractable;
        contents[index].gameObject.SetActive(isInteractable);
    }

    public void battleSeletAll() 
    {
        for (int i = 0; i < chips.Length; i++)
            chips[i].color = selectedColor;
    }

    public void battleUnseletAll() 
    {
        for (int i = 0; i < chips.Length; i++)
            chips[i].color = Color.white;
    }

    public void battleItemSelected(int index) 
    {
        //if it is already selected
        if (BattleSystem.SelectedChips[index + 
            (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)])
        {
            chips[index].color = Color.white;
            return;
        }
        chips[index].color = selectedColor;
    }

    public void battlePageChanged() 
    {
        for (int start = Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage,
            end = start + Inventory.ITEMS_PER_PAGE; start < end; start++)
            if (BattleSystem.SelectedChips[start])
                chips[start].color = selectedColor;
            else
                chips[start].color = Color.white;
    }

}
