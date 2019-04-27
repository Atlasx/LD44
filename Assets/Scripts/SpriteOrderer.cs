using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOrderer : MonoBehaviour
{

    public bool activeOrdering = false;
    private SpriteRenderer renderer;

    public void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        Debug.Assert(renderer != null);

        renderer.sortingOrder = 1-Mathf.RoundToInt(transform.position.y * 20f);
    }

    public void Update()
    {
        if (activeOrdering)
        {
            renderer.sortingOrder = 1-Mathf.RoundToInt(transform.position.y * 20f);
        }
    }
}
