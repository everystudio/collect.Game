using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using EveryStudioLibrary;
using NendUnityPlugin.AD;
using UnityEngine.Advertisements;

public class Startup : Singleton<Startup> {

	static public bool InitializeCheck = false;
	public bool CONFIG_UPDATE = false;
	public enum STEP
	{
		NONE			= 0,
		CHECK_CONFIG	,
		UPDATE_SCENARIO	,
		UPDATE_DOWNLOAD	,
		DATA_DOWNLOAD	,
		UPDATE_CHAPTER	,		// 基本１回のみ
		UPDATE_BOOK		,		// 基本１回のみ

		CHECK_UPDATE	,
		GOTO_GAME		,
		NETWORK_ERROR	,
		END				,
		MAX				,
	}
	public STEP m_eStep;
	public STEP m_eStepPre;
	public int m_iNetworkSerial;
	public List<SpreadSheetData> m_ssdSample;
	public CsvScript m_scriptData;
	public SetupWaiting m_setupWaiting;

	private FileDownload fd;
	public ButtonBase m_btnNetworkError;

	public override void Initialize ()
	{
		// ios対応；基本保存させない
		#if UNITY_IOS
		UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);
		#endif

		GoogleAnalytics.Instance.Log (DataManager.Instance.GA_STARTUP);


		base.Initialize ();
		InitializeCheck = true;
		m_eStep = STEP.CHECK_CONFIG;
		m_eStepPre = STEP.MAX;
		return;
	}

	// Update is called once per frame
	void Update () {
		bool bInit = false;

		if (m_eStepPre != m_eStep) {
			m_eStepPre  = m_eStep;
			bInit = true;
			//Debug.LogError (m_eStep);
		}

		switch (m_eStep) {
		case STEP.CHECK_CONFIG:
			if (bInit) {
				#if UNITY_ANDROID
				NendAdInterstitial.Instance.Load ("1f6b7ab7fb6f2e5ec95ee56c73e731e5b86cffee", "597666");
				#elif UNITY_IPHONE
				NendAdInterstitial.Instance.Load("fb8bc1393982950ccbd6c706b35954725b3666bc", "597676");
				#else
				#endif
				m_iNetworkSerial = CommonNetwork.Instance.RecieveSpreadSheet (DataManager.Instance.SPREAD_SHEET,DataManager.Instance.SPREAD_SHEET_CONFIG_SHEET);
			}

			if (CommonNetwork.Instance.IsConnected (m_iNetworkSerial)) {
				// 一度終了に向かうように設定
				m_eStep = STEP.GOTO_GAME;
				TNetworkData data = EveryStudioLibrary.CommonNetwork.Instance.GetData (m_iNetworkSerial);
				m_ssdSample = EveryStudioLibrary.CommonNetwork.Instance.ConvertSpreadSheetData (data.m_dictRecievedData);
				CsvConfig config_data = new CsvConfig ();
				config_data.Input (m_ssdSample);
				if (false == config_data.Read (CsvConfig.KEY_CONFIG_VERSION).Equals (DataManager.Instance.config.Read (CsvConfig.KEY_CONFIG_VERSION)) || CONFIG_UPDATE == true) {
					config_data.Save (CsvConfig.FILE_NAME);
					DataManager.Instance.config.Load (CsvConfig.FILE_NAME);
					m_eStep = STEP.CHECK_UPDATE;
				}

				if (Advertisement.isSupported) { // If runtime platform is supported...
					string gameId = "";
					#if UNITY_ANDROID
					gameId = config_data.Read( "unity_ads_gameid_android" );
					#elif UNITY_IOS
					gameId = config_data.Read( "unity_ads_gameid_ios" );
					#endif
					Advertisement.Initialize(gameId, true); // ...initialize.
				}
			} else if (CommonNetwork.Instance.IsError (m_iNetworkSerial ) ) {
				m_eStep = STEP.NETWORK_ERROR;
			} else {
			}
			break;

		case STEP.CHECK_UPDATE:
			if (false == DataManager.Instance.config.Read (FileDownloadManager.KEY_DOWNLOAD_VERSION).Equals (DataManager.Instance.kvs_data.Read (FileDownloadManager.KEY_DOWNLOAD_VERSION))) {
				m_eStep = STEP.UPDATE_DOWNLOAD;
			} else if (false == DataManager.Instance.config.Read (DataManager.Instance.KEY_SCENARIO_VERSION).Equals (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_SCENARIO_VERSION))) {
				m_eStep = STEP.UPDATE_SCENARIO;
			} else if (false == DataManager.Instance.config.Read (DataManager.Instance.KEY_CHAPTER_VERSION).Equals (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_CHAPTER_VERSION))) {
				m_eStep = STEP.UPDATE_CHAPTER;
			} else if (false == DataManager.Instance.config.Read (DataManager.Instance.KEY_BOOK_VERSION).Equals (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_BOOK_VERSION))) {
				m_eStep = STEP.UPDATE_BOOK;
/*			} else if (false == DataManager.Instance.config.Read (DataManager.Instance.KEY_IMAGE_LIST_VERSION).Equals (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_IMAGE_LIST_VERSION))) {
				m_eStep = STEP.CHECK_IMAGE_LIST;
			} else if (false == DataManager.Instance.config.Read (DataManager.Instance.KEY_VOICE_LIST_VERSION).Equals (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_VOICE_LIST_VERSION))) {
				m_eStep = STEP.CHECK_VOICE_LIST;
			} else if (false == DataManager.Instance.config.Read (DataManager.Instance.KEY_VOICESET_LIST_VERSION).Equals (DataManager.Instance.kvs_data.Read (DataManager.Instance.KEY_VOICESET_LIST_VERSION))) {
				m_eStep = STEP.CHECK_VOICESET_LIST;
*/			} else {
				m_eStep = STEP.GOTO_GAME;
			}
			break;
		case STEP.UPDATE_SCENARIO:
			if (bInit) {
				m_iNetworkSerial = CommonNetwork.Instance.RecieveSpreadSheet (
					DataManager.Instance.SPREAD_SHEET ,
					DataManager.Instance.config.Read ("scenario"));
			}
			if (CommonNetwork.Instance.IsConnected (m_iNetworkSerial)) {
				TNetworkData data = EveryStudioLibrary.CommonNetwork.Instance.GetData (m_iNetworkSerial);

				m_ssdSample = EveryStudioLibrary.CommonNetwork.Instance.ConvertSpreadSheetData (data.m_dictRecievedData);
				m_scriptData = new CsvScript ();
				Debug.Log (m_ssdSample);
				m_scriptData.Input (m_ssdSample);
				m_scriptData.Save (DataManager.Instance.FILENAME_SCENARIO);
				DataManager.Instance.kvs_data.WriteInt (DataManager.Instance.KEY_SCENARIO_VERSION, DataManager.Instance.config.ReadInt (DataManager.Instance.KEY_SCENARIO_VERSION));
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);

				m_eStep = STEP.CHECK_UPDATE;
			}
			break;
		case STEP.UPDATE_DOWNLOAD:
			if (bInit) {
				m_iNetworkSerial = CommonNetwork.Instance.RecieveSpreadSheet (
					DataManager.Instance.SPREAD_SHEET ,
					DataManager.Instance.config.Read ("download"));
			}
			if (CommonNetwork.Instance.IsConnected (m_iNetworkSerial)) {
				TNetworkData data = EveryStudioLibrary.CommonNetwork.Instance.GetData (m_iNetworkSerial);
				m_ssdSample = EveryStudioLibrary.CommonNetwork.Instance.ConvertSpreadSheetData (data.m_dictRecievedData);
				CsvDownload download_list = new CsvDownload();
				download_list.Input (m_ssdSample);
				download_list.Save (FileDownloadManager.FILENAME_DOWNLOAD_LIST);
				m_eStep = STEP.DATA_DOWNLOAD;
			}
			break;
		case STEP.DATA_DOWNLOAD:
			if (bInit) {
				CsvDownload download_list = new CsvDownload ();
				download_list.Load (FileDownloadManager.FILENAME_DOWNLOAD_LIST);
				Debug.Log (TimeManager.StrGetTime ());
				FileDownloadManager.Instance.Download (DataManager.Instance.config.ReadInt (FileDownloadManager.KEY_DOWNLOAD_VERSION), download_list.list);
			}
			
			int iTotal = 0;
			int iDownloaded = 0;
			if (FileDownloadManager.Instance.IsIdle (out iTotal, out iDownloaded)) {
				m_eStep = STEP.CHECK_UPDATE;
				DataManager.Instance.kvs_data.WriteInt (FileDownloadManager.KEY_DOWNLOAD_VERSION, DataManager.Instance.config.ReadInt (FileDownloadManager.KEY_DOWNLOAD_VERSION));
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
			}
			m_setupWaiting.SetBaseText (string.Format ("データダウンロード中({0}/{1})", iDownloaded, iTotal));
			break;
		case STEP.UPDATE_CHAPTER:
			if (bInit) {
				m_iNetworkSerial = CommonNetwork.Instance.RecieveSpreadSheet (
					DataManager.Instance.SPREAD_SHEET ,
					DataManager.Instance.config.Read ("chapter"));
			}
			if (CommonNetwork.Instance.IsConnected (m_iNetworkSerial)) {
				TNetworkData data = EveryStudioLibrary.CommonNetwork.Instance.GetData (m_iNetworkSerial);
				m_ssdSample = EveryStudioLibrary.CommonNetwork.Instance.ConvertSpreadSheetData (data.m_dictRecievedData);
				CsvChapter chapter_data = new CsvChapter ();
				chapter_data.Input (m_ssdSample);
				chapter_data.Save (CsvChapter.FILE_NAME);
				DataManager.Instance.kvs_data.WriteInt (DataManager.Instance.KEY_CHAPTER_VERSION, DataManager.Instance.config.ReadInt (DataManager.Instance.KEY_CHAPTER_VERSION));
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);
				DataManager.Instance.LoadChapter ();
				m_eStep = STEP.CHECK_UPDATE;
			}
			break;
		case STEP.UPDATE_BOOK:
			if (bInit) {
				m_iNetworkSerial = CommonNetwork.Instance.RecieveSpreadSheet (
					DataManager.Instance.SPREAD_SHEET ,
					DataManager.Instance.config.Read ("book"));
			}
			if (CommonNetwork.Instance.IsConnected (m_iNetworkSerial)) {
				TNetworkData data = EveryStudioLibrary.CommonNetwork.Instance.GetData (m_iNetworkSerial);
				m_ssdSample = EveryStudioLibrary.CommonNetwork.Instance.ConvertSpreadSheetData (data.m_dictRecievedData);
				CsvBook book_data = new CsvBook ();
				book_data.Input (m_ssdSample);
				book_data.Save (CsvBook.FILE_NAME);
				DataManager.Instance.kvs_data.WriteInt (DataManager.Instance.KEY_BOOK_VERSION, DataManager.Instance.config.ReadInt (DataManager.Instance.KEY_BOOK_VERSION));
				DataManager.Instance.kvs_data.Save (DataKvs.FILE_NAME);

				m_eStep = STEP.CHECK_UPDATE;
			}
			break;

		case STEP.GOTO_GAME:
			if (bInit) {
				SceneManager.LoadScene ("title");
			}
			break;
		case STEP.NETWORK_ERROR:
			if (bInit) {
				m_btnNetworkError.gameObject.SetActive (true);
				m_btnNetworkError.TriggerClear ();
			}
			if (m_btnNetworkError.ButtonPushed) {
				m_btnNetworkError.gameObject.SetActive (false);
				m_eStep = STEP.CHECK_CONFIG;
			}
			break;

		case STEP.MAX:
		default:
			break;
		
				}

	}
}
