namespace Game.Maps
{
	using Sirenix.Utilities;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	public struct RectIterator : IEnumerator<Vector2Int>
	{
		RectInt		_rect;

		int		_x;
		int		_y;


		public RectIterator( Vector2Int size )
			: this( new RectInt( Vector2Int.zero, size ))
		{}


		public RectIterator( RectInt rect )
			: this()
		{
			_rect		= rect;

			Reset();
		}


		public void Reset()
		{
			_x		= -1;
			_y		= 0;
		}


		public bool MoveNext()
		{
			if (++ _x >= _rect.size.x)
			{
				_x		= 0;

				if (++ _y >= _rect.size.y)
					return false;
			}

			return true;
		}


		public Vector2Int Current		=> _rect.min + new Vector2Int( _x, _y );
		object IEnumerator.Current		=> Current;


		public void Dispose() {}
	}


	public static class RectIteratorExt
	{
		struct Rect : IEnumerable<Vector2Int>
		{
			public RectInt	rect;

			public IEnumerator<Vector2Int> GetEnumerator()		=> new RectIterator( rect );
			IEnumerator IEnumerable.GetEnumerator()				=> GetEnumerator();
		}


		public static IEnumerable<Vector2Int> Iterate( this RectInt rect )
		=>
			new Rect{ rect = rect };


		public static void Set<T>( this FixedMap<T> map, RectInt rect, T cell ) where T : struct
		=>
			rect.Iterate().ForEach( pos => map.Set( pos, cell ) );
	}
}

