namespace Game.Maps
{
	using System.Collections.Generic;
	using UnityEngine;


	public interface IMap<T> : IEnumerable<Vector2Int>
	{
		RectInt		Rect		{get;}
		Vector2Int	Size		{get;}
		Vector2Int	Min			{get;}
		Vector2Int	Max			{get;}

		void Set( IMap<T> other, Vector2Int dstMin );
		void Set( IMap<T> other, RectInt srcRect, Vector2Int dstMin = default );
		void Set( RectInt rect, T cell );
		void SetAll( T cell );


#region (x, y) methods

		bool InMap( int x, int y );
		bool Is( int x, int y, T cell );
		bool IsSafe( int x, int y, T cell );
		T Get( int x, int y );
		T GetSafe( int x, int y );
		void Set( int x, int y, T cell );
		T this[ int x, int y ] { get; set; }

#endregion
#region (Vector2Int) methods

		bool InMap( Vector2Int pos );
		bool Is( Vector2Int pos, T cell );
		bool IsSafe( Vector2Int pos, T cell );
		T Get( Vector2Int pos );
		T GetSafe( Vector2Int pos );
		void Set( Vector2Int pos, T cell );
		T this[ Vector2Int pos ] { get; set; }

#endregion
	}
}

