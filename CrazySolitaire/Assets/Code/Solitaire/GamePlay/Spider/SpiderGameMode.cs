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
            bool canBeDragged = CanBeDragged(_card);

            _card.SetCanBeDragged( canBeDragged );

            if( canBeDragged ) {
                var auxChild = _card.ChildCard;

                while( auxChild ) {
                    auxChild.ActivatePhysics( false );
                    auxChild = auxChild.ChildCard;
                }
            }
        }


        public void DistributorCardsDistribution( AbstractCardContainer _cardContainer ) {
            Debug.Log("Distributing cards.");

            List<CardFacade> auxCardsToDistribute = _cardContainer.GetCards();

            for( int i = auxCardsToDistribute.Count - 1; i >= 0; i-- ) {
                auxCardsToDistribute[i].RenderOnTop();
                auxCardsToDistribute[i].FlipCard( true );

                // Setting up parenting
                auxCardsToDistribute[i].ParentCard = cardContainers[i].GetTopCard();
                cardContainers[i].GetTopCard().ChildCard = auxCardsToDistribute[i];

                cardContainers[i].AddCard( auxCardsToDistribute[i] );
                _cardContainer.RemoveCard( auxCardsToDistribute[i] );

            }

            Destroy( _cardContainer.gameObject );
        }
        #endregion


        #region Protected methods
        protected override void ValidateCardPlacementWithCollison( CardFacade _placedCard,
                                                        CardFacade _detectedCard ) {
            Debug.Log("ValidateCardPlacementWithCollison");

            // Logic to move card from one container to another
            // Case: Card CANNOT be child of potential parent
            if( !CanBeChildOf( _placedCard, _detectedCard ) ) {
                Debug.Log("!CanBeChildOf");
                _placedCard.transform.position = GetCardOriginalPositionInContainer( _placedCard );



            // Case: Card CAN be child of potential parent
            } else {
                Debug.Log( "Card CAN be child of potential parent" );

                // 1- Removing from previous Parent and setting detected card as new parent's child
                if( _placedCard.ParentCard != null ) {
                    _placedCard.ParentCard.ChildCard = null;
                }

                _detectedCard.ChildCard = _placedCard;
                _placedCard.ParentCard = _detectedCard;

                // 2- Get parent card container
                AbstractCardContainer parentCardContainer = GetCardContainer( _detectedCard );


                // Recursively check childs
                CardFacade auxCardFacade = _placedCard;

                while( auxCardFacade != null ) {
                    // 3- Remove card from its card container
                    GetCardContainer(auxCardFacade).RemoveCard(auxCardFacade);

                    // 4- Add card to new card container
                    parentCardContainer.AddCard(auxCardFacade);

                    // 5- Re-activate child physics
                    auxCardFacade.ActivatePhysics(true);

                    // 6- Set ChildCard as card to check on next loop
                    auxCardFacade = auxCardFacade.ChildCard;
                }
            }
        }


        protected override void ValidateCardPlacementWithoutCollison( CardFacade _card ) {
            // Logic to make card return to previous position
            _card.transform.position = GetCardOriginalPositionInContainer( _card );

            if( _card.ChildCard != null ) {
                Debug.Log("Has a child");
                CardFacade auxCard = _card.ChildCard;

                while( auxCard != null ) {
                    Debug.Log("Has multiple childs");
                    auxCard.transform.position = GetCardOriginalPositionInContainer(auxCard);

                    auxCard = auxCard.ChildCard;
                }

            } else {
                Debug.Log("Does not have a child");
            }
        }


        protected override bool CanBeChildOf( CardFacade _card, CardFacade _potentialParent ) {
            return _potentialParent.GetCardNumber() == _card.GetCardNumber() + 1;
        }


        protected override bool CanBeDragged( CardFacade _card ) {
            if( !_card.ChildCard )
                return true;


            CardFacade auxCard = _card;

            while( auxCard.ChildCard ) {
                if( !CanBeChildOf( auxCard.ChildCard, auxCard )
                            ||  !auxCard.GetSuit().Equals(auxCard.ChildCard.GetSuit() ) ) {
                    return false;
                }

                auxCard = auxCard.ChildCard;
            }

            return true;
        }
        #endregion
    }
}