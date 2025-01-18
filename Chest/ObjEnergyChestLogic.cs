namespace Game.Objects
{
	using Game.Factories;
	using Game.Save;
	using Zenject;
	
	public interface IChest
	{
		bool IsOpened { get; }
		
		bool TryOpen();
	}
	
	public class ObjChestLogic : IChest
	{
		[Inject] ObjModel				_objModelChest;
		
		[Inject] IObjSpawner	_objSpawner;
		[Inject] IAutoSaver		_saver;
		
		public bool IsOpened { get; private set; }

		public bool TryOpen()
		{
			if (IsOpened)
				return false;
			
			_objSpawner.Destroy(_objModelChest, EDestroy.Model);
			IsOpened = true;
			_saver.Save();

			return true;
		}
	}
}