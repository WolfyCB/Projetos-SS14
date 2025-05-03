using Robust.Server.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Content.Shared._GabyStation.Species.Clancker;

namespace Content.Server._GabyStation.Species.Clancker.DrainFuel;

public sealed class DrainFuelSystem : EntitySystem {
	[Dependency] private readonly IGameTiming _timing = default!;
	[Dependency] private readonly SolutionContainerSystem _solutionSystem = default!;
	[Dependency] private readonly PopupSystem _popup = default!;
	
	public override void Update(float frameTime){
		base.Update(frameTime);

		var query = EntityQueryEnumerator<ClanckerComponents>();

		while (query.MoveNext(out var uid, out var component)) {
			//diminui na quantidade de combustivel no clanckers
			float energyLost = component.DrainPerSecond * frameTime;
			solution.RemoveReagent("WeldingFuel", FixedPoint2.New(energyLost));
			
			if (component.CurrentEnergy <= 0) {
				component.CurrentEnergy = 0;

				if (TryComp<FuelSlotComponent>(uid, out var fuelSlot) && fuelSlot.FuelContainer != null) {
					if (_solutionSystem.TryGetSolution(fuelSlot.FuelContainer.Value, FuelSlotComponent.SolutionName, out var solution)) {
							UpdateEnergy(uid, solution);
					}
				}
			}
			
			if (component.CurrentEnergy < component.MaxEnergy * 0.2f){
				_popup.PopupEntity(Loc.GetString("clancker-low-energy-warning"), uid, args.User);
			}
		}
		
	}

	public void UpdateEnergy(EntityUid uid, Solution solution){ //atualiza a energia de acordo com o combustivel interno
		if (!TryComp<ClanckerComponents>(uid, out var drainComp))
			return;
		
		if (drainComp.MaxEnergy != solution.Volume) {
			drainComp.MaxEnergy = solution.Volume;
		}
		drainComp.CurrentEnergy = (float)solution.GetReagentQuantity("WeldingFuel");
		
		if (drainComp.CurrentEnergy > drainComp.MaxEnergy)
			drainComp.CurrentEnergy = drainComp.MaxEnergy;
	}
}