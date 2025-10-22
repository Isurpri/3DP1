using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public enum TStates
    {
        IDLE=0,
        PATROL,
        ALERT,
        ATTACK,
        CHASE,
        HIT,
        DIE
    }
    public TStates m_state;
    private TStates m_previousState;
    NavMeshAgent m_NavMeshAgent;
    public Transform m_target;

    [Header("DistanceChace")]
    public float m_MinDistanceToAttack = 5f;
    public float m_MaxDistanceToAttack = 15f;

    [Header("Patrol")]
    public List<Transform> m_PatrolPosition;
    public int m_currentPatrolPos;

    [Header("Sight")]
    public float m_EyesHeight = 0.5f;
    public float m_SightAngle = 60f;
    public LayerMask m_SightLayerMask;

    [Header("Ears")]
    public float m_MaxEarDistance = 3f;

    [Header("Alert")]
    public float m_AlertRotateSpeed = 90f;
    float m_AlertTimer;
    public float m_AlertMaxTime = 3f;

    [Header("Attack")]
    public float m_ShootDistanceMax = 10;
    public float m_FireRate = 1;
    float m_FireCooldown;
    public GameObject m_Bullet;

    [Header("Life")]
    public float m_life = 50;
    public float m_Maxlife = 50;

    [Header("Hit")]
    private float m_HitDuration = 0.5f;
    private float m_HitTimer = 0f;
    public ParticleSystem m_HitParticles;
    public ParticleSystem m_DieParticles;

    [Header("LifeBar")]
    public Transform m_LifeBarTransform;
    public LifeBarElementUI m_LifeBarElementUI;

    [Header("Dead")]
    public List<MeshRenderer> m_MeshesRend;
    float m_currenTime;
    public float m_DieTime = 1.5f;
    public List<GameObject> m_dropObject;
    public float m_DropChance = 0.9f;

    private void Awake()
    {
        m_NavMeshAgent=GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        InitFade();
        SetIdleState();
    }
    void InitFade()
    {
        foreach (MeshRenderer meshRenderer in m_MeshesRend)
        {
            meshRenderer.sharedMaterial = Material.Instantiate(meshRenderer.sharedMaterial);
        }
    }
    void SetFadeValue(float Pct)
    {
        foreach (MeshRenderer meshRenderer in m_MeshesRend)
        {
            meshRenderer.sharedMaterial.SetFloat("_Cutoff", Pct);
        }
    }
    private void Update()
    {
        switch (m_state) 
        {
            case TStates.IDLE:
                UpdateIdleState(); 
                break;
            case TStates.PATROL:
                UpdatePatrolState(); 
                break;
            case TStates.ALERT:
                UpdateAlertState();
                break;
            case TStates.ATTACK:
                UpdateAttackState(); 
                break;
            case TStates.CHASE:
                UpdateChaseState();
                break;
            case TStates.HIT:
                UpdateHitState();
                break;
            case TStates.DIE:
                UpdateDieState();
                break;
        }
        UpdateLifeBarUI();
    }
    void UpdateLifeBarUI()
    {
        m_LifeBarElementUI.Show(m_LifeBarTransform.position, m_life/(float)m_Maxlife);
    }
    void SetIdleState()
    {
        m_state = TStates.IDLE;
        SetFadeValue(0.0f);
    }
    void UpdateIdleState()
    {
        SetPatrolState();
    }
    void SetPatrolState()
    {
        m_state = TStates.PATROL;
        m_currentPatrolPos = 0;
        MoveToNextPatrolPosition();
    }
    void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus==NavMeshPathStatus.PathComplete)
        {
            MoveToNextPatrolPosition();
        }
        if(HearsPlayer())
            SetAlertState();
    }
    void SetAlertState()
    {
        m_state = TStates.ALERT;
        m_AlertTimer = 0f;

        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.ResetPath();

    }
    void UpdateAlertState()
    {
        transform.Rotate(Vector3.up, m_AlertRotateSpeed * Time.deltaTime);
        m_AlertTimer += Time.deltaTime;

        if (SeePlayer())
        {
           SetChaseState();
        }
        else if (m_AlertTimer >= m_AlertMaxTime)
        {
            SetPatrolState();
        }
    }
    void SetChaseState()
    {
        m_state = TStates.CHASE;
    }
    void UpdateChaseState()
    {
        Vector3 l_PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        float l_Distance = Vector3.Distance(transform.position, l_PlayerPos);

        if (l_Distance > m_MaxDistanceToAttack)
        {
            SetPatrolState();
        }

        if (l_Distance <= m_MinDistanceToAttack)
        {
            m_NavMeshAgent.isStopped = true; 
            FaceTarget(l_PlayerPos);  
            SetAttackState();
        }

        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.destination = SetNextChasePosition();
        FaceTarget(l_PlayerPos);
    }
    void SetAttackState()
    {
        m_state = TStates.ATTACK;
    }
    void UpdateAttackState()
    {
        Vector3 l_PlayerPos = GameManager.GetGameManager().GetPlayer().transform.position;
        float l_Distance = Vector3.Distance(transform.position, l_PlayerPos);
        if (m_ShootDistanceMax<l_Distance)
        {
            SetChaseState();
        }
        
        FaceTarget(l_PlayerPos);

        m_FireCooldown += Time.deltaTime;
        if (m_FireCooldown>=m_FireRate)
        {
            ShootPlayer(l_PlayerPos);
            m_FireCooldown = 0;

        }
    }
    
    void SetHitState()
    {
        m_state = TStates.HIT;
        m_HitTimer = 0f;
        m_NavMeshAgent.isStopped = true;
    }
    void UpdateHitState()
    {
        m_HitTimer += Time.deltaTime;

        if (m_HitTimer >= m_HitDuration)
        {
            if (m_previousState == TStates.IDLE || m_previousState == TStates.PATROL)
                SetAlertState();
            else
                SetPreviousState();
        }
    }
    void SetPreviousState()
    {
        m_state = m_previousState;
        m_NavMeshAgent.isStopped = false;
    }
    void SetDieState()
    {
        m_state = TStates.DIE;
        m_currenTime = 0.0f;
    }
    void UpdateDieState()
    {
        m_currenTime += Time.deltaTime;
        float l_Pct = Mathf.Min(1.0f, m_currenTime / m_DieTime);
        m_DieParticles.Play();
        SetFadeValue(l_Pct);
        if(l_Pct == 1.0f)
        {
            DroppingItems();
            gameObject.SetActive(false);
        }
    }

    Vector3 SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameManager.GetGameManager().GetPlayer().transform.position;
        Vector3 l_direction = (l_PlayerPosition - transform.position).normalized;
        l_direction.Normalize();
        Vector3 l_Position = l_PlayerPosition - l_direction * m_MinDistanceToAttack;
        return l_Position;
    }
    

    void MoveToNextPatrolPosition()
    {
        Vector3 l_Destination=m_PatrolPosition[m_currentPatrolPos].position;
        m_NavMeshAgent.destination= l_Destination;
        ++m_currentPatrolPos;
        if (m_currentPatrolPos>=m_PatrolPosition.Count)
        {
            m_currentPatrolPos = 0;
        }
    }

    void ShootPlayer(Vector3 targetPos)
    {
        if (m_Bullet==null)
        {
            return;
        }
        GameObject bullet = Instantiate(m_Bullet, transform.position + transform.forward,Quaternion.identity);

        Vector3 direction = (targetPos - bullet.transform.position).normalized;

        BulletEnemie ScriptBullet = bullet.GetComponent<BulletEnemie>();

        ScriptBullet.Init(direction);
        Destroy(bullet, ScriptBullet.m_timeToDestroy);
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        if (direction!=Vector3.zero)
        {
            Quaternion lookPlayer = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookPlayer, Time.deltaTime * 5f);
        }
    }
    bool SeePlayer()
    {
        Vector3 l_PlayerPosition = GameManager.GetGameManager().GetPlayer().transform.position;
        Vector3 l_direction = l_PlayerPosition - transform.position;
        float l_Distance = l_direction.magnitude;
        //l_direction.Normalize();
        l_direction/=l_Distance;//Es lo mismo que normalizarlo
        float l_DotValue= Vector3.Dot(l_direction,transform.forward);
        if (l_DotValue>=Mathf.Cos(m_SightAngle*0.5f*Mathf.Deg2Rad))
        {
            Ray l_Ray = new Ray(transform.position+Vector3.up*m_EyesHeight,l_direction);
            //float l_Distance=Vector3.Distance(l_PlayerPosition, transform.position);
            if (Physics.Raycast(l_Ray,l_Distance,m_SightLayerMask.value))
            {
                return true;
            }
        }
        return false;
    }

    bool HearsPlayer()
    {
        Vector3 l_PlayerPosition = GameManager.GetGameManager().GetPlayer().transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, transform.position);
        return l_Distance < m_MaxEarDistance;
    }

    public void Hit(int damage)
    {
        m_life -= damage;
        m_HitParticles.Play();

        if (m_life <= 0)
            SetDieState();

        if (m_state == TStates.DIE || m_state == TStates.HIT)
            return;

        m_previousState = m_state;

        SetHitState();
    }

    public void DroppingItems()
    {
        if (m_dropObject == null || m_dropObject.Count == 0) 
            return;
       
        float randomChance = UnityEngine.Random.value; 
        if (randomChance > m_DropChance)
            return;

        int randomItem = UnityEngine.Random.Range(0, m_dropObject.Count);
        GameObject droppedItem = m_dropObject[randomItem];

        Instantiate(droppedItem,transform.position + Vector3.down * 0.5f, Quaternion.identity);
    }
}

