﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FSM States for the enemy
public enum EnemyState { STATIC, CHASE, REST, MOVING, DEFAULT };

public enum EnemyBehavior { EnemyBehavior1, EnemyBehavior2, EnemyBehavior3 };

public class Enemy : MonoBehaviour
{
    //pathfinding
    protected PathFinder pathFinder;
    public GenerateMap mapGenerator;
    protected Queue<Tile> path;
    protected GameObject playerGameObject;

    public Tile currentTile;
    protected Tile targetTile;
    public Vector3 velocity;

    //properties
    public float speed = 1.0f;
    public float visionDistance = 5;
    public int maxCounter = 5;
    protected int playerCloseCounter;

    protected EnemyState state = EnemyState.DEFAULT;
    protected Material material;

    public EnemyBehavior behavior = EnemyBehavior.EnemyBehavior1;

    // Start is called before the first frame update
    void Start()
    {
        path = new Queue<Tile>();
        pathFinder = new PathFinder();
        playerGameObject = GameObject.FindWithTag("Player");
        playerCloseCounter = maxCounter;
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (mapGenerator.state == MapState.DESTROYED) return;

        // Stop Moving the enemy if the player has reached the goal
        if (playerGameObject.GetComponent<Player>().IsGoalReached() || playerGameObject.GetComponent<Player>().IsPlayerDead())
        {
            //Debug.Log("Enemy stopped since the player has reached the goal or the player is dead");
            return;
        }

        switch (behavior)
        {
            case EnemyBehavior.EnemyBehavior1:
                HandleEnemyBehavior1();
                break;
            case EnemyBehavior.EnemyBehavior2:
                HandleEnemyBehavior2();
                break;
            case EnemyBehavior.EnemyBehavior3:
                HandleEnemyBehavior3();
                break;
            default:
                break;
        }

    }

    public void Reset()
    {
        Debug.Log("enemy reset");
        path.Clear();
        state = EnemyState.DEFAULT;
        currentTile = FindWalkableTile();
        transform.position = currentTile.transform.position;
    }

    Tile FindWalkableTile()
    {
        Tile newTarget = null;
        int randomIndex = 0;
        while (newTarget == null || !newTarget.mapTile.Walkable)
        {
            randomIndex = (int)(Random.value * mapGenerator.width * mapGenerator.height - 1);
            newTarget = GameObject.Find("MapGenerator").transform.GetChild(randomIndex).GetComponent<Tile>();
        }
        return newTarget;
    }

    // Dumb Enemy: Keeps Walking in Random direction, Will not chase player
    private void HandleEnemyBehavior1()
    {
        switch (state)
        {
            case EnemyState.DEFAULT: // generate random path 

                //Changed the color to white to differentiate from other enemies
                material.color = Color.white;

                if (path.Count <= 0) path = pathFinder.RandomPath(currentTile, 20);

                if (path.Count > 0)
                {
                    targetTile = path.Dequeue();
                    state = EnemyState.MOVING;
                }
                break;

            case EnemyState.MOVING:
                //move
                velocity = targetTile.gameObject.transform.position - transform.position;
                transform.position = transform.position + (velocity.normalized * speed) * Time.deltaTime;

                //if target reached
                if (Vector3.Distance(transform.position, targetTile.gameObject.transform.position) <= 0.05f)
                {
                    currentTile = targetTile;
                    state = EnemyState.DEFAULT;
                }

                break;
            default:
                state = EnemyState.DEFAULT;
                break;
        }
    }

    // TODO: Enemy chases the player when it is nearby
    private void HandleEnemyBehavior2()
    {
        switch (state)
        {
            case EnemyState.DEFAULT: // generate random path 

                // If player is within vision distance, CHASE
                if (Vector3.Distance(transform.position, playerGameObject.transform.position) <= visionDistance)
                {
                    state = EnemyState.CHASE;
                }
                else // Player moves randomly
                {
                    //Changed the color to white to differentiate from other enemies
                    material.color = Color.cyan;
                    // If path empty, populate it randomly
                    if (path.Count <= 0) path = pathFinder.RandomPath(currentTile, 20);
                    // If path has tile in queue, move to that tile
                    if (path.Count > 0)
                    {
                        targetTile = path.Dequeue();
                        state = EnemyState.MOVING;
                    }
                }
                break;

            case EnemyState.MOVING:
                //move
                velocity = targetTile.gameObject.transform.position - transform.position;
                transform.position = transform.position + (velocity.normalized * speed) * Time.deltaTime;

                //if target reached
                if (Vector3.Distance(transform.position, targetTile.gameObject.transform.position) <= 0.05f)
                {
                    currentTile = targetTile;
                    state = EnemyState.DEFAULT;
                }
                break;

            case EnemyState.CHASE:
                //Changed the color to red to denote chasing action
                material.color = Color.red;
                // If path is empty, populate it with path to Player tile
                if (path.Count <= 0) path = pathFinder.FindPathAStar(currentTile, playerGameObject.GetComponent<Player>().currentTile);
                // If path has a tile in queue, move to that tile
                if (path.Count > 0)
                {
                    targetTile = path.Dequeue();
                    state = EnemyState.MOVING;
                }

                // If player out of range, switch back to random movement
                //if (Vector3.Distance(transform.position, playerGameObject.transform.position) > visionDistance)
                //{
                //    state = EnemyState.DEFAULT;
                //}
                break;
            default:
                state = EnemyState.DEFAULT;
                break;
        }
    }

    // TODO: Third behavior (Describe what it does)
    private void HandleEnemyBehavior3()
    {
        switch (state)
        {
            case EnemyState.DEFAULT: // generate random path 

                // If player is within vision distance, CHASE
                if (Vector3.Distance(transform.position, playerGameObject.transform.position) <= visionDistance)
                {
                    state = EnemyState.CHASE;
                }
                else // Player moves randomly
                {
                    //Changed the color to white to differentiate from other enemies
                    material.color = Color.magenta;
                    // If path empty, populate it randomly
                    if (path.Count <= 0) path = pathFinder.RandomPath(currentTile, 20);
                    // If path has tile in queue, move to that tile
                    if (path.Count > 0)
                    {
                        targetTile = path.Dequeue();
                        state = EnemyState.MOVING;
                    }
                }
                break;

            case EnemyState.MOVING:
                //move
                velocity = targetTile.gameObject.transform.position - transform.position;
                transform.position = transform.position + (velocity.normalized * speed) * Time.deltaTime;

                //if target reached
                if (Vector3.Distance(transform.position, targetTile.gameObject.transform.position) <= 0.05f)
                {
                    currentTile = targetTile;
                    state = EnemyState.DEFAULT;
                }
                break;

            case EnemyState.CHASE:
                //Changed the color to red to denote chasing action
                material.color = Color.red;
                
                // If path empty, populate it with path adjacent to Player tile
                if (path.Count <= 0)
                {
                    // Instead of finding a random adjacent-adjacent tile, just get first two adjacent tiles that are walkable
                    foreach (Tile t in playerGameObject.GetComponent<Player>().currentTile.Adjacents)
                    {
                        if (t.isPassable)
                        {
                            targetTile = t;
                            break;
                        }
                    }
                    foreach (Tile t in playerGameObject.GetComponent<Player>().currentTile.Adjacents)
                    {
                        if (t.isPassable)
                        {
                            targetTile = t;
                            break;
                        }
                    }
                    path = pathFinder.FindPathAStar(currentTile, targetTile);
                }
                // If path has tile in queue, move to that tile
                if (path.Count > 0)
                {
                    targetTile = path.Dequeue();
                    state = EnemyState.MOVING;
                }

                // If out of range, go back to default
                //if (Vector3.Distance(transform.position, playerGameObject.transform.position) > visionDistance) // If player out of range, switch back to random movement
                //{
                //    state = EnemyState.DEFAULT;
                //}
                break;
            default:
                state = EnemyState.DEFAULT;
                break;
        }
    }
}
