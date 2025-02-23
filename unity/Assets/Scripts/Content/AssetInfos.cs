﻿
/// genereated from <see cref="Osakana4242.Lib.AssetServices.AssetConfig" />.

using System.Collections.Generic;
using Osakana4242.Lib.AssetServices;

namespace Osakana4242.Content {
	public static class AssetInfos {
		static readonly Dictionary<string, AssetInfo> dict_g = new Dictionary<string, AssetInfo>();

		static AssetInfo Add(AssetInfo info) {
			dict_g.Add(info.fileName, info);
			return info;
		}

		public static AssetInfo Get(string name) => dict_g[name];

		public static readonly AssetInfo BLT_01_PREFAB = Add(new ("Assets/Resources/mdl/blt_01.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo BLT_02_PREFAB = Add(new ("Assets/Resources/mdl/blt_02.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo BLT_1_ASSET = Add(new ("Assets/Resources/chara/blt_1.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo BLT_2_ASSET = Add(new ("Assets/Resources/chara/blt_2.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo CONFIG_ASSET = Add(new ("Assets/Resources/config.asset", typeof(Osakana4242.Content.Inners.Config)));
		public static readonly AssetInfo EFT_BLAST_01_PREFAB = Add(new ("Assets/Resources/eft/eft_blast_01.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo ENEMY_1_ASSET = Add(new ("Assets/Resources/chara/enemy_1.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo ENEMY_2_ASSET = Add(new ("Assets/Resources/chara/enemy_2.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo ENEMY_3_ASSET = Add(new ("Assets/Resources/chara/enemy_3.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo ENEMY_4_ASSET = Add(new ("Assets/Resources/chara/enemy_4.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo ENEMY_6_ASSET = Add(new ("Assets/Resources/chara/enemy_6.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo ENEMY_BOSS_01_ASSET = Add(new ("Assets/Resources/chara/enemy_boss_01.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo ENM_01_PREFAB = Add(new ("Assets/Resources/mdl/enm_01.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo ENM_02_PREFAB = Add(new ("Assets/Resources/mdl/enm_02.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo ENM_BOSS_01_PREFAB = Add(new ("Assets/Resources/mdl/enm_boss_01.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo EXIT_BTN_PREFAB = Add(new ("Assets/Resources/exit_btn.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo GAME_OVER_PREFAB = Add(new ("Assets/Resources/game_over.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo PLAYER_ASSET = Add(new ("Assets/Resources/chara/player.asset", typeof(Osakana4242.Content.Inners.CharaInfo)));
		public static readonly AssetInfo PLY_01_PREFAB = Add(new ("Assets/Resources/mdl/ply_01.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo POINTER_01_PREFAB = Add(new ("Assets/Resources/mdl/pointer_01.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo STORM_R_01_01_ASSET = Add(new ("Assets/Resources/storm/storm_r_01_01.asset", typeof(Osakana4242.Content.Inners.StormData)));
		public static readonly AssetInfo STORM_R_02_01_ASSET = Add(new ("Assets/Resources/storm/storm_r_02_01.asset", typeof(Osakana4242.Content.Inners.StormData)));
		public static readonly AssetInfo STORM_R_02_02_ASSET = Add(new ("Assets/Resources/storm/storm_r_02_02.asset", typeof(Osakana4242.Content.Inners.StormData)));
		public static readonly AssetInfo STORM_R_EMPTY_ASSET = Add(new ("Assets/Resources/storm/storm_r_empty.asset", typeof(Osakana4242.Content.Inners.StormData)));
		public static readonly AssetInfo TITLE_PREFAB = Add(new ("Assets/Resources/title/title.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo TITLE_TOUCH_TO_START_PREFAB = Add(new ("Assets/Resources/title/title_touch_to_start.prefab", typeof(UnityEngine.GameObject)));
		public static readonly AssetInfo TITLE_TOUCH_TO_START_HIDDEN_ANIM = Add(new ("Assets/Resources/title/title_touch_to_start_hidden.anim", typeof(UnityEngine.AnimationClip)));
		public static readonly AssetInfo TITLE_TOUCH_TO_START_IDLE_ANIM = Add(new ("Assets/Resources/title/title_touch_to_start_idle.anim", typeof(UnityEngine.AnimationClip)));
		public static readonly AssetInfo WAVE_01_ASSET = Add(new ("Assets/Resources/wave/wave_01.asset", typeof(Osakana4242.Content.Inners.WaveData)));
		public static readonly AssetInfo WAVE_02_ASSET = Add(new ("Assets/Resources/wave/wave_02.asset", typeof(Osakana4242.Content.Inners.WaveData)));
		public static readonly AssetInfo WAVE_03_ASSET = Add(new ("Assets/Resources/wave/wave_03.asset", typeof(Osakana4242.Content.Inners.WaveData)));
		public static readonly AssetInfo WAVE_BOSS_01_ASSET = Add(new ("Assets/Resources/wave/wave_boss_01.asset", typeof(Osakana4242.Content.Inners.WaveData)));
		public static readonly AssetInfo WAVE_NONE_ASSET = Add(new ("Assets/Resources/wave/wave_none.asset", typeof(Osakana4242.Content.Inners.WaveData)));


	}
}
	