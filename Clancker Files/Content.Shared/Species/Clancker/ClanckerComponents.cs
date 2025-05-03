using Robust.Shared.GameStates;

namespace Content.Server._GabyStation.Species.Clancker;

/// <summary>
/// Componente que armazena a energia interna do Clancker.
/// A energia vai sendo drenada com o tempo pelo sistema.
/// </summary>
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class ClanckerComponents : Component
{
    [DataField("currentEnergy"), AutoNetworkedField]
    public float CurrentEnergy = 100f;

    [DataField("maxEnergy")]
    public float MaxEnergy = 100f;

    [DataField("drainPerSecond")]
    public float DrainPerSecond = 0.2f;
		
		
}