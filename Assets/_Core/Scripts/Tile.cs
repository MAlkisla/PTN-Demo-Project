using UnityEngine;

namespace _Core.Scripts
{
    public class Tile : MonoBehaviour
    {
        public bool TileEmpty { get; private set; }
        [SerializeField] private new SpriteRenderer renderer;
        private int _x;
        private int _y;
        
        public void Init(int x, int y, Color color)
        {
            gameObject.name = $"Tile {x} {y}";
            renderer.color = color;
            _x = x;
            _y = y;
        }

        private void Start()
        {
            TileEmpty = true;
        }

        public void SetEmpty(bool isEmpty)
        {
            TileEmpty = isEmpty;
        }

       
    }
}