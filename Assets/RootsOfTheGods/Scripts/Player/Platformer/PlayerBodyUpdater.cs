using System.Collections;
using System.Collections.Generic;
using Scripts.Player.Platformer;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBodyUpdater : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _playerController.CurrentPlayerLocalPosition;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton())
        {
            return;
        }
        
        _playerController.RequestJump();
    }
    
    public void HorizontalMovement(InputAction.CallbackContext context)
    {
        _playerController.SetHorizontalInput(context.ReadValue<float>());
    }
}
