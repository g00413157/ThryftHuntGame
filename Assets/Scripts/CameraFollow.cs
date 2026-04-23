using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 3.8f, -11f);
    public float followSmoothTime = 0.14f;
    public float lookHeight = 1.4f;
    public float lookAhead = 18f;
    public float minDistance = 5.5f;
    public float gameplayFov = 74f;

    private Vector3 followVelocity;
    private CharacterController targetController;

    void Start()
    {
        if (player == null)
        {
            GameObject taggedPlayer = GameObject.FindGameObjectWithTag("Player");

            if (taggedPlayer != null)
                player = taggedPlayer.transform;
        }

        if (player != null)
            targetController = player.GetComponent<CharacterController>();

        Camera cameraComponent = GetComponent<Camera>();

        if (cameraComponent != null)
            cameraComponent.fieldOfView = gameplayFov;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        if (targetController == null)
            targetController = player.GetComponent<CharacterController>();

        Vector3 focusPoint = player.position;

        if (targetController != null)
            focusPoint += targetController.center;
        else
            focusPoint += Vector3.up * lookHeight;

        Vector3 desiredPosition = focusPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);

        Vector3 lookTarget = player.position + Vector3.up * lookHeight + Vector3.forward * lookAhead;
        Vector3 lookDirection = lookTarget - transform.position;

        if (lookDirection.sqrMagnitude < minDistance * minDistance)
            lookDirection = (lookTarget - desiredPosition).normalized * minDistance;

        transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
    }
}
