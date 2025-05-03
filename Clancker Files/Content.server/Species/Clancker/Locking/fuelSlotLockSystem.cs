using Content.Server._gabystation.Species.Clancker.FuelSlot;
using Content.Shared._gabystation.Species.Clancker;
using Content.Server.Operations; // Importar para mostrar a barra de progresso

namespace Content.Server._GabyStation.Species.Clancker.Locking {
	private const float UnlockTime = 3f; // Tempo de destrancamento em segundos
	private const float LockTime = 1f; // Tempo de trancamento em segundos
	
	[Dependency] private readonly IGameTiming _timing = default!;
	[Dependency] private readonly PopupSystem _popupSystem = default!;
	
	public sealed class FuelSlotLockSystem : EntitySystem {
		
		// Sistema que executa a lógica de destrancar/trancar e mostra a barra de progresso
		public override void GetVerbs(EntityUid uid, ClanckerComponent component, GetVerbsEvent<InteractionVerb> args) {
			if (!args.CanAccess || !args.CanInteract)
					return;

			// Destranca automaticamente apos ficar sem energia
			if (component.CurrentEnergy <= 0f && !TryComp<FuelSlotComponent>(uid, out var slot)){
				slot.Locked = false;
			}
		}

		// Método para iniciar o processo de destrancamento/trancamento
		private void StartLockUnlockProcess(EntityUid uid, ClanckerComponent component) {
			if (TryComp<ClanckerComponents>(uid, out var lockComp))
			{
				// Verifica se o Clancker está trancado ou destrancado e inicia a ação
				if (lockComp.Locked) {
					StartUnlockProcess(uid, component, lockComp);
				}
				else {
					StartLockProcess(uid, component, lockComp);
				}
			}
		}

		// Método para iniciar o processo de destrancamento
		private void StartUnlockProcess(EntityUid uid, ClanckerComponent component, LockComponent lockComp) {
			var owner = EntityManager.GetComponent<ActorComponent>(uid).PlayerSession;
			
			_popupSystem.PopupEntity(Loc.GetString("player-locking-verb", ("user", owner.Name)), uid, PopupType.Large);
			
			Timer.Spawn(UnlockTime, () =>  {
				ShowProgressBar(uid, UnlockTime, Loc.GetString("clancker-unlocking-verb"));
				lockComp.Locked = false;
			});
		}
		
		// Método para iniciar o processo de trancamento
		private void StartLockProcess(EntityUid uid, ClanckerComponent component, LockComponent lockComp) {
			var owner = EntityManager.GetComponent<ActorComponent>(uid).PlayerSession;
			
			_popupSystem.PopupEntity(Loc.GetString("player-locking-verb", ("user", owner.Name)), uid, PopupType.Large);
			
			Timer.Spawn(LockTime, () =>  {
					ShowProgressBar(uid, LockTime, Loc.GetString("clancker-locking-verb"));
					lockComp.Locked = true;
			});
		}
		
		// Método para mostrar a barra de progresso na tela (isso seria algo client-side normalmente)
		private void ShowProgressBar(EntityUid uid, float timeRemaining, string actionText) {
			SendProgressBarUpdateToClient(uid, timeRemaining, actionText);
		}
		
		// Envia a atualização da barra de progresso para o cliente
		private void SendProgressBarUpdateToClient(EntityUid uid, float timeRemaining, string actionText) {
			string progressText = actionText == Loc.GetString("clancker-unlocking-verb")
				? Loc.GetString("clancker-unlocking-verb") 
        : Loc.GetString("clancker-locking-verb");
			
			float progress = 1f - (timeRemaining / (actionText == LockAction.Unlock ? UnlockTime : LockTime));
			
			// Envia as informações de progresso (precisa de implementação específica para seu framework de UI/cliente)
			EntityManager.EntityQuery<YourClientComponent>(uid, (comp) => {
				comp.UpdateProgressBar(progress, progressText);
			});
		}
	}
}