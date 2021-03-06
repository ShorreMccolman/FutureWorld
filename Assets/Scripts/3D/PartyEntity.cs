using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PartyEntity : Entity3D
{
    public Party Party => State as Party;

    [SerializeField] Camera _camera;
    public Camera Camera => _camera;

    [SerializeField] Camera MapCam;
    [SerializeField] Light _torchLight;

    float _currentMoveSpeed;
    public float MoveSpeed => _currentMoveSpeed;

    [SerializeField] MouseLook _mouseLook;
    [SerializeField] private bool _isWalking;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] [Range(0f, 1f)] private float _runstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce;
    [SerializeField] private float m_GravityMultiplier;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private float m_StepInterval;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;
    private bool m_Interacting;
    private AudioSource m_AudioSource;
    PartyController _controller;

    public void Init(Party party)
    {
        State = party;

        m_CharacterController = GetComponent<CharacterController>();
        m_OriginalCameraPosition = Camera.transform.localPosition;
        m_FovKick.Setup(Camera);
        m_HeadBob.Setup(Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_Jumping = false;
        m_AudioSource = GetComponent<AudioSource>();

        _controller = PartyController.Instance;

        _mouseLook.Init(transform, Camera.transform);
        _mouseLook.SetCursorLock(true);

        MapCam.pixelRect = new Rect(new Vector2(Screen.width - 250f, Screen.height - 250f), new Vector2(250f, 250f));
        MapCam.enabled = false;
        MapCam.enabled = true;

        Status.OnTorchChanged += TorchLight;
    }

    public void SetControls(ControlState state)
    {
        _mouseLook.SetCursorLock(state == ControlState.LookControl);
    }

    private void Update()
    {
        if (_controller.ControlState == ControlState.MenuLock)
            return;

        _isWalking = !Input.GetKey(KeyCode.LeftShift);

        if(_controller.ControlState == ControlState.LookControl)
            _mouseLook.LookRotation(transform, Camera.transform);
    
        // the jump state needs to read here to make sure it is not missed
        if (!m_Jump && !TurnController.Instance.IsTurnBasedEnabled && m_CharacterController.isGrounded)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }

    private void FixedUpdate()
    {
        if (_controller.ControlState == ControlState.MenuLock)
            return;

        if (TurnController.Instance.IsTurnBasedEnabled)
            CombatControl();
        else
            StandardControls();
    }

    private void CombatControl()
    {

    }

    void TorchLight(bool enabled, SkillProficiency proficiency)
    {
        if(enabled)
        {
            _torchLight.enabled = true;
            switch(proficiency)
            {
                case SkillProficiency.Novice:
                    _torchLight.intensity = 1f;
                    _torchLight.range = 20;
                    break;
                case SkillProficiency.Expert:
                    _torchLight.intensity = 1.5f;
                    _torchLight.range = 25;
                    break;
                case SkillProficiency.Master:
                    _torchLight.intensity = 2f;
                    _torchLight.range = 30;
                    break;
            }
        } 
        else
        {
            _torchLight.enabled = false;
        }
    }

    private void StandardControls()
    {
        GetInput(out _currentMoveSpeed);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                           m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x * _currentMoveSpeed;
        m_MoveDir.z = desiredMove.z * _currentMoveSpeed;


        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;

            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                PlayJumpSound();
                m_Jump = false;
                m_Jumping = true;
            }
        }
        else
        {
            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
        }
        m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

        ProgressStepCycle(_currentMoveSpeed * 0.75f);
        UpdateCameraPosition(_currentMoveSpeed);

        _mouseLook.UpdateCursorLock();
    }

    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
        {
            Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                  (speed * (_isWalking ? 1f : _runstepLenghten)));
            newCameraPosition = Camera.transform.localPosition;
            newCameraPosition.y = Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        Camera.transform.localPosition = newCameraPosition;
    }

    private void PlayLandingSound()
    {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }

    private void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }

    private void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (_isWalking ? 1f : 1 + _runstepLenghten))) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }

    private void PlayFootStepAudio()
    {
        if (!m_CharacterController.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = _isWalking;

        // set the desired speed to be walking or running
        speed = _isWalking ? _walkSpeed : _runSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (_isWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!_isWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
}
