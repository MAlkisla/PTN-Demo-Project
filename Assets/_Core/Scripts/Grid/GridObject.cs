using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    public string objectName;
    public SpriteRenderer spriteRenderer;

    protected virtual void OnMouseDown()
    {
        Debug.Log("object clicked: " + gameObject.name);
    }
}