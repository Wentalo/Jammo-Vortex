using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTutorial.Manager; //Exporta aqui la clase del input xd

namespace UnityTutorial.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {

        #region Camera OG VariablesAndRotation
        [SerializeField] private Transform CameraManager;
        [SerializeField] private Transform CameraPivot;     //The object the camera uses to pivot (LookUp and Down
        [SerializeField] private Transform m_Camera;
        [SerializeField] private float defaultPosition;     //Z inicial  

        private Vector3 cameraFollowVelocity = Vector3.zero;
        private float cameraFollowSpeed = 0.2f; //0.2
        private float cameraLookSpeed = 2;
        private float cameraPivotSpeed = 2;
        public float coeficienteReduccionSensibilidad = 0.1f;  // MUY IMPORTANTE

        public float lookAngle; //Camera looking up and down
        public float pivotAngle; //Camera looking left and right
        private float minimumPivotAngle = -15;
        private float maximumPivotAngle = 30;
        #endregion

        #region Handle camera collisions

        public LayerMask collisionLayers;                   //Capas sobre las que hace efecto el raycast

        public GameObject currentHitObject;
        public float maxDistance;
        private float currentHitDistance;

        private Vector3 originCamera;
        private Vector3 directionCamera;

        private Vector3 cameraVectorPosition;   //Para editar la z de la camara
        public float cameraCollisionRadius = 2;
        public float cameraCollisionOffset = 0.2f; //How much the camera will jump off of the objects its colliding with
        public float minimumCollisionOffset = 1.5f;
        public float raycastCameraDistance = 3.5f;

        #endregion

        #region Animation, Rigibody and velocities basics

        [SerializeField] private float m_AnimBlendSpeed = 8.9f; //Velocidad de interpolacion de animaciones  //Esto deberia de cambiar??

        [SerializeField] private float m_JumpFactor = 4f;  //Cambiar a gusto
        [SerializeField] private float AirResistance = 0.8f;  //Cambiar a gusto

        private Rigidbody m_PlayerRigidbody;
        private InputManager m_InputManager;
        private Animator m_Animator;
        private bool m_HasAnimator;
        private int m_XvelHash;
        private int m_YvelHash;
        private int m_ZvelHash;

        private bool m_Grounded;
        private bool m_Jumping=false;

        private int m_PreJumpHash;
        private int m_JumpHash;
        private int m_GroundHash;
        private int m_FallingHash;
        private int m_JumpBool;

        private const float m_WalkSpeed = 2f;
        private const float m_RunSpeed = 6f;
        private Vector2 m_CurrentVelocity;
        #endregion

        #region Handle jump collisions

        public LayerMask GroundCheck;
        private Vector3 originJump;
        private Vector3 directionJump;
        public float raycastJumpDistance = 3.5f;

        [SerializeField] private float Dis2Ground = 0.8f;  //Revisar

        public GameObject currentHitJumpObject;
        public float maxJumpDistance;
        private float currentHitJumpDistance;



        #endregion

        #region Handle camera stuttering platform
        public LayerMask PlatformCheck;
        private bool m_Platform;
        #endregion

        private void Awake()
        {
            defaultPosition = m_Camera.transform.localPosition.z;
        }

        private void Start()
        {
            m_HasAnimator = TryGetComponent<Animator>(out m_Animator);
            m_PlayerRigidbody = GetComponent<Rigidbody>();
            m_InputManager = GetComponent<InputManager>();

            m_XvelHash = Animator.StringToHash("X_Velocity");
            m_YvelHash = Animator.StringToHash("Y_Velocity");
            m_ZvelHash = Animator.StringToHash("Z_Velocity");  //Asi se sabe a que velocidad ha caido

            m_PreJumpHash = Animator.StringToHash("PreJump"); //partido en dos para que funcioone en fisicas
            m_JumpHash = Animator.StringToHash("Jump");
            m_GroundHash = Animator.StringToHash("Grounded");
            m_FallingHash = Animator.StringToHash("Falling");
            m_JumpBool = Animator.StringToHash("JumpBool");


        }

        private void FixedUpdate()
        {
            SampleGround();
            Move();
            HandleJump();
            if(this.transform.parent !=null) FollowTarget();  //Tiene que ponerse aquí porque se tiene que mover al mismo ritmo que las plataformas
            

        }

        //El problema de stuttering de la camara lo abordamos con si esta sujeto al movimiento de algo (soy un crack)

        private void LateUpdate()
        {
            if (this.transform.parent == null)FollowTarget();
            RotateCamera();
            HandleCameraCollisions();
        }

        private void Move()
        {
            if (!m_HasAnimator) return; //Si no tiene animacion vuelve

            float targetSpeed = m_InputManager.m_Run ? m_RunSpeed : m_WalkSpeed; //Si presiona shift v = correr else v = caminar

            if (m_InputManager.m_Move == Vector2.zero) targetSpeed = 0f; //La v se acerca a 0 pero no lo es


            //Si el jugador está en el suelo se podrá mover con total libertad, si no tendra la resistencia del aire
            if (m_Grounded)
            {
                m_CurrentVelocity.x = Mathf.Lerp(m_CurrentVelocity.x, m_InputManager.m_Move.x * targetSpeed, m_AnimBlendSpeed * Time.fixedDeltaTime); //Smoother animation
                m_CurrentVelocity.y = Mathf.Lerp(m_CurrentVelocity.y, m_InputManager.m_Move.y * targetSpeed, m_AnimBlendSpeed * Time.fixedDeltaTime);

                var xVelDifference = m_CurrentVelocity.x - m_PlayerRigidbody.velocity.x; //Para que la aceleracion no sea infinita
                var zVelDifference = m_CurrentVelocity.y - m_PlayerRigidbody.velocity.z;

                m_PlayerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange); //local velocity to global velocity
            }
            else
            {
                m_PlayerRigidbody.AddForce(transform.TransformVector(new Vector3(m_CurrentVelocity.x * AirResistance, 0, m_CurrentVelocity.y * AirResistance)),ForceMode.VelocityChange); //local velocity to global velocity
            }

            

            m_Animator.SetFloat(m_XvelHash, m_CurrentVelocity.x);
            m_Animator.SetFloat(m_YvelHash, m_CurrentVelocity.y);


        }

        public void FollowTarget()
        {
            Vector3 targetPosition = Vector3.SmoothDamp
                (CameraManager.transform.position, transform.position, ref cameraFollowVelocity, cameraFollowSpeed); 
            CameraManager.transform.position = targetPosition;



            #region FPSCameraMode
            //Hace lo mismo desde dentro del personaje
            //Vector3 targetPosition = Vector3.SmoothDamp
            //    (m_Camera.transform.position, m_CameraRoot.transform.position, ref cameraFollowVelocity, cameraFollowSpeed); //Se acerca suavemente la camara al personaje (positions)
            //m_Camera.transform.position = targetPosition;
            #endregion
        }

        public void RotateCamera()
        {
            Vector3 rotation;
            var Mouse_X = m_InputManager.m_Look.x;
            var Mouse_Y = m_InputManager.m_Look.y;

            lookAngle = lookAngle + (Mouse_X * cameraLookSpeed) * coeficienteReduccionSensibilidad;
            pivotAngle = pivotAngle - (Mouse_Y * cameraPivotSpeed) * coeficienteReduccionSensibilidad;

            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle); //limites de la camara (Arriba y abajo)

            //Permite a la camara rotar alrededor del Jugador (Y)
            rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation); //Si te mueves a lo largo de AD lo normal es que gire respecto al eje Y
            CameraManager.rotation = targetRotation;

            //Permite a la camara rotar por encima y por debajo del Jugador (X)
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation); 
            CameraPivot.localRotation = targetRotation;

            //transform.Rotate(Vector3.up, Mouse_X * cameraLookSpeed * coeficienteReduccionSensibilidad);  //Esto es lo que hace que el jugador tenga la misma rotacion que la camara ? //Causa de stuttering
            m_PlayerRigidbody.MoveRotation(Quaternion.Euler(0, lookAngle, 0));  //El lookAngle al multplicarse por uno se queda bien y no hace falta el delta time



        }

        private void HandleCameraCollisions()
        {
            float targetPosition = defaultPosition;  //defaultPosition=m_camera.transform.localposition.z    Accede a la z de la camara

            RaycastHit hit;
            originCamera = CameraPivot.transform.position; //inicia en el pivote de la camara
            directionCamera = m_Camera.transform.position - CameraPivot.transform.position; //La direccion de la camara hacia el pivote
            directionCamera.Normalize();

            if (Physics.Raycast(originCamera, directionCamera, out hit, Mathf.Abs(targetPosition), collisionLayers)) //Mirar si poner mathf a targetPosition-0.1
            {
                //Info
                //Debug.Log("Distancia pivote a hit: "+ distance + "    targetposition: "+targetPosition);
                currentHitObject = hit.transform.gameObject;
                currentHitDistance = hit.distance;

                float distance = Vector3.Distance(originCamera, hit.point);
                targetPosition = targetPosition + 1.8f; //A la z le resta la distancia menos el offset minimo (lo que salta hacia afuera de los objetos
                
            }
            else
            {
                currentHitDistance = maxDistance;
                currentHitObject = null;
            }
            
            /*
             * Actualmente esto no hace nada
            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)  //Si es menor de lo que se puede se pone al minimo
            {
                targetPosition = targetPosition - minimumCollisionOffset; //Lo minimo que te alejas de los objetos?
            }
            */

            cameraVectorPosition.z = Mathf.Lerp(m_Camera.transform.localPosition.z, targetPosition, 0.05f); //A menor tiempo de actualizacion de lerp mas smooth irá la camara
            m_Camera.transform.localPosition = cameraVectorPosition;

        }

        private void HandleJump()
        {
            if (!m_HasAnimator) return;
            if (!m_InputManager.m_Jump) return; //booleano jump

            m_Animator.SetTrigger(m_PreJumpHash);
        }

        public void JumpAddForce()  
        {
            m_PlayerRigidbody.AddForce(-m_PlayerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);   //Para no resbalar en rampas

            Vector3 vel = m_PlayerRigidbody.velocity; //Le cambiamos la velocidad en y para que suba
            vel.y += m_JumpFactor;
            m_PlayerRigidbody.velocity = vel;

            m_Animator.ResetTrigger(m_PreJumpHash); //Asi no se queda pillada la animacion
        }
 
        private void SampleGround()
        {
            if (!m_HasAnimator) return;

            RaycastHit hitJumpInfo;

            originJump = m_PlayerRigidbody.worldCenterOfMass;                                    //inicia en el pivote de la jump
            directionJump = Vector3.down;   //La direccion de la camara hacia el pivote

            if (Physics.Raycast(originJump, directionJump, out hitJumpInfo, Dis2Ground + 0.15f, GroundCheck)) //Colliding with something //Mide el orginal 0.89 +0.1 y le pone de dis2 ground 0.8
            {

                m_Grounded = true;
                SetAnimationGrounding();
                #region Jump Info
                currentHitJumpObject = hitJumpInfo.transform.gameObject;
                currentHitJumpDistance = hitJumpInfo.distance;

                float distanceJump = Vector3.Distance(originJump, hitJumpInfo.point);

                //Debug.Log("Distancia pivote a hit: "+ distanceJump + "    targetposition: "+ Dis2Ground); //?
                #endregion
                return;

            }
            else
            {
                currentHitObject = null;
            }

            Debug.Log(m_Grounded);
            m_Grounded = false;
            m_Animator.SetFloat(m_ZvelHash, m_PlayerRigidbody.velocity.y);
            SetAnimationGrounding();
            return;

        }

        private void SetAnimationGrounding()
        {
            //Si esta en tierra no estara cayendo y viceversa
            m_Animator.SetBool(m_FallingHash, !m_Grounded);
            m_Animator.SetBool(m_GroundHash, m_Grounded);
        }

        private void OnCollisionEnter(Collision collision)  //Parecido al built-in controller .isgrounded
        {
            if (collision.gameObject.tag == "ground")  //Aqui detecta la colision con el suelo
            {
                //m_Grounded = true;
                //SetAnimationGrounding();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //Debug.DrawLine(origin, origin + direction * currentHitDistance);
            Debug.DrawRay(originCamera, directionCamera * raycastCameraDistance, Color.blue);
            Debug.DrawRay(originJump, directionJump * (Dis2Ground + 0.05f), Color.red);
            // Gizmos.DrawWireSphere(origin + direction * currentHitDistance, cameraCollisionRadius);
        }

    }
}
