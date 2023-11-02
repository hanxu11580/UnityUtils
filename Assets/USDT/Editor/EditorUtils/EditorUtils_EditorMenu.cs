using Bujuexiao.Editor;
using MessagePack;
using RobotCat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using USDT.CustomEditor.CompileSound;
using USDT.CustomEditor.Excel;
using USDT.Utils;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;


namespace USDT.CustomEditor {

    // Editor Menu
    public static class EditorUtils_EditorMenu {

        [MenuItem("EditorUtils/测试")]
        private static void Test() {
            var dataPath = @"C:\Users\A\AppData\LocalLow\CatTrick\SpellRaider\2614099970_1716664215999086592_TSPlayerDetailBlock_0.data";
            var datalist = ReadLutData(dataPath);
            lg.i(datalist.Count);
        }


        private static List<long> ReadLutData(string path) {
            List<long> resData = new List<long>();
            if (File.Exists(path)) {
                using (FileStream fs = new FileStream(path, FileMode.Open)) {
                    using (BinaryReader reader = new BinaryReader(fs)) {
                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            for (int i = 0; i < 174; i++) {
                                if (reader.BaseStream.Position != reader.BaseStream.Length) {
                                    long value = reader.ReadInt64();
                                    resData.Add(value);
                                }
                                else {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return resData;
        }


        [MenuItem("EditorUtils/Path/OpenPersistentDataPath")]
        private static void OpenPersistentDataPath() {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("EditorUtils/Windows/SearchTexture")]
        private static void OpenSearchTextureWindow() {
            EditorWindow.GetWindowWithRect<SearchImageWindow>(GetScreenCenterRect(800, 500)).Show();
        }

        [MenuItem("EditorUtils/Windows/EditorStyleViewer")]
        private static void OpenEditorStyleViewer() {
            EditorWindow.GetWindowWithRect<EditorStyleViewer>(GetScreenCenterRect(800, 500)).Show();
        }

        [MenuItem("EditorUtils/Windows/BJXEditorWindow")]
        private static void OpenBJXEditorWindow() {
            EditorWindow.GetWindowWithRect<BJXEditorWindow>(GetScreenCenterRect(800, 500)).Show();
        }

        #region Excel

        [MenuItem("EditorUtils/Excels/导出表类CS、数据Json")]
        private static void ExportALL() {
            ExcelExportUtil.ExportScriptALL();
            ExcelExportUtil.ExportDataALL();
        }
        [MenuItem("EditorUtils/Excels/导出表类CS")]
        private static void ExportScript() {
            ExcelExportUtil.ExportScriptALL();
        }
        [MenuItem("EditorUtils/Excels/导出表数据")]
        private static void ExportData() {
            ExcelExportUtil.ExportDataALL();
        }
        #endregion

        #region 脚本编译

        [MenuItem("EditorUtils/Compilation/强制请求编译脚本")]
        public static void RequestScriptReload() {
            CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Active)]
        public static void ToggleWaitMusic() {
            CompileSoundSettings.UseWaitMusic ^= true;
            CompileSoundListener.UpdateToggle();
        }
        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Active, true)]
        public static bool CheckToggleWaitMusic() {
            Menu.SetChecked(EditorMenuConst.CompileSound_MenuItemName_Active, CompileSoundSettings.UseWaitMusic);
            return true;
        }


        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Shuffle)]
        public static void ChooseShuffle() {
            CompileSoundSettings.Shuffle ^= true;
        }
        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Shuffle, true)]
        public static bool CheckChooseShuffle() {
            Menu.SetChecked(EditorMenuConst.CompileSound_MenuItemName_Shuffle, CompileSoundSettings.Shuffle);
            return true;
        }
        #endregion

        #region 工具

        private static Rect GetScreenCenterRect(float width, float height) {
            int w = Screen.currentResolution.width;
            int h = Screen.currentResolution.height;
            if (h > w) {
                // OnEnd Display roation
                w = Screen.currentResolution.height;
                h = Screen.currentResolution.width;
            }

            return new Rect(w / 2 - width * 0.5f, h / 2 - height * 0.5f, width, height);
        }


        #endregion

        #region 右键
        [MenuItem("Assets/GenerateNameSpace", false)]
        private static void GenerateNameSpace() {
            if(Selection.assetGUIDs == null || Selection.assetGUIDs.Length != 1) {
                lg.e("请选择对应的目录");
                return;
            }

            var guid = Selection.assetGUIDs[0];
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(assetPath)) {
                lg.e("选择的是非目录");
                return;
            }

            var @namespace = new DirectoryInfo(assetPath).Name;

            var files = Directory.GetFiles(assetPath, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files) {
                GenerateNamespaceSingleFile(file, @namespace);
            }

            AssetDatabase.Refresh();
        }

        private static void GenerateNamespaceSingleFile(string filePath, string @namespace) {
            var allText = File.ReadAllText(filePath);
            if (allText.Contains("namespace")) {
				return;
            }
            string[] lines = File.ReadAllLines(filePath, Encoding.GetEncoding("GB2312"));

            // 分离 using 语句和命名空间内容
            var usingStatements = lines.TakeWhile(line => line.TrimStart().StartsWith("using")).ToList();
            var namespaceContent = lines.Skip(usingStatements.Count).ToList();

            // 添加新的命名空间
            var newContent = new StringBuilder();
            usingStatements.ForEach(line => newContent.AppendLine(line));
            newContent.AppendLine("\n");
            newContent.AppendLine($"namespace {@namespace} {{");
            namespaceContent.ForEach(line => {
                if (!string.IsNullOrEmpty(line)) {
                    newContent.AppendLine(line);
                }
            });
            newContent.AppendLine("}");

            // 写回文件
            File.WriteAllText(filePath, newContent.ToString(), Encoding.UTF8);
        }

        #endregion
    }



	public class Queue<T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T> {
		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator {
			private Queue<T> _q;

			private int _index;

			private int _version;

			private T _currentElement;

			public T Current
			{

				get {
					if (_index < 0) {
						if (_index == -1) {

						}
						else {

						}
					}
					return _currentElement;
				}
			}


			object IEnumerator.Current
			{

				get {
					if (_index < 0) {
						if (_index == -1) {

						}
						else {

						}
					}
					return _currentElement;
				}
			}

			internal Enumerator(Queue<T> q) {
				_q = q;
				_version = _q._version;
				_index = -1;
				_currentElement = default(T);
			}


			public void Dispose() {
				_index = -2;
				_currentElement = default(T);
			}


			public bool MoveNext() {
				if (_version != _q._version) {

				}
				if (_index == -2) {
					return false;
				}
				_index++;
				if (_index == _q._size) {
					_index = -2;
					_currentElement = default(T);
					return false;
				}
				_currentElement = _q.GetElement(_index);
				return true;
			}


			void IEnumerator.Reset() {
				if (_version != _q._version) {

				}
				_index = -1;
				_currentElement = default(T);
			}
		}

		private T[] _array;

		private int _head;

		private int _tail;

		private int _size;

		private int _version;

		[NonSerialized]
		private object _syncRoot;

		private const int _MinimumGrow = 4;

		private const int _ShrinkThreshold = 32;

		private const int _GrowFactor = 200;

		private const int _DefaultCapacity = 4;

		private static T[] _emptyArray = new T[0];


		public int Count
		{

			get {
				return _size;
			}
		}


		bool ICollection.IsSynchronized
		{
			
			get {
				return false;
			}
		}

		
		object ICollection.SyncRoot
		{
			
			get {
				if (_syncRoot == null) {
					Interlocked.CompareExchange<object>(ref _syncRoot, new object(), (object)null);
				}
				return _syncRoot;
			}
		}

		
		public Queue() {
			_array = _emptyArray;
		}

		
		public Queue(int capacity) {
			if (capacity < 0) {

			}
			_array = new T[capacity];
			_head = 0;
			_tail = 0;
			_size = 0;
		}

		
		public Queue(IEnumerable<T> collection) {
			if (collection == null) {

			}
			_array = new T[4];
			_size = 0;
			_version = 0;
			foreach (T item in collection) {
				Enqueue(item);
			}
		}

		
		public void Clear() {
			if (_head < _tail) {
				Array.Clear(_array, _head, _size);
			}
			else {
				Array.Clear(_array, _head, _array.Length - _head);
				Array.Clear(_array, 0, _tail);
			}
			_head = 0;
			_tail = 0;
			_size = 0;
			_version++;
		}

		
		public void CopyTo(T[] array, int arrayIndex) {
			if (array == null) {

			}
			if (arrayIndex < 0 || arrayIndex > array.Length) {

			}
			int num = array.Length;
			if (num - arrayIndex < _size) {

			}
			int num2 = ((num - arrayIndex < _size) ? (num - arrayIndex) : _size);
			if (num2 != 0) {
				int num3 = ((_array.Length - _head < num2) ? (_array.Length - _head) : num2);
				Array.Copy(_array, _head, array, arrayIndex, num3);
				num2 -= num3;
				if (num2 > 0) {
					Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, num2);
				}
			}
		}

		
		void ICollection.CopyTo(Array array, int index) {
			if (array == null) {

			}
			if (array.Rank != 1) {

			}
			if (array.GetLowerBound(0) != 0) {

			}
			int length = array.Length;
			if (index < 0 || index > length) {

			}
			if (length - index < _size) {

			}
			int num = ((length - index < _size) ? (length - index) : _size);
			if (num == 0) {
				return;
			}
			try {
				int num2 = ((_array.Length - _head < num) ? (_array.Length - _head) : num);
				Array.Copy(_array, _head, array, index, num2);
				num -= num2;
				if (num > 0) {
					Array.Copy(_array, 0, array, index + _array.Length - _head, num);
				}
			}
			catch (ArrayTypeMismatchException) {

			}
		}

		
		public void Enqueue(T item) {
			try {
				if (_size == _array.Length) {
					int num = (int)((long)_array.Length * 200L / 100);
					if (num < _array.Length + 4) {
						num = _array.Length + 4;
					}
					//UnityEngine.Debug.Log($"{_size}-{_array.Length}");
					SetCapacity(num);
				}
				_array[_tail] = item;
				_tail = (_tail + 1) % _array.Length;
				_size++;
				_version++;
            }
            catch (Exception e) {
				Console.WriteLine();
            }
		}

		
		public Enumerator GetEnumerator() {
			return new Enumerator(this);
		}

		
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return new Enumerator(this);
		}

		
		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}

		
		public T Dequeue() {
			if (_size == 0) {

			}
			T result = _array[_head];
			_array[_head] = default(T);
			_head = (_head + 1) % _array.Length;
			_size--;
			_version++;
			return result;
		}

		
		public T Peek() {
			if (_size == 0) {

			}
			return _array[_head];
		}

		
		public bool Contains(T item) {
			int num = _head;
			int size = _size;
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			while (size-- > 0) {
				if (item == null) {
					if (_array[num] == null) {
						return true;
					}
				}
				else if (_array[num] != null && @default.Equals(_array[num], item)) {
					return true;
				}
				num = (num + 1) % _array.Length;
			}
			return false;
		}

		internal T GetElement(int i) {
			return _array[(_head + i) % _array.Length];
		}

		
		public T[] ToArray() {
			T[] array = new T[_size];
			if (_size == 0) {
				return array;
			}
			if (_head < _tail) {
				Array.Copy(_array, _head, array, 0, _size);
			}
			else {
				Array.Copy(_array, _head, array, 0, _array.Length - _head);
				Array.Copy(_array, 0, array, _array.Length - _head, _tail);
			}
			return array;
		}

		private void SetCapacity(int capacity) {
			T[] array = new T[capacity];
			if (_size > 0) {
				if (_head < _tail) {
					// 524288-117418-System.Int32[]-1048576-482200
					// 从_array的117418开始拷贝482200个，_array没有599618这么长
					// _size不会出错，出错可能是在Dequeue的_head = (_head + 1) % _array.Length;
					UnityEngine.Debug.Log($"{_array.Length}-{_head}-{array}-{array.Length}-{_size}_{_tail}");
					Array.Copy(_array, _head, array, 0, _size);
				}
				else {
					Array.Copy(_array, _head, array, 0, _array.Length - _head);
					Array.Copy(_array, 0, array, _array.Length - _head, _tail);
				}
			}
			_array = array;
			_head = 0;
			_tail = ((_size != capacity) ? _size : 0);
			_version++;
		}

		
		public void TrimExcess() {
			int num = (int)((double)_array.Length * 0.9);
			if (_size < num) {
				SetCapacity(_size);
			}
		}
	}
}