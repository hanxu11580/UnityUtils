using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;


public struct Spawner:IComponentData
{
    public Entity prefab;
    public float3 spawnPos;
    public float nextSpawnTime;
    public float spawnRate;
}
