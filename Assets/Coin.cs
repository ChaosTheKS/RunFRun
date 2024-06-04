using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /* if (collision.tag == "Player") //Use tag in Unity to get collision, one of the ways to detect
            Debug.Log("Coin"); */

        if (collision.GetComponent<Player>() != null) //There is a player component
        {
            GameManager.instance.coins++;
            Destroy(gameObject);
        }
    }
}
