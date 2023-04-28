using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.SceneManagement;

public class AutoHandController : MonoBehaviour
{
    public AutoHandPlayer player;

    void Update()
    {
        player.Move(InputManager.Instance.playerActions.DefaultControls.CharacterMovement.ReadValue<Vector2>());
        player.Turn(InputManager.Instance.playerActions.DefaultControls.CharacterRotation.ReadValue<Vector2>().x);

        if (InputManager.Instance.playerActions.DefaultControls.Menu.IsPressed())
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
    void FixedUpdate()
    {
        player.Move(InputManager.Instance.playerActions.DefaultControls.CharacterMovement.ReadValue<Vector2>());
    }
}
