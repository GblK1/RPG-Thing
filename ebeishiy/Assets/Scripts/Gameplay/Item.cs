using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "NewItem", order = 1)]
public class Item : ScriptableObject
{
    [HideInInspector] public enum GlobalType 
    {
        WEAPON,
        CONSUMABLE
    }

    public GlobalType globalType;

    [Header("If weapon")]
    public Vector3 holsteredPositionOffset;
    public Quaternion holsteredRotation;
    [HideInInspector] public enum WeaponType
    {
        MELEE_WEAPON
    }

    public WeaponType weaponType;

    [Header("if Melee Weapon")]
    private int a;
    [HideInInspector] public enum MeleeWeaponType
    {
        ONE_HANDED_SWORD,
        TWO_HANDED_AXE
    }

    public MeleeWeaponType meleeWeaponType;
}