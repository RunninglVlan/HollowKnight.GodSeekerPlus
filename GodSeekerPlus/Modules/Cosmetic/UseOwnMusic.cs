using UnityEngine.Audio;

namespace GodSeekerPlus.Modules.Cosmetic;

public sealed class UseOwnMusic : Module {
	private static readonly Lazy<AudioMixerSnapshot> silent = new(() =>
		Resources.FindObjectsOfTypeAll<AudioMixerSnapshot>().First(i => i.name == "Silent")
	);
	private static readonly Lazy<AudioMixerSnapshot> normal = new(() =>
		Resources.FindObjectsOfTypeAll<AudioMixerSnapshot>().First(i => i.name == "Normal")
	);

	public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

	private static readonly SceneEdit[] handles = [
		// Pantheon 1
		new(new("GG_Vengefly", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_Gruz_Mother", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_False_Knight", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_Mega_Moss_Charger", "_SceneManager"), StopMusicAndDestroyFSM),
		new(new("GG_Mega_Moss_Charger", "Mega Moss Charger"), go => go
			.LocateMyFSM("Mossy Control").GetAction("Music", 0).Enabled = false
		),
		new(new("GG_Hornet_1", "_SceneManager"), ModifyFSM),

		new(new("GG_Ghost_Gorb", "_SceneManager"), ModifyFSM),
		new(new("GG_Dung_Defender", "_SceneManager"), StopMusicAndModifyFSM),
		new(new("GG_Mage_Knight", "_SceneManager"), _ => StopMusic()),
		new(new("GG_Brooding_Mawlek", "_SceneManager"), ModifyFSM),
		// GG_Nailmasters = NoOp,


		// Pantheon 2
		new(new("GG_Ghost_Xero", "_SceneManager"), ModifyFSM),
		new(new("GG_Crystal_Guardian", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_Soul_Master", "_SceneManager"), ModifyFSM),
		new(new("GG_Soul_Master", "Mage Lord Phase2"), ModifyMageLord2),
		new(new("GG_Oblobbles", "_SceneManager"), ModifyFSM),
		new(new("GG_Mantis_Lords", "_SceneManager"), ModifyFSM),

		new(new("GG_Ghost_Marmu", "_SceneManager"), ModifyFSM),
		new(new("GG_Nosk", "_SceneManager"), ModifyFSM),
		new(new("GG_Flukemarm", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_Broken_Vessel", "_SceneManager"), ModifyGGMusicControl),
		// GG_Painter = NoOp,


		// Pantheon 3
		new(new("GG_Hive_Knight", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_Hive_Knight", "Boss Scene Controller", "door_dreamEnter"), ModifyFSM),
		new(new("GG_Ghost_Hu", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_Collector", "_SceneManager"), ModifyFSM),
		new(new("GG_God_Tamer", "_SceneManager"), ModifyFSM),
		new(new("GG_Grimm", "_SceneManager"), ModifyFSM),

		new(new("GG_Ghost_Galien", "_SceneManager"), ModifyFSM),
		new(new("GG_Grey_Prince_Zote", "_SceneManager"), ModifyFSM),
		new(new("GG_Uumuu", "_SceneManager"), ModifyFSM),
		new(new("GG_Hornet_2", "_SceneManager"), ModifyFSM),
		// GG_Sly = NoOp,


		// Pantheon 4
		new(new("GG_Crystal_Guardian_2", "_SceneManager"), ModifyFSM),
		new(new("GG_Lost_Kin", "_SceneManager"), ModifyFSM),
		new(new("GG_Ghost_No_Eyes", "_SceneManager"), ModifyFSM),
		// GG_Traitor_Lord = NoOp,
		new(new("GG_White_Defender", "_SceneManager"), ModifyFSM),

		new(new("GG_Failed_Champion", "_SceneManager"), ModifyFSM),
		new(new("GG_Ghost_Markoth", "_SceneManager"), ModifyFSM),
		new(new("GG_Watcher_Knights", "_SceneManager"), ModifyFSM),
		new(new("GG_Soul_Tyrant", "_SceneManager"), ModifyFSM),
		new(new("GG_Soul_Tyrant", "Dream Mage Lord Phase2"), ModifyMageLord2),
		// GG_Hollow_Knight = NoOp,


		// Pantheon 5
		new(new("GG_Vengefly_V", "_SceneManager"), ModifyGGMusicControl),
		new(new("GG_Gruz_Mother_V", "_SceneManager"), ModifyGGMusicControl),

		new(new("GG_Ghost_Gorb_V", "_SceneManager"), ModifyFSM),
		new(new("GG_Mage_Knight_V", "_SceneManager"), _ => {
			StopMusic();
			Ref.GM.AudioManager.ApplyMusicSnapshot(normal.Value, 2.5f, 0.1f);
		}),
		new(new("GG_Brooding_Mawlek_V", "Battle Scene"), go => {
			StopMusic();
			var fsm = go.LocateMyFSM("Activate Boss");
			fsm.ChangeTransition("Call Mawlek", FsmEvent.Finished.Name, "Music Up 2");
			fsm.GetAction<ApplyMusicCue>("Music", 0).musicCue.Value
				= fsm.GetAction<ApplyMusicCue>("Music Up 2", 0).musicCue.Value;
			fsm.GetAction<TransitionToAudioSnapshot>("Music", 1).snapshot.Value
				= fsm.GetAction<TransitionToAudioSnapshot>("Music Up 2", 1).snapshot.Value;
		}),

		new(new("GG_Ghost_Xero_V", "_SceneManager"), ModifyFSM),
		// GG_Mantis_Lords_V = NoOp,

		new(new("GG_Ghost_Marmu_V", "_SceneManager"), ModifyFSM),

		new(new("GG_Collector_V", "_SceneManager"), ModifyFSM),

		new(new("GG_Uumuu_V", "_SceneManager"), ModifyFSM),
		new(new("GG_Nosk_Hornet", "Battle Scene"), go => go
			.LocateMyFSM("Battle Control")
			.ChangeTransition("Music Type", "PANTHEON", "Orig Music")
		),

		new(new("GG_Ghost_No_Eyes_V", "_SceneManager"), ModifyFSM),

		new(new("GG_Ghost_Markoth_V", "_SceneManager"), ModifyFSM),
		new(new("GG_Grimm_Nightmare", "Grimm Control"), go => {
			var fsm = go.LocateMyFSM("Control");
			fsm.GetAction<ApplyMusicCue>("Pantheon", 0).musicCue.Value
				= fsm.GetAction<ApplyMusicCue>("Statue", 0).musicCue.Value;
			fsm.GetAction<TransitionToAudioSnapshot>("Pantheon", 1).snapshot.Value
				= fsm.GetAction<TransitionToAudioSnapshot>("Statue", 1).snapshot.Value;
		}),

		// GG_Radiance = NoOp
	];

	private protected override void Load() =>
		handles.ForEach(handle => handle.Enable());

	private protected override void Unload() =>
		handles.ForEach(handle => handle.Disable());


	private static void StopMusic() =>
		silent.Value.TransitionTo(0f);

	private static void ModifyGGMusicControl(GameObject go) => go
		.LocateMyFSM("gg_music_control")
		.ChangeTransition("Init", FsmEvent.Finished.Name, "Wait");

	private static void ModifyFSM(GameObject go) => go
		.LocateMyFSM("FSM")
		.ChangeTransition("Init", FsmEvent.Finished.Name, "Wait");

	private static void ModifyMageLord2(GameObject go) {
		var fsm = go.LocateMyFSM("Mage Lord 2");
		fsm.GetAction<ApplyMusicCue>("GG Music", 1).musicCue.Value
			= fsm.GetAction<ApplyMusicCue>("Music", 2).musicCue.Value;
	}

	private static void StopMusicAndModifyFSM(GameObject go) {
		StopMusic();
		ModifyFSM(go);
	}

	private static void StopMusicAndDestroyFSM(GameObject go) {
		StopMusic();
		UObject.Destroy(go.LocateMyFSM("FSM"));
	}
}
