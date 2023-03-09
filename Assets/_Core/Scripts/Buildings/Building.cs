using System.Collections;
using _Core.Scripts.Grid;
using _Core.Scripts.HealthSystem;
using _Core.Scripts.Interfaces;
using _Core.Scripts.Managers;
using UnityEngine;

    public class Building : GridObject, IHealth
    {
        public int healthPoints { get; set; }
        private HealthSystem _healthSystem;

        public virtual void TakeDamage(int damageVal)
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

        protected override void OnMouseDown()
        {
            if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
            {
                base.OnMouseDown();
                EventManager.SelectedBuildingForInformation.Invoke(objectName);
            }
        }

        private void Awake()
        {
            _healthSystem = GetComponent<HealthSystem>();
        }

        protected virtual void Start()
        {
            var stats = GameManager.Instance.buildingsStats.GetStats(objectName);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            healthPoints = stats.healthPoints;
            spriteRenderer.sprite = stats.buildingSprite;
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

        public void Die()
        {
            var buildingStats = GameManager.Instance.buildingsStats.GetStats(objectName);
            var transformPos = transform.position;
            for (int x = 0; x < buildingStats.buildingXSize; x++)
            {
                for (int y = 0; y < buildingStats.buildingYSize; y++)
                {
                    var tile = GridManager.Instance.GetTile((int)transformPos.x + x, (int)transformPos.y + y);
                    tile.SetEmpty(true);
                }
            }

            Destroy(gameObject);
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