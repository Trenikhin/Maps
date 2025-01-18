namespace Game.Maps
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	public struct PerimeterIterator : IEnumerator<Vector2Int>
	{
		LocalMap	_localMap;

		int		_localX;
		int		_rotation;


		public PerimeterIterator( RectInt rect )
			: this()
		{
			_localMap	= new LocalMap( rect );

			Reset();
		}


		public void Reset()
		{
			_localX		= -1;
			_rotation	= 0;
		}


		public bool MoveNext()
		{
			_localX ++;

			Vector2Int size		= _localMap.LocalRect().size;

			// Cases: side <= 0
			if (size.x <= 0 || size.y <= 0)
				return false;

			// Case: size = 1:1
			if (size == Vector2Int.one)
				return _localX == 0;

			// Cases: side = 1
			if ((size.x == 1 || size.y == 1) && _rotation >= 2)
				return false;

			while (_localX >= _localMap.Local_Size().x - 1)
			{
				_localX		= 0;

				if (++ _rotation >= 4)
					return false;

				_localMap.Rotate_CCW90();
			}

			return true;
		}


		public Vector2Int Current		=> _localMap.GetWorld( _localX, 0 );
		object IEnumerator.Current		=> Current;


		public void Dispose() {}
	}


	public struct Perimeter : IEnumerable<Vector2Int>
	{
		readonly RectInt	_rect;

		public Perimeter( RectInt rect )
		{
			_rect		= rect;
		}

		public IEnumerator<Vector2Int> GetEnumerator()		=> new PerimeterIterator( _rect );
		IEnumerator IEnumerable.GetEnumerator()				=> GetEnumerator();
	}


	public static class PerimeterIteratorExt
	{
		public static IEnumerable<Vector2Int> Perimeter( this RectInt rect )
		=>
			new Perimeter( rect );


		public static IEnumerable<Vector2Int> Perimeter<T>( this IMap<T> map )
		=>
			new Perimeter( map.Rect );
	}
}

