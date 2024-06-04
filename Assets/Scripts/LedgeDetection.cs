using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatisground;
    [SerializeField] private Player player;

    private bool canDetected = true;

    private BoxCollider2D boxCd => GetComponent<BoxCollider2D>(); //invoke boxcollider of ledge check gizmo in player

    private void Update()
    {
        if (canDetected)
        player.ledgedetected = Physics2D.OverlapCircle(transform.position, radius, whatisground);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetected = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /* Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCd.bounds.center, boxCd.size, 0)

             foreach (var hit in colliders)
         {
             if(hit.gameObject.GetComponent<Platformcontroller>() !=null)
         }
      */
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetected = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
