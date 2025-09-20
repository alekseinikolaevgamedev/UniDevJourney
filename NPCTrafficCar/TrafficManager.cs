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
        private Dictionary<Transform, int> spawnPointCounters = new Dictionary<Transform, int>();

        private void Start()
        {
            // Инициализируем счетчики для всех точек спавна
            foreach (Transform spawnPoint in spawnPoints)
            {
                spawnPointCounters[spawnPoint] = 0;
            }
            
            StartCoroutine(SpawnCars());
        }

        private IEnumerator SpawnCars()
        {
            while (true)
            {
                if (cars.Count < maxCars)
                {
                    GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
                    Transform spawnPoint = GetAvailableSpawnPoint();

                    if (spawnPoint != null)
                    {
                        GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
                        Car carScript = car.AddComponent<Car>();
                        carScript.Configure(speed, detectionDistance);
                        carScript.OnCarDestroyed += RemoveCar; 
                        cars.Add(carScript);
                        
                        // Увеличиваем счетчик для выбранной точки
                        spawnPointCounters[spawnPoint]++;
                    }
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        private Transform GetAvailableSpawnPoint()
        {
            List<Transform> availablePoints = new List<Transform>();
            
            // Собираем все точки, где счетчик меньше 2
            foreach (var point in spawnPoints)
            {
                if (spawnPointCounters[point] < 2)
                {
                    availablePoints.Add(point);
                }
            }
            
            // Если есть доступные точки, выбираем случайную
            if (availablePoints.Count > 0)
            {
                return availablePoints[Random.Range(0, availablePoints.Count)];
            }
            
            // Если все точки заняты (все счетчики >= 2), сбрасываем все счетчики и выбираем случайную точку
            ResetAllSpawnCounters();
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        
        private void ResetAllSpawnCounters()
        {
            foreach (Transform point in spawnPoints)
            {
                spawnPointCounters[point] = 0;
            }
        }
        
        private void RemoveCar(Car car)
        {
            // Находим точку спавна этой машины и уменьшаем счетчик
            foreach (var point in spawnPoints)
            {
                // Проверяем расстояние до точки спавна (с небольшой погрешностью)
                if (Vector3.Distance(car.transform.position, point.position) < 0.1f)
                {
                    spawnPointCounters[point] = Mathf.Max(0, spawnPointCounters[point] - 1);
                    break;
                }
            }
            
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
