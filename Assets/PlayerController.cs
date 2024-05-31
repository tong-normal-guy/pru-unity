using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    Vector2 movementInput;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    List<RaycastHit2D> castCollisions = new();

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // when movement input is 0, try 2 move
        if (movementInput != Vector2.zero)
        {
            bool isSuccess = TryMove(movementInput);
            if (!isSuccess)
            {
                // if the movement was not successful, try moving on the x axis
                isSuccess = TryMove(new Vector2(movementInput.x, 0));
                if (!isSuccess)
                {
                    // if the movement was not successful, try moving on the y axis
                    isSuccess = TryMove(new Vector2(0, movementInput.y));
                }
            }
            animator.SetBool("isMoving", isSuccess);
        } else
        {
            animator.SetBool("isMoving", false);
        }

        //set direction of sprite to move directory of player
        if (movementInput.x < 0)
        {
            spriteRenderer.flipX = false;
        }else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = true;
        }

    }

    private bool TryMove(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return false;
        }

        int count = rb.Cast(
                movementInput, // X and Y values between -1 and 1 that represent the direction from the body 2 look for collisions
                movementFilter, // the settings that determine where a collision is can occur on such as layers 2 collide with
                castCollisions, // list of all the collisions 2 store the found collisions into after the cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset); // the amount to cast equal to movement speed + collision offset

        if (count == 0)
        {
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
            return true;
        } else
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }
}
