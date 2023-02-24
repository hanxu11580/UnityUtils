﻿using UnityEngine;
using System.Collections;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif
	/// <summary>
	/// 内存检测器，目前只是输出Profiler信息
	/// </summary>
	public class MemoryDetector 
	{
		private readonly static string TotalAllocMemroyFormation = "Alloc Memory : {0}M";
		private readonly static string TotalReservedMemoryFormation = "Reserved Memory : {0}M";
		private readonly static string TotalUnusedReservedMemoryFormation = "Unused Reserved: {0}M";
        //private readonly static string RuntimeMemorySizeFormation = "RuntimeMemorySize: {0}M";
		private readonly static string MonoHeapFormation = "Mono Heap : {0}M";
		private readonly static string MonoUsedFormation = "Mono Used : {0}M";
		// 字节到兆
		private float ByteToM = 0.000001f;

		private Rect allocMemoryRect;
		private Rect reservedMemoryRect;
		private Rect unusedReservedMemoryRect;
        //private Rect RuntimeMemorySizeRect;
		private Rect monoHeapRect;
		private Rect monoUsedRect;

		private int x = 0;
		private int y = 0;
		private int w = 0;
		private int h = 0;

        public void Init()
        {
            GUIConsole.onGUICallback += OnGUI;
        }

        void ResetGUISize()
        {
            this.x = 5;
            this.y = GUIUtil.FontSize;
            this.w = 1000;
            this.h = GUIUtil.FontSize;

            this.allocMemoryRect = new Rect(x, y, w, h);
            this.reservedMemoryRect = new Rect(x, y + h, w, h);
            this.unusedReservedMemoryRect = new Rect(x, y + 2 * h, w, h);
            //this.RuntimeMemorySizeRect = new Rect(x, y + 3 * h, w, h);
            this.monoHeapRect = new Rect(x, y + 3 * h, w, h);
            this.monoUsedRect = new Rect(x, y + 4 * h, w, h);
        }

		void OnGUI()
		{
            ResetGUISize();

			GUI.Label(this.allocMemoryRect, 
				string.Format(TotalAllocMemroyFormation, Profiler.GetTotalAllocatedMemory() * ByteToM));
			GUI.Label(this.reservedMemoryRect, 
				string.Format(TotalReservedMemoryFormation, Profiler.GetTotalReservedMemory() * ByteToM));
			GUI.Label(this.unusedReservedMemoryRect, 
				string.Format(TotalUnusedReservedMemoryFormation, Profiler.GetTotalUnusedReservedMemory() * ByteToM));
            //GUI.Label(this.RuntimeMemorySizeRect,
            //    string.Format(TotalUnusedReservedMemoryFormation, Profiler.GetRuntimeMemorySize( .) * ByteToM));
			GUI.Label(this.monoHeapRect,
				string.Format(MonoHeapFormation, Profiler.GetMonoHeapSize() * ByteToM));
			GUI.Label(this.monoUsedRect,
				string.Format(MonoUsedFormation, Profiler.GetMonoUsedSize() * ByteToM));
		}
	}
