using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class GridObject : MonoBehaviour
{
    public string objectName;
    public SpriteRenderer spriteRenderer;

    protected virtual void OnMouseDown()
    {
        Debug.Log("object clicked: " + gameObject.name);
    }
}
