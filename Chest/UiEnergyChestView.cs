namespace Game.UI
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using System;
	using Game.Objects;

	public interface IUiEnergyChestView
	{
		IObservable<Unit> OnClaimClick {get;}

		void SetTooltip(EEnergyChestTooltip tooltip);
		void SetEnergy(string             text);
	}
	
	public class UiEnergyChestView : MonoBehaviour, IUiEnergyChestView
	{
		[SerializeField] TextMeshProUGUI _resText;
		[SerializeField] Button _claimButton;

		[Header("Tooltips")]
		[SerializeField] UiTooltipManager _tooltipManager;
		[SerializeField] UiTooltip _lowEnergyTooltip;
		[SerializeField] UiTooltip _claimEnergyTooltip;
		
		public IObservable<Unit> OnClaimClick => _claimButton.OnClickAsObservable();

		public void SetTooltip( EEnergyChestTooltip tooltip )
		{
			_tooltipManager.Toggle( _lowEnergyTooltip, tooltip == EEnergyChestTooltip.LowEnergy );
			_tooltipManager.Toggle( _claimEnergyTooltip, tooltip == EEnergyChestTooltip.Selected );
		}
		
		public void SetEnergy(string   text)	=> _resText.text = text;
	}
}