using Content.Server._GabyStation.Skeleton.Fracture;
using Content.Shared.GameObjects.Components;
using Content.Server.GameObjects.Components.Movement;
using Content.Server.GameObjects.Components.Health;
using Content.Server.GameObjects.Components.Dizziness;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server._GabyStation.Skeleton.Fracture {
	public class BoneFractureSystem : EntitySystem {
		[Dependency] private readonly IEntityManager _entityManager = default!;
		[Dependency] private readonly IGameTiming _gameTiming = default!;

		public override void Initialize() {
				base.Initialize();
		}

		
		public override void Update(float frameTime) {
				base.Update(frameTime);

				foreach (var entity in _entityManager.GetEntitiesWithComponent<BoneFractureComponent>()) {
						var fracture = _entityManager.GetComponent<BoneFractureComponent>(entity);
						ApplyFractureEffects(fracture);
				}
		}

		// Aplica os efeitos de fratura, como dor e dificuldade de movimento
		private void ApplyFractureEffects(BoneFractureComponent fracture) {
			// Verifica se há fraturas
			if (!fracture.IsFractured)
				return;

			foreach (var bone in fracture.BoneDamage) {
				// Aplica os efeitos dependendo da parte do corpo
				if (bone.Value >= 25f) {
					if (bone.Key == "head") {
							ApplyHeadFractureEffect();
					} else if (bone.Key == "torso") {
							ApplyTorsoFractureEffect(fracture.Owner);
					} else if (bone.Key == "left_leg" || bone.Key == "right_leg") {
							ApplyLegFractureEffect(fracture.Owner);
					}	else if (bone.Key == "left_arm" || bone.Key == "right_arm") {
							ApplyArmFractureEffect(fracture.Owner, bone.Key);
					}	else if (bone.Key == "left_foot" || bone.Key == "right_foot") {
							ApplyFootFractureEffect(fracture.Owner);
					}	else if (bone.Key == "left_hand" || bone.Key == "right_hand") {
							ApplyHandFractureEffect(fracture.Owner, bone.Key);
					}
				}
			}
		}
		
		
		// Efeitos de fraturas
		private void ApplyHeadFractureEffect(IEntity entity) {
			var dizzinessComponent = _entityManager.GetComponent<DizzinessComponent>(entity);

			if (dizzinessComponent != null) {
				dizzinessComponent.ApplyDizziness();
				entity.PopupMessage("Você sente uma dor intensa na cabeça. Sua visão começa a girar.");
			}
			_gameTiming.AddTimer(30f, () => {
        // Após 30 segundos, o personagem desmaia
        entity.PopupMessage("Você desmaiou devido à dor intensa na cabeça.");
        var healthComponent = _entityManager.GetComponent<HealthComponent>(entity);
        healthComponent.ApplyDamage(10); // Aplica dano para simular um desmaio
			});
		}
		
		private void ApplyTorsoFractureEffect(IEntity entity) {
			_gameTiming.AddTimer(5f, () => {
        // Aplica 5 pontos de dano a cada segundo enquanto a fratura estiver presente
        var healthComponent = _entityManager.GetComponent<HealthComponent>(entity);
        healthComponent.ApplyDamage(5);
			});
			entity.PopupMessage("Você sente uma dor forte no peito, dificultando a respiração.");
		}
		
		private void ApplyLegFractureEffect(IEntity entity) {
			var movementComponent = _entityManager.GetComponent<MovementComponent>(entity);

			if (movementComponent != null) {
				// Aplica penalidade de movimento: Incapacidade de correr
				movementComponent.ApplySlowdown(0.5f);  // 50% mais lento ao correr
				entity.PopupMessage("Você sente uma dor intensa nas pernas, dificultando sua corrida.");
			}
			
			// Verificar se ambas as pernas estão fraturadas
			var fractureComponent = _entityManager.GetComponent<BoneFractureComponent>(entity);
			if (fractureComponent != null && fractureComponent.BoneDamage.ContainsKey("left_leg") && fractureComponent.BoneDamage.ContainsKey("right_leg")) {
				// Se ambas as pernas estão fraturadas, o personagem não consegue ficar em pé
				entity.PopupMessage("Ambas as suas pernas estão doendo, você não consegue mais ficar de pé.");
				// Talvez implementar uma lógica para o personagem cair
			}
		}
		
		private void ApplyArmFractureEffect(IEntity entity, string arm) {
			float slowAmount = 0.5f; // 50% mais lento
			string armSide = arm == "left_arm" ? "esquerdo" : "direito";
			var slowEffect = _entityManager.GetComponent<SlowdownComponent>(entity);
    
			if (slowEffect == null) {
				return;
			}
			//diminui a velocidade de ação
			if (arm == "left_arm" || arm == "right_arm") {
        slowEffect.ApplySlowdown(slowAmount);
				entity.PopupMessage($"Seu braço {armSide} parece deslocado e incapaz de se mexer muito.");
			}
		}
		
		private void ApplyFootFractureEffect(IEntity entity) {
			var movementComponent = _entityManager.GetComponent<MovementComponent>(entity);

			if (movementComponent != null) {
				// Aplica penalidade de movimento: anda mais lentamente
				movementComponent.ApplySlowdown(0.3f);  // 30% mais lento ao andar
				
				entity.PopupMessage("Você sente uma dor intensa nos pés, sua caminhada está mais lenta.");
			}

			// Verificar se ambos os pés estão fraturados
			var fractureComponent = _entityManager.GetComponent<BoneFractureComponent>(entity);
			if (fractureComponent != null && fractureComponent.BoneDamage.ContainsKey("left_foot") && fractureComponent.BoneDamage.ContainsKey("right_foot")) {
				// Se ambos os pés estão fraturados, o personagem não consegue ficar de pé
				entity.PopupMessage("Ambos os seus pés estão doendo, você não consegue mais ficar de pé.");
				// Talvez implementar uma lógica para o personagem cair
			}
		}
		
		private void ApplyHandFractureEffect(IEntity entity, string hand) {
			string handSide = hand == "left_hand" ? "esquerdo" : "direito";
			var handComponent = _entityManager.GetComponent<HandComponent>(entity);

			if (handComponent != null) {
				// Desabilita a capacidade de segurar objetos
				handComponent.CanHoldItems = false;
				entity.PopupMessage($"Sua mão {handSide} doi intensamente, você não consegue segurar nada.");
			}
		}
		
	}
}
