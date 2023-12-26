﻿/*
* Author:	Iris Bermudez
* Date:		08/12/2023
*/



using System;
using System.Collections.Generic;
using UnityEngine;
using Solitaire.Cards;



namespace Solitaire.Gameplay.Spider {
    public class SpiderGameMode : AbstractGameMode {
        #region Variables

        #endregion


        #region Public methods
        public override void Initialize() {
            cards = deckController.InitializeCards(suits, amountOfEachSuit);

            List<CardFacade> auxCards = new List<CardFacade>();

            foreach (CardFacade auxCard in cards) {
                auxCards.Add(auxCard);
            }

            foreach (AbstractCardContainer auxCardContainer in cardContainers) {
                auxCards = auxCardContainer.Initialize(auxCards);
            }

            deckController.SubscribeToDragStartEvent( ValidateCardDragging );
            deckController.SubscribeToCardPlacedEventWithCollision( 
                                                ValidateCardPlacementWithCollison );
            deckController.SubscribeToCardPlacedEventWithoutCollision( 
                                                ValidateCardPlacementWithoutCollison );
        }


        public override void SubscribeToOnGameClearedEvent(
                                                    EventHandler eventHandler) {
            deckController.onCardsCleared += eventHandler;
        }


        public void ValidateCardDragging( CardFacade _card ) {
            _card.SetCanBeDragged( CanBeDragged( _card ) );
        }
        #endregion


        #region Protected methods
        protected override void ValidateCardPlacementWithCollison( CardFacade _placedCard,
                                                        CardFacade _detectedCard ) {
            // Logic to move card from one container to another
            // Case: Card CANNOT be child of potential parent
            if( !CanBeChildOf( _placedCard, _detectedCard ) ) {
                _placedCard.transform.position = GetCardOriginalPositionInContainer( _placedCard );



            // Case: Card CAN be child of potential parent
            } else {
                // 1- Get parent card container
                AbstractCardContainer parentCardContainer = GetCardContainer( _detectedCard );

                // 2- Remove card its card container
                GetCardContainer( _placedCard ).RemoveCard( _placedCard );

                // 3- Add card to parent's card container
                parentCardContainer.AddCard( _placedCard );

                // 4- Setting card as new parent's child
                _detectedCard.ChildCard = _placedCard;
                _placedCard.ParentCard = _detectedCard;
            }
        }


        protected override void ValidateCardPlacementWithoutCollison( CardFacade _card ) {
            // Logic to make card return to previous position
            _card.transform.position = GetCardOriginalPositionInContainer( _card );            
        }


        protected override bool CanBeChildOf( CardFacade _card, CardFacade _potentialParent ) {
            return _potentialParent.GetCardNumber() == _card.GetCardNumber() + 1;
        }


        protected override bool CanBeDragged( CardFacade _card ) {
            if( !_card.ChildCard )
                return true;


            CardFacade auxCard = _card;

            while( auxCard.ChildCard ) {
                if( !CanBeChildOf( auxCard.ChildCard, auxCard ) ) {
                    return false;
                }

                auxCard = auxCard.ChildCard;
            }

            return true;
        }
        #endregion
    }
}