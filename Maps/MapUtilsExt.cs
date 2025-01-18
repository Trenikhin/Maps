namespace Game.Maps
{
	using UnityEngine;
	using Game.Utilities;


	public static partial class MapUtils
	{
		public static Vector2Int GetMapPos( Vector3 worldPos )			=> (worldPos + .5f * Vector3.up).FloorToInt().xy();
		public static Vector3 MapPosToWorld( Vector2Int mapPos )		=> mapPos.xy0() + .5f * Vector3.right;
	}
}

