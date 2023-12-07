﻿/*
* Author:	Iris Bermudez
* Date:		06/12/2023
*/


using System.Collections.Generic;
using UnityEngine;



namespace Solitaire.Common {

    public class CardSpritesScriptableObject : ScriptableObject {
        #region Variables

        public Sprite frontSprite;
        public Sprite backSprite;

        public List<Sprite> heartSprites;
        public List<Sprite> cloverSprites;
        public List<Sprite> diamondSprites;
        public List<Sprite> spadesSprites;

        #endregion


        #region Public methods

        public void SetNewSprites( CardSpritesScriptableObject _newSprites ) {
            frontSprite = _newSprites.frontSprite;
            backSprite = _newSprites.backSprite;

            heartSprites = _newSprites.heartSprites;
            cloverSprites = _newSprites.cloverSprites;
            diamondSprites = _newSprites.diamondSprites;
            spadesSprites = _newSprites.spadesSprites;
        }

        #endregion
    }
}