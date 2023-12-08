using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;

namespace EntitiesTest.CubeSpawner {
    public class ClickDetector : MonoBehaviour {
        [SerializeField] Camera _camera;
        [SerializeField] int _spawnCount = 1;
        private EntityManager entityManager;

        private void Start() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Update() {
            // 点击平面，根据找到的CubeSpawner组件，设置位置和生产的数量
            // 然后再根据CubeSpawnerISystem生成实体
            if (Input.GetMouseButtonDown(0)) {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = _camera.ScreenPointToRay(mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit)) {
                    GameObject hitGameObject = hit.collider.gameObject;
                    if(hitGameObject == gameObject) {
                        var entities = entityManager.CreateEntityQuery(typeof(CubeSpawner)).ToEntityArray(Allocator.Temp);
                        foreach (var entity in entities) {
                            var spawner = entityManager.GetComponentData<CubeSpawner>(entity);
                            spawner.spawnCount = _spawnCount;
                            spawner.spawnPosition = hit.point;
                            entityManager.SetComponentData(entity, spawner);
                        }
                    }
                }
            }
        }
    }
}