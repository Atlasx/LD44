using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    // Needing to add type to the enum is less than ideal
    // but good enough for a game jam
    public enum WeaponType
    {
        None,
        Sword,
        Crossbow
    }

    public class Crossbow : Weapon
    {
        public Crossbow()
        {
            type = WeaponType.Crossbow;
        }

        public override void Discard()
        {
            base.Discard();
        }

        public override void Use()
        {
            Debug.Log("Using a crossbow");
        }
    }
}