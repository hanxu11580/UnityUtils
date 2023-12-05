# C#作业系统
- 分配器
	+ Allocator.Persistent	最慢的分配器，用于无限期的生命周期分配。当不需要持久分配集合，必须调用Dispose
	+ Allocator.Temp	最快的分配器，每帧主线程会创建一个临时分配器，这个分配器在帧结束会被全部释放，不用调用Dispose，临时分配不能传递到作业中
	+ Allocator.TempJob 下一个最快的分配器，对于短期分配（4帧生命周期）可以传递到作业中
	

# C#作业和作业依赖性
- 当工作线程完成当前工作时，该线程从队列中取出等待的作业，并调用该作业的Excute()
- IJobParallelFor、IJobEntity、IJobChunk
- 将作业实例放入作业队列，请调用Schedule()，作业只能从主线程调度，不能从其他作业内部调度

# 依赖关系
- Schedule()返回一个JobHandle代表预定的作业。 我们可以使用依赖关系来规定计划作业之间的执行作顺序
- 可以使用JobHandle.CombineDependencies()将多个句柄组合成一个逻辑句柄，从而允许一项作业具有多个直接依赖关系

- 传递给作业的集合必须使用Allocator.Persistent、Allocator.TempJob或其他线程安全分配器进行分配，分配的集合Allocator.Temp不能传递到作业中

# Job安全检查
- 对于访问相同数据的任何两个作业，应当防止两个作业之间存在数据冲突
	+ 当调用Schedule(), 如果先安排一个使用本机数组的作业，然后安排第二个使用相同本机数组，但不依赖第一个作业的作业，则会引发异常

# 并行作业
- 将处理数组或列表的工作拆分到多个线程中，可以使用接口IJobParallelFor
- 当我们安排作业时，可以指定索引计数和批量大小，Execute会被调用0 ~ myArray.Length
- 如果myArray.Length = 250, batchSize = 100将会分成：
	+ 第一批：0 ~ 99
	+ 第二批：100 ~ 199
	+ 第三批：200 ~ 249
	+ 最多将在三个线程执行
```csharp
	public struct TestJob : IJobParallelFor{
		public NativeArray<int> nums;

		public void Execute(int index){
			nums[index] = index
		}
	}

	var job = new TestJob(nums = myArray)
	JobHandle handle = job.Schedule(myArray.Length, 100)
```
- 当处理一个批次，应该只访问自己批次的数组或列表索引，如果我们使用参数以外的任何值对数组或列表进行索引，将会异常（下面将会异常）
```csharp
	public void Execute(int index){
		nums[index] = nums[0]
	}

```

###
- unity可以在CPU上创建worker thread来运行程序，job就是在worker thread上运行的程序
- 只有在main thread中可以创建和schedule job
- 不要在main thread访问正在job中处理的任何数据
- 不要在Schedule()和Complete()之间创建另一个job
- 还可以创建多个依赖 JobHandle.CombineDependencies(handleB,handleC,handleD)
- 不要创建相互依赖的job，会导致死锁

### IJobParallelFor

### Unity.Collections
- https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/EntitiesSamples/Docs/cheatsheet/collections.md

### Unity.Mathematics
- https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/EntitiesSamples/Docs/cheatsheet/mathematics.md