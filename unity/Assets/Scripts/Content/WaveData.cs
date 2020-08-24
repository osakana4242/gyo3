using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEnginUtil;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Osakana4242.Content {
	[System.Serializable]
	public class WaveData : ScriptableObject {
		public WaveEventData[] eventList;

		// 160, 120
		// 16 * 10 = 160
		// 16 * 8 = 128
		// 16 * 9 = 144
		// center = 0, 0
		// spawn right = 5, 0
		// spawn left = -5, 0
		// spawn top = 0, 5
		// spawn bottom = 0, -5
		// 
		public static WaveEventData[] hoge() {
			object[] data = {
				 3000, "enemy1", 5,  3, 180,
				 3500, "enemy1", 5,  3, 180,
				 4000, "enemy1", 5,  3, 180,
				 4500, "enemy1", 5,  3, 180,

				 6000, "enemy1", 5, -3, 180,
				 6500, "enemy1", 5, -3, 180,
				 7000, "enemy1", 5, -3, 180,
				 7500, "enemy1", 5, -3, 180,

				 9000, "enemy1", 5,  2, 180,
				 9500, "enemy1", 5,  1, 180,
				10000, "enemy1", 5, -1, 180,
				10500, "enemy1", 5, -2, 180,
			};
			List<WaveEventData> list = new List<WaveEventData>();
			for (int i = 0, iCount = data.Length; i < iCount; i += 5) {
				var item = new WaveEventData() {
					startTime = (int)data[i + 0],
					enemyName = (string)data[i + 1],
					position = new Vector2((int)data[i + 2], (int)data[i + 3]),
					angle = (int)data[i + 4],
				};
				list.Add(item);
			}
			return list.ToArray();
		}

#if UNITY_EDITOR
		[CustomPropertyDrawer(typeof(WaveEventData))]
		public class WaveEventDataDrawer : PropertyDrawer {
			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
				// https://docs.unity3d.com/ja/current/ScriptReference/PropertyDrawer.html

				WaveEventData model = default;
				using (new EditorGUI.PropertyScope(position, label, property)) {
					position.height = EditorGUIUtility.singleLineHeight;

					var pos21 = new Rect(position.x + position.width * 0.5f * 0, position.y, position.width * 0.5f, position.height);
					var pos22 = new Rect(position.x + position.width * 0.5f * 1, position.y, position.width * 0.5f, position.height);
					if (property.isExpanded) {
						property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
					} else {
						property.isExpanded = EditorGUI.Foldout(pos21, property.isExpanded, label);
						EditorGUI.PropertyField(pos22, property.FindPropertyRelative(nameof(model.startTime)), GUIContent.none);
					}
					position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

					if (property.isExpanded) {
						EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(model.startTime)));
						position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
						EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(model.enemyName)));
						position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
						EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(model.position)));
						position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
						{
							pos21.y = pos22.y = position.y;
							var pAngle = property.FindPropertyRelative(nameof(model.angle));
							EditorGUI.PropertyField(pos21, pAngle);
							var vec = Vector2Util.FromDeg(pAngle.floatValue);
							EditorGUI.Vector2Field(pos22, "", vec);
							position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
						}
					}
				}
			}

			public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
				return property.isExpanded ?
					(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5f :
					EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
		}

		[CustomEditor(typeof(WaveData))]
		public class Inspector : UnityEditor.Editor {

			WaveEventData copied;
			ListEditor<WaveEventData> listEditor = new ListEditor<WaveEventData>() {
				GetElementFunc = (_a, _b) => {
					return ((WaveData)_a.serializedObject.targetObject).eventList[_b];
				},
				SetElementFunc = (_a, _b, _c) => {
					((WaveData)_a.serializedObject.targetObject).eventList[_b] = _c;
				},
			};

			static T CloneObject<T>(T obj) {
				var json = JsonUtility.ToJson(obj);
				var t = JsonUtility.FromJson<T>(json);
				return t;
			}

			static System.Action<SerializedProperty, Inspector> drawHedaerFunc = (_spList, _self) => {
				using (new EditorGUILayout.HorizontalScope()) {
					System.Action<SerializedProperty, int, int, int> timeShiftFunc = (_pElem, _i1, _i2, _prm) => {
						WaveEventData model = default;
						_pElem.FindPropertyRelative(nameof(model.startTime)).floatValue += _prm;
					};
					EditorGUILayout.LabelField("startTime");

					if (GUILayout.Button("<<")) {
						_self.listEditor.ForEachSelected(_spList, -1000, timeShiftFunc);
					}
					if (GUILayout.Button("<")) {
						_self.listEditor.ForEachSelected(_spList, -100, timeShiftFunc);
					}
					if (GUILayout.Button(">")) {
						_self.listEditor.ForEachSelected(_spList, 100, timeShiftFunc);
					}
					if (GUILayout.Button(">>")) {
						_self.listEditor.ForEachSelected(_spList, 1000, timeShiftFunc);
					}
				}
			};

			public override void OnInspectorGUI() {
				base.DrawDefaultInspector();
				this.serializedObject.Update();

				var target = (WaveData)this.serializedObject.targetObject;
				var spEventList = this.serializedObject.FindProperty(nameof(target.eventList));

				listEditor.DrawGUI(spEventList, this, drawHedaerFunc);

				this.serializedObject.ApplyModifiedProperties();
			}
		}
#endif
	}
}
