using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

   void Update()
{
    // 🚨 STOP ALL INPUT WHEN GAME IS PAUSED OR ENDED
    if (GameManager.Instance != null && !GameManager.Instance.IsGameActive)
        return;

    float moveX = Input.GetAxis("Horizontal");
    float moveZ = Input.GetAxis("Vertical");

    Vector3 move = new Vector3(moveX, 0, moveZ);

    if (move.magnitude > 0.1f)
    {
        controller.Move(move * speed * Time.deltaTime);

        Quaternion toRotation = Quaternion.LookRotation(move);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            toRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
}