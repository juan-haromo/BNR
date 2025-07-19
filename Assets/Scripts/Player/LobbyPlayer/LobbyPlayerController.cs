using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-room-player
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomPlayer.html
*/

/// <summary>
/// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
/// The RoomPrefab object of the NetworkRoomManager must have this component on it.
/// This component holds basic room player data required for the room to function.
/// Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.
/// </summary>
public class LobbyPlayerController : NetworkBehaviour
{
    IA_Player input;
    public Rigidbody rb;
    public CapsuleCollider coll;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float jumpForce;
    [SerializeField] float speed;
    [SerializeField] NetworkAnimator newtworkAnimator;
    [SerializeField] Animator animator;
    [SerializeField] GameObject player;
    public GameObject playerModel;
    float moveDirection;
    [SyncVar]
    bool isGrounded = false;
    public void EnableInput()
    {
        if(input == null){ input = new IA_Player(); }
        rb.useGravity = true;
        coll.enabled = true;
        playerModel.SetActive(true);
        input.RoomMovement.Jump.performed += Jump;
        input.RoomMovement.Enable();
    }

    public void DisableInput()
    {
        rb.useGravity = false;
        coll.enabled = false;
        playerModel.SetActive(false);
        input.RoomMovement.Disable();
        input.RoomMovement.Jump.performed -= Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(!isLocalPlayer) {return;}
        if (isGrounded)
        {
            Jump();
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if(!isLocalPlayer) {return;}
        Debug.DrawRay(player.transform.position, Vector3.down * ((coll.height * 0.5f) + 0.1f), Color.magenta);
        isGrounded = Physics.Raycast(player.transform.position, Vector3.down, (coll.height * 0.5f) + 0.1f, groundMask);
        moveDirection = input.RoomMovement.Run.ReadValue<float>();
        rb.linearVelocity = new Vector3(moveDirection * speed, rb.linearVelocity.y, 0);
        SetAnimations(isGrounded, moveDirection);
    }

    [Command]
    void SetAnimations(bool _isGrounded, float _moveDirection)
    {
        newtworkAnimator.animator.SetBool("IsGrounded", _isGrounded);
        newtworkAnimator.animator.SetFloat("Speed", _moveDirection);
    }


    [Command]
    void Jump()
    {
        newtworkAnimator.SetTrigger("Jump");
    }



}

