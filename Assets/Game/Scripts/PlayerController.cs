using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [Header("Movment varriables")]
    [SerializeField] private float mvSpeed;
    [SerializeField] private Rigidbody playerRb;

    [Space]

    [SerializeField, ChildGameObjectsOnly] private PlayerAnimations playerAnimations;
    [SerializeField, ChildGameObjectsOnly] private SpriteRenderer playerSpriteRenderer;

    private PlayerInputs inputs;
    private Vector3 movmentVec;

    public Vector3 GetMovmentVector()
    {
        return movmentVec;
    }

    private void Awake()
    {
        inputs = new();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void Update()
    {
        ReadMovmentInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {
        //playerRb.velocity = mvSpeed * Time.fixedDeltaTime * movmentVec;
        playerRb.MovePosition(transform.position + (mvSpeed * Time.fixedDeltaTime * movmentVec));//this methode will make the movment faster than using dot velocity and the step will be pre calculated
    }

    private void ReadMovmentInputs()
    {
        float x = inputs.PlayerInputsControlls.Move.ReadValue<Vector2>().x;
        float z = inputs.PlayerInputsControlls.Move.ReadValue<Vector2>().y;

        movmentVec = new Vector3(x, 0f, z).normalized;
        ControllMovmentAnimations(movmentVec);
        ControllPlayerXDirectionFliping(movmentVec);
    }

    private void ControllPlayerXDirectionFliping(Vector3 moveVec)
    {
        if (moveVec.x < 0f)
        {
            playerSpriteRenderer.flipX = true;
        }
        else if (moveVec.x > 0f)
        {
            playerSpriteRenderer.flipX = false;
        }
    }

    private void ControllMovmentAnimations(Vector3 moveVec)
    {
        playerAnimations.PlayWalkingAnimation(moveVec);
    }
}
