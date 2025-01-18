namespace Game.Maps
{
	using Sirenix.Utilities;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;


	public static partial class MapUtils
	{
		public static Map<TTo> Remap<TFrom, TTo>( MapBase<TFrom> src, Dictionary<TFrom, TTo> mapping )
		{
			Map<TTo> dst		= new Map<TTo>( src.Rect );

			Remap( src, dst, mapping );

			return dst;
		}


		public static Map<TTo> RemapSafe<TFrom, TTo>( MapBase<TFrom> src, Dictionary<TFrom, TTo> mapping )
		{
			Map<TTo> dst		= new Map<TTo>( src.Rect );

			RemapSafe( src, dst, mapping );

			return dst;
		}


		public static void Remap<TFrom, TTo>( MapBase<TFrom> src, MapBase<TTo> dst, Dictionary<TFrom, TTo> mapping )
		{
			src.ForEach( pos => dst.Set( pos - src.Min + dst.Min, mapping[ src.Get( pos ) ] ));
		}


		public static void RemapSafe<TFrom, TTo>( MapBase<TFrom> src, MapBase<TTo> dst, Dictionary<TFrom, TTo> mapping )
		{
			src.ForEach( pos =>
			{
				if (mapping.TryGetValue( src.Get( pos ), out TTo value ))
					dst.Set( pos - src.Min + dst.Min, value );
			});
		}


		public static void Replace<T>( this MapBase<T> map, Dictionary<T, T> mapping )
		{
			map.ForEach( pos =>
			{
				if (mapping.TryGetValue( map.Get( pos ), out T cell ))
					map.Set( pos, cell );
			});
		}


		// https://stackoverflow.com/questions/159590/way-to-go-from-recursion-to-iteration
		static Stack<Vector2Int> _stack		= new Stack<Vector2Int>();
		public static int Flood<T>( this MapBase<T> map, Vector2Int pos, T src, T dst )
		{
			int n	= 0;

			_stack.Clear();
			_stack.Push( pos );

			while (_stack.Count > 0)
			{
				Vector2Int p	= _stack.Pop();
				if (
						!map.InMap( p ) ||
						!map.Is( p, src )
					)
					continue;

				n ++;

				map.Set( p, dst );

				_stack.Push( p + Vector2Int.left );
				_stack.Push( p + Vector2Int.right );
				_stack.Push( p + Vector2Int.up );
				_stack.Push( p + Vector2Int.down );
			}

			return n;
		}


		public static int Flood( Vector2Int pos, Predicate<Vector2Int> check, Action<Vector2Int> set )
		{
			int n	= 0;

			_stack.Clear();
			_stack.Push( pos );

			while (_stack.Count > 0)
			{
				Vector2Int p	= _stack.Pop();
				if (!check( p ))
					continue;

				n ++;

				set( p );

				_stack.Push( p + Vector2Int.left );
				_stack.Push( p + Vector2Int.right );
				_stack.Push( p + Vector2Int.up );
				_stack.Push( p + Vector2Int.down );
			}

			return n;
		}


		static readonly Dictionary<Vector2Int, int>		_islands		= new Dictionary<Vector2Int, int>();
		public static void FloodSmallIslands<T>( Map<T> map, T land, T floodFill )
		{
			_islands.Clear();

			map.ForEach( pos =>
			{
				if (map.Is( pos, land ))
					_islands[ pos ]		= Flood( map, pos, land, floodFill );
			});

			Vector2Int largest			= _islands.Aggregate( (x, y) => x.Value > y.Value ? x : y ).Key;

			Flood( map, largest, floodFill, land );
		}


		public static bool HasAdjacent<T>( this MapBase<T> map, RectInt r, T cell )
		{
			LocalMap mt					= new LocalMap( r );

			for (int i = 0; i < 4; i ++)
			{
				for (int x = 0; x < mt.Local_Size().x; x ++)
				{
					Vector2Int pos		= mt.GetWorld( x, -1 );

					if (map.Is( pos, cell ))
						return true;
				}

				mt.Rotate_CCW90();
			}

			return false;
		}


		public static bool HasAdjacentSafe<T>( this MapBase<T> map, RectInt r, T cell )
		=>
			HasAdjacentSafe( map, r, comparable => comparable.Equals( cell ) );

		public static bool HasAdjacentSafe<T>( this MapBase<T> map, RectInt r, Predicate<T> predicate )
		{
			LocalMap mt					= new LocalMap( r );

			for (int i = 0; i < 4; i ++)
			{
				for (int x = 0; x < mt.Local_Size().x; x ++)
				{
					Vector2Int pos		= mt.GetWorld( x, -1 );

					if (predicate( map.GetSafe( pos ) ))
						return true;
				}

				mt.Rotate_CCW90();
			}

			return false;
		}


		public static bool HasAdjacentWithD<T>( this MapBase<T> map, Vector2Int pos, T cell )
		=>
			HasAdjacent( map, pos, cell ) ||
			HasAdjacentD( map, pos, cell )
		;
		public static bool HasAdjacentWithDSafe<T>( this MapBase<T> map, Vector2Int pos, T cell )
		=>
			HasAdjacentSafe( map, pos, cell ) ||
			HasAdjacentDSafe( map, pos, cell )
		;
		public static bool HasAdjacent<T>( this MapBase<T> map, Vector2Int pos, T cell )
		=>
			map.Is( pos + Vector2Int.up,	cell ) ||
			map.Is( pos + Vector2Int.left,	cell ) ||
			map.Is( pos + Vector2Int.down,	cell ) ||
			map.Is( pos + Vector2Int.right,	cell )
		;
		public static bool HasAdjacentSafe<T>( this MapBase<T> map, Vector2Int pos, T cell )
		=>
			map.IsSafe( pos + Vector2Int.up,	cell ) ||
			map.IsSafe( pos + Vector2Int.left,	cell ) ||
			map.IsSafe( pos + Vector2Int.down,	cell ) ||
			map.IsSafe( pos + Vector2Int.right,	cell )
		;
		public static bool HasAdjacentD<T>( this MapBase<T> map, Vector2Int pos, T cell )
		=>
			map.Is( pos + new Vector2Int( 1, 1 ),	cell ) ||
			map.Is( pos + new Vector2Int( -1, 1 ),	cell ) ||
			map.Is( pos + new Vector2Int( 1, -1 ),	cell ) ||
			map.Is( pos + new Vector2Int( -1, -1 ),	cell )
		;
		public static bool HasAdjacentDSafe<T>( this MapBase<T> map, Vector2Int pos, T cell )
		=>
			map.IsSafe( pos + new Vector2Int( 1, 1 ),	cell ) ||
			map.IsSafe( pos + new Vector2Int( -1, 1 ),	cell ) ||
			map.IsSafe( pos + new Vector2Int( 1, -1 ),	cell ) ||
			map.IsSafe( pos + new Vector2Int( -1, -1 ),	cell )
		;


		public static void Trim<T>( Map<T> map, T island, int border = 0, T cell = default, bool setAll = false )
		=>
			Trim( map, comparable => comparable.Equals( island ), border, cell, setAll );

		public static void Trim<T>( Map<T> map, Predicate<T> isIsland, int border = 0, T cell = default, bool setAll = false )
		{
			Vector2Int min		= Vector2Int.one * Int32.MaxValue;
			Vector2Int max		= Vector2Int.one * Int32.MinValue;

			map.ForEach( p =>
			{
				if (isIsland( map.Get( p ) ))
				{
					min		= Vector2Int.Min( p, min );
					max		= Vector2Int.Max( p, max );
				}
			});

			// Add border
			min		-= Vector2Int.one * border;
			max		+= Vector2Int.one * (border + 1);		// Add 1 because "max" is inclusive

			Vector2Int extendMin		= map.Min - min;
			Vector2Int extendMax		= max - map.Max;

			map.Extend( extendMin, extendMax, cell, setAll );
		}
	}
}

