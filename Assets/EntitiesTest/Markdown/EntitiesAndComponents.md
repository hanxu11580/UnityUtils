# Entities And Components

### 实体与组件
- 实体中同一种Component只能存在一个
- IComponentData 定义基本组件
- IBufferElementData 定义动态缓冲区（可增长数组）组件类型
- ISharedComponent 定义共享组件类型，由多个实体共享
- ICleanupComponent 定义清理组件类型
- 还有ICleanupShareComponent、ICleanupBufferElementData

### 世界和实体管理器
- 不要通过EntityManager访问Entities和Components
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

### 原型
- 添加或删除实体的组件会更改实体所属原型，从而需要EntityManager实际将实体及其组件从旧原型移动到新原型

### 块
- 原型的实体存储在该原型的16kb内存块中（chunks），每个块最多存储128个实体（具体取决于原型中组件类型数量和大小）
- 具有组件A和组件B的实体原型中，每个块将存储三个数组
	+ 实体ID的一个数组
	+ 存储组件A的第二个数组
	+ 存储组件B的第三个数组

### IComponentData
- IComponentData允许字段有：
	+ Blittable types
	+ bool
	+ char
	+ BlobAssetReference<T>
	+ Collections.FixedString
	+ Collections.FixedList
	+ Fixed array
	+ 符合这些相同限制的结构类型
	
### 托管IComponentData组件
- 类实现IComponentData，这些托管组件可以存储任何托管对象
- 托管组件，存在沉重的成本
	+ 托管组件不能在Burst编译的代码中使用
	+ 托管对象通常不能在作业中安全使用
	+ 托管组件并不能直接存储在块中，相反，世界所有托管组件都存储在一个大数组中，而块仅存储该数组的索引
	+ 和所有托管对象一样，创建托管组件会产生垃圾收集开销
	
### DynamicBuffer Components
- 要定义DynamicBuffer组件类型，请创建一个实现该IBufferElementData接口
- EntityManager使用动态缓冲区有以下关键方法
	+ AddComponent<T>() T可以是动态缓冲区组件类型
	+ AddBuffer<T>() 向实体添加一个类型为T的动态缓冲区组件，并返回DynamicBuffer<T>
	+ RemoveComponent<T> T可以是动态缓冲区组件类型
	+ HasBuffer<T>
	+ GetBuffer<T>
- DynamicBuffer<T>表示单个实体的类型T的动态缓冲区组件，主要属性和方法有：
	+ Length
	+ Capacity
	+ Item[Int32]
	+ Add()
	+ Insert()
	+ RemoveAt()

### Aspects
- 对于简化查询和组件相关代码很有用，TransformAspect，将标准变换组件LocalTransform、ParentTransfrom、WorldTransform

