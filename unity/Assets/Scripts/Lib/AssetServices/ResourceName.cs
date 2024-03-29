
namespace Osakana4242.Lib.AssetServices {

	public readonly struct ResourceName {
		public readonly string value;
		public ResourceName(string path) {
			string pathFromResources;
			{
				var RESOURCES_PATH = "/Resources/";
				var index = path.LastIndexOf(RESOURCES_PATH, System.StringComparison.Ordinal);
				if (0 <= index) {
					pathFromResources = path[(index + RESOURCES_PATH.Length)..];
				} else {
					pathFromResources = path;
				}
			}

			{
				var index = pathFromResources.IndexOf(".", System.StringComparison.Ordinal);
				if (0 <= index) {
					value = pathFromResources[..index];
				} else {
					value = pathFromResources;
				}
			}
		}

		public override string ToString() => value;
	}
}
