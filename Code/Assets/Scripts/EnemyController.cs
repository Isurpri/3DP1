using System;
using System.Collections.Generic;
using System.Numerics;
using System.Transactions;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class EnemyController : MonoBehaviour
{
    enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
        ATTACK,
        CHASE,
        HIT,
        DIE
    }
    TState m_State;
    NavMeshAgent m_NavMeshAgent;
    public Transform m_Target;

    [Header("Distance")]
    public float m_MinDistanceToAttack = 5f;

    [Header("Patrol")]
    public List<Transform> m_PatrolPositions;
    int m_CurrentPatrolPositionId = 0;

    [Header("Sight")]
    public float m_SightAngle = 60.0f;
    public LayerMask m_SightLayerMask;
    public float m_EyesHeight = 1.8f;

    [Header("Ears")]
    public float m_MaxEarDistance = 3.0f;

    [Header("Life")]
    public int m_Life = 50;
    
    private void Start()
    {
        SetIdleState();
    }
    private void Update()
    {
        switch (m_State)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.HIT:
                UpdateHitState();
                break;
            case TState.DIE:
                UpdateDieState();
                break;
        }
    }

    void SetIdleState()
    {
        m_State = TState.IDLE;
    }
    void UpdateIdleState()
    {
        SetPatrolState();
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
    }
    void UpdateAlertState()
    { }
    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_CurrentPatrolPositionId = 0;
        MoveToNextPatrolPosition();
    }
    void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            MoveToNextPatrolPosition();
        if (HearsPlayer())
            SetAlertState();
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
    }
    void UpdateAttackState()
    { }
    void SetChaseState()
    {
        m_State = TState.CHASE;
    }
    void UpdateChaseState()
    { }

    void SetHitState()
    {
        m_State = TState.HIT;
    }
    void UpdateHitState()
    { }
    void SetDieState()
    {
        m_State = TState.DIE;
        gameObject.SetActive(false);
    }
    void UpdateDieState()
    {

    }

    void SetNextPosition()
    {
        Vector3 l_PlayerPosition = GameManager.GetGameManager().GetPlayer().transform.position;
        Vector3 l_Direction = l_PlayerPosition - transform.position;
        l_Direction.Normalize();
        Vector3 l_Position = l_PlayerPosition - l_Direction * m_MinDistanceToAttack;
        m_NavMeshAgent.destination = l_Position;
    }
    // public List<Transform> m_PatrolPositions;
    // int m_CurrentPatrolPositionId = 0;
    void MoveToNextPatrolPosition()
    {
        Vector3 l_Destination = m_PatrolPositions[m_CurrentPatrolPositionId].position;
        m_NavMeshAgent.destination = l_Destination;
        ++m_CurrentPatrolPositionId;
        if (m_CurrentPatrolPositionId >= m_PatrolPositions.Count)
            m_CurrentPatrolPositionId = 0;
    }

    bool SeesPlayer()
    {
        Vector3 l_PlayerPosition = GameManager.GetGameManager().GetPlayer().transform.position;
        Vector3 l_Direction = l_PlayerPosition - transform.position;
        float l_Distance = l_Direction.magnitude;
        // l_Direction.Normalize();
        l_Direction /= l_Distance;
        float l_DotValue = Vector3.Dot(l_Direction, transform.forward);
        if (l_DotValue >= Mathf.Cos(m_SightAngle * 0.5f * Mathf.Deg2Rad))
        {
            Ray l_Ray = new Ray(transform.position + Vector3.up * m_EyesHeight, l_Direction);
            if (Physics.Raycast(l_Ray, l_Distance, m_SightLayerMask.value))
                return true;
        }
        return false;
    }
    bool HearsPlayer()
    {
        Vector3 l_PlayerPosition = GameManager.GetGameManager().GetPlayer().transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, transform.position);
        return l_Distance < m_MaxEarDistance;
    }
  
    public void Hit(int Damage)
    {
        m_Life-=Damage;
        if (m_Life < 0)
            SetDieState();
    }


}
