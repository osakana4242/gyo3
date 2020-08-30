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
		public bool canSkip;
		public Config config = new Config();
		public bool isNeedRepaint;

		public void Update(SerializedProperty spList) {
			while (list.Count < spList.arraySize) {
				list.Add(new Item());
			}
		}

		public void ForEachSelected<T2>(SerializedProperty spEventList, T2 prm, System.Action<SerializedProperty, int, int, T2> act) {
			int i2 = 0;
			for (int i = 0, iCount = spEventList.arraySize; i < iCount; ++i) {
				if (!list[i].selected) continue;
				var pElem = spEventList.GetArrayElementAtIndex(i);
				act(pElem, i, i2, prm);
				++i2;
			}
		}

		void Copy(SerializedProperty spList) {
			copiedList.Clear();
			for (int i = 0, iCount = spList.arraySize; i < iCount; ++i) {
				var selected = i < list.Count ? list[i].selected : false;
				if (!selected) continue;
				var spElement = spList.GetArrayElementAtIndex(i);
				var srcElement = GetElementFunc(spList, i);
				var copiedElement = CloneObject<T>(srcElement);
				copiedList.Add(copiedElement);
			}
		}

		public void DrawGUI<T2>(SerializedProperty spList, T2 prm, System.Action<SerializedProperty, T2> headerDrawFunc) {
			try {
				if (Event.current.type == EventType.Layout) {
					canSkip = true;
				}
				while (list.Count < spList.arraySize) {
					list.Add(new Item());
				}
				var so = spList.serializedObject;
				bool hasExpandedAllOperation = false;
				bool isExpandedAll = false;
				using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
					using (new EditorGUILayout.HorizontalScope()) {
						{
							var selected = list.FindIndex(_item => !_item.selected) == -1;
							var selectedCount = list.Count(_item => _item.selected);
							var nextSelected = EditorGUILayout.ToggleLeft(string.Format("({0})", selectedCount), selected);
							if (selected != nextSelected) {
								selected = nextSelected;
								for (int i = 0, iCount = spList.arraySize; i < iCount; ++i) {
									while (list.Count < i + 1) {
										list.Add(new Item());
									}
									list[i] = new Item() {
										selected = selected,
									};
								}
							}
						}

						if (GUILayout.Button("Copy")) {
							Copy(spList);
						}
						if (GUILayout.Button("Cut")) {
							Copy(spList);
							for (int i = spList.arraySize - 1; 0 <= i; --i) {
								var selected = i < list.Count ? list[i].selected : false;
								if (!selected) continue;
								spList.DeleteArrayElementAtIndex(i);
							}
						}
						if (GUILayout.Button("Ins")) {
							int insertIndex = spList.arraySize;
							for (int i = 0, iCount = spList.arraySize; i < iCount; ++i) {
								var selected = i < list.Count ? list[i].selected : false;
								if (!selected) continue;
								insertIndex = i;
								break;
							}

							UnityEditor.Undo.RecordObject(so.targetObject, "Insert");
							for (int i1 = 0, iCount = copiedList.Count; i1 < iCount; ++i1) {
								int i2 = insertIndex + i1;
								spList.InsertArrayElementAtIndex(i2);
								so.ApplyModifiedPropertiesWithoutUndo();
								SetElementFunc(spList, i2, CloneObject<T>(copiedList[i1]));
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

						if (hasExpandedAllOperation) {
							for (int i = 0, iCount = spList.arraySize; i < iCount; ++i) {
								var spElement = spList.GetArrayElementAtIndex(i);
								spElement.isExpanded = isExpandedAll;
							}
						}
					}
					using (new EditorGUILayout.HorizontalScope()) {
						headerDrawFunc(spList, prm);
					}
				}
				var scrollHeight = 700f;
				using (var scope = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Height(scrollHeight))) {
					var spos = scrollPos;
					if (scrollPos != scope.scrollPosition) {
						EditorGUI.FocusTextInControl( null );
						scrollPos = scope.scrollPosition;
						if (Event.current.type != EventType.Repaint) {
							spos = scrollPos;
						}
					}
					var posY = 0f;
					var skipY = 0f;
					Color bg0 = GUI.backgroundColor;
					Color bg1 = new Color(0.95f, 0.95f, 1.00f, 1f);
					Color bg2 = new Color(0.95f, 1.00f, 0.95f, 1f);

					for (int i = 0, iCount = spList.arraySize; i < iCount; ++i) {
						GUI.backgroundColor = ((i & 0x1) == 0) ? bg1 : bg2;
						var spElement = spList.GetArrayElementAtIndex(i);
						var item = list[i];
						var elemHeight = item.cachedHeight;
						if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint) {
							var nextElemHeight = EditorGUI.GetPropertyHeight(spElement) +
								EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
								8f;
							item.cachedHeight = nextElemHeight;
							list[i] = item;

							if (Event.current.type == EventType.Layout) {
								elemHeight = item.cachedHeight;
							}
						}

						var scrTop = config.debugScrollMargin + spos.y;
						var scrBottom = scrTop + scrollHeight - config.debugScrollMargin * 2;
						var elemTop = posY;
						var elemBottom = posY + elemHeight;
						var outOfBounds = elemBottom < scrTop || scrBottom < elemTop;
						if (outOfBounds && canSkip) {
							posY += elemHeight;
							skipY += elemHeight;
							continue;
						}
						if (0f < skipY) {
							using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.Height(skipY))) {
								EditorGUILayout.LabelField("|");
							}
							skipY = 0f;
						}

						using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(elemHeight))) {
							using (new EditorGUILayout.HorizontalScope()) {
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
										spList.MoveArrayElement(i, i - 1);
									}
								}
								using (new EditorGUI.DisabledScope(iCount <= i)) {
									if (GUILayout.Button("v")) {
										spList.MoveArrayElement(i, i + 1);
									}
								}
								if (GUILayout.Button("-")) {
									spList.DeleteArrayElementAtIndex(i);
									throw new System.OperationCanceledException();
								}
								if (GUILayout.Button("+")) {
									spList.InsertArrayElementAtIndex(i);
									throw new System.OperationCanceledException();
								}
								if (GUILayout.Button("C")) {
									copiedList.Clear();
									var srcElement = GetElementFunc(spList, i);
									var copiedElement = CloneObject<T>(srcElement);
									copiedList.Add(copiedElement);
								}
								using (new EditorGUI.DisabledScope(copiedList.Count <= 0)) {
									if (GUILayout.Button("P")) {
										UnityEditor.Undo.RecordObject(so.targetObject, "Paste");
										SetElementFunc(spList, i, CloneObject<T>(copiedList[0]));
										spList.serializedObject.Update();
										throw new System.OperationCanceledException();
									}
								}
							}
							EditorGUILayout.PropertyField(spElement, true);
							posY += elemHeight;
						}
					}
					GUI.backgroundColor = bg0;

					if (0f < skipY) {
						using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.Height(skipY))) {
							EditorGUILayout.LabelField("|");
						}
						skipY = 0f;
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
			public float cachedHeight;
		}

		public class Config {
			public float debugScrollMargin = 0f;
		}
	}
}
#endif
