using UnityEngine;
using System.Collections;

public class SetupWaiting : MonoBehaviourEx {

	public string m_strBase;
	public UILabel m_lbTest;

	public float m_fTimer;
	public float INTERVAL;
	public int m_iCount;
	public int m_iCountMax;

	public void SetBaseText(string _strText){
		m_strBase = _strText;
	}

	void Start(){
		if (m_strBase.Equals ("")) {
			m_strBase = "データ準備中";
		}
		m_lbTest.text = m_strBase;
		m_fTimer = 0.0f;
		INTERVAL = 1.0f;
		m_iCountMax = 4;
		m_iCount = 0;
	}

	void Update () {

		m_fTimer += Time.deltaTime;
		if (INTERVAL <= m_fTimer) {
			m_fTimer -= INTERVAL;
			m_iCount += 1;
			m_iCount %= m_iCountMax;
			string strText = m_strBase;
			for (int i = 0; i < m_iCount; i++) {
				strText += "・";
			}
			m_lbTest.text = strText;
		}
	
	}
}
