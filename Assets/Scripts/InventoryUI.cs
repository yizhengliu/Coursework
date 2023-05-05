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
    private const int MAIN_CITY = 3;
    private const int CLOSET = 4;

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
    [SerializeField]
    private Button transfer;
    private Color selectedColor = new Color(59f / 255f, 159f / 255f, 255f / 255f);
    [SerializeField]
    private int type;
    private int lastSelectedItem = -1;

    private void Awake()
    {
        if(type == TENT || type == DUNGEON)
            DungeonManager.sceneOver += unsubEvents;
        Inventory.Instance.ItemAdded += refreshUI;
        Inventory.Instance.ItemRemoved += refreshUI;
        Inventory.Instance.ItemUsed += refreshUI;
        if (type == BATTLE)
        {
            Inventory.Instance.ItemUsed += refreshUI;
            BattleSystem.battleStarted += refreshUI;
            BattleSystem.battleStarted += unselect;
        }
        BattleSystem.battleEnd += unsubEvents;
    }

    private void Start()
    {
        refreshUI(null, null);
        if (type == DUNGEON)
            foreach (Button b in buttons)
                b.interactable = false;
    }
    private void unselect(object sender, InventoryEventArgs args)
    {
        battleUnseletAll();
    }
    private void refreshUI(object sender, InventoryEventArgs args)
    {
        showPage();
        if(type != CLOSET)
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
        if (type == CLOSET)
            temp = Inventory.Instance.Closet;
        for (int i = 0; i < Inventory.ITEMS_PER_PAGE; i++)
        {
            if (i < temp.Count)
            {
                //if the item is current already
                if (temp[i].CurrentCooldown == 0)
                    setAttributesProperty(i, SpriteHolder.OCCUPIED_MULTI_TIME_ALREADY, false, true, true);
                //if the item is on cooldown
                else
                {
                    setAttributesProperty(i, SpriteHolder.OCCUPIED_MULTI_TIME_ALREADY, true, false, true);
                    cooldowns[i].text = "Turn Wait " + temp[i].CurrentCooldown;
                }
                //set the content of the chip (the weapon sprite)
                contents[i].sprite = SpriteHolder.Instance.getItemSprite(
                    temp[i].getProperty(IInventoryItem.INDEX));
            }
            else
            {
                contents[i].sprite = null;

                if (type != CLOSET)
                {
                    // if it is empty chip
                    if (i + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)
                        < Inventory.Instance.ItemLimit)
                        setAttributesProperty(i, SpriteHolder.NO_ITEM_SQUARE, false, false, false);
                    //if it is unavailable
                    else
                        setAttributesProperty(i, SpriteHolder.UNAVAILABLE, false, false, false);
                }
                else 
                    setAttributesProperty(i, SpriteHolder.NO_ITEM_SQUARE, false, false, false);
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


        if (type != CLOSET)
        {//if this is selected last time and user want to cancel it
            if (lastSelectedItem == (index + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)))
            {
                chips[index].color = Color.white;
                seeDetail.interactable = false;
                discard.interactable = false;
                if (type == MAIN_CITY)
                    transfer.interactable = false;
                lastSelectedItem = -1;
                return;
            }
        }
        else 
        {
            if (lastSelectedItem == index)
            {
                chips[index].color = Color.white;
                seeDetail.interactable = false;
                discard.interactable = false;
                transfer.interactable = false;
                lastSelectedItem = -1;
                return;
            }
        }
        //if another item has been selected
        if (lastSelectedItem != -1)
            if (type != CLOSET)
                chips[lastSelectedItem - (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)].color = Color.white;
            else
                chips[lastSelectedItem].color = Color.white;
        IInventoryItem item;
        if (type == CLOSET) item = Inventory.Instance.Closet[index];
        else item = Inventory.Instance.getCurrentPageItems()[index];
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
            ? "PHYSICAL" : "MAGICAL");
        chips[index].color = selectedColor;
        seeDetail.interactable = true;
        discard.interactable = true;
        if (type == MAIN_CITY)
            transfer.interactable = !(Inventory.Instance.Closet.Count == Inventory.CLOSET_LIMIT);
        else if (type == CLOSET)
            transfer.interactable = !(Inventory.Instance.Items.Count == Inventory.Instance.ItemLimit);
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
        if (type == MAIN_CITY || type == CLOSET)
            transfer.interactable = false;
    }
    public void transferItem() 
    {
        if (type != CLOSET)
        {
            IInventoryItem itemToTransfer = Inventory.Instance.Items[lastSelectedItem];
            Inventory.Instance.Items.Remove(Inventory.Instance.Items[lastSelectedItem]);
            Inventory.Instance.Closet.Add(itemToTransfer);
        }
        else 
        {
            IInventoryItem itemToTransfer = Inventory.Instance.Closet[lastSelectedItem];
            Inventory.Instance.Closet.Remove(Inventory.Instance.Closet[lastSelectedItem]);
            Inventory.Instance.Items.Add(itemToTransfer);
        }
        tentClosed();
        refreshUI(null, null);
    }

    public void discardItem()
    {
        if (type == CLOSET)
            Inventory.Instance.discardItemCloset(lastSelectedItem);
        else
            Inventory.Instance.removeItem(Inventory.Instance.getCurrentPageItems()[lastSelectedItem]);
        tentClosed();
    }

    private void setAttributesProperty(int index,int chipFrameIndex, bool isCooldown, bool isInteractable, bool isVisiable) 
    {
        chips[index].sprite = SpriteHolder.Instance.getItemChipFrame(chipFrameIndex);
        cooldowns[index]?.gameObject.SetActive(isCooldown);
        buttons[index].interactable = isInteractable;
        contents[index]?.gameObject.SetActive(isVisiable);
    }

    public void battleSeletAll() 
    {
        for (int i = 0; i < chips.Length; i++)
        { 
            if(i + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage) 
                < BattleSystem.SelectedChips.Length)
                if(BattleSystem.SelectedChips[i + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)])
                    chips[i].color = selectedColor;
        }
            
    }

    public void battleUnseletAll() 
    {
        for (int i = 0; i < chips.Length; i++) 
        {
            if (i + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)
                   < BattleSystem.SelectedChips.Length)
                if (!BattleSystem.SelectedChips[i + (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage)])
                    chips[i].color = Color.white;
        }
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

    public void unsubEvents(object sender, int args) 
    {
        Inventory.Instance.ItemAdded -= refreshUI;
        Inventory.Instance.ItemRemoved -= refreshUI;
        Inventory.Instance.ItemUsed -= refreshUI;
        if (type == BATTLE)
        {
            Inventory.Instance.ItemUsed -= refreshUI;
            BattleSystem.battleStarted -= refreshUI;
            BattleSystem.battleStarted -= unselect;
            BattleSystem.battleEnd -= unsubEvents;
        }
    }

    public void unsub() 
    {
        unsubEvents(null, -1);
    }
}
