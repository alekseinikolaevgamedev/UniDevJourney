using System.Collections.Generic;
using Game.Components;
using UnityEngine;

namespace Game.NPC.Traffic.Cars
{
    public class DirectionController : MonoBehaviour
    {
        [Header("Available Directions")]
        [SerializeField] private bool left;
        [SerializeField] private bool straight;
        [SerializeField] private bool right;
        [SerializeField] private float leftTurnDelay;
        [SerializeField] private float rightTurnDelay;

        private List<int> availableDirections = new List<int>(4);
        private int currentDirection = -1;
        private bool hasAvailableDirections;

        private void Start()
        {
            InitializeDirections();
            currentDirection = GetRandomDirection();
        }

        private void InitializeDirections()
        {
            availableDirections.Clear();

            if (left) availableDirections.Add(0);
            if (straight) availableDirections.Add(1);
            if (right) availableDirections.Add(2);
            
            hasAvailableDirections = availableDirections.Count > 0;

            if (!hasAvailableDirections)
            {
                Debug.LogWarning("No available directions set on " + gameObject.name);
            }
        }

        private int GetRandomDirection()
        {
            if (!hasAvailableDirections) return -1;

            return availableDirections[Random.Range(0, availableDirections.Count)];
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<GameTagManager>().IsCar())
            {
                currentDirection = GetRandomDirection();
            }
        }
        
        public float GetLeftTurnDelay() => leftTurnDelay;
        public float GetRightTurnDelay() => rightTurnDelay;
        public int GetCurrentDirectionIndex() => currentDirection;
    }
}
