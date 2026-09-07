using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]

public class PlayerAnimatorDriver : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private PlayerMotor playerMotor;

    void Awake()
    {
        if(animator == null)
        {
            Debug.LogError("PlayerAnimatorDriver: Animator is not assigned.", this);
            enabled = false;
            return;
        }

        playerMotor = GetComponent<PlayerMotor>();
    }

    void LateUpdate()
    {
        animator.SetFloat("Speed", playerMotor.CurrentMoveSpeed);
    }
}