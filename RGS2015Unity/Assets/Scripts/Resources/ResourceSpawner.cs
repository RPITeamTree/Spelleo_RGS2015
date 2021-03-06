﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceSpawner : MonoBehaviour
{
    private const int RESOURCE_CAP = 35;
    private const float CHECK_DIST = 0.3f;

    // General
    public Resource rock_prefab, crystal_prefab;
    private Resource root_resource = null;

    // Resource spawn probabilities
    private float chance_crystal = 0.2f;

    // Sound
    public WorldSound rock_break_sound_prefab;
    public WorldSound crystal_break_sound_prefab;


    private void Awake()
    {
        if (rock_break_sound_prefab != null) ObjectPool.Instance.RequestObjects(rock_break_sound_prefab, 5, true);
        if (crystal_break_sound_prefab != null) ObjectPool.Instance.RequestObjects(crystal_break_sound_prefab, 5, true);
    }
    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(StartSpawningAfterDelay());
    }

    private IEnumerator StartSpawningAfterDelay()
    {
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(UpdateSpawning());
    }
    private IEnumerator UpdateSpawning()
    {
        while (true)
        {
            if (transform.childCount < RESOURCE_CAP)
            {
                // grow resources
                for (int i = 0; i < Random.Range(10, 20); ++i)
                {
                    ResourceType type = Random.value < chance_crystal ? ResourceType.Crystal : ResourceType.Rock;
                    SpawnResource(type);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(Random.Range(10, 15f));
        }
    }

    private void SpawnResource(ResourceType type)
    {
        Resource res = Instantiate<Resource>(
            type == ResourceType.Rock ? rock_prefab : crystal_prefab);
        res.transform.parent = transform;

        if (root_resource == null)
        {
            res.transform.position = transform.position;
            root_resource = res;
        }
        else
        {
            Stack<Resource> parent_chain = new Stack<Resource>();
            parent_chain.Push(root_resource);
            if (!PlaceResource(res, parent_chain))
            {
                Destroy(res.gameObject);
            }
        }
    }
    private bool PlaceResource(Resource res, Stack<Resource> parent_chain)
    {
        // Check if there is a free spot or another resource in each orthogonal direction
        Vector2[] dir_list = new Vector2[] { Vector2.right, -Vector2.right, Vector2.up, -Vector2.up };
        dir_list = GeneralHelpers.ShuffleArray(dir_list);

        bool prefer_chaining = true;


        for (int i = 0; i < 4; ++i)
        {
            Vector2 point = (Vector2)parent_chain.Peek().transform.position + dir_list[i] * CHECK_DIST;
            Collider2D col = Physics2D.OverlapPoint(point);
            if (col == null)
            {
                if (prefer_chaining) continue; // try another direction to find a resource to chain onto
                else
                {
                    // place resource here (base case)
                    res.transform.position = point;
                    return true;
                }
            }
            
            // is there another resource in the way? (that was not previously a parent in this recursion)
            Resource new_parent = col.GetComponent<Resource>();
            if (new_parent != null && !parent_chain.Contains(new_parent))
            {
                // try to place resource further down the chain
                Stack<Resource> new_chain = new Stack<Resource>(parent_chain);
                new_chain.Push(new_parent);
                if (PlaceResource(res, new_chain)) return true; 
            }
        }

        // we tried to find a spot further down the chain, but couldn't find one
        // now look at the immediatly open spots again
        if (prefer_chaining)
        {
            for (int i = 0; i < 4; ++i)
            {
                Vector2 point = (Vector2)parent_chain.Peek().transform.position + dir_list[i] * CHECK_DIST;
                Collider2D col = Physics2D.OverlapPoint(point);
                if (col == null)
                {
                    // place resource here
                    res.transform.position = point;
                    return true;
                }
            }
        }

        // no free locations, and we are as far down the chain as possible (unusual)
        return false;
    }
}
