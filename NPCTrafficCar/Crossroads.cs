using System.Collections.Generic;
using Game.Components;
using UnityEngine;

namespace Game.NPC.Traffic.Cars
{
    public class Crossroads : MonoBehaviour
    {
        private readonly HashSet<GameObject> objectsInCrossroads = new HashSet<GameObject>();
        private Car firstCarInCrossroads;

        private void OnTriggerEnter(Collider other)
        {
            GameTagManager gameTagManager = other.GetComponent<GameTagManager>();
            if (gameTagManager != null && gameTagManager.IsCar())
            {
                var car = other.GetComponent<Car>();
                if (car == null) return;

                if (objectsInCrossroads.Count > 0)
                {
                    car.DownSpeedCollisionCrossroad();
                }

                objectsInCrossroads.Add(other.gameObject);
                
                // Кэшируем первую машину для быстрого доступа
                if (objectsInCrossroads.Count == 1)
                {
                    firstCarInCrossroads = car;
                }

                Debug.Log($"Object entered: {other.gameObject.name}");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            GameTagManager gameTagManager = other.GetComponent<GameTagManager>();
            if (gameTagManager != null && gameTagManager.IsCar())
            {
                objectsInCrossroads.Remove(other.gameObject);
                Debug.Log($"Object exited: {other.gameObject.name}");

                if (objectsInCrossroads.Count > 0)
                {
                    // Находим новую первую машину
                    foreach (var obj in objectsInCrossroads)
                    {
                        var nextCar = obj.GetComponent<Car>();
                        if (nextCar != null)
                        {
                            nextCar.UpSpeedCollisionCrossroad();
                            firstCarInCrossroads = nextCar;
                            break;
                        }
                    }
                }
                else
                {
                    firstCarInCrossroads = null;
                }
            }
        }
    }
}
