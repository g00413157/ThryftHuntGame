using UnityEngine;

public class PickupBob : MonoBehaviour
{
    public float bobHeight = 0.22f;
    public float bobSpeed = 2.2f;
    public float spinSpeed = 90f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }
}
