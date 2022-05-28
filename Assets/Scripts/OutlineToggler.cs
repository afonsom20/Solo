using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AllIn1SpriteShader;

public class OutlineToggler : MonoBehaviour
{
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void ToggleOutline()
    {
        Color color = image.color;

        if (color.a == 0)
            image.color = new Color (color.r, color.g, color.b, 1);
        else
            image.color = new Color(color.r, color.g, color.b, 0);
    }
}
