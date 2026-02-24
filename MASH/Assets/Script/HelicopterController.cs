using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 4f;

    [Header("Boundaries")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Rigidbody2D rb;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gameManager != null && !gameManager.IsGameActive())
            return;

        HandleMovement();
        ClampPosition();
    }

    void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.leftArrowKey.isPressed) horizontal = -1f;
        if (keyboard.rightArrowKey.isPressed) horizontal = 1f;
        if (keyboard.upArrowKey.isPressed) vertical = 1f;
        if (keyboard.downArrowKey.isPressed) vertical = -1f;

        Vector2 movement = new Vector2(horizontal * moveSpeed, vertical * verticalSpeed);
        rb.linearVelocity = movement;

        // Flip sprite based on direction
        if (horizontal < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontal > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Soldier"))
        {
            gameManager.PickUpSoldier(other.gameObject);
        }
        else if (other.CompareTag("Tree"))
        {
            gameManager.TriggerGameOver();
        }
        else if (other.CompareTag("Hospital"))
        {
            gameManager.DropOffSoldiers();
        }
    }
}
