using UnityEngine;

namespace Actors.Player
{
    [RequireComponent(typeof(Animator), typeof(CharacterController))]
    public class PlayerStateMachine : StateMachine
    {
        [Header("Components")] private Animator animator;
        private CharacterController controller;
        private Player player;

        [Header("Movement Settings")] [SerializeField]
        private float walkSpeed = 4f;

        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float groundCheckDistance = 0.2f;

        public Vector2 MoveInput { get; set; }
        public bool SprintHeld { get; set; }
        public bool JumpPressed { get; set; }
        public bool AttackPressed { get; set; }
        public bool MagicPressed { get; set; }

        private Vector3 velocity;
        private bool grounded;
        
        private readonly float gravity = -9.81f;

        private static readonly int MovingParam = Animator.StringToHash("Moving");
        private static readonly int JumpingParam = Animator.StringToHash("Jumping");
        private static readonly int VelocityXParam = Animator.StringToHash("Velocity X");
        private static readonly int VelocityZParam = Animator.StringToHash("Velocity Z");
        private static readonly int ActionParam = Animator.StringToHash("Action");
        private static readonly int TriggerParam = Animator.StringToHash("Trigger");
        private static readonly int StunnedParam = Animator.StringToHash("Stunned");
        private static readonly int BlockingParam = Animator.StringToHash("Blocking");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            player = GetComponent<Player>();
        }

        private void Start()
        {
            ChangeState(State.Idle);
        }

        private new void Update()
        {
            base.Update();
        }

        #region State Enter/Exit/Update

        protected override void EnterState(State state)
        {
            switch (state)
            {
                case State.Idle:
                case State.Walking:
                case State.Sprinting:
                    break;
                case State.Jumping:
                    velocity.y = jumpForce;
                    animator.SetInteger(JumpingParam, 1); // 1 = true
                    SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);
                    break;
                case State.Falling:
                    animator.SetInteger(JumpingParam, 2);
                    SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);
                    break;
                case State.PhysicalAttack:
                    animator.SetInteger(ActionParam, 0); // 0 - физическая атака
                    animator.SetTrigger(TriggerParam);
                    StateTimer = 0.6f; // длительность анимации
                    break;
                case State.MagicAttack:
                    animator.SetInteger(ActionParam, 1); // 1 - магическая атака
                    animator.SetTrigger(TriggerParam);
                    StateTimer = 0.8f;
                    break;
                case State.Blocking:
                    animator.SetBool(BlockingParam, true);
                    break;
                case State.Damaged:
                    animator.SetBool(StunnedParam, true);
                    animator.SetTrigger(
                        TriggerParam); // возможно, отдельный триггер, но предположим, что Trigger универсален
                    StateTimer = 0.5f; // длительность стана
                    break;
                case State.Dead:
                    animator.SetTrigger(TriggerParam); // или специальный триггер "Death"
                    // Отключить управление, коллайдер и т.д.
                    break;
            }
        }

        protected override void UpdateState(State state)
        {
            switch (state)
            {
                case State.Idle:
                case State.Walking:
                case State.Sprinting:
                    HandleLocomotion();
                    break;
                case State.Jumping:
                    HandleJumping();
                    break;
                case State.Falling:
                    HandleFalling();
                    break;
                case State.PhysicalAttack:
                case State.MagicAttack:
                    HandleAttackState();
                    break;
                case State.Damaged:
                    StateTimer -= Time.deltaTime;
                    if (StateTimer <= 0f)
                        ChangeState(State.Idle);
                    break;
                case State.Dead:
                    break;
            }
        }

        protected override void ExitState(State state)
        {
            switch (state)
            {
                case State.Jumping:
                    animator.SetInteger(JumpingParam, 0); // сброс
                    SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);
                    break;
                case State.Falling:
                    animator.SetInteger(JumpingParam, 0);
                    SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);
                    break;
                case State.PhysicalAttack:
                    SetAnimatorTrigger(AnimatorTrigger.AttackTrigger);
                    animator.SetInteger(ActionParam, 1);
                    break;
                case State.MagicAttack:
                    animator.SetInteger(ActionParam, -1); // сброс действия
                    break;
                case State.Blocking:
                    animator.SetBool(BlockingParam, false);
                    break;
                case State.Damaged:
                    animator.SetBool(StunnedParam, false);
                    break;
            }
        }

        #endregion

        #region Поведение состояний

        private void HandleLocomotion()
        {
            float targetSpeed = walkSpeed;
            bool moving = MoveInput.magnitude > 0.1f;

            if (SprintHeld && MoveInput.y > 0 && Mathf.Abs(MoveInput.x) < 0.1f)
            {
                targetSpeed = walkSpeed * sprintMultiplier;
                if (moving) ChangeState(State.Sprinting);
                else ChangeState(State.Idle);
            }
            else if (moving)
            {
                ChangeState(State.Walking);
            }
            else
            {
                ChangeState(State.Idle);
            }

            // Обновление аниматора
            animator.SetBool(MovingParam, moving);
            // Velocity X (боковое) и Y (вперед-назад)
            animator.SetFloat(VelocityXParam, MoveInput.x, 0.1f, Time.deltaTime);
            animator.SetFloat(VelocityZParam, MoveInput.y, 0.1f, Time.deltaTime);

            // Физическое движение
            Vector3 move = transform.right * MoveInput.x + transform.forward * MoveInput.y;
            controller.Move(move * (targetSpeed * Time.deltaTime));

            // Гравитация и проверка земли
            grounded = controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
            if (grounded && velocity.y < 0)
                velocity.y = -2f;

            if (JumpPressed && grounded)
            {
                ChangeState(State.Jumping);
                return;
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            // Атаки
            if (AttackPressed)
                ChangeState(State.PhysicalAttack);
            else if (MagicPressed)
                ChangeState(State.MagicAttack);
        }

        private void HandleJumping()
        {
            Vector3 airMove = transform.right * MoveInput.x + transform.forward * MoveInput.y;
            controller.Move(airMove * (walkSpeed * Time.deltaTime));
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            if (velocity.y <= 0)
            {
                ChangeState(State.Falling);
            }
        }

        private void HandleFalling()
        {
            Vector3 airMove = transform.right * MoveInput.x + transform.forward * MoveInput.y;
            controller.Move(airMove * (walkSpeed * Time.deltaTime));

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            grounded = controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
            if (grounded)
            {
                velocity.y = -2f;
                ChangeState(State.Idle);
            }
        }
        
        private void HandleAttackState()
        {
            StateTimer -= Time.deltaTime;
            // Во время атаки обычно не двигаются (или двигаются с малой скоростью)
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            if (StateTimer <= 0f)
                ChangeState(State.Idle);
        }

        #endregion

        // Метод для внешнего вызова получения урона
        public void TakeDamage()
        {
            if (currentState != State.Dead)
                ChangeState(State.Damaged);
        }

        public void Die()
        {
            ChangeState(State.Dead);
        }

        private void SetAnimatorTrigger(AnimatorTrigger trigger)
        {
            animator.SetInteger("TriggerNumber", ( int )trigger);
            animator.SetTrigger("Trigger");
        }
    }
    
    public enum AnimatorTrigger
    {
        NoTrigger = 0,
        JumpTrigger = 1,
        ActionTrigger = 2,
        DashTrigger = 3,
        AttackTrigger = 4,
        JumpAttackTrigger = 5,
        DeathTrigger = 6,
        ReviveTrigger = 7,
        LightHitTrigger = 8,
        RollTrigger = 9,
        AttackSpecialTrigger = 10,
        AttackMoveTrigger = 11,
        AttackRanged = 12,
        BlockBreakTrigger = 13,
        ReloadTrigger = 14,
        WeaponSwitchTrigger = 15,
        BlockTrigger = 16
    }
}