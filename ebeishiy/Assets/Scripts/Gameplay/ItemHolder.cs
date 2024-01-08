using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    public Item content;
    public int amount;
    public bool droppable;
    public bool stackable;
    public Sprite icon;
    private void Awake()
    {
        
    }
}
