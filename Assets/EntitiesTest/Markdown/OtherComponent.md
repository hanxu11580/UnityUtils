# OtherComponent

doc:https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/EntitiesSamples/Docs/additional-entities-features.md

### Enableable Components
- 实现IComponentData、IBufferElementData也可以实现IEnableableComponent，实现此接口可以针对每个实体启用和禁用，禁用的组件仍然可以正常读取和修改，例如通过EntityManager
- 可以通过一下方式检查和设置实体组件的启用状态：
	+ EntityManager
		* IsComponentEnabled<T>() 如果实体当前具有启用T组件，则返回true
		* SetComponentEnabled<T>() 设置启用状态
	+ ComponentLookup<T>
	+ BufferLookup<T> 对于动态缓冲区
	+ EnabledRefRW<T> 用在SystemAPI.Query foreach或IJobEntity
	+ ArchetypeChunk
	
- 在IJobChunk中，Execute方法参数表明块中的哪些实体参与查询匹配
	+ 如果useEnableMask 参数false，则块中所有实体参数查询匹配
	+ 如果useEnableMask 参数true，则chunkEnableMask参数的位表示块中的哪些实体参与查询匹配，同时考虑查询的所有可启用组件类型
	+ ChunkEntityEnumerator可用更方便的迭代匹配实体，而不是手动检查这些掩码位

### Shared Components
- 对于共享组件类型，块中所有实体共享相同的组件值，而不是每个实体都有自己的值，因此设置实体的共享组件值会执行结构更改，实体被移动到具有新值的块
- 世界不是将共享组件值直接存储在块中，而是将他们存储在一组数组中，并且块仅将索引存储到数组中
- 共享组件实现ISharedComponentData，如果结构体包含任何托管类型字段，则共享组件本身将是托管组件类型
- EntityManager共享组件有以下关键方法：
	+ AddComponent<T>() 将 T 组件添加到实体，其中 T 可以是共享组件类型
	+ AddSharedComponent()	将非托管共享组件添加到实体并设置其初始值
	+ AddSharedComponentManaged()	将托管共享组件添加到实体并设置其初始值
	+ RemoveComponent<T>()	从实体中删除 T 组件，其中 T 可以是共享组件类型
	+ HasComponent<T>()	如果实体当前具有 T 组件，则返回 true，其中类型 T 可以是共享组件类型
	+ GetSharedComponent<T>()	检索实体的非托管共享 T 组件的值
	+ SetSharedComponent<T>()	覆盖实体的非托管共享 T 组件的值
	+ GetSharedComponentManaged<T>()	检索实体的托管共享 T 组件的值
	+ SetSharedComponentManaged<T>()	覆盖实体的托管共享 T 组件的值

- 拥有太多唯一的共享组件可以能导致块碎片
	+ 如果具有共享组件的原型有 500 个实体，并且每个实体都有唯一的共享组件值，则每个实体都单独存储在单独的块中。这浪费了每个块中的大部分空间，并且还意味着循环原型的所有实体需要访问 500 个块

### Cleanup Components
- 当具有清理组件的实体被销毁时，非清理组件将被删除，但该实体实际上会继续存在，直到单独删除所有清理组件
- 当一个实体被复制到另一个世界，以序列化方式复制或通过Instantiate复制时，原始实体的任何清理组件都不会添加到新实体中
- 清理组件有四种：
	+ Struct实现ICleanupComponentData 非托管类型的清理变体IComponentData
	+ Class实现ICleanupComponentData 托管IComponentData类型的清理变体
	+ Struct实现ICleanupBufferElementData 动态缓冲区类型的清理变体
	+ Struct实现ICleanupSharedComponentData 共享组件类型的清理变体
	
### Chunk Components
- 与常规组件不同，块组件属于整个块的单个值，而不是块内的任何实体
- 就像常规组件一样，块组件被定义为实现的结构或类IComponentData，但块组件可以使用以下EntityManager方法添加、删除、获取和设置：
	+ AddChunkComponentData<T>	将类型 T 的块组件添加到块中，其中 T 是托管或非托管IComponentData
	+ RemoveChunkComponentData<T>	从块中删除类型 T 的块组件，其中 T 是托管或非托管IComponentData
	+ HasChunkComponent<T>	如果块具有类型 T 的块组件，则返回 true
	+ GetChunkComponentData<T>	检索类型 T 的块的块组件的值
	+ SetChunkComponentData<T>	设置类型 T 的块的块组件的值

### Blob Assets

### 版本号
- 一个世界，世界的系统，和它的块维护几个版本号，通过比较版本号，你可以确定某些数据是否已更改
```csharp

	bool changed = (VersionB - VersionA) > 0

```
- 
	+ World.Version	每当世界添加或删除系统或系统组时都会增加
	+ EntityManager.GlobalSystemVersion	在世界上每次系统更新之前都会增加
	+ SystemState.LastSystemVersion	GlobalSystemVersion每次系统更新后立即分配该值
	+ EntityManager.EntityOrderVersion	每当世界发生结构性变化时都会增加

- 每个组件类型都有自己的版本号 EntityManager.GetComponentOrderVersion
- 每个共享组件值还具有一个版本号，每当结构更改影响具有该值的块时，该版本号就会增加
- 块存储块中每个组件类型的版本号，当访问块中的组件类型进行写入时，其版本号被分配为值EntityManager.GlobalSystemVersion，无论是否实际修改了任何组件值，可用通过调用该方法ArchetypeChunk.GetChangeVersion来检索这些块版本号
- EntityManager.GlobalSystemVersion块还存储每个组件类型的版本号，每当结构变化影响块时，该版本号就会被分配值，可以通过调用ArchetypeChunk.GetOrderVersion来检索这些块版本号