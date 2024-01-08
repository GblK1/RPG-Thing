using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    private Animator anim;
    PlayerInputs inputs;
    private Player player;
    private CameraController cam;
    [Header("PickingUp")]
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private float checkRayDistance;
    private RaycastHit checkForItems;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject invSlotInstance;
    private List<List<InventorySlot>> inventorySections = new List<List<InventorySlot>>();
    [Space]
    [Header("Selecting")]
    [SerializeField] private InventorySlot selectedSlot;
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedItemAmount;
    [Space]
    [Header("UIManagement")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private List<GameObject> inventorySectionsUI;
    [SerializeField] private Text currentSectionText;
    [SerializeField] private Text previousSectionText;
    [SerializeField] private Text nextSectionText;
    [SerializeField] private int currentSection;
    private bool inventoryOpened;
    [Space]
    [Header("WeaponManager")]
    [HideInInspector] public InventorySlot equippedWeapon;
    [SerializeField] private Transform sheath;
    [SerializeField] private Transform handPosition;
    [SerializeField] private bool holstered;
    [HideInInspector] public bool isAttacking;
    [Space]
    private MeleeAttack meleeAttack;

    private void Awake()
    {
        player = GetComponent<Player>();
        cam = GetComponent<CameraController>();

        meleeAttack = GetComponentInChildren<MeleeAttack>();
        anim = GetComponent<Animator>();

        inputs = new PlayerInputs();
        inputs.Enable();

        inputs.Main.Interaction.performed += x => PickUp();
        inputs.Main.Inventory.performed += x => ToggleInventoruUI();
        inputs.Main.Holstering.performed += x => Holster(!holstered);
        inputs.Main.Attack.performed += x => MainAttack();

        inventoryUI.SetActive(false);

        InventorySectionsCreation();

        holstered = true;
    }

    private void FixedUpdate()
    {
        Physics.Raycast(shootingPoint.transform.position, shootingPoint.forward, out checkForItems, checkRayDistance);

        if (checkForItems.collider != null && checkForItems.collider.gameObject.layer == 8)
        {
            ItemHolder item = checkForItems.collider.gameObject.GetComponent<ItemHolder>();

            infoText.text = item.content.name + " " + "(" + item.amount + ")";
        }
        else
        {
            infoText.text = "";
        }
    }

    public void IsAttackingTrue()
    {
        isAttacking = true;
    }

    public void IsAttackingFalse()
    {
        isAttacking = false;
    }

    private void MainAttack()
    {
        if (equippedWeapon != null && inventoryUI.activeSelf == false)
        {
            Holster(false);

            switch(equippedWeapon.content.weaponType)
            {
                case Item.WeaponType.MELEE_WEAPON:
                    meleeAttack.MainAttack();
                    break;
            }

        }
    }

    private void InventorySectionsCreation()
    {
        for (int i = 0; i < inventorySectionsUI.Count; i++)
        {
            List<InventorySlot> listToAdd = new List<InventorySlot>();;

            inventorySections.Add(listToAdd);
        }
    }

    public void AddItem(Item itemToAdd, int amount, GameObject itemHolder)
    {
        int sectionToGo = (int)itemToAdd.globalType;

        for (int i = 0; i < inventorySections[sectionToGo].Count; i++)
        {
            if (inventorySections[sectionToGo][i].content == itemToAdd && itemHolder.GetComponent<ItemHolder>().stackable)
            {
                inventorySections[sectionToGo][i].amount += amount;
                inventorySections[sectionToGo][i].itemAmount.text = "x" + inventorySections[sectionToGo][i].amount;

                return;
            }
        }

        GameObject newItemSlot = Instantiate(invSlotInstance, Vector3.zero, Quaternion.identity, inventorySectionsUI[sectionToGo].transform.GetChild(0));

        RectTransform sectionToGoRectTransform = inventorySectionsUI[sectionToGo].transform.GetChild(0).GetComponent<RectTransform>();

        sectionToGoRectTransform.sizeDelta = new Vector2(sectionToGoRectTransform.sizeDelta.x, sectionToGoRectTransform.sizeDelta.y + 25);
        sectionToGoRectTransform.anchoredPosition = new Vector2(sectionToGoRectTransform.anchoredPosition.x, sectionToGoRectTransform.anchoredPosition.y - 25);

        InventorySlot newItemToAddInvSlotScript = newItemSlot.GetComponent<InventorySlot>();

        newItemToAddInvSlotScript.content = itemToAdd;
        newItemToAddInvSlotScript.amount = amount;

        if (amount > 1)
        {
            newItemToAddInvSlotScript.itemAmount.text = "x" + amount;
        }

        newItemToAddInvSlotScript.itemHolder = itemHolder;
        newItemToAddInvSlotScript.droppable = itemHolder.GetComponent<ItemHolder>().droppable;
        newItemToAddInvSlotScript.inventorySlot = sectionToGo;
        newItemSlot.transform.GetChild(1).GetComponent<Image>().sprite = itemHolder.GetComponent<ItemHolder>().icon;
        newItemSlot.SetActive(true);

        inventorySections[sectionToGo].Add(newItemSlot.GetComponent<InventorySlot>());
    }

    public void SetSlotForInteraction(InventorySlot slotToSet)
    {
        selectedSlot = slotToSet;

        selectedItemName.text = slotToSet.content.name;
        selectedItemAmount.text = "Amount: " + slotToSet.amount.ToString();
    }

    public void DropItem()
    {
        if (selectedSlot.droppable && !isAttacking)
        {
            selectedSlot.itemHolder.GetComponent<spin>().pickedUp = false;

            if (selectedSlot == equippedWeapon)
            {
                EnableWeaponAnimations(false);

                sheath.localPosition = Vector3.zero;
                sheath.localRotation = Quaternion.identity;

                equippedWeapon.itemHolder.transform.SetParent(null);

                selectedItemName.text = "";
                selectedItemAmount.text = "";

                selectedSlot.itemHolder.transform.position = transform.position;
                selectedSlot.itemHolder.SetActive(true);

                selectedSlot.itemHolder.GetComponent<ItemHolder>().amount = 1;

                equippedWeapon.content = null;
                equippedWeapon.itemHolder = null;
                equippedWeapon.transform.GetChild(1).GetComponent<Image>().sprite = null;

                selectedSlot.itemHolder.GetComponent<BoxCollider>().enabled = true;

                selectedSlot = null;

                holstered = true;
            }
            else
            {
                for (int i = 0; i < inventorySections[(int)selectedSlot.content.globalType].Count; i++)
                {
                    if (inventorySections[(int)selectedSlot.content.globalType][i] == selectedSlot)
                    {
                        if (selectedSlot.amount > 1)
                        {
                            selectedSlot.amount--;
                            selectedSlot.itemAmount.text = "x" + selectedSlot.amount;
                            selectedItemAmount.text = selectedSlot.amount.ToString();

                            GameObject droppedItem = Instantiate(selectedSlot.itemHolder);
                            droppedItem.transform.position = transform.position;
                            droppedItem.SetActive(true);

                            droppedItem.GetComponent<ItemHolder>().amount = 1;

                            droppedItem.GetComponent<BoxCollider>().enabled = true;

                            print(droppedItem.name);

                            selectedSlot = null;
                        }
                        else
                        {
                            Destroy(inventorySections[(int)selectedSlot.content.globalType][i].gameObject);
                            inventorySections[(int)selectedSlot.content.globalType].Remove(selectedSlot);

                            selectedItemName.text = "";
                            selectedItemAmount.text = "";

                            selectedSlot.itemHolder.transform.position = transform.position;
                            selectedSlot.itemHolder.SetActive(true);

                            selectedSlot.itemHolder.GetComponent<ItemHolder>().amount = 1;

                            selectedSlot.itemHolder.GetComponent<BoxCollider>().enabled = true;

                            selectedSlot = null;
                        }
                    }
                }
            }
        }
    }

    public void DropStack()
    {
        if (selectedSlot.droppable)
        {
            for (int i = 0; i < inventorySections[(int)selectedSlot.content.globalType].Count; i++)
            {
                if (inventorySections[(int)selectedSlot.content.globalType][i] == selectedSlot)
                {
                    Destroy(inventorySections[(int)selectedSlot.content.globalType][i].gameObject);
                    inventorySections[(int)selectedSlot.content.globalType].Remove(selectedSlot);

                    selectedItemName.text = "";
                    selectedItemAmount.text = "";

                    selectedSlot.itemHolder.transform.position = transform.position;
                    selectedSlot.itemHolder.SetActive(true);

                    selectedSlot.itemHolder.GetComponent<ItemHolder>().amount = selectedSlot.amount;

                    checkForItems.collider.gameObject.GetComponent<BoxCollider>().enabled = true;

                    selectedSlot = null;
                }
            }
        }
    }

    public void EquipWeapon()
    {
        if (selectedSlot.content.globalType == Item.GlobalType.WEAPON && !isAttacking)
        {
            sheath.localPosition = Vector3.zero;
            sheath.localRotation = Quaternion.identity;

            InventorySlot _selectedSlot = selectedSlot;

            if (equippedWeapon.content != null)
            {
                selectedSlot = equippedWeapon;
                UnequipWeapon();
            }

            selectedSlot = _selectedSlot;

            selectedSlot.itemHolder.SetActive(false);
            selectedItemName.text = "";
            selectedItemAmount.text = "";

            equippedWeapon.content = selectedSlot.content;
            equippedWeapon.amount = selectedSlot.amount;
            equippedWeapon.itemHolder = selectedSlot.itemHolder;
            equippedWeapon.droppable = selectedSlot.droppable;
            equippedWeapon.transform.GetChild(1).GetComponent<Image>().sprite = selectedSlot.itemHolder.GetComponent<ItemHolder>().icon;
            equippedWeapon.content.name = selectedSlot.content.name;

            for (int i = 0; i < inventorySections[(int)selectedSlot.content.globalType].Count; i++)
            {
                if (inventorySections[(int)selectedSlot.content.globalType][i] == selectedSlot)
                {
                    Destroy(inventorySections[(int)selectedSlot.content.globalType][i].gameObject);
                }
            }

            equippedWeapon.itemHolder.transform.rotation = sheath.rotation;

            SetSlotForInteraction(equippedWeapon);

            Holster(holstered);
        }
    }

    public void UnequipWeapon()
    {
        if (selectedSlot == equippedWeapon && !isAttacking)
        {
            EnableWeaponAnimations(false);

            sheath.localPosition = Vector3.zero;
            sheath.localRotation = Quaternion.identity;

            selectedSlot.itemHolder.SetActive(false);

            AddItem(equippedWeapon.content, equippedWeapon.amount, equippedWeapon.itemHolder);

            InventorySlot slotForInteraction;

            for (int i = 0; i < inventorySections[(int)equippedWeapon.content.globalType].Count; i++)
            {
                if (inventorySections[(int)equippedWeapon.content.globalType][i] == equippedWeapon)
                {
                    slotForInteraction = inventorySections[(int)equippedWeapon.content.globalType][i];

                    SetSlotForInteraction(slotForInteraction);
                }
            }

            equippedWeapon.itemHolder.transform.parent = null;
            equippedWeapon.content = null;
            equippedWeapon.amount = 0;
            equippedWeapon.itemHolder = null;
            equippedWeapon.transform.GetChild(1).GetComponent<Image>().sprite = null;
        }
    }

    private void Holster(bool _holster)
    {
        if (!isAttacking)
        {
            equippedWeapon.itemHolder.SetActive(true);

            if (_holster)
            {
                sheath.localPosition = Vector3.zero;
                sheath.localRotation = Quaternion.identity;

                equippedWeapon.itemHolder.transform.position = sheath.position;
                equippedWeapon.itemHolder.transform.rotation = sheath.rotation;

                equippedWeapon.itemHolder.transform.parent = sheath;

                sheath.localPosition = equippedWeapon.content.holsteredPositionOffset;
                sheath.localRotation = equippedWeapon.content.holsteredRotation;

                EnableWeaponAnimations(false);
            }
            else
            {
                sheath.localPosition = Vector3.zero;
                sheath.localRotation = Quaternion.identity;

                equippedWeapon.itemHolder.transform.position = handPosition.position;
                equippedWeapon.itemHolder.transform.rotation = handPosition.rotation;

                equippedWeapon.itemHolder.transform.parent = handPosition;

                EnableWeaponAnimations(true);
            }

            holstered = _holster;
        }
    }

    private void EnableWeaponAnimations(bool enable)
    {
        anim.SetBool("WeildingSmth", enable);

        switch (equippedWeapon.content.weaponType)
        {
            case Item.WeaponType.MELEE_WEAPON:
                switch (equippedWeapon.content.meleeWeaponType)
                {
                    case Item.MeleeWeaponType.TWO_HANDED_AXE:
                        anim.SetBool("TwoHandedAxeInHands", enable);
                        break;
                }
                break;
        }
    }

    private void PickUp()
    {
        if (!isAttacking)
        {
            if (checkForItems.collider != null)
            {
                if (checkForItems.collider.GetComponent<ItemHolder>() != null)
                {
                    checkForItems.collider.gameObject.GetComponent<spin>().pickedUp = true;
                    checkForItems.collider.gameObject.SetActive(false);
                    checkForItems.collider.gameObject.transform.rotation = Quaternion.identity;

                    AddItem(checkForItems.collider.GetComponent<ItemHolder>().content, checkForItems.collider.GetComponent<ItemHolder>().amount, checkForItems.collider.gameObject);

                    checkForItems.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }

    public void NavigatingThroughSections(bool direction)
    {
        if (direction == true)
        {
            if (currentSection < inventorySectionsUI.Count - 1)
            {
                inventorySectionsUI[currentSection].SetActive(false);
                currentSection++;
                inventorySectionsUI[currentSection].SetActive(true);
            }
        }
        else if (direction == false)
        {
            if (currentSection > 0)
            {
                inventorySectionsUI[currentSection].SetActive(false);
                currentSection--;
                inventorySectionsUI[currentSection].SetActive(true);
            }
        }

        SectionNamesChange();

        if (selectedSlot != null)
        {
            selectedItemName.text = "";
            selectedItemAmount.text = "";

            selectedSlot.itemHolder.transform.position = Vector3.zero;
            selectedSlot.itemHolder.SetActive(false);

            selectedSlot = null;
        }
    }

    private void SectionNamesChange()
    {
        currentSectionText.text = inventorySectionsUI[currentSection].name;

        if (currentSection == 0)
        {
            previousSectionText.text = " ";
        }
        else
        {
            previousSectionText.text = inventorySectionsUI[currentSection - 1].name;
        }

        if (currentSection == inventorySectionsUI.Count - 1)
        {
            nextSectionText.text = " ";
        }
        else
        {
            nextSectionText.text = inventorySectionsUI[currentSection + 1].name;
        }
    }

    public void ToggleInventoruUI()
    {
        selectedItemName.text = "";
        selectedItemAmount.text = "";

        inventoryUI.SetActive(!inventoryUI.activeSelf);
        inventorySectionsUI[currentSection].SetActive(inventoryUI.activeSelf);

        if (inventoryUI.activeSelf == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        SectionNamesChange();
        cam.moveCamera = !inventoryUI.activeSelf;
    }
}
