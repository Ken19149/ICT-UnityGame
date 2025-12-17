using UnityEngine;

public class spin : MonoBehaviour
{
    // Make these public so you can edit them in the Inspector
    public float moveSpeed = 3f;
    public float height = 0.2f;
    public float spinSpeed = 50f; // New variable for rotation speed

    Vector3 startPos;

    void Start()
    {
        // 1. Remember exactly where you placed the object when the game starts
        startPos = transform.position;
    }

    void Update()
    {
        // --- BOBBING (UP & DOWN) ---
        // Calculate the new Y based on the START position, not the current position
        float newY = startPos.y + Mathf.Sin(Time.time * moveSpeed) * height;

        // Apply the new position (Keep X and Z the same as the start)
        transform.position = new Vector3(startPos.x, newY, startPos.z);


        // --- SPINNING ---
        // Rotate around the Y axis (Vector3.up)
        // Time.deltaTime ensures it spins at the same speed on fast and slow computers
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }
}
