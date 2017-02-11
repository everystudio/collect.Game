using UnityEngine;
using System.Collections;

public class FooterIcon : ButtonBase {
	[SerializeField]
	private UI2DSprite m_sprBack;
	[SerializeField]
	private UI2DSprite m_sprFront;

	public void Initialize( string _strBack , string _strFront ){
		m_sprBack.sprite2D = SpriteManager.Instance.Load (_strBack);
		if (_strFront.Equals ("")) {
			m_sprFront.gameObject.SetActive (false);
		} else {
			m_sprFront.gameObject.SetActive (true);
			m_sprFront.sprite2D = SpriteManager.Instance.Load (_strFront);
		}
	}

}
