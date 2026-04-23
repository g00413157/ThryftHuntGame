using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class PlayerLaneRunner : MonoBehaviour
{
    public float forwardSpeed = 8f;
    public float laneDistance = 2.5f;
    public float laneChangeSpeed = 10f;

    public float jumpForce = 7f;
    public float gravity = -20f;

    public float slideDuration = 0.8f;
    public float slideHeight = 1f;
    public float swipeThreshold = 30f;
    public float obstacleHitPadding = 0.1f;

    private CharacterController controller;
    private int currentLane = 0;

    private float verticalVelocity;
    private bool isSliding;
    private float slideTimer;

    private float normalHeight;
    private Vector3 normalCenter;

    private float speedBoostTimer;

    private Vector2 swipeStart;
    private bool isSwiping;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        normalHeight = controller.height;
        normalCenter = controller.center;

        Vector3 pos = transform.position;
        pos.x = 0;
        pos.y = Mathf.Max(pos.y, 1.05f);
        transform.position = pos;
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive)
            return;

        HandleTouchInput();
        HandleKeyboardInput();

        HandleMovement();
        CheckObstacleHits();
        HandleSlide();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Obstacle obstacle = hit.gameObject.GetComponentInParent<Obstacle>();
        if (obstacle != null)
            obstacle.HandlePlayerHit(gameObject);
    }

    void CheckObstacleHits()
    {
        Vector3 worldCenter = transform.TransformPoint(controller.center);
        float halfHeight = Mathf.Max(0f, controller.height * 0.5f - controller.radius);

        Vector3 top = worldCenter + Vector3.up * halfHeight;
        Vector3 bottom = worldCenter - Vector3.up * halfHeight;
        float radius = controller.radius + obstacleHitPadding;

        Collider[] hits = Physics.OverlapCapsule(
            top,
            bottom,
            radius,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform)
                continue;

            Obstacle obstacle = hit.GetComponentInParent<Obstacle>();
            if (obstacle != null)
                obstacle.HandlePlayerHit(gameObject);
        }
    }

    void HandleTouchInput()
    {
        if (EventSystem.current != null)
        {
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
                isSwiping = true;
            }

            if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                Vector2 delta = touch.position - swipeStart;
                isSwiping = false;

                if (delta.magnitude < swipeThreshold)
                    return;

                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 0)
                        MoveRight();
                    else
                        MoveLeft();
                }
                else
                {
                    if (delta.y > 0)
                        Jump();
                    else
                        Slide();
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 delta = (Vector2)Input.mousePosition - swipeStart;
            isSwiping = false;

            if (delta.magnitude < swipeThreshold)
                return;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                    MoveRight();
                else
                    MoveLeft();
            }
            else
            {
                if (delta.y > 0)
                    Jump();
                else
                    Slide();
            }
        }
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            Jump();

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Slide();
    }

    void MoveLeft()
    {
        currentLane = Mathf.Max(-1, currentLane - 1);
    }

    void MoveRight()
    {
        currentLane = Mathf.Min(1, currentLane + 1);
    }

    void HandleMovement()
    {
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        float speed = speedBoostTimer > 0 ? forwardSpeed * 1.5f : forwardSpeed;
        speedBoostTimer -= Time.deltaTime;

        float targetX = currentLane * laneDistance;
        float moveX = (targetX - transform.position.x) * laneChangeSpeed;

        Vector3 move = new Vector3(moveX, verticalVelocity, speed);
        controller.Move(move * Time.deltaTime);
    }

    void Jump()
    {
        if (!controller.isGrounded || isSliding)
            return;

        verticalVelocity = jumpForce;
    }

    void Slide()
    {
        if (!controller.isGrounded)
            return;

        isSliding = true;
        slideTimer = slideDuration;

        controller.height = slideHeight;
        controller.center = new Vector3(0, slideHeight / 2f, 0);
    }

    void HandleSlide()
    {
        if (!isSliding)
            return;

        slideTimer -= Time.deltaTime;

        if (slideTimer <= 0)
        {
            isSliding = false;
            controller.height = normalHeight;
            controller.center = normalCenter;
        }
    }

    public void ActivateSpeedBoost(float duration)
    {
        speedBoostTimer = Mathf.Max(speedBoostTimer, duration);
    }

    public float SpeedBoostTimeRemaining => Mathf.Max(0, speedBoostTimer);
}
