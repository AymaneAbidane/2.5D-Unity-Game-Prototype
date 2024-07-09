using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputs inputs;

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
        float x = inputs.PlayerInputsControlls.Move.ReadValue<Vector2>().x;
        float z = inputs.PlayerInputsControlls.Move.ReadValue<Vector2>().y;

        Debug.Log(x + " , " + z);
    }
}
