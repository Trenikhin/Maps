namespace Game.Objects
{
	using System;
	using System.Threading;
	using Configs;
	using Core;
	using Cysharp.Threading.Tasks;
	using Factories;
	using Save;
	using Tweens;
	using UI;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public enum EEnergyChestTooltip
	{
		None,
		
		NoTooltips,
		LowEnergy,
		Selected,
	}
	
	public class EnergyChestPresenter : IInitializable, IDisposable
	{
		// Model
		[Inject] IChest					_chest;
		[Inject] IChestOpener			_chestOpener;
		[Inject] ObjModel				_objModelChest;
		[Inject] EnergyChestConfig		_config;
		
		// View
		[Inject] IObjFacade				_objFacade;
		[Inject] IUiEnergyChestView		_view;
		[Inject] Animator				_animator;
		[Inject] AnimatorListener		_animatorListener;
		
		// Services
		[Inject] IObjSpawner	_objSpawner;
		[Inject] IGcEnergy		_gcEnergy;
		[Inject] IFlyIcons		_flyIcons;
		 
		BoolReactiveProperty Selected = new BoolReactiveProperty();
		
		CancellationTokenSource _cts = new CancellationTokenSource();
		CompositeDisposable _disposables = new CompositeDisposable();
		
		public void Initialize()
		{
			const int energyWhenShow = 5;
			
			// Choose tooltip
			Observable
				.Merge
				(
					Selected.AsUnitObservable(),
					_gcEnergy.EnergyShow.AsUnitObservable(),
					_objFacade.ShortTapPerformed
				)
				.Subscribe( s =>
				{
					bool selected      = Selected.Value;
					bool needToShow    = _gcEnergy.EnergyShow.Value <= energyWhenShow;
					
					EEnergyChestTooltip tooltip = _chest.IsOpened ?
						EEnergyChestTooltip.NoTooltips :
						selected ? 
							EEnergyChestTooltip.Selected : 
							needToShow ? 
								EEnergyChestTooltip.LowEnergy : 
								EEnergyChestTooltip.NoTooltips;
					
					_view.SetTooltip( tooltip );
				} )
				.AddTo( _disposables );
            
			// On Open clicked
			_view.OnClaimClick
				.Subscribe( _ =>
				{
					_chestOpener.TryOpen();
					RunOpenAnimation(_cts.Token).Forget();
					Selected.Value = false;
				} )
				.AddTo( _disposables );
			
			// Select deselect
			_objFacade.ShortTapPerformed
				.Merge( _objFacade.GrabPerformed )
				.Subscribe( _ =>
				{
					Selected.Value = true;
					
					if ( !_chest.IsOpened )
						_animator.SetTrigger( A_Interact );
				} )
				.AddTo( _disposables );
			_objFacade.Deselect
				.Subscribe( _ => Selected.Value = false  )
				.AddTo( _disposables );
			
			// Set cost
			_view.SetEnergy( $"{_config.ResAmount}<space=-12>{_config.Resource.GetCurrencyCode_100()}" );
		}

		public void Dispose()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_disposables?.Dispose();
		}

		void StopAnimation( bool stopAnim ) => _animator.speed = stopAnim ? 0f : 1f;
		
		async UniTask RunOpenAnimation( CancellationToken ct )
		{
			_gcEnergy.ChangeEnergyFake( -_config.ResAmount );
			_animator.SetTrigger(A_Open);

			await UniTask.WaitForSeconds(1, cancellationToken: ct);
			
			StopAnimation( true );
			
			_gcEnergy.ChangeEnergyFake( _config.ResAmount );
			_flyIcons.EnergyFromCell( _config.ResAmount, _objModelChest.Pos );
			
			await UniTask.WaitForSeconds(1, cancellationToken: ct);
			StopAnimation(false);
			await UniTask.WaitForSeconds(1, cancellationToken: ct);
			_objSpawner.Destroy( _objModelChest, EDestroy.All );
		}
		
		// Animations names
		const string	A_Interact 		= "Interact";
		const string	A_Open 			= "Open";
	}
}