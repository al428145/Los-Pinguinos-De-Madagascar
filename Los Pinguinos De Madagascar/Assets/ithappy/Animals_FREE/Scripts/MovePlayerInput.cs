using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CreatureMover))]
    [RequireComponent(typeof(AudioSource))]
    public class MovePlayerInput : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private string m_HorizontalAxis = "Horizontal";
        [SerializeField] private string m_VerticalAxis = "Vertical";
        [SerializeField] private string m_JumpButton = "Jump";
        [SerializeField] private KeyCode m_RunKey = KeyCode.LeftShift;

        [Header("Camera")]
        [SerializeField] private PlayerCamera m_Camera;
        [SerializeField] private string m_MouseX = "Mouse X";
        [SerializeField] private string m_MouseY = "Mouse Y";
        [SerializeField] private string m_MouseScroll = "Mouse ScrollWheel";

        [Header("Audio")]
        [SerializeField] private AudioClip walkClip;
        [SerializeField] private AudioClip runClip;
        [SerializeField] private float runStartAt = 1f; // segundos desde donde empieza el clip de correr

        private CreatureMover m_Mover;
        private AudioSource m_AudioSource;

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;

        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.loop = true; // ahora los clips se repiten
            m_AudioSource.playOnAwake = false;
        }

        private void Update()
        {
            GatherInput();
            SetInput();
            HandleFootsteps();
        }

        public void GatherInput()
        {
            m_Axis = new Vector2(Input.GetAxis(m_HorizontalAxis), Input.GetAxis(m_VerticalAxis));
            m_IsRun = Input.GetKey(m_RunKey);
            m_IsJump = Input.GetButton(m_JumpButton);

            m_Target = (m_Camera == null) ? Vector3.zero : m_Camera.Target;
            m_MouseDelta = new Vector2(Input.GetAxis(m_MouseX), Input.GetAxis(m_MouseY));
            m_Scroll = Input.GetAxis(m_MouseScroll);
        }

        public void BindMover(CreatureMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);

            if (m_Camera != null)
                m_Camera.SetInput(in m_MouseDelta, m_Scroll);
        }

        private void HandleFootsteps()
        {
            bool isMoving = m_Axis.magnitude > 0.1f && !m_IsJump;

            if (isMoving)
            {
                AudioClip targetClip = m_IsRun ? runClip : walkClip;

                // si cambia el clip, actualízalo
                if (m_AudioSource.clip != targetClip)
                {
                    m_AudioSource.clip = targetClip;

                    if (m_IsRun)
                        m_AudioSource.time = runStartAt; // arrancar desde la mitad

                    m_AudioSource.Play();
                }

                // ajustar volumen y rango según camine o corra
                if (m_IsRun)
                {
                    m_AudioSource.volume = 1f;       // fuerte
                    m_AudioSource.maxDistance = 30f; // más rango audible
                }
                else
                {
                    m_AudioSource.volume = 0.5f;     // más suave
                    m_AudioSource.maxDistance = 15f; // menos rango
                }

                // reiniciar si por alguna razón se detuvo
                if (!m_AudioSource.isPlaying)
                {
                    if (m_IsRun)
                        m_AudioSource.time = runStartAt;

                    m_AudioSource.Play();
                }
            }
            else
            {
                if (m_AudioSource.isPlaying)
                    m_AudioSource.Stop();
            }
        }

    }
}
