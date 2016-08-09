using UnityEngine;
using System.Collections;

public class DataManager : DataManagerBase<DataManager> {


	#if UNITY_ANDROID
	public readonly string APIKEY_GAMESTART = "b21aa6bf51bd0d8b3ff419e06a86d56fcc612909";
	public readonly string SPOTID_GAMESTART = "637720";
	public readonly string APIKEY_MENU = "d5d77477d05514852f260b95f7ef429b07444c3e";
	public readonly string SPOTID_MENU = "637721";
	#elif UNITY_IPHONE
	public readonly string APIKEY_GAMESTART = "13aa0f0bfc70a5722ee6602acc756e232f7fc927";
	public readonly string SPOTID_GAMESTART = "637714";
	public readonly string APIKEY_MENU = "2324679d079bb875a4880128f634be3d7534ea2e";
	public readonly string SPOTID_MENU = "637715";
	#else
	#endif

	public readonly string SPREAD_SHEET = "1otKwiwW0U1K8RfeKaD2UbekCw9tNljENQvtYcnGqJys";
	public readonly string SPREAD_SHEET_CONFIG_SHEET = "oqo0ytr";

	public readonly string FILENAME_SCENARIO = "ScenarioFile";
	public readonly string FILENAME_TARGET_DATA = "TargetData";

	public readonly string KEY_SCENARIO_VERSION = "scenario_version";
	public readonly string KEY_CHAPTER_VERSION = "chapter_version";
	public readonly string KEY_BOOK_VERSION = "book_version";

	public readonly string KEY_SCRIPT_ID= "script_id";
	public readonly string KEY_SCRIPT_INDEX= "script_index";
	public readonly string KEY_TARGET_NUM = "target_num";
	public readonly string KEY_TARGET_MAX = "target_max";
	public readonly string KEY_SKIT_TYPE = "skit_type";
	public readonly string KEY_BGM_NAME = "bgm_name";
	public readonly string KEY_BGM_PATH = "bgm_path";

	public readonly string KEY_CHARACTER0 = "character0";
	public readonly string KEY_CHARACTER1 = "character1";
	public readonly string KEY_CHARACTER2 = "character2";
	public readonly string KEY_CHARACTER3 = "character3";
	public readonly string KEY_BACKGROUND = "background";
	public readonly string KEY_TARGET = "target";

	public readonly string CHARACTER_DEFAULT = "texture/chara/totoki_airi_";
	public readonly string BACKGROUND_DEFAULT = "texture/background/bg_main";
	public readonly string TARGET_DEFAULT = "texture/target/default";

	public readonly string KEY_TARGET_DOUBLE_END = "target_double_end";
	public readonly string KEY_TARGET_SPEEDUP_END = "target_speedup_end";

	public readonly string KEY_SHARE_MESSAGE_ANDROID = "key_share_message_android";
	public readonly string KEY_SHARE_IMAGE_ANDROID = "key_share_image_android";
	public readonly string KEY_SHARE_MESSAGE_IPHONE= "key_share_message_iphone";
	public readonly string KEY_SHARE_IMAGE_IPHONE = "key_share_image_iphone";

	public readonly string GA_STARTUP = "start_up";
	public readonly string GA_GAMESTART_START = "gamestart_start";
	public readonly string GA_GAMESTART_CONTINUE = "gamestart_continue";
	public readonly string GA_GAMESTART_SAVELOAD = "gamestart_saveload";
	public readonly string GA_PAGE_BOOK = "page_book";
	public readonly string GA_PAGE_SHARE = "page_share";
	public readonly string GA_PAGE_CHAPTER = "page_chapter";
	public readonly string GA_PAGE_SAVELOAD = "page_saveload";
	public readonly string GA_SAVE= "play_save";
	public readonly string GA_LOAD= "play_load";
	public readonly string GA_SCRIPT_START_FORMAT = "script_start_{0:D4}_{1:D4}";

	public readonly string KEY_ADD_TARGET = "key_add_target";
	public readonly string KEY_TARGET_DEFAULT_NUM = "target_limit";
	public readonly string KEY_TARGET_LIMIT = "key_target_limit";
	public readonly string KEY_SHAREBONUS_ADD_TARGET_NUM = "key_sharebonus_add_target_num";

	private CsvChapter m_csvChapter = new CsvChapter();
	public CsvChapter csv_chapter{
		get{ return m_csvChapter; }
	}

	public override void Initialize ()
	{
		base.Initialize ();
		kvs_data.Load (DataKvs.FILE_NAME);

		config.Load (CsvConfig.FILE_NAME);

		LoadChapter ();

		m_eActiveAdType = AD_TYPE.NEND;

		return;
	}

	public AD_TYPE m_eActiveAdType;

	public void LoadChapter(){
		m_csvChapter.Load (CsvChapter.FILE_NAME);
	}



	public int ShareBonusAddTargetNum(){
		if( config.HasKey( KEY_SHAREBONUS_ADD_TARGET_NUM )){
			return config.ReadInt( KEY_SHAREBONUS_ADD_TARGET_NUM );
		}
		return 5;
	}

	public int GetTargetLimit(){
		if( config.HasKey( KEY_TARGET_LIMIT )){
			return config.ReadInt( KEY_TARGET_LIMIT );
		}
		return 30;
	}

	#region サウンド名
	public readonly string SOUND_NAME_POPUP = "popup_01";
	public readonly string SOUND_NAME_DECIDE= "decide_01";
	public readonly string SOUND_NAME_CANCEL = "cancel_02";
	public readonly string SOUND_NAME_CURSOR = "cursor_01";
	public readonly string SOUND_NAME_TARGETGET = "targetget_01";
	public readonly string SOUND_NAME_BUTTON = "button_01";
	#endregion


}
