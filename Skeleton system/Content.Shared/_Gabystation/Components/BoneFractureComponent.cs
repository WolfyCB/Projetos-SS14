using Content.Shared.Prototypes;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using YamlDotNet.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Content.Shared._GabyStation.Skeleton.Components {
	[RegisterComponent]
	public sealed class BoneFractureComponent : Component {
		public override string Name => "BoneFracture";

		// Dicionário para armazenar o dano blunt de cada parte do corpo
		// "Cabeça", "Braços", "Torso", etc.
		public Dictionary<string, float> BoneDamage = new Dictionary<string, float>() {
			{ "head", 0f },
			{ "torso", 0f },
			{ "left_arm", 0f },
			{ "right_arm", 0f },
			{ "left_leg", 0f },
			{ "right_leg", 0f },
			{ "left_hand", 0f },
			{ "right_hand", 0f },
			{ "left_foot", 0f },
			{ "right_foot", 0f }
		};
		
		public Dictionary<string, float> fractureThreshold = new();
		public Dictionary<string, float> damageTransfer = new();
		
		// Fraturas
		public bool IsFractured => BoneDamage.Values.Any(damage => damage >= 1);
		
		// Método para curar o dano blunt de um osso específico
		public void CureBluntDamage(float amount) {
			foreach (var bone in BoneDamage.Keys.ToList()) {
				if (BoneDamage[bone] > 0) {
					BoneDamage[bone] = Math.Max(0, BoneDamage[bone] - amount);
				}
			}
		}

		// Método para aplicar dano blunt em um osso específico
		public void ApplyBluntDamage(string bone, float amount) {
			if (BoneDamage.ContainsKey(bone)) {
					BoneDamage[bone] += amount;
			}
		}
		
		// Método para aplicar dano blunt em um osso específico
		public void ApplyBluntDamage(string bone, float amount) {
			if (BoneDamage.ContainsKey(bone)) {
					BoneDamage[bone] += amount;
			}
		}

		// Método para carregar os dados do arquivo YML
		public void LoadBoneData(string filePath) {
			var deserializer = new DeserializerBuilder().Build();
			var fileContent = File.ReadAllText(filePath);
			var bonesData = deserializer.Deserialize<BoneDataContainer>(fileContent);

			foreach (var bone in bonesData.Bones) {
				// Armazena o limite de fratura e o limite de dano no dicionário
				fractureThreshold[bone.Key] = bone.Value.FractureThreshold;
				damageTransfer[bone.Key] = bone.Value.DamageTransfer;
			}
		}

		// Representação dos dados de cada osso
		public class BoneData {
			public float DamageThreshold { get; set; }
			public float FractureThreshold { get; set; }
		}

		// Contêiner para armazenar todos os ossos
		public class BoneDataContainer {
			public Dictionary<string, BoneData> Bones { get; set; }
		}
	}
}
