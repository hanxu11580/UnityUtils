-   详细文档    https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/index.html
-   快速文档    https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/EntitiesSamples/Assets/README.md#entities-api-overview

### 概念

-   Entity
    +   https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/concepts-entities.html
    +   Entity类似轻量级GameObject，实体充当使得各个唯一组件关联起来的ID，而不是充当容器
    +   一个世界的EntityManager管理所有Entity
    
-   Component
    +   使用IComponentData没有方法的接口将结构标记为组件类型。此组件类型只能包含非托管数据，并且它们可以包含方法，但最佳实践是它们只是纯数据。

-   System
    +   ISystem（非托管）和SystemBase（托管）
    +   一般创建的是一个包含三个系统组的世界 
        *   InitializationSystemGroup、SimulationSystemGroup、PresentationSystemGroup
        *   默认添加到SimulationSystemGroup，可以使用[UpdateInGroup]覆盖

-   原型
    +   具有组件A、B的所有实体共享一个原型、具有组件A、B、C的所有实体共享一个原型
    +   当对实体进行组件的添加、删除，EntityManager会移动实体。例如具有组件A、B、C的实体，删除了B组件。将会把实体移动到A、C原型，如果没有A、C原型将会创建原型

-   原型块
    +   实体存储在与原型匹配的块中、具有相同原型的实体和组件都统一存储到一个块内，一个块16kib大小
    +   一个具有A、B的原型中，每个块有3个数组，一个数组存储A的组件值，一个数组存储B的组件值，最后一个数组存储实体ID
    +   第一实体存储在数组的index0位置，第一实体存储在数组的index1位置。当删除某个实体,块的最后一个实体将被移动以填充间隙
    +   当块最后一个实体被移除，块将会被销毁。当添加实体时，块满了将会创建新块
    
- 结构变更
    + 以下三种都视为结构变更
        * 创建、删除实体
        * 添加、删出组件
            - 当添加、删除组件，就改变了原型。也就意味着需要给实体换个块存放。没有的话则会创建块
        * 设置共享组件的值
            - 当设置时，会将实体移动到与新的共享组件值匹配的块中（不同值的共享组件会单独占有一个块？）
    + 同步点
        * 是迄今为止已调度的所有作业完成的点。同步点限制你在一段时间内使用作业系统中可用的所有工作线程的能力，所以应该避免同步点
        

### 组件概览

-   将组件添加到实体
    +   代码见ComponentAPITest.cs AddComponentToSingletonEntityEg AddComponentToMultipleEntitiesSystemExample
    
-   从实体中删除组件
    +   代码见ComponentAPITest.cs RemoveComponentSystemEg

-   读写实体组件的值
    +   访问一个实体的单个组件
    +   访问多个组件
    +   推迟组件值的更改：使用EntityCommandBuffer记录写入（不读取）组件的意图，稍后这些意图在主线程回放时更改
    
-   Aspect concepts
    +   概念
        *   Aspect是一个类似对象的包装器，可以使用他将实体组件的子集组合到单个C#结构中


### 系统概览

-   ISystem
    +   可以选择实现ISystemStartStop
        *   OnStartRunning
            -   第一次调用OnUpdate，系统停止或禁用后恢复回调
        *   OnStopRunning
            -   当系统被禁用或与系统更新所需的任何组件不匹配时系统事件回调
        
    +   调用顺序
        *   OnCreate->OnStartRunning->OnUpdate->OnStopRunning->OnDestory

    +   系统在主线程中调用，安排作业可以用以下方法
        *   IJobEntity  迭代多个实体的组件数据
        *   IJobChunk   按照原型块迭代数据
        
-   SystemBase
    +   系统也在主线程中调用，安排作业可以用以下方法
        *   Entities.ForEach    迭代组件数据
        *   Job.WithCode    使用lambda表达式后台执行
        *   IJobEntity  迭代多个实体的组件数据
        *   IJobChunk   按照原型块迭代数据
        
    +   调用顺序
        *   OnCreate->OnStartRunning->OnUpdate->OnStopRunning->OnDestory
        
-   定义管理系统数据
    +   系统上使用公共数据的问题
        *   系统之前存在了依赖关系，和面向数据冲突
        *   无法保证访问系统实例的线程和生命周期安全
        
    +   如何解决上面的问题？
        *   应当将公共数据存储在系统中的组件中，而不是作为系统组件存储
        *   state.SystemHandle代替系统实例
        
-   系统组
    +   用到再看

-   管理多个世界中系统
    +   用到再看
    
-   如何使用系统
    +   访问数据
        *   https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/systems-access-data-intro.html

    +   SystemAPI概述
        *   https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/systems-systemapi.html
        
-   安排数据变更
    +   ECB（实体命令缓冲区）概述
        *   ECB存储线程安全命令队列，你可以将其添加到队列中并稍后播放。你可以使用ECB来安排作业的结构更改，并且在作业完成后在主线程上执行更改。
        *   https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/systems-entity-command-buffers.html
        *   使用ECB
            -   UseEntityCommandBuffer.cs
        *   ECB Playback
            -   
