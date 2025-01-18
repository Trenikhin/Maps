namespace Game.Fog
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	public static class NeighborsIteratorExt
	{
		static IEnumerator<Vector2Int> NeighborsEnumerator( this Vector2Int pos )
		{
			for (int i = 0; i < 4; i ++)
				yield return pos + ((EDir)i).Vec();
		}

		
		struct NeighborsEnumerable : IEnumerable<Vector2Int>
		{
			public Vector2Int	Pos;

			public IEnumerator<Vector2Int> GetEnumerator()		=> Pos.NeighborsEnumerator();
			IEnumerator IEnumerable.GetEnumerator()				=> GetEnumerator();
		}


		public static IEnumerable<Vector2Int> Neighbors( this Vector2Int pos )
		=>
			new NeighborsEnumerable{ Pos = pos };
	}
}

