using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Animator))]
    public class Weapon : MonoBehaviour
    {
        public WeaponType type = WeaponType.None;
        public Animator anim;
        public float cooldownTime;
        public float powerupTime;
        public float actionDuration;
        public int damageDealt;

        public virtual void Use() {}
        public virtual void Discard() {}
    }
}