using UnityEngine;
using System.Collections;

public class SpriteManager : SpriteManagerBase<SpriteManager> {

	public Sprite Load( string _strName ){
		return base.LoadSprite (_strName);
	}

}
