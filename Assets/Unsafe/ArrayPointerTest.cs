using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;
using System;

public class ArrayPointerTest : MonoBehaviour
{
    unsafe void Start()
    {
        var array = new Vector3 [0x100];

        //  Pinned 固定对象可防止垃圾回收器在内存中移动它，从而降低垃圾回收器的效率。
        var handle1 = GCHandle.Alloc(array, GCHandleType.Pinned);
        // 在 Pinned 句柄中检索对象数据的地址
        var pointer1 = handle1.AddrOfPinnedObject();

        ulong handle2 = 0;
        // 保证对象在移动 GC 中的内存位置不会移动。返回该对象的内存位置地址
        var pointer2 = (IntPtr)UnsafeUtility.PinGCObjectAndGetAddress(array, out handle2);

        var pointer3 = (IntPtr)UnsafeUtility.AddressOf(ref array[0]);

        Debug.Log("Comparison between pointers of an array acquired with different functions.");
        Debug.Log("1: " + pointer1.ToString("X") + " (System.Runtime.InteropServices.GCHandle)");
        Debug.Log("2: " + pointer2.ToString("X") + " (UnsafeUtility.PinGCObjectAndGetAddress)");
        Debug.Log("3: " + pointer3.ToString("X") + " (UnsafeUtility.AddressOf)");

        handle1.Free();
        UnsafeUtility.ReleaseGCObject(handle2);

        // 1、3地址是相同的
    }
}
