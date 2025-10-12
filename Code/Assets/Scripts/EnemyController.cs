using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    enum TStates
    {
        IDLE=0,
        PATROL,
        ALERT,
        ATTACK,
        CHASE,
        HIT,
        DIE
    }
    TStates m_state;
    NavMeshAgent m_NavMeshAgent;
    public Transform m_target;

    [Header("Distance")]
    public float m_MinDistanceToAttack = 5f;

    [Header("Patrol")]
    public List<Transform> m_PatrolPosition;
    public int m_currentPatrolPos;

    [Header("Sight")]
    public float m_EyesHeight = 1.8f;
    public float m_SightAngle = 60f;
    public LayerMask m_SightLayerMask;

    [Header("Ears")]
    public float m_MaxEarDistance = 3f;

    [Header("Life")]
    public float m_life = 50;

    private void Awake()
    {
        m_NavMeshAgent=GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        SetIdleState();
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
    }
    void SetIdleState()
    {
        m_state = TStates.IDLE;
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
        SeePlayer();
    }
    void UpdateAlertState()
    {

    }
    void SetAttackState()
    {
        m_state = TStates.ATTACK;
    }
    void UpdateAttackState()
    {

    }
    void SetChaseState()
    {
        m_state = TStates.CHASE;
    }
    void UpdateChaseState()
    {

    }
    void SetHitState()
    {
        m_state = TStates.HIT;
    }
    void UpdateHitState()
    {

    }
    void SetDieState()
    {
        m_state = TStates.DIE;
        gameObject.SetActive(false);
    }
    void UpdateDieState()
    {

    }

    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameManager.GetGameManager().GetPlayer().transform.position;
        Vector3 l_direction= l_PlayerPosition -transform.position;
        l_direction.Normalize();
        Vector3 l_Position = l_PlayerPosition-l_direction*m_MinDistanceToAttack;
        m_NavMeshAgent.destination = l_Position;
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
        m_life -=damage;
        if (m_life <= 0)
            SetDieState();
    }
}
