C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float walkSpeed2 = 2f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float gravity = -10.0f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.01f;

    [SerializeField] bool lockCursor = true;
    [SerializeField] KeyCode runKey = KeyCode.LeftShift;

    [SerializeField] bool useFootsteps = true;
    [SerializeField] float baseStepSpeed = 0.5f;
    [SerializeField] float sprintMultiplier = 0.7f;
    [SerializeField] AudioSource footstepAudioSource = default;
    [SerializeField] AudioClip[] tileClips = default;
    [SerializeField] AudioClip[] metalClips = default;
    [SerializeField] AudioClip[] concClips = default;
    float footstepTimer = 0;
    float GetCurrentOffSet;


    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {   
        controller = GetComponent<CharacterController>();
        if(lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
        if (useFootsteps) {
        UpdateFootstep();
        }

    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }
    void UpdateMovement()
    {
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir.Normalize();

        Vector3 velocity = (transform.forward * inputDir.y + transform.right * inputDir.x) * walkSpeed;

        if(controller.isGrounded) {
            velocityY = 0.0f;
        }

        velocityY += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        SetMovementSpeed();
    }
    void SetMovementSpeed()
    {
        if(Input.GetKey(runKey))
        {
            walkSpeed = Mathf.Lerp(walkSpeed,runSpeed,0.5f);
        }
        else
        {
            walkSpeed = Mathf.Lerp(walkSpeed,walkSpeed2,0.5f);
        }

    }
    void UpdateFootstep()
    {
        if(controller.isGrounded)
        {   
            if(Input.GetKey(runKey))
            {
                footstepTimer += Time.deltaTime * sprintMultiplier;
            }
            else
            {
                footstepTimer += Time.deltaTime;
            }
            if(footstepTimer >= baseStepSpeed)
            {
                if(Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 2))
                {
                    switch (hit.collider.tag) {
                        case "TILES":
                            footstepAudioSource.PlayOneShot(tileClips [Random.Range(0, tileClips.Length - 1)]);
                            break;
                        case "CONCRETE":
                            footstepAudioSource.PlayOneShot(concClips [Random.Range(0, concClips.Length - 1)]);
                            break;
                        case "METAL":
                            footstepAudioSource.PlayOneShot(metalClips [Random.Range(0, metalClips.Length - 1)]);
                            break;
                        default:
                            footstepAudioSource.PlayOneShot(tileClips [Random.Range(0, tileClips.Length - 1)]);
                            break;
                    }
                }

                footstepTimer = GetCurrentOffSet;
            }
        }
    }
}
