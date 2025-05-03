using Content.Shared.Interaction;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using Robust.Server.GameObjects;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Server._GabyStation.Skeleton.Fracture;

namespace Content.Server._GabyStation.Skeleton.HealBones {
	[RegisterComponent]
	public sealed class OrthopedicPlasterComponent : Component {
		public override string Name => "OrthopedicPlaster";

		[Dependency] private readonly IEntityManager _entityManager = default!;
		[Dependency] private readonly IGameTiming _timing = default!;

		private const float CureAmount = 0.1f; // Cura de 0.1 blunt por tick
		private const float ApplicationTime = 3.0f; // Tempo de aplicação de 3 segundos

		public override void Initialize() {
			base.Initialize();
		}

		[InteractionVerb]
		private sealed class ApplyPlasterVerb : Verb<OrthopedicPlasterComponent> {
			protected override void GetData(IEntity user, OrthopedicPlasterComponent component, VerbData data) {
				data.Text = Loc.GetString("Aplicar Gesso");
			}

			protected override void Activate(IEntity user, OrthopedicPlasterComponent component) {
					// Acionado quando o jogador clica para usar o item
			}
		}

		[InteractionTarget]
		public void OnInteractUsing(InteractUsingEvent args) {
			if (args.Target == null || args.Used == null)
				return;

			// Verifica se o alvo tem o componente BodyComponent
			if (!_entityManager.TryGetComponent(args.Target.Value, out BodyComponent body)) {
				args.User.PopupMessage(Loc.GetString("Este corpo não é válido para aplicar gesso."));
				return;
			}

			// Obtém a parte do corpo clicada usando as coordenadas do clique
			if (args.TargetLocation is EntityCoordinates coordinates) {
				var clickedPart = body.GetBodyPartAt(coordinates);
				if (clickedPart != null) {
					// Verifica se o osso da parte clicada está fraturado
					if (_entityManager.TryGetComponent(clickedPart.Owner, out BoneFractureComponent fracture)) {
						if (fracture.IsFractured) {
							// Inicia o processo de aplicação com DoAfter (tempo de aplicação)
							var doAfter = new DoAfterEventArgs(args.User, ApplicationTime, target: args.Target) {
								BreakOnMove = true,
								BreakOnDamage = true,
								BreakOnStun = true,
								NeedHand = true
							};

							_entityManager.EntitySysManager.GetEntitySystem<DoAfterSystem>().DoAfter(doAfter).OnCompleted(() => {
								// Quando o processo de aplicação termina, começa a cura do dano blunt
								fracture.CureBluntDamage(CureAmount);
								args.User.PopupMessage(Loc.GetString("Você aplica o gesso na {0}.", clickedPart.Name));
								
								// Deleta o gesso após o uso
								_entityManager.QueueDelete(EntityUid);
							});
						} else {
							args.User.PopupMessage(Loc.GetString("Essa parte do corpo não está fraturada."));
						}
					}
				}
			}
			
		}
	}
}
