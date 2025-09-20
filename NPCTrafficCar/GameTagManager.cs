using UnityEngine;

namespace Game.Components
{
    public class GameTagManager : MonoBehaviour
    {
        [SerializeField] private bool car;
        [SerializeField] private bool carDestroy;


        public bool IsCar() => car;
        public bool IsCarDestroy() => carDestroy;
    }
}
