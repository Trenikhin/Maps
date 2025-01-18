namespace Game.Maps
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;


	public interface IPixelMap
	{
		Texture2D Texture		{get;}

		void Set( Vector2Int pos, Color color );
		// void Set<T>( IMap<T> map, ColorsT<T> colors ) where T : struct;
		void Set<T>( IMap<T> map, Func<T, Color> cellToColor );

		void Refresh();
		void Disable();
		void Toggle();
		void ApplyTexture();
	}


	public class PixelMap : MonoBehaviour, IPixelMap
	{
#region External

		[Header( "Settings" )]
		[SerializeField] float	_scale		= 10;

		[Header( "Refs" )]
		[SerializeField] Image	_image;

#endregion

		Vector2Int	Size	=> _rect.size;

		RectInt						_rect;
		Func<Vector2Int, Color>		_getColor;


		void Awake()
		{
			EnsureInit();
		}


		void OnValidate()		=> RefreshSafe();


#region IMapView

		public Texture2D Texture		{ get; private set; }


		public void Set( Vector2Int pos, Color color )
		{
			Texture.SetPixel( pos.x - _rect.xMin, pos.y - _rect.yMin, color );
		}


		// public void Set<T>(IMap<T> map, ColorsT<T> colors) where T : struct			=> Set( map, colors.Get );


		public void Set<T>( IMap<T> map, Func<T, Color> cellToColor )
		{
			_rect			= map.Rect;
			_getColor		= p => cellToColor( map.Get( p ) );

			Refresh();
		}


		public void Refresh()
		{
			EnsureInit();
			SetTexture();
			SetScale();
		}


		public void Disable()			=> ShowHide( false );
		public void Toggle()			=> ShowHide( !gameObject.activeSelf );
		public void ApplyTexture()		=> Texture.Apply();

#endregion


		void EnsureInit()
		{
			if (Texture == null)
				Init();
		}


		void Init()
		{
			Texture			= new Texture2D( 1, 1, TextureFormat.RGBA32, false ) { filterMode = FilterMode.Point };
			_image.color		= Color.white;
		}


		void SetTexture()
		{
			// Resize texture
			if (
					Texture.width		!= Size.x ||
					Texture.height		!= Size.y
				)
			{
				Texture.Reinitialize( Size.x, Size.y );
				_image.sprite		= null;
			}

			// Create sprite
			if (_image.sprite == null)
				_image.sprite		= Sprite.Create( Texture, new Rect( 0, 0, Size.x, Size.y ), Vector2.zero, 100 );

			// Set pixels
			foreach (Vector2Int pos in _rect.allPositionsWithin)
			{
				Color color			= _getColor( pos );

				Texture.SetPixel( pos.x - _rect.xMin, pos.y - _rect.yMin, color );
			}

			ApplyTexture();
		}


		void SetScale()
		{
			RectTransform rectTransform		= GetComponent< RectTransform >();
			rectTransform.sizeDelta			= Vector2.Min( (Vector2)Size * _scale, rectTransform.parent.GetComponent<RectTransform>().rect.size );
		}

		
		void ShowHide( bool toShow )		=> gameObject.SetActive( toShow );


		void RefreshSafe()
		{
			if (_getColor != null)
				Refresh();
		}
	}
}

