using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpawnableTab : MonoBehaviour
{
    [SerializeField] private GameObject spawnableParentObject;

    [SerializeField] private TMP_Text selectedSpawnableNameText;
    [SerializeField] private Image selectedSpawnableImage;

    private List<Spawnable> _spawnables;
    private Barracks _spawnableBuilding;

    private int _lastUsedIndex;

    public void SetSpawnableInformation(List<Spawnable> spawnables, Barracks spawnableBuilding)
    {
        _spawnableBuilding = spawnableBuilding;
        this._spawnables = spawnables;
        _lastUsedIndex = 0;
        ChangeSpawnableType(0);
        spawnableParentObject.SetActive(true);
    }

    public void CloseSpawnableTab()
    {
        if (spawnableParentObject.activeSelf) spawnableParentObject.SetActive(false);
    }

    public void ChangeSpawnableType(int spawnableIndex)
    {
        _lastUsedIndex += spawnableIndex;
        if (_lastUsedIndex < 0) _lastUsedIndex += _spawnables.Count;
        if (_lastUsedIndex >= _spawnables.Count) _lastUsedIndex = 0;
        selectedSpawnableNameText.text = _spawnables[_lastUsedIndex].objectName;
        selectedSpawnableImage.sprite = _spawnables[_lastUsedIndex].spawnableSprite;
    }

    public void SpawnSpawnable()
    {
        var spawnPointPos = _spawnableBuilding.spawnPoint.position;
        var spawnTile = GridManager.Instance.GetClosestTile(_spawnableBuilding.spriteRenderer.transform.position);
        var toGoTile = GridManager.Instance.GetClosestTile(spawnPointPos);

        if (!spawnTile || !spawnTile.tileEmpty) return;

        var newSoldierObject = PoolManager.Instance.GetPoolObject(_spawnables[_lastUsedIndex].poolObjectType);
        var newSoldier = newSoldierObject.GetComponent<Soldier>();
        newSoldier.transform.position = spawnTile.transform.position - new Vector3(0.5f, 0.5f, 0f);
        newSoldier.onTile = spawnTile;
        newSoldierObject.SetActive(true);
        if (!toGoTile || !toGoTile.tileEmpty) return;
        newSoldier.Move(toGoTile);
        toGoTile.SetEmpty(false);
    }
}