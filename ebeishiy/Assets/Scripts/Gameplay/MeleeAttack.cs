using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    PlayerInputs inputs;
    [SerializeField] private Animator torsoAnim;
    private Inventory inv;
    private int meleeWeaponType;
    

    private void Awake()
    {
        inv = GetComponentInParent<Inventory>();
        torsoAnim = GetComponentInParent<Animator>();
    }

    public void MainAttack()
    {
        switch (inv.equippedWeapon.content.meleeWeaponType)
        {
            case Item.MeleeWeaponType.TWO_HANDED_AXE:
                torsoAnim.SetTrigger("TwoHandedAxeAttack");
                break;
        }
    }
}
