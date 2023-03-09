using UnityEngine;

namespace _Core.Scripts.Grid
{
    public abstract class GridObject : MonoBehaviour
    {
        public string objectName;
        public SpriteRenderer spriteRenderer;

        protected virtual void OnMouseDown()
        {
            Debug.Log("object clicked: " + gameObject.name);
        }
    }
}