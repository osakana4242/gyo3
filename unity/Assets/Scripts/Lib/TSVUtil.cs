using System.Collections.Generic;
using System.Linq;

namespace Osakana4242.Lib {

	public static class TSVUtil {
		public static string ToTSV<T>(T[] list) {
			var sb = new System.Text.StringBuilder();
			var type = typeof(T);
			var fields = type.GetFields();
			{
				for (int fi = 0, fiCount = fields.Length; fi < fiCount; ++fi) {
					var field = fields[fi];
					sb.Append(string.Format("{0}\t", field.Name));
				}
				sb.Append("\n");
			}

			for (int i = 0, iCount = list.Length; i < iCount; ++i) {
				var item = list[i];
				for (int fi = 0, fiCount = fields.Length; fi < fiCount; ++fi) {
					var field = fields[fi];
					var value = field.GetValue(item);
					if (field.FieldType.IsEnum) {
						value = System.Convert.ToInt32(value);
					}
					sb.Append(string.Format("{0}\t", value));
				}
				sb.Append("\n");
			}
			return sb.ToString();
		}

		public static T[] FromTSV<T>(string tsv) {

			var lines = tsv.Replace("\r\n", "\n").Split('\n');
			var fieldNames = lines[0].Split('\t');
			var type = typeof(T);

			var fieldIndexPairList = type.GetFields().
				Select(_item =>
					(
						field: _item,
						index: System.Array.IndexOf(fieldNames, _item.Name)
					)
				).
				Where(_item => {
					return 0 <= _item.index;
				}).
				ToArray();
			// var fields = fieldNames.Select(_item => type.GetField(_item)).ToArray();
			var records = new List<T>();
			for (int lineI = 1, lineICount = lines.Length; lineI < lineICount; ++lineI) {
				var rows = lines[lineI].Split('\t');

				// 空行のスキップ.
				if (System.Array.FindIndex(rows, _row => 0 < _row.Length) == -1) continue;

				//
				var record = (T)type.GetConstructor(System.Array.Empty<System.Type>()).Invoke(System.Array.Empty<object>());
				for (int fieldI = 0, fieldICount = fieldIndexPairList.Length; fieldI < fieldICount; ++fieldI) {
					var fieldIndexPair = fieldIndexPairList[fieldI];
					var field = fieldIndexPair.field;
					if (field == null) continue;
					var strValue = rows[fieldIndexPair.index];
					object objValue;
					try {
						if (field.FieldType == typeof(string)) {
							objValue = strValue;
						} else if (field.FieldType.IsEnum) {
							var intValue = System.Convert.ToInt32(strValue);
							objValue = intValue;
						} else if (field.FieldType == typeof(int)) {
							objValue = (int)System.Convert.ToDouble(strValue);
						} else if (field.FieldType == typeof(float)) {
							objValue = (float)System.Convert.ToDouble(strValue);
						} else {
							throw new System.NotSupportedException("fieldType: " + field.FieldType);
						}
					} catch (System.Exception ex) {
						throw new System.Exception($"要素の解析に失敗. 行番号: {lineI}, fieldName: {field.Name}, fieldType: {field.FieldType}, strValue: '{strValue}'", ex);
					}
					field.SetValue(record, objValue);
				}
				records.Add(record);
			}
			return records.ToArray();
		}
	}
}
