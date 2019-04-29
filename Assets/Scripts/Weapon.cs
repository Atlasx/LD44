using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public enum WeaponOrientation
    {
        Left,
        Right
    }

    [RequireComponent(typeof(Animator))]
    public class Weapon : MonoBehaviour
    {
        public WeaponType type = WeaponType.None;
        public Animator anim;
        public float cooldownTime;
        public float powerupTime;
        public float actionDuration;
        public int damageDealt;
        public Collider2D hitbox;

        public Transform rightHandhold;
        public Transform leftHandhold;

        private float cooldownTimer = 0f;

        public virtual void Use() 
        {
            if (cooldownTimer <= 0f)
            {
                anim.SetTrigger("Use");
                hitbox.enabled = true;
                cooldownTimer = cooldownTime;
            }
        }

        public virtual void Discard() {}

        public void SetOrientation(WeaponOrientation or)
        {
            switch (or)
            {
                case WeaponOrientation.Left:
                    transform.localPosition = leftHandhold.localPosition;
                    anim.SetFloat("Right", -1f);
                    break;
                case WeaponOrientation.Right:
                    transform.localPosition = rightHandhold.localPosition;
                    anim.SetFloat("Right", 1f);
                    break;
            }
        }

        public void Start()
        {
            anim = GetComponent<Animator>();
            hitbox = GetComponent<Collider2D>();
            hitbox.enabled = false;

            SetOrientation(WeaponOrientation.Right);
        }

        public void Update()
        {
            if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) { hitbox.enabled = false; }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            // Ignore the owner of the weapon
            if (!IsColliderParent(collision))
            {
                GameObject other = collision.gameObject;
                Health otherHealth = other.GetComponent<Hitbox>()?.owner.GetComponent<Health>();
                if (otherHealth != null)
                {
                    otherHealth.Harm(damageDealt);
                }
            }
        }

        private bool IsColliderParent(Collider2D col)
        {
            Transform cur = gameObject.transform;
            while (cur.parent != null)
            {
                if (cur.GetComponent<Collider2D>() == col)
                {
                    return true;
                }
                cur = cur.parent;
            }
            return false;
        }
    }
}