using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Sword : Weapon
    {
        public Sword()
        {
            type = WeaponType.Sword;
        }

        public override void Use()
        {
            base.Use();
        }

        public override void Discard()
        {
            base.Discard();
        }
    }
}