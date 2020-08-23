#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Osakana4242.Content {
	public class ListEditor<T> {
		public List<Item> list = new List<Item>();
		public List<T> copiedList = new List<T>();
		public System.Action<SerializedProperty, int, T> SetElementFunc;
		public System.Func<SerializedProperty, int, T> GetElementFunc;
		public Vector2 scrollPos;

		void Copy(SerializedProperty spEventList) {
			copiedList.Clear();
			for (int i = 0, iCount = spEventList.arraySize; i < iCount; ++i) {
				var selected = i < list.Count ? list[i].selected : false;
				if (!selected) continue;
				var spElement = spEventList.GetArrayElementAtIndex(i);
				var srcElement = GetElementFunc(spEventList, i);
				var copiedElement = CloneObject<T>(srcElement);
				copiedList.Add(copiedElement);
			}
		}

		public void DrawGUI(SerializedProperty spEventList) {
			try {
				var so = spEventList.serializedObject;
				bool hasExpandedAllOperation = false;
				bool isExpandedAll = false;
				using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
					{
						var selected = list.FindIndex(_item => !_item.selected) == -1;
						var selectedCount = list.Count(_item => _item.selected);
						var nextSelected = EditorGUILayout.ToggleLeft(string.Format("({0})", selectedCount), selected);
						if (selected != nextSelected) {
							selected = nextSelected;
							for (int i = 0, iCount = spEventList.arraySize; i < iCount; ++i) {
								while (list.Count < i + 1) {
									list.Add(new Item());
								}
								list[i] = new Item() {
									selected = selected,
								};
							}
						}
					}

					if (GUILayout.Button("C")) {
						Copy(spEventList);
					}
					if (GUILayout.Button("Cut")) {
						Copy(spEventList);
						for (int i = spEventList.arraySize - 1; 0 <= i; --i) {
							var selected = i < list.Count ? list[i].selected : false;
							if (!selected) continue;
							spEventList.DeleteArrayElementAtIndex(i);
						}
					}
					if (GUILayout.Button("Ins")) {
						int insertIndex = spEventList.arraySize;
						for (int i = 0, iCount = spEventList.arraySize; i < iCount; ++i) {
							var selected = i < list.Count ? list[i].selected : false;
							if (!selected) continue;
							insertIndex = i;
							break;
						}

						UnityEditor.Undo.RecordObject(so.targetObject, "Insert");
						for (int i1 = 0, iCount = copiedList.Count; i1 < iCount; ++i1) {
							int i2 = insertIndex + i1;
							spEventList.InsertArrayElementAtIndex(i2);
							so.ApplyModifiedPropertiesWithoutUndo();
							SetElementFunc(spEventList, i2, CloneObject<T>(copiedList[i1]));
							so.Update();
						}
						throw new System.OperationCanceledException();
					}
					if (GUILayout.Button("_")) {
						hasExpandedAllOperation = true;
						isExpandedAll = false;
					}
					if (GUILayout.Button("□")) {
						hasExpandedAllOperation = true;
						isExpandedAll = true;
					}
				}
				using (var scope = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Height(700))) {
					scrollPos = scope.scrollPosition;
					for (int i = 0, iCount = spEventList.arraySize; i < iCount; ++i) {
						var spElement = spEventList.GetArrayElementAtIndex(i);
						using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
							using (new EditorGUILayout.HorizontalScope()) {
								while (list.Count < i + 1) {
									list.Add(new Item());
								}
								var item = list[i];
								var nextSelected = EditorGUILayout.Toggle(item.selected);
								if (item.selected != nextSelected) {
									{
										int min;
										int max;
										if (Event.current.shift) {
											var idx = list.FindLastIndex(i, list.Count - (list.Count - i), _item => _item.selected == nextSelected);
											min = (idx == -1) ? 0 : idx + 1;
											max = i;
										} else if (Event.current.control) {
											var idx = list.FindIndex(i, list.Count - i, _item => _item.selected == nextSelected);
											min = i;
											max = (idx == -1) ? list.Count - 1 : idx - 1;
										} else {
											min = i;
											max = i;
										}
										for (int i2 = min; i2 <= max; ++i2) {
											var item2 = list[i2];
											item2.selected = nextSelected;
											list[i2] = item2;
										}
									}
								}

								using (new EditorGUI.DisabledScope(i <= 0)) {
									if (GUILayout.Button("^")) {
										spEventList.MoveArrayElement(i, i - 1);
									}
								}
								using (new EditorGUI.DisabledScope(iCount <= i)) {
									if (GUILayout.Button("v")) {
										spEventList.MoveArrayElement(i, i + 1);
									}
								}
								if (GUILayout.Button("-")) {
									spEventList.DeleteArrayElementAtIndex(i);
									throw new System.OperationCanceledException();
								}
								if (GUILayout.Button("+")) {
									spEventList.InsertArrayElementAtIndex(i);
									throw new System.OperationCanceledException();
								}
								if (GUILayout.Button("C")) {
									copiedList.Clear();
									var srcElement = GetElementFunc(spEventList, i);
									var copiedElement = CloneObject<T>(srcElement);
									copiedList.Add(copiedElement);
								}
								using (new EditorGUI.DisabledScope(copiedList.Count <= 0)) {
									if (GUILayout.Button("P")) {
										UnityEditor.Undo.RecordObject(so.targetObject, "Paste");
										SetElementFunc(spEventList, i, CloneObject<T>(copiedList[0]));
										spEventList.serializedObject.Update();
										throw new System.OperationCanceledException();
									}
								}
							}
							if (hasExpandedAllOperation) {
								spElement.isExpanded = isExpandedAll;
							}
							EditorGUILayout.PropertyField(spElement, true);
						}
					}
				}
			} catch (System.OperationCanceledException) {
			}
		}

		static T2 CloneObject<T2>(T obj) {
			var json = JsonUtility.ToJson(obj);
			var t = JsonUtility.FromJson<T2>(json);
			return t;
		}

		public struct Item {
			public bool selected;
		}
	}
}
#endif
