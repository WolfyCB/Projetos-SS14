using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

namespace Content.Server._Gabystation.Species.Clancker.FuelSlot;

[RegisterComponent]
[NetworkedComponent]
public sealed partial class FuelSlotComponent : Component{
  [DataField("hasFuelContainer", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
  public bool HasFuelContainer = false;
	
	[DataField("fuelContainer")]
	public EntityUid? FuelContainer
	
	[DataField("locked")]
	public bool Locked = true;
}
