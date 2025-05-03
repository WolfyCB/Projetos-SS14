using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._GabyStation.Species.Clancker.FuelConsume;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class FuelConsumeComponent : Component
{
	
	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("energyPerFuelUnit")]
  public float EnergyPerFuelUnit = 100f;
	
	[ViewVariables(VVAccess.ReadWrite)]
  [DataField("fuelPerUse")]
  public float FuelConsume = 100f;
}