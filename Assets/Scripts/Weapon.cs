using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Weapon
    {
        public GameObject gameObject;

        public WeaponType type;
        public float cooldownTime;
        public float powerupTime;

        public Weapon()
        {
            type = WeaponType.None;
        }

        public virtual void Use()
        {
            Debug.Log("Base Weapon Use");
        }

        public virtual void Discard()
        {
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }
        }
    }
}