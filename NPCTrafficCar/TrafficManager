using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.NPC.Traffic.Cars
{
    public class TrafficManager : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;
        
        [Space(10)]
        [Header("Car Settings")]
        [SerializeField] private GameObject[] carPrefabs; 
        [SerializeField] private int maxCars; 
        [SerializeField] private float spawnInterval;
        [SerializeField] private float speed;
        [SerializeField] private float detectionDistance;

        private List<Car> cars = new List<Car>(); 

        private void Start()
        {
            StartCoroutine(SpawnCars());
        }

        private IEnumerator SpawnCars()
        {
            while (true)
            {
                if (cars.Count < maxCars)
                {
                    GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                    GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
                    Car carScript = car.AddComponent<Car>();
                    carScript.Configure(speed, detectionDistance);
                    carScript.OnCarDestroyed += RemoveCar; 
                    cars.Add(carScript);
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        private void RemoveCar(Car car)
        {
            cars.Remove(car); 
        }

        private void Update()
        {
            foreach (var car in cars)
            {
                car.Move(); 
            }

            cars.RemoveAll(car => car == null);
        }
    }
}
