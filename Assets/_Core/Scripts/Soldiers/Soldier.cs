using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Soldier : Spawnable, IHealth, IDamage
{
    public PoolObjectType PoolObjectType { get; set; }
    public int healthPoints { get; set; }
    public int damagePoints { get; set; }
    
    public float attackRate { get; set; }
    private HealthSystem _healthSystem;
    
    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
    }
    
    public void TakeDamage(int damageVal)
    {
        healthPoints -= damageVal;
        _healthSystem.Damage(damageVal);
        if (healthPoints <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(TakeDamageFlashSprite());
    }

    public void InflictDamage(int damageVal, IHealth target)
    {
        target.TakeDamage(damageVal);
    }

    public Tile onTile { get; set; }

    private List<Tile> _path;
    private bool _isMoving;

    private bool _attacking;

    private void OnEnable()
    {
        var stats = GameManager.Instance.soldiersStats.GetStats(objectName); //ObjectName only serialized before initialization on soldiers.
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        healthPoints = stats.healthPoints;
        damagePoints = stats.damagePoints;
        attackRate = stats.attackRate;
        spriteRenderer.sprite = stats.soldierSprite;
    }

    public void Move(Tile toGoTile)
    {
        if (_isMoving) return;
        _path = GridManager.Instance.FindPath(onTile.x, onTile.y, toGoTile.x, toGoTile.y);
        if (_path == null) return;
        _isMoving = true;
        StartCoroutine(StartMoving());
    }

    IEnumerator StartMoving()
    {
        onTile.SetEmpty(true);
        onTile = _path[_path.Count-1];
        onTile.SetEmpty(false);
        var cellOffset = GridManager.Instance.cellSize / 2f;
        while (_path.Count > 0)
        {
            var moveDuration = 0.25f;
            for (float t = 0; t < moveDuration; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, _path[0].transform.position - new Vector3(cellOffset, cellOffset, 0f), t/moveDuration);
                yield return null;
            }
            _path.RemoveAt(0);
            yield return null;
        }
        
        _isMoving = false;
    }
    
    protected override void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            base.OnMouseDown();
            EventManager.SelectedSoldierForInformation.Invoke(this);
        }
    }
    
    private void OnMouseOver()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EventManager.ToBeAttackedObjectSelected.Invoke(transform.position, this);
            }
        }
    }

    public void Attack(Vector3 attackingPos, IHealth attackingUnit)
    {
        var movingTile = GridManager.Instance.GetClosestTile(attackingPos);
        Move(movingTile);
        _attacking = true;
        StartCoroutine(StartAttacking(attackingUnit));
    }

    IEnumerator StartAttacking(IHealth attackingUnit)
    {
        while (_isMoving)
        {
            yield return null;
        }
        while (attackingUnit as Object && _attacking && !_isMoving)
        {
            InflictDamage(damagePoints, attackingUnit);
            if (attackingUnit.healthPoints <= 0)
            {
                _attacking = false;
                yield break;
            }
            yield return new WaitForSeconds(attackRate);
        }
    }

    public void StopAttacking()
    {
        _attacking = false;
    }

    public void Die()
    {
        onTile.SetEmpty(true);
        PoolManager.Instance.CoolObject(gameObject, PoolObjectType);
    }
    IEnumerator TakeDamageFlashSprite()
    {
        var col = spriteRenderer.color;
        var defAlpha = col.a;
        col.a = 0f;
        spriteRenderer.color = col;
        yield return new WaitForSeconds(0.1f);
        col.a = defAlpha;
        spriteRenderer.color = col;
    }
}
