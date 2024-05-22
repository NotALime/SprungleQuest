using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Modules;
using System.Collections.Specialized;

[RequireComponent(typeof(Entity))]
public class Humanoid : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 30;
    public float sprintMultiplier = 2;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float friction = 0.95f;

    [Header("Jumping")]
    public float jumpForce;
    float jumpCooldown = 0.1f;
    public float drag;
    bool readyToJump;

    public float gravity = 3;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [HideInInspector]
    public bool movementEnabled = true;
    public Rigidbody rb;
    [HideInInspector]
    public Entity entity;
    [HideInInspector]
    public Inventory inv;

    public HumanoidRig rig;

    public MovementState state;
    public enum MovementState
    {
        walking,
        air
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        entity = GetComponent<Entity>();
        inv = GetComponent<Inventory>();
        rb.freezeRotation = true;
        readyToJump = true;

        exitingSlope = false;
    }
    private void Update()
    {
        SpeedControl();
        StateHandler();
        entity.AI();
        Jump();

        if (entity.mob.interactInput)
        {
            entity.Interact();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        HandleRig();
    }

    //goofy
    void SnapToGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f))
        {
            Vector3 groundNormal = hit.normal;
            Vector3 upDirection = transform.up;
            Quaternion rotation = Quaternion.FromToRotation(upDirection, groundNormal) * transform.rotation;
            rb.MoveRotation(rotation);
        }
    }

    private void StateHandler()
    {
        if (isGrounded())
        {
            state = MovementState.walking;
            desiredMoveSpeed = moveSpeed * entity.mob.stats.moveSpeed * 1 + (sprintMultiplier * System.Convert.ToInt32(entity.mob.specialInput));
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }
    private void MovePlayer()
    {
        // calculate movement direction
        Vector3 moveDirection = flatForwardOrientation() * entity.mob.input.z + flatRightOrientation() * entity.mob.input.x;

        // on slope
        if ((OnSlope() && !exitingSlope) || isGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x * friction, rb.velocity.y, rb.velocity.z * friction);
            if (movementEnabled)
            {
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * desiredMoveSpeed * 10f, ForceMode.Force);
            }

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * gravity * 2, ForceMode.Force);
            }
        }
        else
        {
            if (movementEnabled)
            {
                rb.AddForce(moveDirection.normalized * desiredMoveSpeed * 10f, ForceMode.Force);
            }
            rb.velocity = new Vector3(rb.velocity.x * drag, rb.velocity.y, rb.velocity.z * drag);
            rb.AddForce(Vector3.down * gravity);
        }

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
            // limiting speed on slope
            if (OnSlope() && !exitingSlope)
            {
                if (rb.velocity.magnitude > desiredMoveSpeed)
                    rb.velocity = rb.velocity.normalized * desiredMoveSpeed;
            }

            // limiting speed on ground or in air
            else
            {
                Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

                // limit velocity if needed
                if (flatVel.magnitude > desiredMoveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * desiredMoveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
                //  }
            }
    }

    public Vector3 flatForwardOrientation()
    {
        return Vector3.Normalize(new Vector3(entity.mob.orientation.forward.x, 0, entity.mob.orientation.forward.z));
    }
    public Vector3 flatRightOrientation()
    {
        return Vector3.Normalize(new Vector3(entity.mob.orientation.right.x, 0, entity.mob.orientation.right.z));
    }

    public void Jump()
    {
        if (isGrounded() && readyToJump)
        {
            // reset y velocity

            if (entity.mob.input.y > 0)
            {
                exitingSlope = true;
                readyToJump = false;

                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * entity.mob.scale * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * entity.mob.scale * 0.5f + 0.1f, groundLayer);
    }

    float neckRotateSpeed = 30;
    float torsoRotateSpeed = 10;
    float BodyRotateSpeed = 5f;

    float limbGroundSpeed = 0.5f;
    float limbMaxDistance;
    public void HandleRig()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, entity.mob.orientation.eulerAngles.y, 0));
       //  Quaternion torsoRotation = entity.mob.orientation.rotation;
         rig.spine.spineMiddle.transform.rotation = Quaternion.Slerp(rig.spine.spineMiddle.transform.rotation, rotation, torsoRotateSpeed * Time.deltaTime);

        Vector3 inputHorizontal = new Vector3(entity.mob.input.x, 0, Mathf.Abs(entity.mob.input.z)).normalized;
        if (inputHorizontal.magnitude != 0)
        {
            rig.spine.spineLower.transform.rotation = Quaternion.Slerp(rig.spine.spineLower.transform.rotation, rotation * Quaternion.LookRotation(inputHorizontal), BodyRotateSpeed * Time.deltaTime);
        }
        else
        {
            rig.spine.spineLower.transform.rotation = Quaternion.Slerp(rig.spine.spineLower.transform.rotation, rotation, BodyRotateSpeed * 0.5f * Time.deltaTime);
        }

        rig.spine.neck.transform.rotation = Quaternion.Slerp(rig.spine.spineLower.transform.rotation, entity.mob.orientation.rotation, 500 * Time.deltaTime);

        //   GroundLimb(rig.legLeft.leg, rig.legLeft.foot, rig.legLeft.hip);
        //   GroundLimb(rig.legRight.leg, rig.legRight.foot, rig.legRight.hip);
        AnimateRig();


       // ArmCalculation(entity.mob.orientation.position + entity.mob.orientation.forward, rig.armRight.hand, rig.armRight.forearm, rig.armRight.arm, rig.armRight.shoulder);
       // ArmCalculation(Camera.main.transform.position + Camera.main.transform.forward * 3, rig.armLeft.hand, rig.armLeft.forearm, rig.armLeft.arm, rig.armLeft.shoulder);
    }

    public void AnimateRig()
    {
        Vector3 inputHorizontal = new Vector3(entity.mob.input.x, 0, entity.mob.input.z).normalized;
        rig.anim.SetInteger("Horizontal", (int)inputHorizontal.magnitude * 100);
        rig.anim.SetBool("Grounded", isGrounded() || OnSlope());

        if ((isGrounded() && readyToJump) && entity.mob.input.y > 0)
        {
            rig.anim.SetTrigger("Jump");
        }
    }

    public void ArmCalculation(Vector3 handPos,EntityLimb hand, EntityLimb forearm, EntityLimb arm, EntityLimb shoulder)
    {
        arm.transform.forward = entity.mob.orientation.forward;
        //    arm.transform.rotation = Quaternion.LookRotation((forearm.transform.position - arm.transform.position).normalized);
    }

    public void GroundLimb(EntityLimb moveLimb, EntityLimb groundLimb, EntityLimb rootLimb)
    {
        RaycastHit hit;
     //
        if (Physics.Raycast(moveLimb.transform.position, Vector3.down, out hit, playerHeight * 0.6f))
        {
            moveLimb.transform.position = Vector3.MoveTowards(moveLimb.transform.position, hit.point + Vector3.up, limbGroundSpeed * Time.deltaTime);
        //    moveLimb.transform.position = Vector3.MoveTowards(moveLimb.transform.position, new Vector3(hit.point.x, moveLimb.transform.position.y, hit.point.z), limbGroundSpeed * 0.5f * Time.deltaTime);

            //       float groundHeight = hit.point.y + (moveLimb.initialPos.y - groundLimb.transform.localPosition.y);
            //       moveLimb.transform.position = Vector3.MoveTowards(moveLimb.transform.position, new Vector3(moveLimb.transform.position.x, groundHeight, moveLimb.transform.position.z), limbGroundSpeed * Time.deltaTime);
        }
        //   else
        //   {
        //       moveLimb.transform.position = Vector3.MoveTowards(moveLimb.transform.localPosition, rootLimb.transform.position + moveLimb.initialPos, limbGroundSpeed * Time.deltaTime);
        //   }
    }
    public void ProcessDialogue(string dialogue)
    {
        dialogue.Replace("[item]", inv.items[inv.hotbarIndex].itemName);
        dialogue.Replace("[me]", entity.baseEntity.gameName);
        dialogue.Replace("[target]", entity.mob.target.baseEntity.gameName);
    }
}
[System.Serializable]
public class HumanoidRig
{
    public Animator anim;

    public SkinnedMeshRenderer renderer;

    public Texture2D baseTexture;

    public SpriteRenderer faceRender;

    public HumanoidSpineRig spine;
    public HumanoidArmRig armLeft;
    public HumanoidArmRig armRight;
    public HumanoidLegRig legLeft;
    public HumanoidLegRig legRight;
}

[System.Serializable]
public class AIStuff
{
    public string[] dialogue;
}

[System.Serializable]
public class HumanoidSpineRig
{
    public EntityLimb neck;
    public EntityLimb spineUpper;
    public EntityLimb spineMiddle;
    public EntityLimb spineLower;
}
[System.Serializable]
public class HumanoidArmRig
{
    public EntityLimb shoulder;
    public EntityLimb arm;
    public EntityLimb forearm;
    public EntityLimb hand;
}
[System.Serializable]
public class HumanoidLegRig
{
    public EntityLimb hip;
    public EntityLimb leg;
    public EntityLimb calf;
    public EntityLimb foot;
}