using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image fullHeart;
    public Image halfHeart;
    public Image emptyHeart;

    public float heartWidth = 0.25f;
    public float heartPadding = 0.05f;

    public List<Image> healthImages;

    public void SetHealth(int hp, int maxHp)
    {
        int fullHearts = hp / 2;
        int halfHearts = hp % 2;
        int emptyHearts = hp > 0 ? (maxHp - hp) / 2 : maxHp / 2;

        int numHearts = (maxHp / 2);
        float currentPos = -(numHearts * heartWidth + (numHearts - 1) * heartPadding) * 0.5f;

        // Clear old hearts
        for (int i = healthImages.Count - 1; i >= 0; i--)
        {
            Destroy(healthImages[i]);
            healthImages.RemoveAt(i);
        }

        // Add full hearts
        for (int i = 0; i < fullHearts; i++)
        {
            Image heart = Instantiate(fullHeart, new Vector3(currentPos, 0f, 0f) + transform.position, Quaternion.identity, transform);
            healthImages.Add(heart);
            currentPos += heartWidth + heartPadding;
        }

        // Add half heart
        if (halfHearts > 0)
        {
            Image heart = Instantiate(halfHeart, new Vector3(currentPos, 0f, 0f) + transform.position, Quaternion.identity, transform);
            healthImages.Add(heart);
            currentPos += heartWidth + heartPadding;
        }

        // Add empty hearts
        for (int i = 0; i < emptyHearts; i++)
        {
            Image heart = Instantiate(emptyHeart, new Vector3(currentPos, 0f, 0f) + transform.position, Quaternion.identity, transform);
            healthImages.Add(heart);
            currentPos += heartWidth + heartPadding;
        }
    }
}
