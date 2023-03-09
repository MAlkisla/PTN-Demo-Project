using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool _tileEmpty;
    public bool tileEmpty => _tileEmpty;
    [SerializeField] private SpriteRenderer _renderer;

    private void Start()
    {
        _tileEmpty = true;
    }

    public void SetEmpty(bool isEmpty)
    {
        _tileEmpty = isEmpty;
    }

    public void Init(int x, int y, Color defaultColor)
    {
        gameObject.name = $"Tile {x} {y}";
        _renderer.color = defaultColor;
        this.x = x;
        this.y = y;
    }

    #region a* pathfinding

    public int x { get; set; }
    public int y { get; set; }
    public int gCost { get; set; }
    public int hCost { get; set; }
    public int fCost { get; set; }

    public Tile cameFromTile;

    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }

    #endregion
}