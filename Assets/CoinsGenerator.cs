using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinsGenerator : MonoBehaviour
{
    [SerializeField] private int AmountOfCoins;
    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;
    //[SerializeField] private float chanceToSpawn; spawnchance variable

    void Start()
    {
        AmountOfCoins = Random.Range(minCoins, maxCoins);

        int additionalOffset = AmountOfCoins / 2;

        for (int i = 0; i < AmountOfCoins; i++)
        {

            //bool canSpawn = chanceToSpawn > Random.Range(0, 100); add spawn chance for coins
            //Debug.Log(i);
            Vector3 offset = new Vector2(i - additionalOffset, 0); //offset for coingroups under parent. additionaloffset brings transform center to center of set of coins
            
            /*if (canSpawn == true) */ //Use with canspawn for a chance
            Instantiate(coinPrefab, transform.position + offset, Quaternion.identity, transform); //assign parent as the current transform. offset subsequent coins using offset

        }
    }

}
