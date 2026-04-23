using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMobile : MonoBehaviour
{
    public float forwardSpeed = 8f;
    public float sideSpeed = 6f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = 0;

        // TOUCH INPUT
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Left side of screen
            if (touch.position.x < Screen.width / 2)
                moveX = -1;
            else
                moveX = 1;
        }

        // ALSO works in editor (mouse)
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
                moveX = -1;
            else
                moveX = 1;
        }

        // Movement
        Vector3 move = transform.forward * forwardSpeed;
        move += transform.right * moveX * sideSpeed;

        controller.Move(move * Time.deltaTime);
    }
}