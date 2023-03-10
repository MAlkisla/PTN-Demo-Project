using System;
using _Core.Scriptables;
using UnityEngine;

namespace _Core.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        private static GameManager _instance;

        public static GameManager Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        #endregion

        private GameState _currentState;

        public GameState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                switch (_currentState)
                {
                    case GameState.Idle:
                        break;
                    case GameState.Building:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public BuildingsStats buildingsStats;
        public SoldiersStats soldiersStats;

        private void Start()
        {
            _currentState = GameState.Idle;
            EventManager.SelectedBuildingForProduction.AddListener(StartBuilding);
            EventManager.ProductionBuildingCompleted.AddListener(EndBuilding);
        }

        private void OnDestroy()
        {
            EventManager.SelectedBuildingForProduction.RemoveListener(StartBuilding);
            EventManager.ProductionBuildingCompleted.RemoveListener(EndBuilding);
        }

        public void StartBuilding(string buildingName)
        {
            CurrentState = GameState.Building;
        }

        public void EndBuilding()
        {
            CurrentState = GameState.Idle;
        }
    }

    public enum GameState
    {
        Idle,
        Building,
    }
}