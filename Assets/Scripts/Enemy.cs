﻿using UnityEngine;

public class Enemy : LivingEntityData
{
    private Transform   myPlayer = null;
    private Rigidbody2D myRigidbody2D = null;
    private float       mySpeed = 1.0f;

    [SerializeField]
    private int         mySoulsGiven = 10;

    private void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (PauseManager.GetInstance().GetPauseMenuOpened())
        {
            return;
        }

        if (myLife <= 0)
        {
            FindObjectOfType<PlayerData>().AddSoulsCollected(mySoulsGiven);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(myPlayer != null)
        {
            Vector2 dir = myPlayer.position - transform.position;
            myRigidbody2D.MovePosition(myRigidbody2D.position + dir * mySpeed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D aCollision)
    {
        PlayerData playerData = aCollision.GetComponent<PlayerData>();
        if(playerData != null)
        {
            myPlayer = playerData.transform;
        }
    }

    private void OnTriggerExit(Collider aCollision)
    {
        PlayerData playerData = aCollision.GetComponent<PlayerData>();
        if (playerData != null)
        {
            myPlayer = null;
        }
    }
}
