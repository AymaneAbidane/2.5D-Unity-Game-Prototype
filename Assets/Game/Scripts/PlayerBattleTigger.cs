using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBattleTigger : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private PlayerController playerController;

    [SerializeField] private int stepsInGrass;
    [SerializeField] private float timePerStep;
    [SerializeField] private LayerMask grassLayer;
    private bool movingInGrass;
    private float stepTimer;

    public bool GetMovingInGrass()
    {
        return movingInGrass;
    }

    public void SetMovingInGrass(bool value)
    {
        movingInGrass = value;
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, grassLayer);

        movingInGrass = colliders.Length != 0 && playerController.GetMovmentVector() != Vector3.zero;

        if (movingInGrass == true)
        {
            stepTimer += Time.fixedDeltaTime;

            if (stepTimer >= timePerStep)
            {
                stepsInGrass++;
                stepTimer = 0;

                //send an event to trigger the battle if the player reach the tresh hold of the steps before battle
            }
        }

    }
}
