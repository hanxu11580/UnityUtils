# Entity Command Buffers

- EntityCommandBuffer.PlayBack() 记录命令稍后在我们调用主线程时执行
- EntityCommandBuffer有许多和EntityManager方法相同的方法：
	+ CreateEntity() 记录创建新实体命令，返回临时实体ID
	+ DestoryEntity() 记录摧毁实体的命令
	+ AddComponent<T>() 记录类型T的组件添加实体的命令
	+ RemoveComponent<T>() 记录从实体中移除T类型组件的命令
	+ SetComponent<T>() 记录设置T类型组件的命令
	+ AppendToBuffer() 记录一个命令，该命令会将单个值附加到实体现在缓冲区的末尾
	+ AddBuffer() 返回一个DynamicBuffer，存储在录制的命令中的缓冲区，该缓冲区的内容将在播放时创建实体时复制到实体的实际缓冲区中。实际上，写入返回的缓冲区允许你设置组件的初始内容
	+ SetBuffer() 与AddBuffer类似，但它假设实体已经具有组件类型的缓冲区，在播放时，实体已存在的缓冲区内容将被返回缓冲区的内容覆盖

### Job安全

### 临时实体
- 当调用EntityCommandBuffer的CreateEntity()、Instantiate()，在播放中执行该命令前不会创建新实体，因此这些方法返回的实体ID是临时的，索引号为负，后续EntityCommandBuffer的AddComponent、SetComponent、SetBuffer可以使用临时ID，在回放时，记录命令中的任何临时ID将被重新映射到实际的现有实体
- EntityCommandBuffer由于临时实体 ID在创建它的实例之外没有任何意义，因此它只能在同一EntityCommandBuffer实例的后续方法调用中使用

### EntityCommandBuffer.ParallelWriter
- 为了安全地记录来自并行作业的命令，我们需要一个EntityCommandBuffer.ParalleWriter。ParalleWriter具有大部分与EntityCommandBuffer自身相同的方法，但是ParalleWriter为了保证确定性，这些方法都采用int sortKey参数
- 因此IJobEntity通常使用的排序键是ChunkIndexInQuery，在IJobChunk，我们可以使用unfilteredChunkIndex等效参数

### 多重播放
- EntityCommandBuffer如果使用该选项，则可以多次调用PlaybackPolicy.MultiPlayback方法，多次调用Playback将引发异常，当你想要重复生成一组实体时，多重播放有用

### EntityCommandBufferSystem
- EntityCommandBufferSystem是一个提供延迟播放EntityCommandBuffer的便捷方法的系统。