using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class AutoHandController : MonoBehaviour
{
    public AutoHandPlayer player;

    void Update()
    {
        player.Move(InputManager.Instance.playerActions.DefaultControls.CharacterMovement.ReadValue<Vector2>());
        player.Turn(InputManager.Instance.playerActions.DefaultControls.CharacterRotation.ReadValue<Vector2>().x);
    }
    void FixedUpdate()
    {
        player.Move(InputManager.Instance.playerActions.DefaultControls.CharacterMovement.ReadValue<Vector2>());
    }
}
