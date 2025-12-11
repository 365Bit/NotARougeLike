using System.Runtime.CompilerServices;
using UnityEngine;

public class ApplyRootNodeMotionOnDeath : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Death")) // your one-shot animation
        {
            // Apply root motion only for Attack
            transform.position += animator.deltaPosition;
            transform.rotation *= animator.deltaRotation;
        }
        else
        {
            // Ignore root motion for other states
        }
    }

}
