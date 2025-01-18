namespace Game.Maps
{
	using System.Collections.Generic;
	using UnityEngine;


	public interface IPixelMaps
	{
		IPixelMap Add();
	}


	public class PixelMaps : MonoBehaviour, IPixelMaps
	{
#region External

		[SerializeField] PixelMap	_pixelMapPrefab;

#endregion

		readonly List<IPixelMap> _pixelMaps		= new List<IPixelMap>();


		public IPixelMap Add()
		{
			IPixelMap pixelMap		= Instantiate( _pixelMapPrefab, transform.position, Quaternion.identity, transform );

			_pixelMaps.Add( pixelMap );

			return pixelMap;
		}
	}
}

