using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//IMPORTANT NOTE:
//This script is extended from the ThirdPersonCharacter script provided in the Standard Assets pack.

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonCharacterMod : MonoBehaviour
{
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.1f;

	Rigidbody m_Rigidbody;
	Animator m_Animator;
	public bool m_IsGrounded, doublejump_enabled, downburst_enabled;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	public Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	CapsuleCollider m_Capsule;
	bool m_Crouching;

    public Vector3 up, forward;
    public Vector3 gravity_dir, gravity_centre;
    readonly Vector3 null_vec = new Vector3(0, -999999, 0); 

	void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		m_CapsuleHeight = m_Capsule.height;
		m_CapsuleCenter = m_Capsule.center;

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;

        gravity_dir = Physics.gravity.normalized;
        SetGravityCentre();

        up = transform.up;
        forward = transform.forward;
    }

    private void FixedUpdate()
    {
        SetGravityDirection();

        up = transform.TransformDirection(transform.up);
        forward = transform.TransformDirection(transform.forward);
    }

    public void Move(Vector3 move, bool crouch, bool jump)
	{

		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.
		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);
		CheckGroundStatus();
		
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
		m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;

		ApplyExtraTurnRotation();

		// control and velocity handling is different when grounded and airborne:
		if (m_IsGrounded)
		{
			HandleGroundedMovement(crouch, jump);
            doublejump_enabled = true;
            downburst_enabled = true;
		}
		else
		{
			HandleAirborneMovement();
            m_Rigidbody.AddForce(transform.TransformVector(new Vector3(move.x * 2, 0, move.z * 2)));


            if (!CheckWallJump())
            {
                if (doublejump_enabled && CrossPlatformInputManager.GetButtonDown("Jump"))
                {
                    m_Rigidbody.AddForce(transform.TransformVector(new Vector3(move.x * 6, 2, move.z * 6)));
                    doublejump_enabled = false;
                }
                else if (downburst_enabled && Input.GetKeyDown(KeyCode.C))
                {
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.AddForce(transform.TransformVector(new Vector3(0, -10, 0)));
                    downburst_enabled = false;
                }
            }
        }

		ScaleCapsuleForCrouching(crouch);
		PreventStandingInLowHeadroom();

		// send input and other state parameters to the animator
		UpdateAnimator(move);


        RotateTowardsGroundNormal();
    }


	void ScaleCapsuleForCrouching(bool crouch)
	{
		if (m_IsGrounded && crouch)
		{
			if (m_Crouching) return;
			m_Capsule.height = m_Capsule.height / 2f;
			m_Capsule.center = m_Capsule.center / 2f;
			m_Crouching = true;
		}
		else
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + transform.up * m_Capsule.radius * k_Half, transform.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
				return;
			}
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
			m_Crouching = false;
		}
	}

	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!m_Crouching)
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + transform.up * m_Capsule.radius * k_Half, transform.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
			}
		}
	}


	void UpdateAnimator(Vector3 move)
	{
		// update the animator parameters
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
 		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
		m_Animator.SetBool("Crouch", m_Crouching);
		m_Animator.SetBool("OnGround", m_IsGrounded);
		if (!m_IsGrounded)
		{
			m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
		}

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle =
			Mathf.Repeat(
				m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
		if (m_IsGrounded)
		{
			m_Animator.SetFloat("JumpLeg", jumpLeg);
		}

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (m_IsGrounded && move.magnitude > 0)
		{
			m_Animator.speed = m_AnimSpeedMultiplier;
		}
		else
		{
			// don't use that while airborne
			m_Animator.speed = 1;
		}
	}


	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (gravity_dir * m_GravityMultiplier) - gravity_dir;
		m_Rigidbody.AddForce(extraGravityForce);

		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
	}


	void HandleGroundedMovement(bool crouch, bool jump)
	{
		// check whether conditions are right to allow a jump:
		if (jump && m_IsGrounded)//m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
		{
            Vector3 jumpV;
            if (!crouch)
            {
                // jump!
                jumpV = new Vector3(m_Rigidbody.velocity.x, m_JumpPower + (Vector3.Project(m_Rigidbody.velocity, transform.forward).magnitude) * 0.2f, m_Rigidbody.velocity.z);
            }
            else
            {
                if (Mathf.Abs(m_Rigidbody.velocity.x) < 0.5 && Mathf.Abs(m_Rigidbody.velocity.z) < 0.5)
                {
                    jumpV = new Vector3(0, m_JumpPower * 2, 0);
                }
                else
                {
                    jumpV = transform.TransformVector(new Vector3(0, m_JumpPower * 1.1f, m_JumpPower * 2f));
                }
            }


            m_Rigidbody.velocity = jumpV;
            m_IsGrounded = false;
			m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.1f;
		}
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}

    void RotateTowardsGroundNormal()
    {

        //float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        //transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);

        Vector3.Lerp(transform.up, -gravity_dir, 0.7f);

        //Vector3.RotateTowards(transform.up, -gravity_dir, Mathf.PI, 0);
        //transform.up = -gravity_dir;
    }

    public void OnAnimatorMove()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (m_IsGrounded && Time.deltaTime > 0)
		{
			Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

			// we preserve the existing y part of the current velocity.
			v.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = v;
		}
	}


	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (transform.up * 0.1f), transform.position + (transform.up * 0.1f) + (-transform.up * m_GroundCheckDistance));
#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out hitInfo, m_GroundCheckDistance))
		{
			m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			m_Animator.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			m_GroundNormal = transform.up;
			m_Animator.applyRootMotion = false;
		}
	}

    bool CheckWallJump()
    {
        RaycastHit hitInfo1, hitInfo2;
        Vector3 capsule_top = new Vector3(transform.position.x, transform.position.y + m_CapsuleHeight, transform.position.z);
#if UNITY_EDITOR
        // helper to visualise the wall check ray in the scene view
        Debug.DrawLine(transform.position, transform.position + (transform.forward * 0.2f));
        Debug.DrawLine(capsule_top, capsule_top + (transform.forward * 0.2f));
#endif
        
        // it is also good to note that the transform position in the sample assets is at the base of the character
        // raycast from base and top of character
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo1, m_Capsule.radius*2) && Physics.Raycast(capsule_top, transform.forward, out hitInfo2, m_Capsule.radius*2))
        {
            if (hitInfo1.normal == hitInfo2.normal && CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                m_Rigidbody.velocity = Vector3.Scale(new Vector3(3, 0, 3), hitInfo1.normal) + new Vector3(0, m_JumpPower * 2, 0);
                m_Rigidbody.gameObject.transform.Rotate(0, 180, 0);
                return true;
            }
        }
        return false;
    }

    void SetGravityDirection()
    {
        Vector3 gdir;

        if (gravity_centre == null_vec) gdir = Vector3.down;
        else gdir = (gravity_centre - transform.position).normalized;

#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (-transform.up * 0.1f), transform.position + gdir);
#endif

        gravity_dir = gdir;
        gravity_dir *= 9.81f;
    }

    public void SetGravityCentre(Vector3 planet_centre)
    {
        gravity_centre = planet_centre;
    }

    public void SetGravityCentre()
    {
        gravity_centre = null_vec;
    }
}

