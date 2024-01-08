using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Item content;
    public GameObject itemHolder;
    public int amount;
    public bool droppable;
    public TMP_Text itemAmount;
    public int inventorySlot;
    public int placeInInvSection;
    public Sprite icon;
    [HideInInspector] public int numberInTheArray;

    private void Awake()
    {
        
    }
}
