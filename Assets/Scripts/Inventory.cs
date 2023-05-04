using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Inventory : MonoBehaviour
{
    public const int ITEMS_PER_PAGE = 7;
    public const int CLOSET_LIMIT = 7;

    private int itemLimit = 5;
    private static Inventory instance = null;
    private List<IInventoryItem> items = new List<IInventoryItem>();

    private List<IInventoryItem> closet = new List<IInventoryItem>();
    private int currentPage = 0;
    private int pageLimit = 1;

    public List<IInventoryItem> Closet { get { return closet; } }
    public int CurrentPage { get { return currentPage; } }
    public int PageLimit { get { return pageLimit; } }
    public int ItemLimit { get { return itemLimit; } }
    public List<IInventoryItem> Items { get { return items; } }
   


    public static Inventory Instance
    {
        get
        {
            // test if the instance is null
            // if so, try to get it using FindObjectOfType
            if (instance == null)
                instance = FindObjectOfType<Inventory>();

            // if the instance is null again
            // create a new game object
            // attached this class on it
            // set the instance to the new attached Singleton
            // call don't destroy on load

            if (instance == null)
            {
                GameObject gObj = new GameObject();
                gObj.name = "Inventory";
                instance = gObj.AddComponent<Inventory>();
                DontDestroyOnLoad(gObj);
            }
            return instance;
        }
    }
    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            // Then destroy this. This enforces the singleton pattern,
            // meaning there can only ever be one instance of a ScoreManager.
            Destroy(gameObject);
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    public void addItem(int index)
    {
        TextAsset textAsset = Resources.Load("Battle/Weapons/" + index) as TextAsset;
        string[] itemAttributes = textAsset.text.Split('\n');
        IInventoryItem item = new IInventoryItem(itemAttributes, index);
        items.Add(item);
        ItemAdded?.Invoke(this, new InventoryEventArgs(item, items.Count - 1));
    }

    public void addItemCloset(int index)
    {
        TextAsset textAsset = Resources.Load("Battle/Weapons/" + index) as TextAsset;
        string[] itemAttributes = textAsset.text.Split('\n');
        IInventoryItem item = new IInventoryItem(itemAttributes, index);
        closet.Add(item);
        ItemAdded?.Invoke(this, null);
    }
    public void useItem(int indexCount)
    {
        //ItemUsed?.Invoke(this, new InventoryEventArgs(item));
    }
    public void removeItem(IInventoryItem item)
    {
        items.Remove(item);
        ItemRemoved?.Invoke(this, null);
    }

    public void refreshCooldown()
    {
        foreach (IInventoryItem item in items)
            item.resetCooldown();
    }

    public int getSelectedItemsATK(bool[] selectedChips) 
    {
        int totalDmg = 0;
        for (int i = 0; i < selectedChips.Length; i++)
            if (selectedChips[i])
            {
                totalDmg += items[i].getProperty(IInventoryItem.ATK);
                items[i].used();
            }
            else 
                items[i].onCooldownReduced();
        ItemUsed?.Invoke(this, null);
        return totalDmg;
    }

    public int isSelectedItemsPHY(bool[] selectedChips) 
    {
        for (int i = 0; i < selectedChips.Length; i++) 
            if (selectedChips[i])
                //if one of them is magic then all of them are magic
                if (items[i].getProperty(IInventoryItem.PROPERTY) ==
                    IInventoryItem.PROPERTY_MAGIC)
                    return IInventoryItem.PROPERTY_MAGIC;
        return IInventoryItem.PROPERTY_PHYSIC;
    }

    public void lvlUp(int amount) { itemLimit += amount; }

    public List<IInventoryItem> getCurrentPageItems() 
    {
        List<IInventoryItem> temp = new List<IInventoryItem>();
        for (int i = 0 + (CurrentPage * ITEMS_PER_PAGE), j = i + 7; i < j; i++) 
            if (i < items.Count)
                temp.Add(items[i]);
        return temp;
    }
    public bool isInventoryAddable() { return items.Count < itemLimit; }
    public void toRight() { currentPage++; }
    public void toLeft() { currentPage--; }

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public event EventHandler<InventoryEventArgs> ItemUsed;
}
