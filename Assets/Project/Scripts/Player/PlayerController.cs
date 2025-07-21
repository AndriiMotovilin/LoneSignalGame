using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpForce = 9f;
    public float groundCheckRadius = 0.2f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float runStaminaCostPerSecond = 20f;
    public float jumpStaminaCost = 25f;
    public Slider staminaSlider;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;
    public float damagePerSecondWithoutOxygen = 10f;

    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public Animator animator;
    public Transform modelTransform;
    public OxygenSystem oxygenSystem;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector3 moveInput;
    private bool isGrounded;
    private bool isCrouchingState = false;
    private bool isDead = false;

    private List<ItemWorld> highlightedItems = new List<ItemWorld>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        currentStamina = maxStamina;
        currentHealth = maxHealth;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();
        HandleInput();
        HandleJump();
        RotateModel();
        UpdateAnimations();
        HighlightNearbyItems();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupNearbyItems();
        }

        RegenerateStamina();
        UpdateStaminaUI();

        HandleOxygenDamage();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        ApplyMovement();
    }

    void HandleOxygenDamage()
    {
        if (oxygenSystem != null && oxygenSystem.currentOxygen <= 0f)
        {
            currentHealth -= damagePerSecondWithoutOxygen * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (healthSlider != null)
                healthSlider.value = currentHealth;

            if (currentHealth <= 0f)
            {
                Die();
            }
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Игрок погиб.");

        // Остановим движения
        moveInput = Vector3.zero;

        // Запустим анимацию смерти
        animator.SetTrigger("Dying");

        // Отключим управление (можно гибко по флагу)
        StartCoroutine(HandleDeathTransition());
    }

    IEnumerator HandleDeathTransition()
    {
        yield return new WaitForSeconds(2f); // длительность анимации Dying
        animator.SetTrigger("Dead");
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        bool isRunningKey = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = Mathf.Abs(moveX) > 0.1f;

        if (isRunningKey && isMoving && isCrouchingState)
            isCrouchingState = false;

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
            isCrouchingState = !isCrouchingState;

        float currentSpeed = walkSpeed;
        if (isRunningKey && isMoving && currentStamina > 0f)
        {
            currentSpeed = runSpeed;
            currentStamina -= runStaminaCostPerSecond * Time.deltaTime;
        }

        moveInput = new Vector3(moveX, 0f, 0f).normalized * currentSpeed;
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump") && currentStamina >= jumpStaminaCost)
        {
            if (isCrouchingState)
            {
                isCrouchingState = false;
                animator.SetBool("IsCrouching", false);
                animator.SetBool("IsCrouchWalking", false);
            }

            currentStamina -= jumpStaminaCost;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            animator.SetTrigger("Jump");
        }
    }

    void ApplyMovement()
    {
        rb.linearVelocity = new Vector3(-moveInput.x, rb.linearVelocity.y, 0f);
    }

    void RotateModel()
    {
        if (moveInput.x > 0)
        {
            modelTransform.localRotation = Quaternion.Euler(0, -90, 0);
        }
        else if (moveInput.x < 0)
        {
            modelTransform.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }

    void UpdateAnimations()
    {
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f;
        bool isWalking = isMoving && !isRunning;
        bool isCrouchWalking = isCrouchingState && isGrounded && isWalking;

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);

        bool allowCrouch = isGrounded && isCrouchingState;
        animator.SetBool("IsCrouching", allowCrouch);
        animator.SetBool("IsCrouchWalking", isCrouchWalking);
    }

    void RegenerateStamina()
    {
        bool isRecovering = !Input.GetKey(KeyCode.LeftShift) || Mathf.Abs(moveInput.x) < 0.1f;
        if (isRecovering)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    void UpdateStaminaUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
    }

    ItemWorld FindClosestItem(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        ItemWorld closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            ItemWorld item = hit.GetComponent<ItemWorld>();
            if (item != null)
            {
                float dist = Vector3.Distance(transform.position, item.transform.position);
                if (dist < closestDist)
                {
                    closest = item;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }

    void TryPickupNearbyItems()
    {
        float pickupRadius = 2f;
        ItemWorld item = FindClosestItem(pickupRadius);
        if (item != null && item.canBePickedUp) // <-- добавлена проверка
        {
            Inventory.Instance.AddItem(item.itemData);
            Destroy(item.gameObject);
        }
    }


    void HighlightNearbyItems()
    {
        float highlightRadius = 2f;
        ItemWorld closestItem = FindClosestItem(highlightRadius);


        foreach (var item in highlightedItems)
        {
            if (item != null)
                item.Highlight(false);
        }

        highlightedItems.Clear();

        if (closestItem != null)
        {
            closestItem.Highlight(true);
            highlightedItems.Add(closestItem);
        }
    }


    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
