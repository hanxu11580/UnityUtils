# System

- 系统可以另外实现 ISystemStartStop
	+ OnStartRunning() 第一次调用OnUpdate、系统Enabled属性从false->true
	+ OnStopRunning() OnDestory()、系统Enabled属性从true->false
	
### 系统组、系统更新顺序
- 