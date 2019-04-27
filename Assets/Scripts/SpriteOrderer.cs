using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOrderer : MonoBehaviour
{

    public bool activeOrdering = false;
    public bool aboveParent = false;
    public int Offset;
    public int ParentOffset;
    private SpriteRenderer renderer;
    private SpriteRenderer pRenderer;

    public void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        Debug.Assert(renderer != null);

        renderer.sortingOrder = 1 - Mathf.RoundToInt(transform.position.y * 20f) + Offset;
        pRenderer = transform.parent?.GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (activeOrdering)
        {
            renderer.sortingOrder = 1 - Mathf.RoundToInt(transform.position.y * 20f) + Offset;
        }
        if (aboveParent)
        {
            if (pRenderer != null)
            {
                renderer.sortingOrder = pRenderer.sortingOrder + ParentOffset; 
            }
        }
    }
}
