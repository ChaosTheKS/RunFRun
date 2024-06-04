using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] LevelPart;
    [SerializeField] private Vector3 NextPartPosition;

    [SerializeField] private float DistanceToSpawn;
    [SerializeField] private float DistanceToDelete;
    [SerializeField] private Transform Player;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GeneratePlatform();
        DeletePlatform();

    }

    private void GeneratePlatform()
    {
        while (Vector2.Distance(Player.transform.position, NextPartPosition) < DistanceToSpawn)
        {
            Transform part = LevelPart[Random.Range(0, LevelPart.Length)];

            Vector2 NewPosition = new Vector2(NextPartPosition.x - part.Find("StartPoint").position.x, 0);

            Transform newPart = Instantiate(part, NewPosition, transform.rotation, transform);

            NextPartPosition = newPart.Find("EndPoint").position;


        }
    }

    private void DeletePlatform()
    {
        if (transform.childCount > 0)
        {
            Transform PartToDelete = transform.GetChild(0);

            if (Vector2.Distance(Player.transform.position, PartToDelete.transform.position) > DistanceToDelete)
            {
                Destroy(PartToDelete.gameObject);
            }
        }
    }

}
