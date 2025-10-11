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

        [Header("Debug")]
        [SerializeField] private NoiseCircle noiseCircle;
        [SerializeField] private int radiowalkEarth = 3;
        [SerializeField] private int radiorunEarth = 7;
        [SerializeField] private int radiowalkAsphalt = 5;
        [SerializeField] private int radiorunAsphalt = 9;

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

        // para suavizar el cambio de radio
        private float targetRadius;
        private float surfaceType; //to know in which surface is in, 0=land, 1=asphalt

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.loop = true; // ahora los clips se repiten
            m_AudioSource.playOnAwake = false;

            if (noiseCircle != null)
            {
                noiseCircle.visible = true;
                noiseCircle.radius = 1;
                targetRadius = 1;
            }

            surfaceType = 0;
        }

        private void Update()
        {
            GatherInput();
            SetInput();
            HandleFootsteps();

            // transicion suave del radio del circulo
            if (noiseCircle != null)
            {
                noiseCircle.radius = Mathf.Lerp(noiseCircle.radius, targetRadius, Time.deltaTime * 5f);
            }
        }

        public void setSurface(int surface)
        {
            surfaceType = surface;
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

                if (m_AudioSource.clip != targetClip)
                {
                    m_AudioSource.clip = targetClip;

                    if (m_IsRun)
                        m_AudioSource.time = runStartAt;

                    m_AudioSource.Play();
                }

                float baseVolume = m_IsRun ? 1f : 0.5f;
                float surfaceVolumeMultiplier = 1f;
                int targetRadiusWalk = 0;
                int targetRadiusRun = 0;

                if(surfaceType == 0)
                {
                    surfaceVolumeMultiplier = 0.7f;
                    targetRadiusWalk = radiowalkEarth;
                    targetRadiusRun = radiorunEarth;
                }
                else if(surfaceType == 1)
                {
                    surfaceVolumeMultiplier = 1.2f;
                    targetRadiusWalk = radiowalkAsphalt;
                    targetRadiusRun = radiorunAsphalt;
                }

                m_AudioSource.volume = baseVolume * surfaceVolumeMultiplier;
                m_AudioSource.maxDistance = m_IsRun ? 30f : 15f;

                if (noiseCircle != null)
                {
                    noiseCircle.visible = true;
                    targetRadius = m_IsRun ? targetRadiusRun : targetRadiusWalk; // transicion hacia correr
                }

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

                if (noiseCircle != null)
                    targetRadius = 1f; // transicion al radio minimo
            }
        }
    }
}
