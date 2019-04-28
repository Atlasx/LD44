using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public bool DebugMovement = false;

    [Header("Component References")]
    public Animator anim;

    [Header("Movement Settings")]
    public float MoveSpeed = 1f;
    public float Drag = 0.95f;
    public float SleepSpeed = 0.1f;
    public float MaxMoveSpeed = 5f;
    public float CrouchMoveSpeed = 30f;
    public float InternalGravity = 9.81f;
    public float DamagedBounceHeight = 0.5f;

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
    public KeyCode crouchKey = KeyCode.LeftShift;

    [HideInInspector]
    public Vector2 velocity;
    private Vector2 internalVelocity;
    private bool internalRest = true;
    private float currentMoveSpeed;
    private bool crouching = false;

    public Weapon activeWeapon;

    private Collider2D col;
    private Rigidbody2D rb;
    private Health pHealth;

    private GameObject childSprite;

    public void Start()
    {
        velocity = Vector2.zero;
        internalVelocity = Vector2.zero;
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        pHealth = GetComponent<Health>();
        childSprite = transform.GetChild(0)?.gameObject;

        // Subscribe to our health messages
        UnityAction healthAction = HealthAction;
        pHealth.Subscribe(GetInstanceID(), healthAction);
    }

    public void HealthAction()
    {
        Debug.Log("Logged a health action");
        internalRest = false;
        internalVelocity += Vector2.up * Mathf.Sqrt(2f * InternalGravity * DamagedBounceHeight);
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

    private void OnDrawGizmos()
    {
        if (DebugMovement)
        {
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(internalVelocity.x, internalVelocity.y, 0f));
        }
    }

    private void AnimatorUpdate()
    {
        // Update movement parameters
        anim.SetFloat("MoveX", velocity.x);
        anim.SetFloat("MoveY", velocity.y);
        anim.SetBool("Idle", velocity.sqrMagnitude < SleepSpeed);

        // Crouching
        anim.SetBool("Crouch", crouching);   
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
            pHealth.Harm(1);
        }
    }

    private void MovementUpdate()
    {
        Vector2 desiredMove = Vector2.zero;

        // Read inputs
        currentMoveSpeed = MoveSpeed;

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

        // Crouch
        crouching = false;
        if (Input.GetKey(crouchKey))
        {
            currentMoveSpeed = CrouchMoveSpeed;
            crouching = true;
        }

        //Normalize movement direction
        desiredMove.Normalize();
        desiredMove *= currentMoveSpeed;

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

        rb.velocity = velocity;

        // Update internal child velocity
        if (!internalRest)
        {
            if (childSprite.transform.localPosition.y < 0f)
            {
                internalVelocity = Vector2.zero;
                childSprite.transform.localPosition = Vector3.zero;
                internalRest = true;
            }
            else
            {
                // only add gravity if we need it
                internalVelocity += InternalGravity * Vector2.down * Time.deltaTime;
            }

            // Move our child if we have velocity
            if (internalVelocity.sqrMagnitude > 0.05f)
            {
                childSprite.transform.localPosition += new Vector3(internalVelocity.x, internalVelocity.y, 0f) * Time.deltaTime;
            }
        }
    }

    private static Vector3 To3DVector(Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, 0f);
    }
}
