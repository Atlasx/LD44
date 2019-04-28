using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Sword : Weapon
    {
        [HideInInspector]
        public BoxCollider2D hitbox;

        private float cooldownTimer = 0f;

        public Sword()
        {
            type = WeaponType.Sword;
        }

        public void Start()
        {
            anim = GetComponent<Animator>();
            hitbox = GetComponent<BoxCollider2D>();
            hitbox.enabled = false;
        }

        public void Update()
        {
            if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) { hitbox.enabled = false; }
        }

        public override void Use()
        {
            if (cooldownTimer <= 0f)
            {
                anim.SetTrigger("Use");
                hitbox.enabled = true;
                cooldownTimer = cooldownTime;
            }
        }

        public override void Discard()
        {
            base.Discard();
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Hit Object With Sword");
            // Ignore the player
            if (!collision.gameObject.CompareTag("Player"))
            {
                GameObject other = collision.gameObject;
                Health otherHealth = other.GetComponent<Health>();
                if (otherHealth != null)
                {
                    otherHealth.Harm(damageDealt);
                }
                else
                {
                    Debug.Log("Couldn't find object health");
                }
            }
        }
    }
}