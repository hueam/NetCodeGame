using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimater : MonoBehaviour
{
    private readonly int yVelocityHash = Animator.StringToHash("y_velocity");
    private readonly int isJumpHash = Animator.StringToHash("isJump");

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetYVelocity(float yVelocity)
    {
        animator.SetFloat(yVelocityHash, yVelocity);
    }
    public void SetIsJump(bool value)
    {
        animator.SetBool(isJumpHash, value);
    }
    public void Filp(bool value)
    {
        spriteRenderer.flipX = value;
    }
}
