using System.Linq;

namespace Game.Maps
{
	using Sirenix.Utilities;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Game.Serializable;


	[Serializable]
	public abstract class MapBase<T> : IMap<T>
	{
		public RectInt Rect			=> _rect;
		public Vector2Int Size		=> _rect.size;
		public Vector2Int Min		=> _rect.min;
		public Vector2Int Max		=> ((RectInt)_rect).max;

		// [SerializeField] - required, otherwise Odin Serializer will not serialize protected/private members
		[SerializeField] protected SRectInt		_rect;
		[SerializeField] protected T[,]			_cells;


#region Constructors

		protected MapBase( int width, int height, T cell )			: this( new Vector2Int(width, height), cell ) {}
		protected MapBase( int width, int height )					: this( new Vector2Int(width, height) ) {}
		protected MapBase( Vector2Int size )						: this( new RectInt( Vector2Int.zero, size ) ) {}


		protected MapBase( Vector2Int size, T cell )		: this( size )
		{
			SetAll( cell );
		}

		protected MapBase( MapBase<T> other )				: this( other._rect )
		{
			_cells		= (T[,])other._cells.Clone();
		}

		protected MapBase( RectInt rect )
		{
			_rect		= rect;
			_cells		= new T[ rect.width, rect.height ];
		}

#endregion


		public void Set( MapBase<T> other )
		{
			if (Size != other.Size)
				throw new Exception( "Can't copy, map sizes not match." );

			Array.Copy( other._cells, _cells, Size.x * Size.y );	
		}


		public void Set( IMap<T> other, Vector2Int dstMin )
		{
			Set( other, new RectInt( Vector2Int.zero, other.Size ), dstMin );
		}


		public void Set( IMap<T> other, RectInt srcRect, Vector2Int dstMin = default )
		{
			LocalMap localSrc		= new LocalMap( srcRect );
			LocalMap localDst		= new LocalMap( new RectInt( dstMin, srcRect.size ) );

			localSrc.ForEach( l => Set(
										localDst.GetWorld( l ),
										other.Get( localSrc.GetWorld( l ) )
			));
		}


		public void Set( RectInt rect, T cell )				=> rect.Iterate().ForEach( p => Set( p, cell ) );
		public void SetAll( T cell )						=> this.ForEach( p => Set( p, cell ) );


#region (X, Y) methods

		public bool InMap( int x, int y )					=> ((RectInt)_rect).Contains( new Vector2Int( x, y ) );
		public bool IsSafe( int x, int y, T cell )			=> InMap( x, y ) && Is( x, y, cell );

		public bool Is( int x, int y, T cell )
		{
			T v		= Get( x, y );

			return
				cell == null ?
				v == null :
				cell.Equals( v );
		}


		public T GetSafe( int x, int y )					=> InMap( x, y ) ? Get( x, y ) : default;
		public abstract T Get( int x, int y );
		public void SetSafe( int x, int y, T cell )			{ if (InMap( x, y )) Set( x, y, cell ); }
		public abstract void Set( int x, int y, T cell );
		public T this[ int x, int y ]
		{
			get												=> Get( x, y );
			set												=> Set( x, y, value );
		}

#endregion
#region (Vector2Int) methods (forwards)

		public bool InMap( Vector2Int pos )					=> InMap( pos.x, pos.y );
		public bool InMap( RectInt pos )					=> pos.Iterate().All( InMap );
		public bool Is( Vector2Int pos, T cell )			=> Is( pos.x, pos.y, cell );
		public bool Is( RectInt pos, T cell )				=> pos.Iterate().All( p => Is( p.x, p.y, cell )) ;
		public bool IsSafe( Vector2Int pos, T cell )		=> IsSafe( pos.x, pos.y, cell );
		public T GetSafe( Vector2Int pos )					=> GetSafe( pos.x, pos.y );
		public T Get( Vector2Int pos )						=> Get( pos.x, pos.y );
		public void SetSafe( Vector2Int pos, T cell )		=> SetSafe( pos.x, pos.y, cell );
		public void Set( Vector2Int pos, T cell )			=> Set( pos.x, pos.y, cell );
		public T this[ Vector2Int pos ]
		{
			get												=> this[ pos.x, pos.y ];
			set												=> this[ pos.x, pos.y ]		= value;
		}

#endregion


		public void SetRect( Vector2Int pos, Vector2Int size, T cell )
		{
			for (int y = 0; y < size.y; y ++)
			for (int x = 0; x < size.x; x ++)
				Set( pos + new Vector2Int( x, y ), cell );
		}


		public IEnumerator<Vector2Int> GetEnumerator()		=> new RectIterator( _rect );
		IEnumerator IEnumerable.GetEnumerator()				=> GetEnumerator();
	}
}

