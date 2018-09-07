using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public LayerMask hangingCheckLayer;
    public LayerMask groundLayer;
    public float castDistance = 0.2f;

    private Rigidbody2D m_Rigidbody2D;
    private CapsuleCollider2D m_CapsuleCollider2D;
    private ContactFilter2D m_ContactFilter2D = new ContactFilter2D();
    private RaycastHit2D[] m_HitResults = new RaycastHit2D[2];
    private float m_MovedDistance;



    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CapsuleCollider2D = GetComponent<CapsuleCollider2D>();

        m_ContactFilter2D.useTriggers = true;
        m_ContactFilter2D.useLayerMask = true;
        m_ContactFilter2D.layerMask = hangingCheckLayer;
        Physics2D.queriesStartInColliders = false;
    }

    private void Update()
    {
        CheckForHanging();
    }

    private void FixedUpdate()
    {
        m_Rigidbody2D.velocity = new Vector2(speed, m_Rigidbody2D.velocity.y);

        m_MovedDistance += speed * Time.deltaTime;
    }

    private void CheckForHanging()
    {
        int hitCount = Physics2D.Raycast(transform.position + transform.up * (m_CapsuleCollider2D.size.y / 2 + m_CapsuleCollider2D.offset.y) * transform.localScale.y, 
                                         transform.up, m_ContactFilter2D, m_HitResults, castDistance);

        Debug.DrawRay(transform.position + transform.up * (m_CapsuleCollider2D.size.y/2 + m_CapsuleCollider2D.offset.y) * transform.localScale.y, transform.up * castDistance);

        if (hitCount > 0)
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, QuaternionExtension.RotateToDirection(m_HitResults[0].normal, -90), 5f * Time.deltaTime);
            m_Rigidbody2D.position = m_HitResults[0].point;
            Debug.Log("Hit");
        }
        else
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, QuaternionExtension.RotateToDirection(Vector3.down, 90), 5f * Time.deltaTime);
        }

    }


}
