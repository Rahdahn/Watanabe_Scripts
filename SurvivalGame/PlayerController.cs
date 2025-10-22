using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int requiredItemsForAttack = 3;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private int collectedItems = 0;
    private bool canAttack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if (move > 0) spriteRenderer.flipX = false;
        else if (move < 0) spriteRenderer.flipX = true;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        if (canAttack && Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Attack");
            canAttack = false;
        }

        if (isGrounded)
        {
            animator.SetBool("Run", Mathf.Abs(move) > 0.01f);
            animator.SetBool("Idle", Mathf.Abs(move) < 0.01f);
            animator.SetBool("Fall", false);
        }
        else
        {
            animator.SetBool("Run", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Fall", rb.velocity.y < -0.1f);
        }
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }


    public void AddItem()
    {
        collectedItems++;
        Debug.Log("Collected Items: " + collectedItems);

        if (collectedItems >= requiredItemsForAttack)
        {
            canAttack = true;
            collectedItems = 0;
            Debug.Log("çUåÇâã÷");
        }
    }
}
