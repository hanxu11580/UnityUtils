# Transform Components And Systems

- Parent组件存储实体父级的ID
- 动态缓冲区组件Child存储实体子实体的ID
- 该PreviousParent组件存储实体父级的ID副本
- 尽管可以安全的读取实体的Child缓冲区组件，但不应该直接修改它，仅通过设置实体的Parent组件来改变变换层次结构
- 在每一帧中，LocalToWorldSystem计算每个实体的世界空间变换（来自LocalTransform实体及其祖先的组件）并将其分配给实体的LocalToWorld组件
- Entity.Graphics读取该LocalToWorld组件，但不读取任何其他转换组件，因此LocalToWorld是实体需要渲染的唯一转换组件