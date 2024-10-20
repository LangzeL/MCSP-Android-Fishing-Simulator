using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public float swimSpeed = 2.0f;
    public float directionChangeInterval = 1.0f;
    private Vector3 swimDirection;

    void Start()
    {
        InvokeRepeating("ChangeDirection", 0f, directionChangeInterval);
    }

    void ChangeDirection()
    {
        // Generate a random direction
        float xDirection = Random.Range(-1f, 1f);
        float yDirection = Random.Range(-1f, 1f);
        float zDirection = Random.Range(-1f, 1f);
        swimDirection = new Vector3(xDirection, yDirection, zDirection).normalized;
    }

    void Update()
    {
        transform.position += swimDirection * swimSpeed * Time.deltaTime;
    }
}
