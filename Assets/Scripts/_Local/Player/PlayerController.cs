using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCastDistance = 0.2f;

    [Header("Hanging Check")]
    public LayerMask hangingCheckLayer;
    public float hangingCastDistance = 0.2f;

    [Header("Jumping Check")]
    public float jumpCastDistance = 0.5f;
    public float jumpCastRadius = 0.3f;


    private Rigidbody2D m_Rigidbody2D;
    private CapsuleCollider2D m_CapsuleCollider2D;
    private Animator m_Animator;
    private ContactFilter2D m_HangingCheckContactFilter2D = new ContactFilter2D();
    private ContactFilter2D m_GroundCheckContactFilter2D = new ContactFilter2D();
    private RaycastHit2D[] m_HitResults = new RaycastHit2D[2];


    private bool m_IsGrounded;
    private bool m_IsHanging;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        m_Animator = GetComponentInChildren<Animator>();

        m_HangingCheckContactFilter2D.useTriggers = true;
        m_HangingCheckContactFilter2D.useLayerMask = true;
        m_HangingCheckContactFilter2D.layerMask = hangingCheckLayer;

        m_GroundCheckContactFilter2D.useTriggers = false;
        m_GroundCheckContactFilter2D.useLayerMask = true;
        m_GroundCheckContactFilter2D.layerMask = groundLayer;

        Physics2D.queriesStartInColliders = false;
    }

    private void Update()
    {
        CheckForGround();
        
        if(!m_IsHanging)
        {
            CheckForHangableObject();
        }

        CheckForHanging();
    }

    private void FixedUpdate()
    {
        m_Rigidbody2D.velocity = new Vector2(speed, m_Rigidbody2D.velocity.y);
        
        //m_MovedDistance += speed * Time.deltaTime;
    }

    private void CheckForGround()
    {
        int hitCount = Physics2D.Raycast(transform.position + ((Vector3)m_CapsuleCollider2D.offset - Vector3.up * m_CapsuleCollider2D.size.y/2) * transform.lossyScale.y, 
                                            Vector3.down, m_GroundCheckContactFilter2D, m_HitResults, groundCastDistance);

        Debug.DrawRay(transform.position + ((Vector3)m_CapsuleCollider2D.offset - Vector3.up * m_CapsuleCollider2D.size.y / 2) * transform.lossyScale.y, 
                                            Vector3.down * groundCastDistance);

        if(hitCount>0)
        {
            m_IsGrounded = true;
        }
        else
        {
            m_IsGrounded = false;
        }

    }

    private void CheckForHangableObject()
    {
        int hitCount = Physics2D.CircleCast(transform.position + ((Vector3)m_CapsuleCollider2D.offset + Vector3.up * m_CapsuleCollider2D.size.y / 2) * transform.lossyScale.y,
                                        jumpCastRadius, Vector3.one, m_HangingCheckContactFilter2D, m_HitResults, jumpCastDistance);

        Debug.DrawRay(transform.position + ((Vector3)m_CapsuleCollider2D.offset + Vector3.up * m_CapsuleCollider2D.size.y / 2) * transform.lossyScale.y,
                        Vector3.one * jumpCastDistance);

        if (hitCount > 0)
        {
            m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            m_Rigidbody2D.position = m_HitResults[0].point;
            m_IsHanging = true;
        }       
    }

    private void CheckForHanging()
    {
        int hitCount = Physics2D.Raycast(transform.position + ((Vector3)m_CapsuleCollider2D.offset + Vector3.up * m_CapsuleCollider2D.size.y / 2) * transform.lossyScale.y, 
                                        Vector3.up, m_HangingCheckContactFilter2D, m_HitResults, hangingCastDistance);

        Debug.DrawRay(transform.position + ((Vector3)m_CapsuleCollider2D.offset + Vector3.up * m_CapsuleCollider2D.size.y / 2) * transform.lossyScale.y, 
                        Vector3.up * hangingCastDistance);

        if (hitCount > 0)
        {
            m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            m_Rigidbody2D.position = m_HitResults[0].point;
        }
        else
        {
            m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            m_IsHanging = false;
        }

    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jumpCastRadius);
        Gizmos.DrawWireSphere(transform.position + Vector3.one * jumpCastDistance/2, jumpCastRadius);
        Gizmos.DrawWireSphere(transform.position + Vector3.one * jumpCastDistance, jumpCastRadius);
    }
#endif
}
