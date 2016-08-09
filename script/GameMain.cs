using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using EveryStudioLibrary;
using UnityEngine.Advertisements;
using NendUnityPlugin.AD;

[System.Serializable]
public class GameData{
	public int script_id;// = DataManager.Instance.kvs_data.ReadInt ("script_id");
	public int script_index;// = DataManager.Instance.kvs_data.ReadInt ("script_index");
	public int target_num;// = DataManager.Instance.kvs_data.ReadInt ("target_num");
	public int target_max;// = DataManager.Instance.kvs_data.ReadInt ("target_max");

	public List<string> character = new List<string>();
	public string background;
	public string target;
	public string skit_type;

	public string bgm_name;
	public string bgm_path;
	public GameData(){
		character.Add ("");
		character.Add ("");
		character.Add ("");
	}
}

public class GameMain : Singleton<GameMain> {
	public enum  STEP
	{
		NONE				= 0,
		STARTUP				,
		INITIALIZE			,

		IDLE				,

		SCRIPT_START		,
		SCRIPT_CHECK		,
		CHAPTER_START		,
		SKIT_TYPE			,
		SKIT				,
		SET_SCRIPT_ID		,
		SET_SCRIPT_INDEX	,
		BLACK				,
		STILL				,
		BACKGROUND			,
		CHARACTER			,
		SELECT				,
		FLAG_SET			,
		SOUND_BGM			,
		SOUND_SE			,
		ENDING				,
		ENDING_WAIT			,
		TUTORIAL			,
		SHARE_EXPAND		,

		SUPPORT_DOUBLE		,
		SUPPORT_SPEEDUP		,

		BOOK				,
		SHARE				,
		SECRET				,
		CHAPTER				,
		README				,
		SAVE				,

		MAX					,
	};
	public STEP m_eStep;
	public STEP m_eStepPre;

	public float m_fTimer;
	public bool m_bStartupCheck = false;
	[SerializeField]
	private GameData m_gameData = new GameData();

	public NendAdBanner m_nendBanner;
	public NendAdBanner m_nendRectangle;
	#if UNITY_ANDROID
	public NendAdIcon m_nendIcon;
	#endif

	public DataManager.AD_TYPE m_eAdType;

	public UILabel m_lbDebug;
	public UILabel m_lbChapterName;

	public UISlider m_slGauge;

	public ButtonBase m_btnSupportDouble;
	public ButtonBase m_btnSupportSpeedup;

	[SerializeField]
	private UIGrid m_gridFooterIconRoot;
	public enum FOOTER_ICON{
		ALBUM		= 0,
		SHARE		,
		CHAPTER		,
		SAVELOAD	,
		MAX			,
	}
	public FooterIcon[] m_btnFooterIconArr = new FooterIcon[(int)FOOTER_ICON.MAX];

	public SkitRoot m_skitRoot;

	public int m_iScriptIndexMax;
	public int m_iTargetNext;
	public bool m_bSetScriptIndex;

	public int m_iNetworkSerial;
	public CsvScript m_scriptData;

	public UI2DSprite m_sprBackground;
	public List<CtrlCharacter> m_CharacterList;
	public GameObject m_goFrontRoot;
	public GameObject m_goSaveRoot;

	public PageBase m_pageActive;
	public BookMain m_bookMain;

	public WindowBase m_windowBase;
	public SelectMain m_selectMain;
	public StillMain m_stillMain;

	public GameObject m_objBlack;
	public GameObject m_objBlackBack;

	#region シェアしてねのチェック関連
	private bool m_bIsShareRequest;
	//private int m_iShareRequestInterval;
	//private bool m_bIsShareRequestEmpty;
	#endregion

	[SerializeField]
	private CtrlYesNo m_ctrlYesNo;

	public List<CsvScriptParam> m_scriptActiveList = new List<CsvScriptParam>();
	public SkitRoot.TYPE m_eSkitType;

	public void ResetScriptId( int _iScriptId ){
		m_gameData.script_id = _iScriptId;
		m_gameData.script_index = 0;
		m_gameData.target_num = 0;
		m_gameData.target_max = GetTargetMax (m_gameData.script_id);
		m_iScriptIndexMax = m_scriptData.GetMaxIndex (m_gameData.script_id);
		m_iTargetNext = GetNextTarget (m_gameData.script_index, m_iScriptIndexMax, m_gameData.target_max);
		return;
	}

	private void SetBackground ( string _strBackground ){
		m_sprBackground.sprite2D = SpriteManager.Instance.Load(_strBackground);
		return;
	}

	private void SetCharacter ( int _iIndex , string _strCharacter){

		m_gameData.character [_iIndex] = _strCharacter;
		m_CharacterList[_iIndex].Initialize (_strCharacter);
		return;
	}
	private void SetTarget( string _strTarget ){
		return;
	}

	static private void _writeGameData(CsvKvs _kvs , GameData _data ){
		_kvs.WriteInt (DataManager.Instance.KEY_SCRIPT_ID, _data.script_id);
		_kvs.WriteInt (DataManager.Instance.KEY_SCRIPT_INDEX, _data.script_index);
		_kvs.WriteInt (DataManager.Instance.KEY_TARGET_NUM, _data.target_num);
		_kvs.WriteInt (DataManager.Instance.KEY_TARGET_MAX, _data.target_max);
		_kvs.Write (DataManager.Instance.KEY_CHARACTER0 , _data.character[0]);
		_kvs.Write (DataManager.Instance.KEY_CHARACTER1 , _data.character[1]);
		_kvs.Write (DataManager.Instance.KEY_CHARACTER2 , _data.character[2]);
		_kvs.Write (DataManager.Instance.KEY_BACKGROUND , _data.background);
		_kvs.Write (DataManager.Instance.KEY_TARGET, _data.target);
		_kvs.Write (DataManager.Instance.KEY_SKIT_TYPE , _data.skit_type);
		_kvs.Write (DataManager.Instance.KEY_BGM_NAME , _data.bgm_name);
		_kvs.Write (DataManager.Instance.KEY_BGM_PATH , _data.bgm_path);
		return;
	}
	private void QuickSaveGameData( CsvKvs _kvs , GameData _data ){
		_writeGameData(_kvs , _data);
		_kvs.Save (DataKvs.FILE_NAME);
	}
	private void SaveGameData( CsvSave _save , int _iNo , GameData _data , string _strTime ){
		_writeGameData (_save, _data);
		_save.Save (_iNo , _strTime );
	}
	public void Save(int _iNo , string _strTime ){
		SaveGameData (DataManager.Instance.save, _iNo, m_gameData , _strTime );
	}

	static private void _readCore(CsvKvs _kvs , ref GameData _data ){
		_data.script_id = _kvs.ReadInt (DataManager.Instance.KEY_SCRIPT_ID);
		_data.script_index = _kvs.ReadInt (DataManager.Instance.KEY_SCRIPT_INDEX);
		_data.target_num = _kvs.ReadInt (DataManager.Instance.KEY_TARGET_NUM);
		_data.target_max = _kvs.ReadInt (DataManager.Instance.KEY_TARGET_MAX);

		_data.character[0] = _kvs.Read (DataManager.Instance.KEY_CHARACTER0);
		_data.character[1] = _kvs.Read (DataManager.Instance.KEY_CHARACTER1);
		_data.character[2] = _kvs.Read (DataManager.Instance.KEY_CHARACTER2);

		_data.background= _kvs.Read (DataManager.Instance.KEY_BACKGROUND);
		_data.target= _kvs.Read (DataManager.Instance.KEY_TARGET);
		_data.skit_type = _kvs.Read (DataManager.Instance.KEY_SKIT_TYPE);

		_data.bgm_name = _kvs.Read (DataManager.Instance.KEY_BGM_NAME);
		_data.bgm_path = _kvs.Read (DataManager.Instance.KEY_BGM_PATH);
	}

	private void _readGameData( CsvKvs _kvs , ref GameData _data ){

		_readCore (_kvs, ref _data);

		if (_data.script_id == 0) {
			_data.script_id = 1;
			_data.script_index = 0;
			_data.target_num = 0;
			//Debug.Log (string.Format ("story{0:D3}_target_max", _data.script_id));
			_data.target_max = GetTargetMax (_data.script_id);
			//_data.target_max = DataManager.Instance.config.ReadInt (string.Format ("story{0:D3}_target_max", _data.script_id));
			_data.character[0]  = "";
			_data.character[1]  = "";
			_data.character[2]  = "";
			_data.background = DataManager.Instance.config.Read (DataManager.Instance.KEY_BACKGROUND);
			_data.target = DataManager.Instance.TARGET_DEFAULT;
			_data.skit_type = "all";
			QuickSaveGameData (_kvs, _data);
		}
		for (int i = 0; i < _data.character.Count ; i++) {
			SetCharacter (i, _data.character[i]);
		}
		SetBackground (_data.background);
		SetTarget (_data.target);

		m_iScriptIndexMax = m_scriptData.GetMaxIndex (m_gameData.script_id);
		m_iTargetNext = GetNextTarget (m_gameData.script_index, m_iScriptIndexMax, m_gameData.target_max);

		return;
	}
	private void QuickLoadGameData( CsvKvs _kvs , ref GameData _data ){
		_readGameData(_kvs , ref _data);
		return;
	}
	private void LoadGameData( CsvSave _save , int _iNo , ref GameData _data , bool _bLoadOnly ){
		_save.Load (_iNo);
		_readGameData (_save, ref _data);
	}
	public void Load(int _iNo , bool _bLoadOnly ){
		LoadGameData (DataManager.Instance.save, _iNo, ref m_gameData , _bLoadOnly);
		return;
	}

	public static void LoadOnly( int _iNo ){
		GameData gamedata = new GameData ();
		DataManager.Instance.save.Load (_iNo);
		_readCore (DataManager.Instance.save, ref gamedata);
		_writeGameData (DataManager.Instance.kvs_data, gamedata);
		DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
		return;
	}

	private void SetChapterName( int _iScriptId ){
		int iChapterId = DataManager.Instance.csv_chapter.GetChapterId (_iScriptId);
		AddChapter (_iScriptId, iChapterId);
		m_lbChapterName.text = DataManager.Instance.csv_chapter.GetChapterName (_iScriptId);
		return;
	}

	public int GetTargetMax( int _iScriptId ){
		return DataManager.Instance.config.ReadInt (string.Format ("story{0:D3}_target_max", _iScriptId));
	}


	private int GetNextTarget (int _iScriptIndex ,int _iScriptIndexMax, int _iTargetMax ){
		if (_iTargetMax <= 0) {
			return 1;
		}
		float fRate = (float)(_iScriptIndex+1) / (float)_iScriptIndexMax;
		return (int)(_iTargetMax * fRate);
	}

	private SkitRoot.TYPE GetSkitType( string _strParam ){
		switch (_strParam) {
		case "window":
			return SkitRoot.TYPE.WINDOW;
		case "all":
			return SkitRoot.TYPE.ALL;
		case "stand":
			return SkitRoot.TYPE.STAND;
		default:
			break;
		}
		return SkitRoot.TYPE.NONE;
	}

	private float GetGauge( GameData _data){
		if( 0 < _data.target_max ){
			return (float)_data.target_num/(float)_data.target_max; 
		}
		return 0.0f;
	}

	private void AddChapter ( int _iScriptId , int _iChapterId){
		if (DataManager.Instance.kvs_data.HasKey (CsvChapter.GetChapterKey (_iChapterId)) == false ) {
			DataManager.Instance.kvs_data.WriteInt (CsvChapter.GetChapterKey (_iChapterId),  _iScriptId);
			DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
		}
	}


	private bool isResourcesCheck(){
		CsvScript csvscript =  new CsvScript ();
		if (csvscript.Load (DataManager.Instance.FILENAME_SCENARIO) == false) {
			return false;
		}

		return true;
	}

	// Use this for initialization
	public override void Initialize ()
	{
		m_ctrlYesNo.gameObject.SetActive (false);
		m_eAdType = DataManagerBase<DataManager>.AD_TYPE.NEND;
		base.Initialize ();
		//Debug.Log ("initialize GameMain");
		if (m_bStartupCheck == true || isResourcesCheck() == false) {
			m_eStep = STEP.STARTUP;
			m_bStartupCheck = false;
		} else {
			m_eStep = STEP.INITIALIZE;
		}

		for (int i = 0; i < (int)FOOTER_ICON.MAX; i++) {

			string strIconBack  = DataManager.Instance.config.Read (string.Format ("footer_back{0}", i+1));
			string strIconFront= DataManager.Instance.config.Read (string.Format ("footer_front{0}", i+1));
			//Debug.LogError (strIconBack);
			FooterIcon footer_icon = PrefabManager.Instance.MakeScript<FooterIcon> ("prefab/FooterIcon", m_gridFooterIconRoot.gameObject);
			footer_icon.Initialize (strIconBack, strIconFront);
			m_btnFooterIconArr [i] = footer_icon;
		}
		m_gridFooterIconRoot.enabled = true;

		m_eStepPre = STEP.MAX;
		m_eSkitType = SkitRoot.TYPE.ALL;
		m_objBlackBack.SetActive (false);

		if (ManagerTarget.Instance.TotalNum () <= DataManager.Instance.GetTargetLimit ()) {
			m_bIsShareRequest = true;
			//m_iShareRequestInterval = 0;
			//m_bIsShareRequestEmpty = false;
		} else {
			m_bIsShareRequest = false;
		}

		return;
	}
	
	// Update is called once per frame
	void Update () {
		bool bInit = false;
		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
			//Debug.Log (m_eStep);
		}
		int adinit = 0;
		if (Advertisement.IsReady ("rewardedVideoZone")) {
			adinit += 1;
		}
		if (Advertisement.IsReady () ){
			adinit += 10;
		}
		m_lbDebug.text = adinit.ToString ();
		m_slGauge.value = GetGauge(m_gameData);

		switch (m_eStep) {
		case STEP.STARTUP:
			if (bInit) {
				SceneManager.LoadScene ("startup");
			}
			break;
		case STEP.INITIALIZE:
			if (bInit) {
				if (m_scriptData == null) {
					m_scriptData = new CsvScript ();
					m_scriptData.Load (DataManager.Instance.FILENAME_SCENARIO);
				}
				QuickLoadGameData (DataManager.Instance.kvs_data, ref m_gameData);
				if (m_gameData.bgm_name.Equals ("") == false && m_gameData.bgm_name.Equals (CsvKvs.READ_ERROR_STRING) == false) {
					SoundManager.Instance.PlayBGM (m_gameData.bgm_name, m_gameData.bgm_path);
				}
			}
			m_eStep = STEP.IDLE;
			if (m_gameData.script_id == 0) {
				m_eStep = STEP.SCRIPT_START;
			} else {
				SetChapterName (m_gameData.script_id);
				m_iScriptIndexMax = m_scriptData.GetMaxIndex (m_gameData.script_id);
				m_iTargetNext = GetNextTarget (m_gameData.script_index, m_iScriptIndexMax, m_gameData.target_max);
			}
			break;
		case STEP.IDLE:
			if (bInit) {
				m_objBlackBack.SetActive (false);
				AdManager.Instance.ShowIcon (m_eAdType, true);
				AdManager.Instance.ShowBanner (m_eAdType, true);

				// ０は自動進行する
				if (m_gameData.script_index == 0) {
					m_gameData.target_num = m_iTargetNext;
				}

				foreach (FooterIcon footer_icon in m_btnFooterIconArr) {
					footer_icon.TriggerClear ();
				}
			}
			if (m_bIsShareRequest) {
				if( ManagerTarget.Instance.GetActiveNum() == 0 ){
					m_eStep = STEP.SHARE_EXPAND;
				}
			}

			int iGetTarget = ManagerTarget.Instance.GetTarget (true);
			if (0 < iGetTarget) {
				if (DataManager.Instance.kvs_data.HasKey (DataManager.Instance.KEY_TARGET_DOUBLE_END)) {
					//Debug.Log (TimeManager.Instance.GetDiffNow (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_TARGET_DOUBLE_END)).TotalSeconds);
					if (0 < TimeManager.Instance.GetDiffNow (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_TARGET_DOUBLE_END)).TotalSeconds) {
						iGetTarget *= 2;
					}
				}
				m_gameData.target_num+= iGetTarget;
				QuickSaveGameData (DataManager.Instance.kvs_data, m_gameData);
			}
			if (m_iTargetNext <= m_gameData.target_num) {
				m_eStep = STEP.SCRIPT_START;
			}
			if (m_btnSupportDouble.ButtonPushed) {
				m_btnSupportDouble.TriggerClear ();
				m_windowBase = (WindowBase)MakeComponent<TargetDouble> (m_goFrontRoot);
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_BUTTON);
				m_eStep = STEP.SUPPORT_DOUBLE;
			} else if (m_btnSupportSpeedup.ButtonPushed) {
				m_btnSupportSpeedup.TriggerClear ();
				m_windowBase = (WindowBase)MakeComponent<TargetSpeedup> (m_goFrontRoot);
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_BUTTON);
				m_eStep = STEP.SUPPORT_SPEEDUP;
			} else if (m_btnFooterIconArr[(int)FOOTER_ICON.ALBUM].ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_BUTTON);
				m_eStep = STEP.BOOK;
			} else if (m_btnFooterIconArr[(int)FOOTER_ICON.SHARE].ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_BUTTON);
				m_eStep = STEP.SHARE;
			} else if (m_btnFooterIconArr[(int)FOOTER_ICON.CHAPTER].ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_BUTTON);
				m_eStep = STEP.CHAPTER;
			} else if (m_btnFooterIconArr[(int)FOOTER_ICON.SAVELOAD].ButtonPushed) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_BUTTON);
				m_eStep = STEP.SAVE;
			}/* else if(!DataManager.Instance.data_kvs.HasKey("tutorial_end")) {
				m_eStep = STEP.TUTORIAL;
			}*/
			else {
			}
			break;
		case STEP.SCRIPT_START:
			if (bInit) {
				GoogleAnalytics.Instance.LogScriptStart (m_gameData.script_id,m_gameData.script_index);

				m_objBlackBack.SetActive (true);

				m_bSetScriptIndex = false;
				m_scriptActiveList.Clear ();
				foreach (CsvScriptParam param in m_scriptData.list) {
					if (param.id == m_gameData.script_id && param.index == m_gameData.script_index) {
						m_scriptActiveList.Add (param);
					}
				}
			}
			m_eStep = STEP.SCRIPT_CHECK;
			break;
		case STEP.SCRIPT_CHECK:
			if (0 < m_scriptActiveList.Count) {
				//Debug.Log (m_scriptActiveList [0].command);
				switch (m_scriptActiveList [0].command) {
				case "chapter":
					m_eStep = STEP.CHAPTER_START;
					break;

				case "mode":
					m_eStep = STEP.SKIT_TYPE;
					break;
				case "name":
				case "text":
					m_eStep = STEP.SKIT;
					break;
				case "levelup":
				case "setscriptid":
					m_eStep = STEP.SET_SCRIPT_ID;
					break;
				case "setindex":
					m_eStep = STEP.SET_SCRIPT_INDEX;
					break;
				case "black":
					m_eStep = STEP.BLACK;
					break;
				case "still":
					m_eStep = STEP.STILL;
					break;
				case "background":
					m_eStep = STEP.BACKGROUND;
					break;
				case "character":
					m_eStep = STEP.CHARACTER;
					break;
				case "select":
					m_eStep = STEP.SELECT;
					break;
				case "flag_set":
					m_eStep = STEP.FLAG_SET;
					break;
				case "bgm":
					m_eStep = STEP.SOUND_BGM;
					break;
				case "se":
					m_eStep = STEP.SOUND_SE;
					break;
				case "ending":
					m_eStep = STEP.ENDING;
					break;
				default:
					m_scriptActiveList.RemoveAt (0);
					break;
				}
			} else {
				m_eStep = STEP.IDLE;
				// セットされてない場合のみインクリメント（されない方が多いです）
				if (m_bSetScriptIndex == false ) {
					m_gameData.script_index += 1;
				}
				m_iTargetNext = GetNextTarget (m_gameData.script_index, m_iScriptIndexMax, m_gameData.target_max);
				QuickSaveGameData (DataManager.Instance.kvs_data, m_gameData);
			}
			break;

		case STEP.CHAPTER_START:
			int iChapterId = int.Parse (m_scriptActiveList [0].param);
			AddChapter ( m_scriptActiveList [0].id , iChapterId); 
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;

		case STEP.SKIT_TYPE:
			m_gameData.skit_type = m_scriptActiveList [0].param;
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;

		case STEP.SKIT:
			if (bInit) {
				List<CsvScriptParam> list = new List<CsvScriptParam> ();

				while (m_scriptActiveList [0].command == "text" ||
				       m_scriptActiveList [0].command == "stand" ||
				       m_scriptActiveList [0].command == "name") {


					list.Add (m_scriptActiveList [0]);
					/*
					if (m_scriptActiveList [0].command == "text") {
						string strMessage = "";
						if (m_scriptActiveList [0].option1.Equals ("") == false) {
							strName = m_scriptActiveList [0].option1;
						}
						if (strName.Equals ("") == false) {
							strMessage = string.Format ("{0}\n{1}", strName, m_scriptActiveList [0].param);
							strName = "";

						} else {
							strMessage = m_scriptActiveList [0].param;
						}
						list.Add (strMessage);
					} else if (m_scriptActiveList [0].command.Equals ("name")) {
						strName = m_scriptActiveList [0].param;
					} else {
					}
					*/
					m_scriptActiveList.RemoveAt (0);
					if (m_scriptActiveList.Count == 0) {
						break;
					}
				}
				m_eSkitType = GetSkitType (m_gameData.skit_type);//  (SkitRoot.TYPE)int.Parse (m_scriptActiveList [0].param);
				m_skitRoot.SkitStart (list , m_eSkitType);
			}
			if (m_skitRoot.EndSkit()) {
				m_eStep = STEP.SCRIPT_CHECK;
			}
			break;
		case STEP.SET_SCRIPT_ID:
			Debug.LogError (STEP.SET_SCRIPT_ID);
			int iNextScriptId = -1;
			if (m_scriptActiveList [0].param.Contains ("select")) {
				if (DataManager.Instance.kvs_data.HasKey (SelectMain.GetSelectKey (m_scriptActiveList [0].option1))) {
					foreach (CsvKvsParam param in DataManager.Instance.kvs_data.list) {
						Debug.LogError (string.Format ("{0}:{1}", param.key, param.value));
					}
					Debug.LogError (SelectMain.GetSelectKey (m_scriptActiveList [0].option1));
					//iNextScriptId = DataManager.Instance.kvs_data.ReadInt (SelectMain.GetSelectKey (m_scriptActiveList [0].option1));

					iNextScriptId = DataManager.Instance.kvs_data.ReadInt( (SelectMain.GetSelectKey (m_scriptActiveList [0].option1)));
					Debug.LogError ("a");
				} else {
					//iNextScriptId = DataManager.Instance.kvs_data.ReadInt (SelectMain.GetSelectKey (m_scriptActiveList [0].option2));


					Debug.LogError (m_scriptActiveList [0].serial);
					Debug.LogError (m_scriptActiveList [0].param);
					Debug.LogError (m_scriptActiveList [0].option1);
					Debug.LogError (m_scriptActiveList [0].option2);

					iNextScriptId = int.Parse (m_scriptActiveList [0].option2);
					Debug.LogError ("b");
				}
			} else {
				iNextScriptId = int.Parse (m_scriptActiveList [0].param);
				Debug.LogError ("c");
			}
			Debug.LogError (iNextScriptId);
			if (m_gameData.script_id != iNextScriptId) {
				m_bSetScriptIndex = true;
				m_gameData.script_id = iNextScriptId;
				m_gameData.script_index = 0;
				m_gameData.target_num = 0;
				m_gameData.target_max = GetTargetMax (m_gameData.script_id);
				m_iScriptIndexMax = m_scriptData.GetMaxIndex (m_gameData.script_id);
				m_iTargetNext = GetNextTarget (m_gameData.script_index, m_iScriptIndexMax, m_gameData.target_max);
				SetChapterName (m_gameData.script_id);
			}
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;
		case STEP.SET_SCRIPT_INDEX:
			m_bSetScriptIndex = true;
			int iSetScriptIndex = 0;
			if (m_scriptActiveList [0].param.Contains ("select")) {
				iSetScriptIndex = DataManager.Instance.kvs_data.ReadInt (SelectMain.GetSelectKey (m_scriptActiveList [0].option1));
			} else {
				iSetScriptIndex = int.Parse (m_scriptActiveList [0].param);
			}
			m_gameData.script_index = iSetScriptIndex;
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;
		case STEP.BLACK:
			if (bInit) {
				float fTime = float.Parse (m_scriptActiveList [0].option1);
				if (fTime <= 0.1f) {
					fTime = 0.1f;
				}
				TweenAlpha ta = TweenAlphaAll (m_objBlack, fTime, float.Parse (m_scriptActiveList [0].option2));
				EventDelegate.Set (ta.onFinished, EndTween);
			}
			if (m_bEndTween) {
				m_scriptActiveList.RemoveAt (0);
				m_eStep = STEP.SCRIPT_CHECK;
			}
			break;
		case STEP.STILL:
			if (bInit) {
				m_stillMain = PrefabManager.Instance.MakeScript<StillMain> ("prefab/StillMain", m_goFrontRoot);
				m_stillMain.Initialize (m_scriptActiveList [0].param);
			}
			if (m_stillMain.IsEnd ()) {
				Release (m_stillMain.gameObject);
				m_scriptActiveList.RemoveAt (0);
				m_eStep = STEP.SCRIPT_CHECK;
			}
			break;
		case STEP.BACKGROUND:
			m_gameData.background = m_scriptActiveList [0].param;
			SetBackground (m_gameData.background);
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;
		case STEP.CHARACTER:
			int iSelectCharacterIndex = int.Parse (m_scriptActiveList [0].option1);
			string strChara = m_scriptActiveList [0].param;
			SetCharacter (iSelectCharacterIndex , strChara);
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;
		case STEP.SELECT:
			if (bInit) {
				m_selectMain = PrefabManager.Instance.MakeScript<SelectMain> ("prefab/SelectMain", m_goFrontRoot);
				m_selectMain.Initialize (ref m_scriptActiveList);
			}
			if (m_selectMain.IsEnd ()) {
				Release (m_selectMain.gameObject);
				m_eStep = STEP.SCRIPT_CHECK;
			}
			break;
		case STEP.FLAG_SET:
			bool bOk = true;
			Debug.LogError ("flag_set");
			if (!m_scriptActiveList [0].option2.Equals ("")) {
				string[] checkArr = m_scriptActiveList [0].option2.Split ('-');
				foreach (string strCheck in checkArr) {
					if (false == DataManager.Instance.data_kvs.HasKey (strCheck)) {
						Debug.LogError (strCheck);
						bOk = false;
					}
				}
				string strKey = SelectMain.GetSelectKey (m_scriptActiveList [0].option1);
				if (DataManager.Instance.kvs_data.HasKey (strKey)) {
					iSetScriptIndex = DataManager.Instance.kvs_data.ReadInt (strKey);
				}
			}
			if (bOk) {
				Debug.LogError (m_scriptActiveList [0].option1);
				Debug.LogError (m_scriptActiveList [0].param);
				DataManager.Instance.data_kvs.Write (m_scriptActiveList [0].option1, m_scriptActiveList [0].param);
			} else {
				Debug.LogError ("no insert");
			}
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;
		case STEP.SOUND_BGM:
			if (bInit) {
				m_gameData.bgm_name = m_scriptActiveList [0].param;
				m_gameData.bgm_path= m_scriptActiveList [0].option1;
				if (m_gameData.bgm_name.Equals ("") == false) {
					SoundManager.Instance.PlayBGM (m_gameData.bgm_name, m_gameData.bgm_path);
				} else {
					SoundManager.Instance.StopBGM ();
				}
			}
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;
		case STEP.SOUND_SE:
			if (bInit) {
				SoundManager.Instance.PlaySE (m_scriptActiveList [0].param,m_scriptActiveList [0].option1);
			}
			m_scriptActiveList.RemoveAt (0);
			m_eStep = STEP.SCRIPT_CHECK;
			break;

		case STEP.ENDING:
			if (bInit) {
				TweenAlpha ta = TweenAlphaAll (m_objBlack, 3.0f, 1.0f );
				EventDelegate.Set (ta.onFinished, EndTween);
			}
			if (m_bEndTween) {
				m_eStep = STEP.ENDING_WAIT;
			}
			break;

		case STEP.ENDING_WAIT:
			if (bInit) {
				m_fTimer = 0.0f;
			}
			m_fTimer += Time.deltaTime;
			if (3.0f < m_fTimer) {
				DataManager.Instance.kvs_data.WriteInt (DataManager.Instance.KEY_SCRIPT_ID, 0);
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
				SceneManager.LoadScene ("title");
			}
			break;

		case STEP.TUTORIAL:
			/*
			if (bInit) {
				List<string> list = new List<string > ();
				list.Add ("このゲームは画面内にあるターゲットを集めることで左上の経験値がたまります");
				list.Add ("経験値をためていくことでストーリーが進んでいきます");
				list.Add ("あつめたターゲットは時間が経過すると再び現れます");
				list.Add ("出現スピード2倍などを有効に使いストーリーを進めてください");
				m_skitRoot.SkitStart (list , SkitRoot.TYPE.WINDOW);
			}
			if (m_skitRoot.EndSkit()) {
				m_eStep = STEP.IDLE;
				DataManager.Instance.data_kvs.Write ("tutorial_end" , "end");
			}
			*/
			m_eStep = STEP.IDLE;
			break;

		case STEP.SHARE_EXPAND:
			if (bInit) {
				m_ctrlYesNo.gameObject.SetActive (true);
				m_ctrlYesNo.Init ("今、ツイートでシェアすると、出現するターゲットの最大数が５つ増えます！\n\nツイートしますか？");
				m_bIsShareRequest = false;
			}
			if (m_ctrlYesNo.IsYes ()) {
				// WebブラウザのTwitter投稿画面を開く
				string strMessage = "【泣けるアプリ】切なくて泣ける、青春ラブストーリー！\n\n思いを打ち明けれない青年「鈴木」\nその彼に厚意を寄せる「野田」\n\n二人の行く末はどうなってしまうのか。 #シークレットアップル";
				#if UNITY_ANDROID
				if( DataManager.Instance.config.HasKey( DataManager.Instance.KEY_SHARE_MESSAGE_ANDROID )){
					strMessage = DataManager.Instance.config.Read( DataManager.Instance.KEY_SHARE_MESSAGE_ANDROID);
				}
				#elif UNITY_IPHONE
				if( DataManager.Instance.config.HasKey( DataManager.Instance.KEY_SHARE_MESSAGE_IPHONE )){
					strMessage = DataManager.Instance.config.Read( DataManager.Instance.KEY_SHARE_MESSAGE_IPHONE);
				}
				#endif
				int iAddNum = DataManager.Instance.ShareBonusAddTargetNum();
				DataManager.Instance.data_kvs.WriteInt (DataManager.Instance.KEY_ADD_TARGET , iAddNum);
				ManagerTarget.Instance.AddTargetTail (iAddNum);

				Application.OpenURL("http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(strMessage));
				m_ctrlYesNo.gameObject.SetActive (false);
				m_eStep = STEP.IDLE;

			} else if (m_ctrlYesNo.IsNo ()) {
				m_ctrlYesNo.gameObject.SetActive (false);
				m_eStep = STEP.IDLE;
			} else {
			}
			break;

		case STEP.SUPPORT_DOUBLE:
		case STEP.SUPPORT_SPEEDUP:
			if (bInit) {
				m_windowBase.WindowStart ();
			}
			if (m_windowBase.IsEnd ()) {
				Destroy (m_windowBase.gameObject);
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.BOOK:
			if (bInit) {
				GoogleAnalytics.Instance.Log (DataManager.Instance.GA_PAGE_BOOK);
				AdManager.Instance.ShowIcon( m_eAdType, false);
				m_bookMain.PageStart ();
			}
			if (m_bookMain.IsEnd ()) {
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.SHARE:
			if (bInit) {
				GoogleAnalytics.Instance.Log (DataManager.Instance.GA_PAGE_SHARE);
				AdManager.Instance.ShowIcon( m_eAdType, false);
				//AdManager.Instance.ShowBanner( m_eAdType, false);
				m_pageActive = PrefabManager.Instance.MakeObject ("prefab/ShareMain", m_goFrontRoot).GetComponent<PageBase> ();
				m_pageActive.PageStart ();
			}
			if (m_pageActive.IsEnd ()) {
				Release (m_pageActive.gameObject);
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.SECRET:
		case STEP.CHAPTER:
			if (bInit) {
				GoogleAnalytics.Instance.Log (DataManager.Instance.GA_PAGE_SHARE);
				AdManager.Instance.ShowIcon( m_eAdType, false);
				//AdManager.Instance.ShowBanner( m_eAdType, false);
				m_pageActive = PrefabManager.Instance.MakeObject ("prefab/ChapterMain", m_goFrontRoot).GetComponent<PageBase> ();
				m_pageActive.PageStart ();
			}
			if (m_pageActive.IsEnd ()) {
				Release (m_pageActive.gameObject);
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.README:
		case STEP.SAVE:
			if (bInit) {
				GoogleAnalytics.Instance.Log (DataManager.Instance.GA_PAGE_SAVELOAD);
				AdManager.Instance.ShowIcon( m_eAdType, false);
				//AdManager.Instance.ShowBanner( m_eAdType, false);
				m_pageActive = PrefabManager.Instance.MakeObject ("prefab/SaveMain", m_goSaveRoot).GetComponent<PageBase> ();
				m_pageActive.PageStart ();
			}
			if (m_pageActive.IsEnd ()) {
				Release (m_pageActive.gameObject);
				SoundHolder.Instance.Call (DataManager.Instance.SOUND_NAME_CANCEL);
				m_eStep = STEP.IDLE;
			}
			break;
		case STEP.MAX:
		default:
			break;
		}
	}
}












