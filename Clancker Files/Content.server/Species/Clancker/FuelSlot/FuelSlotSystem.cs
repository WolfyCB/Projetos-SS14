using Content.Server.Chemistry.Components;
using Content.Server.Popups;
using Content.Shared.Interaction;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._Gabystation.Species.Clancker.FuelSlot;

public sealed class FuelSlotSystem : EntitySystem {
	
	[Dependency] private readonly PopupSystem _popup = default!;
	[Dependency] private readonly SharedInteractionSystem _interaction = default!;
	[Dependency] private readonly TagSystem _tagSystem = default!;

	public override void Initialize(){
		base.Initialize();
		SubscribeLocalEvent<FuelSlotComponent, InteractUsingEvent>(OnInteractUsing);
		SubscribeLocalEvent<FuelSlotComponent, InteractHandEvent>(OnInteractHand);
	}

	private void OnInteractUsing(EntityUid uid, FuelSlotComponent comp, InteractUsingEvent args){
		if (comp.HasFuelContainer){
			_popup.PopupEntity(Loc.GetString("clancker-full-slot"), uid, args.User);
			return;
		}

		if (!HasComp<SolutionContainerManagerComponent>(args.Used)){
			_popup.PopupEntity(Loc.GetString("clancker-no-liquid"), uid, args.User);
			return;
		}

		if (!_tagSystem.HasTag(args.Used, "WeldingFuel")){
			_popup.PopupEntity(Loc.GetString("clancker-no-weld-fuel"), uid, args.User);
			return;
		}

		comp.FuelContainer = args.Used;
		comp.HasFuelContainer = true;

		_popup.PopupEntity(Loc.GetString("clancker-connected"), uid, args.User);
		args.Handled = true;
	}

	private void OnInteractHand(EntityUid uid, FuelSlotComponent comp, InteractHandEvent args){
		if (!comp.HasFuelContainer || comp.FuelContainer == null){
			_popup.PopupEntity(Loc.GetString("clancker-not-connected"), uid, args.User);
			return;
		}

		var fuel = comp.FuelContainer.Value;

		if (!Deleted(fuel)){
				_interaction.DoDrop(args.User, fuel);
		}

		comp.FuelContainer = null;
		comp.HasFuelContainer = false;

		_popup.PopupEntity(Loc.GetString("clancker-remove-slot"), uid, args.User);
		args.Handled = true;
	}
}
