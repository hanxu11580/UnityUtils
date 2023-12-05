# System

- 系统可以另外实现 ISystemStartStop
	+ OnStartRunning() 第一次调用OnUpdate、系统Enabled属性从false->true
	+ OnStopRunning() OnDestory()、系统Enabled属性从true->false
	
### 系统组、系统更新顺序
- [UpdateBefore]、[UpdateAfter]可用于确定组中子项之间的相对排序顺序，例如：
	+ FooSystem具有属性[UpdateBefore(typeof(BarSystem))]，FooSystem将会安排在BarSystem之前某个位置，但是如果FooSystem和BarSystem不属于同一组，将忽略此属性
	
### 创建世界和系统
- 默认情况，自动引导会创建一个包含3个系统组的默认世界：
	+ InitializationSystemGroup Initialization
	+ SimulationSystemGroup Update
	+ PresentationSystemGroup PreLateUpdate

- 如果系统具有属性UpdateInGroup(typeof(InitializationSystemGroup))则该系统将被添加到InitializationSystemGroup而不是SimulationSystemGroup

- 自动引导
	+ #UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_RUNTIME_WORLD 禁用默认世界自动引导
	+ #UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_EDITOR_WORLD 禁用编辑器世界的自动引导
	+ #UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP 禁用默认世界和编辑器世界的自动引导

- 当自动引导被禁用，你的代码负责
	+ 创建你需要的任何世界
	+ 调用World.GetOrCreateSystem<T>() 将系统和系统组实例添加到世界中
	+ 注册顶级系统组（如SimulationSystemGroup）以在Unity PlayerLoop中更新
	+ 或者可以通过创建实现ICustomBootstrap

### 世界和系统中的时间
- 世界有一个Time属性，返回TimeData。 时间由UpdateWorldTimeSystem，可以使用以下方法操控时间
	+ SetTime 设置时间值
	+ PushTime 暂时更改时间值
	+ PopTime 恢复上次Push之前的时间值
	
### 系统状态
- 系统会传递一个参数SystemState：
	+ World 世界
	+ EntityManager
	+ Dependency 一个JobHandle用于传递系统之间的作业依赖关系
	+ GetEntityQuery() 返回EntityQuery
	+ GetComponentTypeHandle<T>()
	+ GetComponentLookup<T>()

### SystemAPI
- 涵盖了 World EntityManager SystemState相同的大部分功能
- SystemAPI方法依赖于source generators，因此仅适用于System和IJobEntity，但不适用于IJobChunk
- SystemAPI提供Query方法，通过source生成，有助于方便的与查询匹配实体和组件上创建foreach循环
