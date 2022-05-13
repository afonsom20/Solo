using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AllIn1SpriteShader;

public class OutlineToggler : MonoBehaviour
{
    Material mat;

    void Start()
    {
        mat = GetComponent<Image>().material;    
    }

    public void ToggleOutline()
    {
        if (mat.IsKeywordEnabled("OUTBASE_ON"))
            mat.DisableKeyword("OUTBASE_ON");
        else
            mat.EnableKeyword("OUTBASE_ON");
    }
}
