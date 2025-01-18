namespace Game.Objects
{
	using Configs;
	using Core;
	using Zenject;

	public interface IChestOpener
	{
		bool TryOpen();
	}
	
	// Use case
	public class EnergyChestOpener : IChestOpener
	{
		// Obj
		[Inject] IChest				_chest;
		[Inject] EnergyChestConfig	_config;
	
		// Resources
		[Inject] IGcEnergy			_gcEnergy;
		[Inject] IGcLevel			_gcLevel;
		
		public bool TryOpen()
		{
			if (_chest.TryOpen())
			{
				GetRewards();
				return true;
			}

			return false;
		}
		
		void GetRewards()
		{
			int	amount = _config.ResAmount;
            
			switch (  _config.Resource )
			{
				case EResourceType.Energy:
					_gcEnergy.AddEnergy( amount, true );
					break;
				case EResourceType.Rubies:
					_gcLevel.AddGems( amount, true );
					break;
				case EResourceType.Gold:
					_gcLevel.AddGold( amount, true );
					break;
			}
		}
	}
}