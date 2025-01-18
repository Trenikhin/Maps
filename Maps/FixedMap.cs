namespace Game.Maps
{
	using System;
	using UnityEngine;


	[Serializable]
	public class FixedMap<T> : MapBase<T>
	{
		public FixedMap(int width, int height, T cell)		: base(width, height, cell) {}
		public FixedMap(int width, int height)				: base(width, height) {}
		public FixedMap(Vector2Int size, T cell)			: base(size, cell) {}
		public FixedMap(Vector2Int size)					: base(size) {}
		public FixedMap(MapBase<T> other)					: base(other) {}


		public override T Get(int x, int y)					=> _cells[ x, y ];
		public override void Set(int x, int y, T cell)		=> _cells[ x, y ]		= cell;
	}
}

