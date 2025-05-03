using Content.Shared.Chemistry.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;


namespace Content.Server._GabyStation.Species.Clancker.FuelConsume;

private static readonly SoundSpecifier _sloshSound = new SoundPathSpecifier("/Audio/Effect/Fluids/slosh.ogg"); // som de fluido

/// <summary>
/// Sistema que lida com a lógica de sucção de combustível dos Clanckers.
/// </summary>
public sealed class FuelConsumeSystem : EntitySystem {
  [Dependency] private readonly SharedPopupSystem _popup = default!;
	[Dependency] private readonly SolutionContainerSystem _solutionSystem = default!;
	[Dependency] private readonly ClanckerDrainFuelSystem _drainerSystem = default!;

	public override void Initialize() {
		base.Initialize();
		SubscribeLocalEvent<FuelConsumeComponent, AfterInteractEvent>(OnAfterInteract);
	}

	private void OnAfterInteract(EntityUid uid, FuelConsumeComponent component, AfterInteractEvent args){
		if (!args.CanReach || args.Target == null)
			return;

		var target = args.Target.Value;

		// Verifica se o alvo possui solução com o nome "tank"
		if (!_solutionSystem.TryGetSolution(target, "tank", out var solution)) {
			_popup.PopupEntity(Loc.GetString("target-no-fuel"), uid, PopupType.Medium);
			return;
		}

		// Verifica se há combustível suficiente
		if (solution.Volume < component.FuelPerUse) {
			_popup.PopupEntity(Loc.GetString("target-low-fuel"), uid, PopupType.Medium);
			return;
		}

		// Remove o combustível
		var removed = _solutionSystem.TryRemoveReagent(target, solution, "WeldingFuel", component.FuelConsume);
		
		if (removed > 0 && !TryComp<FuelSlotComponent>(clancker, out var slot)) {
			_popup.PopupEntity(Loc.GetString("clancker-success-fuel"), uid, PopupType.Medium);
			SoundSystem.Play(_sloshSound.GetSound(), Filter.Pvs(uid), uid);
			
			TryAddFuelToInternalContainer(uid, removed);
		}
		else{
			_popup.PopupEntity(Loc.GetString("clancker-fail-fuel"), uid, PopupType.Medium);
		}
	}
	
	private void TryAddFuelToInternalContainer(EntityUid clancker, FixedPoint2 amount) {
    // Procura o componente de slot de combustível
    if (!TryComp<FuelSlotComponent>(clancker, out var slot))
			return;

    if (slot.ContainerSlot.ContainedEntity is not { Valid: true } contained)
			return;

    // Verifica se o recipiente interno tem uma solução "tank"
    if (!_solutionSystem.TryGetSolution(contained, "tank", out var internalSolution))
			return;

    // Adiciona combustível de solda ao recipiente interno
    _solutionSystem.TryAddReagent(contained, internalSolution, "WeldingFuel", amount);
	}
}