https://zhuanlan.zhihu.com/p/368307575
https://www.ronja-tutorials.com/

- CPU和RAM、GPU和VRAM、CPU和GPU之间相对内存带宽速度（./cpu_gpu.png），可以发现瓶颈在于cpu和gpu之间的内存传输
- [numthreads(8,8,1)] 定义一个线程组中可以被执行的线程总数量
- 什么是线程组？
	+ 一个线程组在单个流多处理器（Stream Multiprocessor 简称SM）
	+ 如果GPU架构有16个SM，那么最少需要16个线程组来保证所有SM都有事情做
	+ 每个线程组都有一个各自的共享内存（Shard Memory），该组中所有线程都可以访问该组对应的共享内存，但是不能访问别的组的共享内存，因此同组的线程之间可以进行同步操作
	+ numthreads(tX, tY, tZ) 线程总数量为 tX * tY * tZ
- SV_GroupID 			int3	当前线程所在的线程组的ID，取值范围（0,0,0）~ (gX-1,gY-1,gZ-1)
- SV_GroupThreadID  	int3	当前线程所在线程组内的ID  取值范围（0,0,0）~ (tX-1,tY-1,tZ-1)
- SV_DispatchThreadID	int3 	当前线程在所有线程组中的所有线程里的ID (0,0,0) ~ (gX*tX-1, gY*tY-1, gZ*tZ-1)
- SV_GroupIndex			int3	当前线程组内的下标 取值范围 0 ~ tX * tY * tZ - 1
- 这里需要注意的是，不管是group还是thread，它们的顺序都是先X再Y最后Z，用表格的理解就是先行(X)再列(Y)然后下一个表(Z)，例如我们tX=5，tY=6那么第1个thread的SV_GroupThreadID=(0,0,0)，第2个的SV_GroupThreadID=(1,0,0)，第6个的SV_GroupThreadID=(0,1,0)，第30个的SV_GroupThreadID=(4,5,0)，第31个的SV_GroupThreadID=(0,0,1)。group同理，搞清顺序后，SV_GroupIndex的计算公式就很好理解了。
- 再举个例子，比如SV_GroupID为(0,0,0)和(1,0,0)的两个group，它们内部的第1个thread的SV_GroupThreadID都为(0,0,0)且SV_GroupIndex都为0，但是前者的SV_DispatchThreadID=(0,0,0)而后者的SV_DispatchThreadID=(tX,0,0)。

- Unity为我们提供了ComputeBuffer类来与RWStructuredBuffer或StructuredBuffer相对应
- 