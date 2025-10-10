using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    Vector3 m_startPosition;
    Quaternion m_startRotation;

    float m_Yaw;
    float m_Pitch;
    public float m_YawSpeed;
    public float m_PitchSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;
    public Transform m_PitchController;
    public bool m_UseInvertedYaw;
    public bool m_UseInvertedPitch;
    public CharacterController m_CharacterController;
    private float m_VerticalSpeed = 0.0f;

    public bool m_AngleLocked = false;
    public float m_Speed;
    public float m_Jumpspeed;
    public float m_SpeedMultiplier;
    public int m_Health = 100;
    public float m_totalAmount = 30;

    public Camera m_Camera;

    [Header("Shoot")]
    public float m_ShootMaxDistance = 50.0f;
    public LayerMask m_ShootLayerMask;
    public GameObject m_ShootParticles;
    public float m_ChargerAmmoCount = 24f;
    public float m_costAmmoShot = 1f;
    private float m_initialAmmo;
    bool isReloading = false;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_ReloadKeyCode = KeyCode.R;
    public int m_ShootMouseButton = 0;

    [Header("Ddebug Input")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;

    [Header("Animation")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ReloadAnimationClip;
    public AnimationClip m_ShootAnimationClip;

    public int m_Life = 100;
    void Start()
    {
        m_initialAmmo = m_ChargerAmmoCount;
        SetIdleAnimation();
        var l_Player = GameManager.GetGameManager().GetPlayer();
        if (l_Player != null)
        {
            l_Player.m_CharacterController.enabled = false;
            l_Player.transform.position = transform.position;
            l_Player.transform.position = transform.position;
            l_Player.m_CharacterController.enabled = true;
            l_Player.m_startPosition=transform.position;
            l_Player.m_startRotation=transform.rotation;
            GameObject.Destroy(gameObject);
            return;
        }
        m_startPosition = transform.position;
        m_startRotation = transform.rotation;
        DontDestroyOnLoad(gameObject);
        GameManager.GetGameManager().SetPlayer(this);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;

        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseX * m_YawSpeed * Time.deltaTime * (m_UseInvertedYaw ? -1.0f : 1.0f);
            m_Pitch = m_Pitch + l_MouseY * m_PitchSpeed * Time.deltaTime * (m_UseInvertedPitch ? -1.0f : 1.0f);

            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

            transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
            m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
        }

        Vector3 l_Movement = Vector3.zero;
        float l_YawPiRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90PiRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_ForwardDirection = new Vector3(Mathf.Sin(l_YawPiRadians), 0.0f, Mathf.Cos(l_YawPiRadians));
        Vector3 l_RightDirection = new Vector3(Mathf.Sin(l_Yaw90PiRadians), 0.0f, Mathf.Cos(l_Yaw90PiRadians));


        if (Input.GetKey(m_RightKeyCode))
            l_Movement = l_RightDirection;
        else if (Input.GetKey(m_LeftKeyCode))
            l_Movement = -l_RightDirection;

        if (Input.GetKey(m_UpKeyCode))
            l_Movement = l_ForwardDirection;
        else if (Input.GetKey(m_DownKeyCode))
            l_Movement = -l_ForwardDirection;

        float l_SpeedMultiplier = 1.0f;

        if (Input.GetKey(m_RunKeyCode))
            l_SpeedMultiplier = m_SpeedMultiplier;

        l_Movement.Normalize();
        l_Movement *= m_Speed * l_SpeedMultiplier * Time.deltaTime;

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;


        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if (m_VerticalSpeed < 0.0f && (l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            if (Input.GetKeyDown(m_JumpKeyCode))
                m_VerticalSpeed = m_Jumpspeed;
        }
        else if (m_VerticalSpeed > 0.0f && (l_CollisionFlags & CollisionFlags.Above) != 0)
            m_VerticalSpeed = 0.0f;

        if (CanShoot() && Input.GetMouseButtonDown(m_ShootMouseButton))
            Shoot();
        if (CanReload() && Input.GetKeyDown(m_ReloadKeyCode))
            Reload();
    }
    bool CanReload()
    {
        if (m_VerticalSpeed!=0)
            return false;

        if (Input.GetKey(m_RunKeyCode))
            return false;

        if (m_ChargerAmmoCount >= m_initialAmmo) //Cargador lleno
            return false;

        if (m_totalAmount <= 0)
            return false;

        return true;
    }
    void Reload()
    {
        SetReloadAnimation();

        float needed = m_initialAmmo - m_ChargerAmmoCount;

        if (needed <= 0)
            return; 

        if (m_totalAmount >= needed)
        {
            m_totalAmount -= needed;
            m_ChargerAmmoCount += needed;
        }
        else 
        {
            m_ChargerAmmoCount += m_totalAmount;
            m_totalAmount = 0;
        }
    }
    bool CanShoot()
    {
        if (m_ChargerAmmoCount<=0 || isReloading)
        {
            return false;
        }
        return true;
    }
    void Shoot()
    {
        m_ChargerAmmoCount -= m_costAmmoShot;
        SetShootAnimation();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_ShootMaxDistance, m_ShootLayerMask.value))
        {
            if (l_RayCastHit.collider.CompareTag("HitCollider"))
                l_RayCastHit.collider.GetComponent<HitCollider>().Hit();
            else
                CreateShootHitParticles(l_RayCastHit.point, l_RayCastHit.normal);
        }
    }
    void CreateShootHitParticles(Vector3 Position, Vector3 Normal)
    {
        GameObject l_ShootParticles = GameObject.Instantiate(m_ShootParticles);
        l_ShootParticles.transform.position = Position;
        l_ShootParticles.transform.rotation = Quaternion.LookRotation(Normal);
        l_ShootParticles.SetActive(true);
    }

    void SetIdleAnimation()
    {
        m_Animation.CrossFade(m_IdleAnimationClip.name);
    }
    void SetReloadAnimation()
    {
        if (!isReloading)
        {
            isReloading = true;
            m_Animation.CrossFade(m_ReloadAnimationClip.name, 0.1f);
            m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.1f);
            StartCoroutine(WaitForReloadEnd());
        }
    }

    IEnumerator WaitForReloadEnd()
    {
        yield return new WaitWhile(() => m_Animation.IsPlaying(m_ReloadAnimationClip.name));

        isReloading = false; 
    }
    void SetShootAnimation()
    {
        m_Animation.CrossFade(m_ShootAnimationClip.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.1f);
    }
    public void AddAmmo(int Ammo)
    {
        m_totalAmount += Ammo;
        /*if (m_ChargerAmmoCount>=m_initialAmmo)
        {
            m_ChargerAmmoCount = m_initialAmmo;
        }*/
    }
    public void AddHealing(int Healing)
    {
        m_Health += Healing;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item l_Item = other.GetComponent<Item>();
            if (l_Item.CanPick())
            {
                l_Item.Pick();
            }
        }else if (other.CompareTag("Deadzone"))
        {
            Kill();
        }
    }
    void Kill()
    {
        GameManager.GetGameManager().RestartLevel();
    }
    public void Restart()
    {
        m_CharacterController.enabled = false;
        transform.position = m_startPosition;
        transform.position = m_startPosition;
        m_CharacterController.enabled = true;
    }
}
