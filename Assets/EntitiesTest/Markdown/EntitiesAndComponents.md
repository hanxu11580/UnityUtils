# Entities And Components

### 实体与组件
- IComponentData 定义基本组件
- IBufferElementData 定义动态缓冲区（可增长数组）组件类型
- ISharedComponent 定义共享组件类型，由多个实体共享
- ICleanupComponent 定义清理组件类型
- 还有ICleanupShareComponent、ICleanupBufferElementData

### 世界和实体管理器
- EntityManager方法包括
	+ CreateEntity() 创建一个新实体 √
	+ Instantiate() 使用现有实体的所有组件的副本创建一个新实体 √
	+ DestoryEntity() 销毁实体 √
	+ AddComponent<T>() 添加组件 √
	+ RemoveComponent<T>() 移出组件 √
	+ HasComponent<T>() 是否含有T组件
	+ GetComponent<T>() 获得组件
	+ SetComponent<T>() 覆盖组件
- 上方 √ 均为结构变化操作