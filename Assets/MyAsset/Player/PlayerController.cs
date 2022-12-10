﻿//by EvolveGames
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveGames
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("PlayerController")]
        [SerializeField] public Transform Camera;
        [SerializeField, Range(1, 10)] float walkingSpeed = 3.0f;
        [Range(0.1f, 5)] public float CroughSpeed = 1.0f;
        [SerializeField, Range(2, 20)] float RuningSpeed = 4.0f;
        [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
        [SerializeField, Range(0.5f, 10)] float lookSpeed = 2.0f;
        [SerializeField, Range(10, 120)] float lookXLimit = 80.0f;
        [Space(20)]
        [Header("Advance")]
        [SerializeField] float RunningFOV = 65.0f;
        [SerializeField] float SpeedToFOV = 4.0f;
        [SerializeField] float CroughHeight = 1.0f;
        [SerializeField] float gravity = 20.0f;
        [SerializeField] float timeToRunning = 2.0f;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool CanRunning = true;
        [Space(20)]
        [Header("State")]
        public float hp = 100f;

        [Space(20)]
        [Header("Climbing")]
        [SerializeField] bool CanClimbing = true;
        [SerializeField, Range(1, 25)] float Speed = 2f;
        bool isClimbing = false;

        [Space(20)]
        [Header("HandsHide")]
        [SerializeField] bool CanHideDistanceWall = true;
        [SerializeField, Range(0.1f, 5)] float HideDistance = 1.5f;
        [SerializeField] int LayerMaskInt = 1;

        [Space(20)]
        [Header("Input")]
        [SerializeField] KeyCode CroughKey = KeyCode.LeftControl;


        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Vector3 moveDirection = Vector3.zero;
        bool isCrough = false;
        float InstallCroughHeight;
        float rotationX = 0;
        [HideInInspector] public bool isRunning = false;
        Vector3 InstallCameraMovement;
        float InstallFOV;
        Camera cam;
        [HideInInspector] public bool Moving;
        [HideInInspector] public float vertical;
        [HideInInspector] public float horizontal;
        [HideInInspector] public float Lookvertical;
        [HideInInspector] public float Lookhorizontal;
        float RunningValue;
        float installGravity;
        bool WallDistance;
        [HideInInspector] public float WalkingValue;
        [HideInInspector] public bool isEnemyInTrigger = false;
        [Header("Hit")]
        public Image bloodImg;
        public Image hurtImg;
        bool isHpWarning = false;
        bool isDie = false;
        bool enterWarning = false;
        [SerializeField] CameraShake cs;
        [SerializeField] QuestController qc;
        [SerializeField] AudioClip dieClip;
        AudioSource audioSource;
        public bool isSlaveSafe = false;


        void Start()
        {
            characterController = GetComponent<CharacterController>();
            cam = GetComponentInChildren<Camera>();
            audioSource = GetComponent<AudioSource>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InstallCroughHeight = characterController.height;
            InstallCameraMovement = Camera.localPosition;
            InstallFOV = cam.fieldOfView;
            RunningValue = RuningSpeed;
            installGravity = gravity;
            WalkingValue = walkingSpeed;
        }

        void Update()
        {
            if (QuestController.Instance.gameEnd)
            {
                return;
            }

            RaycastHit CroughCheck;
            RaycastHit ObjectCheck;

            if (!characterController.isGrounded && !isClimbing)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            isRunning = !isCrough ? CanRunning ? Input.GetKey(KeyCode.LeftShift) : false : false;
            vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Vertical") : 0;
            horizontal = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Horizontal") : 0;
            if (isRunning) RunningValue = Mathf.Lerp(RunningValue, RuningSpeed, timeToRunning * Time.deltaTime);
            else RunningValue = WalkingValue;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * vertical) + (right * horizontal);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isClimbing)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
            characterController.Move(moveDirection * Time.deltaTime);
            Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;

            if (Cursor.lockState == CursorLockMode.Locked && canMove)
            {
                Lookvertical = -Input.GetAxis("Mouse Y");
                Lookhorizontal = Input.GetAxis("Mouse X");

                rotationX += Lookvertical * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                Camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Lookhorizontal * lookSpeed, 0);

                if (isRunning && Moving) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
                else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, InstallFOV, SpeedToFOV * Time.deltaTime);
            }

            if (Input.GetKey(CroughKey))
            {
                isCrough = true;
                float Height = Mathf.Lerp(characterController.height, CroughHeight, 5 * Time.deltaTime);
                characterController.height = Height;
                WalkingValue = Mathf.Lerp(WalkingValue, CroughSpeed, 6 * Time.deltaTime);

            }
            else if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.up), out CroughCheck, 0.8f, 1))
            {
                if (characterController.height != InstallCroughHeight)
                {
                    isCrough = false;
                    float Height = Mathf.Lerp(characterController.height, InstallCroughHeight, 6 * Time.deltaTime);
                    characterController.height = Height;
                    WalkingValue = Mathf.Lerp(WalkingValue, walkingSpeed, 4 * Time.deltaTime);
                }
            }

            if(WallDistance != Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.forward), out ObjectCheck, HideDistance, LayerMaskInt) && CanHideDistanceWall)
            {
                WallDistance = Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.forward), out ObjectCheck, HideDistance, LayerMaskInt);
            }

        }

        IEnumerator HpWarning()
        {
            hurtImg.gameObject.SetActive(true);
            enterWarning = true;

            while (true)
            {
                hurtImg.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f);
                hurtImg.gameObject.SetActive(false);
                yield return new WaitForSeconds(1f);

                if (isDie)
                {
                    PlayerDie();
                    break;
                }
                yield return null;
            }
        }

        void PlayerDie()
        {
            qc.endText.GetComponent<Text>().text = "Game Over";
            qc.GameEnd();
        }

        public void PlayerOnDamaged(float damage)
        {
            hp -= damage;

            if (hp <= 30 && !enterWarning)
            {
                isHpWarning = true;
                StartCoroutine(nameof(HpWarning));
            }
            else if(hp <= 0)
            {
                if (!isDie)
                {
                    audioSource.clip = dieClip;
                    audioSource.loop = false;
                    audioSource.Stop();
                    audioSource.Play();
                }
                isDie = true;
                Debug.Log("IsDie");
            }

            StartCoroutine(cs.Shake(0.1f, 0.5f));
            StartCoroutine(nameof(HitImgCor));
        }

        IEnumerator HitImgCor()
        {
            bloodImg.gameObject.SetActive(true);

            float time = 0f;
            Color color = bloodImg.color;
            color.a = 1f;

            while (time <= 3f)
            {
                time += Time.deltaTime;
                color.a -= Time.deltaTime / 1f;
                bloodImg.color = color;

                yield return null;
            }

            bloodImg.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy")) isEnemyInTrigger = true;
        }
       
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy")) isEnemyInTrigger = false;
        }

    }
}