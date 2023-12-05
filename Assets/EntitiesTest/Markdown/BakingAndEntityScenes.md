# Baking And Entity Scenes

- 子场景通过SubScene MonoBehaviour嵌入到另一个场景中Unity场景资源
- 实体场景是可以在运行时加载的一组序列化的实体和组件
- Baker是一个拓展类，其中Baker<T> T是MonoBehaviour，带Baker的MonoBehaviour称为创作组件
- 烘焙系统是一个标有属性的普通系统[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]

### 烘焙子场景通过几个主要步骤
- 对于子场景的每个GameObject，都会创建一个相应实体
- 执行子场景中每个创作组件的Baker，都可以读取创作组件并将组件添加到相应的实体中

### 访问Baker中数据
- GetComponent<T> 访问子场景中任何游戏对象的任何组件
- DependsOn() 跟踪此资产Baker
- GetEntity() 返回在子场景中烘焙或从预制体烘焙的实体的ID

### 加载和卸载实体场景
- 出于流式传输的目的，场景的实体被分成由索引号标识的部分。实体属于哪个部分由其SceneSection共享组件指定。默认情况下，实体属于第0部分，但这可以通过SceneSection烘焙期间的设置进行更改。
- 加载场景时，它由具有有关场景的元数据的实体表示，并且其各个部分也由一个实体表示。通过操作其实体的组件来加载和卸载单个部分RequestSceneLoaded：当该组件发生更改时，SceneSectionStreamingSystem会做出响应。SceneSystemGroup包含SceneSystem用于加载和卸载实体场景的静态方法：
	+ LoadSceneAsync() 启动场景加载。返回表示已加载场景的实体
	+ LoadPrefabAsync() 启动预制件的加载。返回引用已加载预制件的实体
	+ UnloadScene() 销毁已加载场景的所有实体
	+ IsSceneLoaded() 如果场景已加载，则返回 true
	+ IsSectionLoaded() 如果加载了某个部分，则返回 true
	+ GetSceneGUID() 返回表示场景资源的 GUID（由其文件路径指定）
	+ GetScenePath() 返回场景资源的路径（由其 GUID 指定）
	+ GetSceneEntity() 返回表示场景的实体（由其 GUID 指定）
-