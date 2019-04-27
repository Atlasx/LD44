using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MoveSpeed = 1f;
    public float Drag = 0.95f;
    public float SleepSpeed = 0.1f;
    public float MaxMoveSpeed = 5f;

    [Header("Control Settings")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode leftAltKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode rightAltKey = KeyCode.RightArrow;
    public KeyCode upKey = KeyCode.W;
    public KeyCode upAltKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.S;
    public KeyCode downAltKey = KeyCode.DownArrow;
    public KeyCode grabKey = KeyCode.E;
    public KeyCode attackKey = KeyCode.Space;
    public KeyCode discardKey = KeyCode.Q;

    [HideInInspector]
    public Vector2 velocity;

    public Weapon activeWeapon;

    private Collider2D collider;
    private Rigidbody2D rigidbody;
    private Animator anim;
    private Health pHealth;

    public void Start()
    {
        velocity = Vector2.zero;
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pHealth = GetComponent<Health>();
    }

    public void Update()
    {
        WeaponUpdate();
        AnimatorUpdate();
    }

    public void FixedUpdate()
    {
        MovementUpdate();
    }

    private void AnimatorUpdate()
    {
        // Update movement parameters
        anim.SetFloat("MoveX", velocity.x);
        anim.SetFloat("MoveY", velocity.y);
        anim.SetBool("Idle", velocity.sqrMagnitude < SleepSpeed);    
    }

    private void WeaponUpdate()
    {
        // Grabbing weapons
        if (Input.GetKeyDown(grabKey))
        {
            Debug.Log("Attempt to grab closest weapon");

            List<GameObject> weaponList = ItemManager.Instance.weaponItems;
            int nearestIndex = -1;
            float nearestSqrDist = float.MaxValue;
            for (int i = 0; i < weaponList.Count; i++)
            {
                float sqrDist = (weaponList[i].transform.position - transform.position).sqrMagnitude;
                if (sqrDist < nearestSqrDist)
                {
                    nearestIndex = i;
                    nearestSqrDist = sqrDist;
                }
            }

            // No weapons on the ground
            if (nearestIndex == -1)
            {
                return;
            }

            // Check for replacement
            if (activeWeapon.type == WeaponType.None)
            {
                return;
            }
        }

        // Using weapons
        if (Input.GetKeyDown(attackKey))
        {
            activeWeapon.Use();
        }

        // Discarding weapons
        if (Input.GetKeyDown(discardKey))
        {
            activeWeapon.Discard();
        }
    }

    private void MovementUpdate()
    {
        Vector2 desiredMove = Vector2.zero;

        // Read inputs

        // Left
        if (Input.GetKey(leftKey) || Input.GetKey(leftAltKey))
        {
            desiredMove += Vector2.left;
        }

        // Right
        if (Input.GetKey(rightKey) || Input.GetKey(rightAltKey))
        {
            desiredMove += Vector2.right;
        }

        // Up
        if (Input.GetKey(upKey) || Input.GetKey(upAltKey))
        {
            desiredMove += Vector2.up;
        }

        // Down
        if (Input.GetKey(downKey) || Input.GetKey(downAltKey))
        {
            desiredMove += Vector2.down;
        }

        //Normalize movement direction
        desiredMove.Normalize();
        desiredMove *= MoveSpeed;

        // Update player velocity
        velocity += desiredMove * Time.deltaTime;
        if (velocity.sqrMagnitude > SleepSpeed)
        {
            velocity *= Drag;
        }
        else
        {
            velocity = Vector2.zero;
        }

        rigidbody.velocity = velocity;

        //Vector2 delta = velocity * Time.deltaTime;

        // Apply velocity to player
        //transform.position += To3DVector(delta);
    }

    private static Vector3 To3DVector(Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, 0f);
    }
}
