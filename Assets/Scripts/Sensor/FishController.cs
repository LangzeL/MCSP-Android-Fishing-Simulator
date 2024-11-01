using UnityEngine;

public class FishController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionInterval = 2f;

    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        SetRandomTargetPosition();
        timer = changeDirectionInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SetRandomTargetPosition();
            timer = changeDirectionInterval;
        }

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void SetRandomTargetPosition()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHalfHeight = Camera.main.orthographicSize;

        float randomX = Random.Range(-screenHalfWidth + 0.5f, screenHalfWidth - 0.5f);
        float randomY = Random.Range(-screenHalfHeight + 0.5f, screenHalfHeight - 0.5f);

        targetPosition = new Vector3(randomX, randomY, 0f);
    }
}
