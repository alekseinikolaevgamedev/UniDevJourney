using System.Collections;
using Game.Components;
using UnityEngine;

namespace Game.NPC.Traffic.Cars
{
    public class Car : MonoBehaviour
    {
        private float speed;
        private float detectionDistance;
        private float savedSpeed;
        private Vector3 moveDirection;
        private bool crossroadCarStop;
        private LayerMask carLayerMask;
        private bool isCarInFront;
        private Coroutine speedChangeCoroutine;

        public delegate void CarDestroyedHandler(Car car);
        public event CarDestroyedHandler OnCarDestroyed;

        public void Configure(float setSpeed, float setDetectionDistance)
        {
            speed = setSpeed;
            savedSpeed = speed;
            detectionDistance = setDetectionDistance;
            moveDirection = transform.forward;
            carLayerMask = 1 << LayerMask.NameToLayer("Cars");
            StartCoroutine(CheckForCarsCoroutine());
        }

        private IEnumerator CheckForCarsCoroutine()
        {
            if (!crossroadCarStop)
            {
                while (true)
                {
                    CheckForCarsInFront();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        public void Move()
        {
            if (speed > 0)
            {
                transform.position += moveDirection * speed * Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            DirectionController directionController = other.GetComponent<DirectionController>();
            if (directionController != null)
            {
                StartCoroutine(HandleTurn(directionController, directionController.GetCurrentDirectionIndex()));
            }

            GameTagManager gameTagManager = other.GetComponent<GameTagManager>();
            if (gameTagManager != null)
            {
                if (gameTagManager.IsCarDestroy())
                {
                    Destroy(gameObject);
                }
            }
        }

        private void CheckForCarsInFront()
        {
            isCarInFront = IsCarInFront();
            if (isCarInFront && !crossroadCarStop)
            {
                if (speedChangeCoroutine != null)
                {
                    StopCoroutine(speedChangeCoroutine);
                }
                speedChangeCoroutine = StartCoroutine(SmoothSpeedChange(0f, 0.5f));
            }
            else if (!isCarInFront && !crossroadCarStop)
            {
                if (speedChangeCoroutine != null)
                {
                    StopCoroutine(speedChangeCoroutine);
                }
                speedChangeCoroutine = StartCoroutine(SmoothSpeedChange(savedSpeed, 0.5f));
            }
        }

        private bool IsCarInFront()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance, carLayerMask))
            {
                return hit.collider.gameObject != gameObject;
            }
            return false;
        }

        private void OnDestroy()
        {
            OnCarDestroyed?.Invoke(this);
        }

        private IEnumerator HandleTurn(DirectionController directionController, int directionIndex)
        {
            float turnDistance;
            switch (directionIndex)
            {
                case 0: // Влево
                    turnDistance = directionController.GetLeftTurnDelay();
                    break;
                case 1: // Прямо
                    yield break;
                case 2: // Вправо
                    turnDistance = directionController.GetRightTurnDelay();
                    break;
                default:
                    yield break;
            }

            Vector3 targetPosition = transform.position + moveDirection.normalized * turnDistance;

            while (Vector3.Distance(transform.position, targetPosition) > 0.5f)
            {
                yield return null;
            }

            switch (directionIndex)
            {
                case 0: // Влево
                    transform.Rotate(0, -90, 0);
                    break;
                case 1: // Прямо
                    break;
                case 2: // Вправо
                    transform.Rotate(0, 90, 0);
                    break;
            }

            moveDirection = transform.forward;
        }

        public void DownSpeedCollisionCrossroad()
        {
            crossroadCarStop = true;
            
            if (speedChangeCoroutine != null)
            {
                StopCoroutine(speedChangeCoroutine);
            }
            
            speedChangeCoroutine = StartCoroutine(SmoothSpeedChange(0f, 0.25f));
        }

        public void UpSpeedCollisionCrossroad()
        {
            crossroadCarStop = false;
            
            if (speedChangeCoroutine != null)
            {
                StopCoroutine(speedChangeCoroutine);
            }
            
            speedChangeCoroutine = StartCoroutine(SmoothSpeedChange(savedSpeed, 0.25f));
        }

        private IEnumerator SmoothSpeedChange(float targetSpeed, float duration)
        {
            float startSpeed = speed;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                
                // Плавное изменение скорости с использованием SmoothStep
                speed = Mathf.SmoothStep(startSpeed, targetSpeed, t);
                
                yield return null;
            }

            // Гарантируем, что достигли целевой скорости
            speed = targetSpeed;
        }
        
        private void OnDrawGizmos()
        {
            // Устанавливаем цвет гизмос в зависимости от состояния
            Gizmos.color = isCarInFront ? Color.red : Color.green;
            // Рисуем линию впереди машины
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * detectionDistance);
        }
    }
}
