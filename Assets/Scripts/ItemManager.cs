using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            // Search for existing instance.
            Instance = (ItemManager)FindObjectOfType(typeof(ItemManager));

            // Create new instance if one doesn't already exist.
            if (Instance == null)
            {
                // Need to create a new GameObject to attach the singleton to.
                GameObject singletonObject = new GameObject();
                Instance = singletonObject.AddComponent<ItemManager>();
                singletonObject.name = typeof(ItemManager).ToString() + " (Singleton)";

                // Make instance persistent.
                DontDestroyOnLoad(singletonObject);
            }
        }
    }

    public List<GameObject> weaponItems = new List<GameObject>();
    public List<GameObject> goldItems = new List<GameObject>();
}

