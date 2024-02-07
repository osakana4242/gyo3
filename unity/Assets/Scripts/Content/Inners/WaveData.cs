using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using Osakana4242.SystemExt;
using Osakana4242.Lib;
using System.Linq;
using Osakana4242.Lib.AssetServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Osakana4242.Content.Inners {
	[System.Serializable]
	public class WaveData : ScriptableObject {
		static WaveData empty_g_;
		public static WaveData Empty => (null == empty_g_) ?
			(empty_g_ = WaveData.CreateInstance<WaveData>()) :
			empty_g_;

		public WaveEventData[] eventList = System.Array.Empty<WaveEventData>();

#if UNITY_EDITOR
		[CustomPropertyDrawer(typeof(WaveEventData))]
		public class WaveEventDataDrawer : PropertyDrawer {
			WaveEventData model = default;

			static float LineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			static void PropertyField(ref Rect position, bool isLayout, SerializedProperty sp, GUIContent content = null) {
				if (!isLayout) EditorGUI.PropertyField(position, sp, content);
				position.y += LineHeight;
			}

			float Draw(Rect position, SerializedProperty property, GUIContent label, bool isLayout) {
				// https://docs.unity3d.com/ja/current/ScriptReference/PropertyDrawer.html

				var startY = position.y;
				if (!isLayout) EditorGUI.BeginProperty(position, label, property);
				{
					position.height = EditorGUIUtility.singleLineHeight;

					var spType = property.FindPropertyRelative(nameof(model.type));
					var spComment = property.FindPropertyRelative(nameof(model.comment));
					var spStartTime = property.FindPropertyRelative(nameof(model.startTime));
					var spEnemyName = property.FindPropertyRelative(nameof(model.enemyName));
					var spPositionX = property.FindPropertyRelative(nameof(model.positionX));
					var spPositionY = property.FindPropertyRelative(nameof(model.positionY));
					var spAngle = property.FindPropertyRelative(nameof(model.angle));

					var type = EnumUtil<WaveEventType>.GetValueAt(spType.enumValueIndex);

					var pos21 = new Rect(position.x + position.width * 0.5f * 0, position.y, position.width * 0.5f, position.height);
					var pos22 = new Rect(position.x + position.width * 0.5f * 1, position.y, position.width * 0.5f, position.height);

					var pos31 = new Rect(position.x + position.width * 0.3f * 0, position.y, position.width * 0.3f, position.height);
					var pos32 = new Rect(position.x + position.width * 0.3f * 1, position.y, position.width * 0.3f, position.height);
					var pos33 = new Rect(position.x + position.width * 0.3f * 2, position.y, position.width * 0.3f, position.height);

					bool isExpanded = property.isExpanded && type != WaveEventType.None;
					{
						if (!isLayout) {
							if (isExpanded) {
								property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
							} else {
								property.isExpanded = EditorGUI.Foldout(pos31, property.isExpanded, label);
								PropertyField(ref pos32, isLayout, spType, GUIContent.none);
								if (type == WaveEventType.None) {
									PropertyField(ref pos33, isLayout, spComment, GUIContent.none);
								} else {
									PropertyField(ref pos33, isLayout, spStartTime, GUIContent.none);
								}
							}
						}
						position.y += LineHeight;
					}

					if (isExpanded) {
						PropertyField(ref position, isLayout, spType);

						if (type == WaveEventType.None) {
							PropertyField(ref position, isLayout, spComment);
						} else if (type == WaveEventType.Spawn) {
							PropertyField(ref position, isLayout, spStartTime);
							PropertyField(ref position, isLayout, spEnemyName);
							PropertyField(ref position, isLayout, spPositionX);
							PropertyField(ref position, isLayout, spPositionY);
							{
								pos21.y = pos22.y = position.y;
								PropertyField(ref pos21, isLayout, spAngle);
								var vec = Vector2Util.FromDeg(spAngle.floatValue);
								if (!isLayout) EditorGUI.BeginDisabledGroup(true);
								if (!isLayout) EditorGUI.Vector2Field(pos22, "", vec);
								if (!isLayout) EditorGUI.EndDisabledGroup();
								position.y += LineHeight;
							}
						} else if (type == WaveEventType.EndIfDestroyedEnemy) {
							PropertyField(ref position, isLayout, spStartTime);
							PropertyField(ref position, isLayout, spEnemyName);
						}
					}
				}
				if (!isLayout) EditorGUI.EndProperty();
				return position.y - startY;
			}

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
				// https://docs.unity3d.com/ja/current/ScriptReference/PropertyDrawer.html
				Draw(position, property, label, false);
			}

			public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
				return Draw(new Rect(), property, label, true);
			}
		}

		[CustomEditor(typeof(WaveData))]
		public class Inspector : UnityEditor.Editor {

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
				using (new EditorGUILayout.HorizontalScope()) {
					if (GUILayout.Button("TSVコピー")) {
						var data = (WaveData)_self.target;
						EditorGUIUtility.systemCopyBuffer = TSVUtil.ToTSV(data.eventList);

					}
					if (GUILayout.Button("TSVペースト")) {
						var data = (WaveData)_self.target;
						var s = EditorGUIUtility.systemCopyBuffer;
						data.eventList = TSVUtil.FromTSV<WaveEventData>(s);
						_self.serializedObject.Update();
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

	public enum WaveEventType {
		None = 0,
		Spawn = 1,
		EndIfDestroyedEnemy = 2,
	}

	[System.Serializable]
	public class WaveEventData {
		public WaveEventType type;
		public float startTime;
		public string comment;
		public string enemyName;
		public float positionX;
		public float positionY;
		// 0: 右, 90 下, 180: 左, 270: 上
		public float angle;

		public Vector2 Position => new Vector2(positionX, positionY);
		public AssetInfo GetEnemyCharaInfoAssetInfo() => AssetInfos.Get($"{enemyName}.asset");
		public CharaInfo GetEnemyCharaInfo() => AssetService.Instance.Get<CharaInfo>(GetEnemyCharaInfoAssetInfo());
	}
}
