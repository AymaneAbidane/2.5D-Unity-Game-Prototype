using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    private const string WALKING_ANIMATION_STRING = "IsWalking";

    public void PlayWalkingAnimation(Vector3 moveVec)
    {
        playerAnimator.SetBool(WALKING_ANIMATION_STRING, moveVec != Vector3.zero);
    }
}
