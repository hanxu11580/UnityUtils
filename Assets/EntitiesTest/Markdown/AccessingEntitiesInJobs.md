# 在作业中搜索实体

- IJobChunk 对于与查询匹配的每个单独块调用一次
- IJobEntity 对呀每个查询匹配的实体调用一次
- IJobEntity实际上并不是真正的作业类型：源代码生成IJobEntity使用IJobChunk，所以事实上，IJobEntity最终被安排为IJobChunk
- 作业中进行结构更改是不安全的，因此通常你应该只在主线程上进行结构更改。为了解决此限制，作业可以在EntityCommandBuffer中记录结构更改命令，然后稍后在主线程上回放命令

### 同步点
### 组件安全手柄
### SystemState.Dependency
### ComponentLookup<T>
- 以上文档：https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/EntitiesSamples/Docs/entities-jobs.md