using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class CreatureMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float m_WalkSpeed = 1f;
        [SerializeField] private float m_RunSpeed = 4f;
        [SerializeField, Range(0f, 360f)] private float m_RotateSpeed = 90f;
        [SerializeField] private Space m_Space = Space.Self;
        [SerializeField] private float m_JumpHeight = 5f;
        public Transform cameraTransform;

        [Header("Animator")]
        [SerializeField] private string m_VerticalID = "Vert";
        [SerializeField] private string m_StateID = "State";
        [SerializeField] private LookWeight m_LookWeight = new LookWeight(1f, 0.3f, 0.7f, 1f);

        private Transform m_Transform;
        private CharacterController m_Controller;
        private Animator m_Animator;

        private MovementHandler m_Movement;
        private AnimationHandler m_Animation;

        private Vector2 m_Axis;
        private Vector3 m_Target;
        private bool m_IsRun;
        private bool m_IsMoving;

        public Vector2 Axis => m_Axis;
        public Vector3 Target => m_Target;
        public bool IsRun => m_IsRun;

        private void OnValidate()
        {
            m_WalkSpeed = Mathf.Max(m_WalkSpeed, 0f);
            m_RunSpeed = Mathf.Max(m_RunSpeed, m_WalkSpeed);
            m_RotateSpeed = Mathf.Max(m_RotateSpeed, 0f);

            // Mantener las unidades consistentes: las velocidades serializadas se tratan como m/s aquí.
            m_Movement?.SetStats(m_WalkSpeed, m_RunSpeed, m_RotateSpeed, m_JumpHeight, m_Space);
        }

        private void Awake()
        {
            m_Transform = transform;
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();

            // Evitamos que el Animator mueva el transform por root motion (provoca conflicto con CharacterController).
            m_Animator.applyRootMotion = false;

            m_Movement = new MovementHandler(m_Controller, m_Transform, m_WalkSpeed, m_RunSpeed, m_RotateSpeed, m_JumpHeight, m_Space);
            m_Animation = new AnimationHandler(m_Animator, m_VerticalID, m_StateID);
        }

        private void Update()
        {
            // La dirección "forward" de la cámara, pero proyectada en el plano XZ
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            // El target ahora es la posición del jugador + la forward de la cámara
            Vector3 moveTarget = transform.position + cameraForward;

            m_Movement.Move(Time.deltaTime, in m_Axis, in moveTarget, m_IsRun, m_IsMoving, out var animAxis, out var isAir);
            m_Animation.Animate(in animAxis, m_IsRun ? 1f : 0f, Time.deltaTime);
        }

        private void OnAnimatorIK()
        {
            m_Animation.AnimateIK(in m_Target, m_LookWeight);
        }

        public void SetInput(in Vector2 axis, in Vector3 target, in bool isRun, in bool isJump)
        {
            m_Axis = axis;
            m_Target = target;
            m_IsRun = isRun;

            if (m_Axis.sqrMagnitude < Mathf.Epsilon)
            {
                m_Axis = Vector2.zero;
                m_IsMoving = false;
            }
            else
            {
                m_Axis = Vector2.ClampMagnitude(m_Axis, 1f);
                m_IsMoving = true;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Comparamos en ángulo con slopeLimit: si la normal es lo suficientemente "horizontal",
            // la usamos como normal de superficie para proyectar el movimiento.
            if (m_Controller == null) return;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (hit.normal.y > 0.001f && angle <= m_Controller.slopeLimit + 0.1f)
            {
                m_Movement.SetSurface(hit.normal);
            }
        }

        [Serializable]
        private struct LookWeight
        {
            public float weight;
            public float body;
            public float head;
            public float eyes;

            public LookWeight(float weight, float body, float head, float eyes)
            {
                this.weight = weight;
                this.body = body;
                this.head = head;
                this.eyes = eyes;
            }
        }

        #region Handlers
        private class MovementHandler
        {
            private readonly CharacterController m_Controller;
            private readonly Transform m_Transform;

            private float m_WalkSpeed;
            private float m_RunSpeed;
            private float m_RotateSpeed;

            private Space m_Space;

            private readonly float m_Luft = 75f; // umbral usado por el autor original (se puede ajustar)

            private float m_TargetAngle;
            private bool m_IsRotating = false;

            // Normal de la superficie (para proyectar movimiento)
            private Vector3 m_Normal = Vector3.up;

            // Velocidad vertical (en m/s). Evita acumular aceleraciones en un vector.
            private float m_VerticalVelocity = 0f;

            private float m_jumpTimer;
            private Vector3 m_LastForward;

            public MovementHandler(CharacterController controller, Transform transform, float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_Controller = controller;
                m_Transform = transform;

                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;

                m_Space = space;

                m_Normal = Vector3.up;
                m_LastForward = m_Transform != null ? m_Transform.forward : Vector3.forward;
            }

            public void SetStats(float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;
                m_Space = space;
            }

            public void SetSurface(in Vector3 normal)
            {
                if (normal.sqrMagnitude > 0.0001f)
                    m_Normal = normal.normalized;
            }

            public void Move(float deltaTime, in Vector2 axis, in Vector3 target, bool isRun, bool isMoving, out Vector2 animAxis, out bool isAir)
            {
                // Cálculo de look/cámara
                Vector3 cameraLook = target - m_Transform.position;
                if (cameraLook.sqrMagnitude <= 0.000001f)
                    cameraLook = m_Transform.forward;
                else
                    cameraLook.Normalize();

                // Convertir input -> movimiento en world
                ConvertMovement(in axis, in cameraLook, out var movement);
                if (movement.sqrMagnitude > 0.000001f)
                {
                    m_LastForward = Vector3.Normalize(movement);
                }

                // Gravedad: calculamos la velocidad vertical (no acumulamos aceleración en un vector)
                CalculateGravity(deltaTime, out isAir);

                // Desplazamiento horizontal (m/s)
                Vector3 horizontal = (isRun ? m_RunSpeed : m_WalkSpeed) * movement;

                // Componemos el desplazamiento final (y en m/s)
                Vector3 displacement = horizontal;
                displacement.y = m_VerticalVelocity;

                // Move espera desplazamiento en metros; multiplicamos por deltaTime
                CollisionFlags flags = m_Controller.Move(displacement * deltaTime);

                // Si hemos pegado con el suelo, aseguramos una pequeña velocidad hacia abajo para "pegar".
                if ((flags & CollisionFlags.Below) != 0 && m_VerticalVelocity < 0f)
                {
                    m_VerticalVelocity = -2f;
                }

                // Rotación
                Turn(in m_LastForward, isMoving);
                UpdateRotation(deltaTime);

                // Axis de animación
                GenAnimationAxis(in movement, out animAxis);
            }

            private void ConvertMovement(in Vector2 axis, in Vector3 targetForward, out Vector3 movement)
            {
                Vector3 forward;
                Vector3 right;

                if (m_Space == Space.Self)
                {
                    forward = new Vector3(targetForward.x, 0f, targetForward.z).normalized;
                    if (forward.sqrMagnitude < 0.000001f) forward = m_Transform.forward;
                    right = Vector3.Cross(Vector3.up, forward).normalized;
                }
                else
                {
                    forward = Vector3.forward;
                    right = Vector3.right;
                }

                movement = axis.x * right + axis.y * forward;

                // Proyectar sobre la normal de la superficie para seguir pendientes
                movement = Vector3.ProjectOnPlane(movement, m_Normal);
            }

            private void Displace(float deltaTime, in Vector3 movement, bool isRun)
            {
                // NO usado: lógica integrada en Move()
            }

            private void CalculateGravity(float deltaTime, out bool isAir)
            {
                // Si estamos en suelo, mantenemos una pequeña velocidad negativa para "pegar" al suelo.
                if (m_Controller.isGrounded)
                {
                    if (m_VerticalVelocity < 0f)
                        m_VerticalVelocity = -2f;
                    isAir = false;
                    return;
                }

                // En el aire: integrar la velocidad vertical con gravity.y (ya está en m/s^2)
                isAir = true;
                m_VerticalVelocity += Physics.gravity.y * deltaTime;
            }

            private void GenAnimationAxis(in Vector3 movement, out Vector2 animAxis)
            {
                if (m_Space == Space.Self)
                {
                    animAxis = new Vector2(Vector3.Dot(movement, m_Transform.right), Vector3.Dot(movement, m_Transform.forward));
                }
                else
                {
                    animAxis = new Vector2(Vector3.Dot(movement, Vector3.right), Vector3.Dot(movement, Vector3.forward));
                }
            }

            private void Turn(in Vector3 targetForward, bool isMoving)
            {
                Vector3 proj = Vector3.ProjectOnPlane(targetForward, Vector3.up);
                if (proj.sqrMagnitude < 0.000001f)
                {
                    m_TargetAngle = 0f;
                    return;
                }

                var angle = Vector3.SignedAngle(m_Transform.forward, proj.normalized, Vector3.up);

                if (!m_IsRotating)
                {
                    if (!isMoving && Mathf.Abs(angle) < m_Luft)
                    {
                        m_IsRotating = false;
                        m_TargetAngle = 0f;
                        return;
                    }

                    m_IsRotating = true;
                }

                m_TargetAngle = angle;
            }

            private void UpdateRotation(float deltaTime)
            {
                if (!m_IsRotating) return;

                var rotDelta = m_RotateSpeed * deltaTime;

                // Corregido: comparamos en la misma unidad (grados). Antes se mezclaban radianes.
                if (Mathf.Abs(m_TargetAngle) <= rotDelta)
                {
                    rotDelta = m_TargetAngle;
                    m_IsRotating = false;
                }
                else
                {
                    rotDelta *= Mathf.Sign(m_TargetAngle);
                }

                m_Transform.Rotate(Vector3.up, rotDelta);
            }
        }

        private class AnimationHandler
        {
            private readonly Animator m_Animator;
            private readonly string m_VerticalID;
            private readonly string m_StateID;

            private readonly float k_InputFlow = 4.5f;

            private float m_FlowState;
            private Vector2 m_FlowAxis;

            public AnimationHandler(Animator animator, string verticalID, string stateID)
            {
                m_Animator = animator;
                m_VerticalID = verticalID;
                m_StateID = stateID;
            }

            public void Animate(in Vector2 axis, float state, float deltaTime)
            {
                // Smoothing más estable
                m_FlowAxis = Vector2.ClampMagnitude(Vector2.MoveTowards(m_FlowAxis, axis, k_InputFlow * deltaTime), 1f);
                m_FlowState = Mathf.MoveTowards(m_FlowState, state, k_InputFlow * deltaTime);

                // Valores a animator (se puede ajustar la variable que quieras exponer)
                m_Animator.SetFloat(m_VerticalID, m_FlowAxis.magnitude);
                m_Animator.SetFloat(m_StateID, Mathf.Clamp01(m_FlowState));
            }

            public void AnimateIK(in Vector3 target, in LookWeight lookWeight)
            {
                m_Animator.SetLookAtPosition(target);
                m_Animator.SetLookAtWeight(lookWeight.weight, lookWeight.body, lookWeight.head, lookWeight.eyes);
            }
        }
        #endregion
    }
}
