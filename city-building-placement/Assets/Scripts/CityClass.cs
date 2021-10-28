using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityClass : MonoBehaviour
{
    public delegate void AttributeUpdate(CityClass city);
    public event AttributeUpdate OnAttributeUpdate;

    public int superficyRadius; //city radius
    public int etendue;
    public int densite;
    public Vector2 position;
    public float partieCentreVille;

    public void OnValidate()
    {
        OnAttributeUpdate?.Invoke(this);
    }
}
