namespace Game.Maps
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Game.Utilities;


	public struct LocalMap : IEnumerable<Vector2Int>
	{
		Vector2Int	_world_Size;
		Vector2Int	_world_MinIN;
		Vector2Int	_world_MaxIN;

		Vector2Int	_local_Size;
		Vector2Int	_local_MinIN;
		Vector2Int	_local_MaxIN;

		Vector2Int	_world_Zero;
		Vector2Int	_world_AxisX;
		Vector2Int	_world_AxisY;


		public LocalMap( LocalMap other, RectInt rect )
			: this( other.GetWorld( rect ) )
		{
			// Copy rotation
			SetAxisX( other.AxisX() );
			SetAxisY( other.AxisY() );
			SetZeroToLocalMin();
		}


		public LocalMap( RectInt r )			: this( r.min, r.size ) {}
		public LocalMap( Vector2Int size )		: this( Vector2Int.zero, size ) {}
		public LocalMap( Vector2Int world_Min, Vector2Int size )
			: this()
		{
			_local_Size			= size;
			_local_MinIN		= Vector2Int.zero;
			_local_MaxIN		= size - Vector2Int.one;

			_world_Size			= size;
			_world_MinIN		= world_Min;
			_world_MaxIN		= world_Min + size - Vector2Int.one;

			_world_Zero			= world_Min;
			_world_AxisX		= Vector2Int.right;
			_world_AxisY		= Vector2Int.up;
		}


		void _Recalc_localSize()
		{
			_local_Size			= new Vector2Int(
													Utils.Dot( _world_Size, _world_AxisX.Abs() ),
													Utils.Dot( _world_Size, _world_AxisY.Abs() )
			);
		}


		void _Recalc_localMinMax()
		{
			Vector2Int w_zero2min		= _world_MinIN - _world_Zero;
			Vector2Int w_zero2max		= _world_MaxIN - _world_Zero;

			if (_world_AxisX.x == 0)
			{
				w_zero2min				= w_zero2min.Swap();
				w_zero2max				= w_zero2max.Swap();
			}

			int signX			= _world_AxisX.x + _world_AxisX.y;
			int signY			= _world_AxisY.x + _world_AxisY.y;

			w_zero2min.x		*= signX;
			w_zero2max.x		*= signX;
			w_zero2min.y		*= signY;
			w_zero2max.y		*= signY;

			_local_MinIN.x		= Mathf.Min( w_zero2min.x, w_zero2max.x );
			_local_MaxIN.x		= Mathf.Max( w_zero2min.x, w_zero2max.x );
			_local_MinIN.y		= Mathf.Min( w_zero2min.y, w_zero2max.y );
			_local_MaxIN.y		= Mathf.Max( w_zero2min.y, w_zero2max.y );
		}


		public void MirrorX_local()
		{
			_world_AxisX				*= -1;
			_Recalc_localMinMax();
		}


		public void MirrorY_local()
		{
			_world_AxisY				*= -1;
			_Recalc_localMinMax();
		}


		public void MirrorX_global()
		{
			ref Vector2Int axis			= ref _world_AxisX.x == 0 ? ref _world_AxisY : ref _world_AxisX;
			axis						*= -1;
			_Recalc_localMinMax();
		}


		public void MirrorY_global()
		{
			ref Vector2Int axis			= ref _world_AxisY.y == 0 ? ref _world_AxisX : ref _world_AxisY;
			axis						*= -1;
			_Recalc_localMinMax();
		}


		public void Rotate_CCW90( int times = 1 )
		{
			for (int i = 0; i < times; i ++)
			{
				_world_AxisX		= _world_AxisX.Rotated_ccw_90();
				_world_AxisY		= _world_AxisY.Rotated_ccw_90();
			}

			_Recalc_localSize();
			_Recalc_localMinMax();
			SetZeroToLocalMin();
		}


		void SetZero( Vector2Int world )
		{
			_world_Zero		= world;

			_Recalc_localMinMax();
		}


		public void SetZeroToLocalMin()
		{
			SetZero( GetWorld( Local_MinIN() ) );
		}


		public RectInt GetWorld( RectInt local )
		{
			Vector2Int w_cornerIN_1		= GetWorld( local.min );
			Vector2Int w_cornerIN_2		= GetWorld( local.max - Vector2Int.one );

			int w_x0					= Mathf.Min( w_cornerIN_1.x, w_cornerIN_2.x );
			int w_y0					= Mathf.Min( w_cornerIN_1.y, w_cornerIN_2.y );
			int w_x1					= Mathf.Max( w_cornerIN_1.x, w_cornerIN_2.x );
			int w_y1					= Mathf.Max( w_cornerIN_1.y, w_cornerIN_2.y );

			Vector2Int w_minIN			= new Vector2Int( w_x0, w_y0 );
			Vector2Int w_maxIN			= new Vector2Int( w_x1, w_y1 );

			return new RectInt(
				w_minIN,
				w_maxIN - w_minIN + Vector2Int.one
			);
		}


		public void SetAxisX( Vector2Int world_axisX )
		{
			_world_AxisX	= world_axisX;

			_Recalc_localSize();
			_Recalc_localMinMax();
		}


		public void SetAxisY( Vector2Int world_axisY )
		{
			_world_AxisY	= world_axisY;

			_Recalc_localSize();
			_Recalc_localMinMax();
		}


		public Vector2Int GetLocal( Vector2Int world )
		{
			Vector2Int w_delta		= world - _world_Zero;

			return new Vector2Int(
				Utils.Dot( w_delta, _world_AxisX ),
				Utils.Dot( w_delta, _world_AxisY )
			);
		}


		public Vector2Int GetWorld( int localX, int localY )	=> _world_Zero + _world_AxisX * localX + _world_AxisY * localY;
		public Vector2Int GetWorld( Vector2Int local )			=> GetWorld( local.x, local.y );
		public Vector2Int Local_Size()							=> _local_Size;
		public Vector2Int Local_MinIN()							=> _local_MinIN;
		public Vector2Int Local_MaxIN()							=> _local_MaxIN;
		public Vector2Int AxisX()								=> _world_AxisX;
		public Vector2Int AxisY()								=> _world_AxisY;
		public RectInt LocalRect()								=> new RectInt( _local_MinIN, _local_Size );


		public IEnumerator<Vector2Int> GetEnumerator()			=> LocalRect().Iterate().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator()					=> GetEnumerator();
	}
}

