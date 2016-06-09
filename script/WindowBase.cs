using UnityEngine;
using System.Collections;

public abstract class WindowBase : PageBase {

	public GameObject m_goBlack;
	public CtrlMessage m_ctrlMessage;

	public ButtonBase m_btnClose;

	protected abstract void windowStart ();
	protected abstract void windowEnd ();

	public void WindowStart ()
	{
		base.PageStart ();

		m_goBlack  = PrefabManager.Instance.MakeObject ("prefab/Black" , gameObject );
		m_goBlack.SetActive (false);
		m_ctrlMessage = PrefabManager.Instance.MakeObject ("prefab/Window", gameObject).GetComponent<CtrlMessage> ();
		m_btnClose = PrefabManager.Instance.MakeObject ("prefab/CloseButton" , gameObject ).GetComponent<ButtonBase>();

		windowStart ();

		return;
	}

	public void WindowEnd(){
		base.PageEnd ();
		windowEnd ();
		return;
	}

}
