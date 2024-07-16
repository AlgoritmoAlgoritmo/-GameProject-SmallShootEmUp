﻿/*
* Author:	Iris Bermudez
* Date:		26/06/2024
*/



using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Test;
using Solitaire.GameModes.Klondike;
using Solitaire.Gameplay;
using Solitaire.Gameplay.CardContainers;
using System.Collections.Generic;
using Solitaire.Gameplay.Cards;



namespace Tests.Solitaire.GameModes.Klondike {
    public class KlondikeGameModeTest {
        #region Variables
        private GameObject klondikeGameModeMockGameObject;
        private KlondikeGameModeMock klondikeGameModeMock;
        #endregion


        #region Tests setup
        [SetUp]
        public void SetUp() {
            klondikeGameModeMockGameObject = GameObject.Instantiate( AssetDatabase
                    .LoadAssetAtPath<GameObject>( TestConstants.KLONDIKEGAMEMODEMOCK_PREFAB_PATH ) );

            if( !klondikeGameModeMockGameObject )
                throw new NullReferenceException( "GameObject at "
                        + $"{TestConstants.KLONDIKEGAMEMODEMOCK_PREFAB_PATH} could not be loaded." );

            klondikeGameModeMock = klondikeGameModeMockGameObject.GetComponent<KlondikeGameModeMock>();

            if( !klondikeGameModeMock )
                throw new NullReferenceException( "GameObject at "
                        + $"{TestConstants.SPIDERGAMEMODEMOCK_PREFAB_PATH} does not contain "
                        + "a SpiderGameMode component." );
        }
        #endregion


        #region Tests
        [Test]
        public void KlondikeGameModeMock_Is_KlondikeGameMode() {
            Assert.IsInstanceOf(typeof( KlondikeGameMode ), klondikeGameModeMock,
                               "Make sure KlondikeGameModeMock inherits from KlondikeGameMode." );
        }

        [Test]
        public void KlondikeGameModeMock_Is_AbstractGameMode() {
            Assert.IsInstanceOf( typeof( AbstractGameMode ), klondikeGameModeMock,
                               "Make sure KlondikeGameMode inherits from AbstractGameMode." );
        }

        [Test]
        [TestCase( 2, 4 )]
        [TestCase( 3, 3 )]
        [TestCase( 4, 6 )]
        public void WhenInitializingKlondikeGameMode_ThenDistributesProperAmountOfCardsToEverySingleContainer(
                                                                        short _amountOfContainersToSpawn,
                                                                         short _amountOfCardsPerContainer) {
            // Instantiate Card containers and add them to KlondikeGameModeMock
            List<AbstractCardContainer> cardContainerList = SpawnTheFollowingAmountOfAbstractCardContainers(
                                                                        _amountOfContainersToSpawn );
            foreach( var auxContainer in cardContainerList ) {
                auxContainer.SetDefaultAmountOfCards( _amountOfCardsPerContainer );
            }
            klondikeGameModeMock.SetCardContainers( cardContainerList );

            // Instantiate cards
            List<CardFacade> cardFacadeList = SpawnTheFollowingAmountOfCards( 
                                                    _amountOfCardsPerContainer * _amountOfContainersToSpawn );

            // Check amount of cards before initialization is zero
            Assert.Zero( klondikeGameModeMock.GetAmountOfDistributedCards(),
                        "klondikeGameModeMock shouldn't have any card." );

            // Initialize GameModeMock
            klondikeGameModeMock.Initialize( cardFacadeList );

            // Assert every single container has the correct amount of cards
            Assert.AreEqual( _amountOfCardsPerContainer * _amountOfContainersToSpawn,
                            klondikeGameModeMock.GetAmountOfDistributedCards(),
                            $"klondikeGameModeMock should contain {_amountOfCardsPerContainer * _amountOfContainersToSpawn} "
                                    + $"instead of {klondikeGameModeMock.GetAmountOfDistributedCards()}." );
        }


        [Test]
        public void WhenInitializingEmptyListOfCards_ThenThrowIndexOutOfRangeException() {
            List<CardFacade> listOfCards = new List<CardFacade>();
            int currentAmountOfCardsBeforeInitialization = klondikeGameModeMock.GetAmountOfDistributedCards();


            Assert.Throws<IndexOutOfRangeException>( () => klondikeGameModeMock.Initialize(
                                 listOfCards ),
                        "klondikeGameModeMock didn't throw IndexOutOfRangeException as expected. "
                                + "Check if the list passed is empty."
            );

            // Assert the amount of cards from spiderGameModeMock didn't change
            Assert.AreEqual( currentAmountOfCardsBeforeInitialization,
                            klondikeGameModeMock.GetAmountOfDistributedCards(),
                            "klondikeGameModeMock amount of cards changed: it should have "
                                    + $"{currentAmountOfCardsBeforeInitialization} instead of "
                                    + $"{klondikeGameModeMock.GetAmountOfDistributedCards() }." );
        }


        [Test]
        public void WhenInitializingListWithNullElement_ThenThrowNullReferenceException() {
            // Create list of cards
            List<CardFacade> listOfCards = SpawnTheFollowingAmountOfCards( 10 );
            int currentAmountOfCardsBeforeInitialization = klondikeGameModeMock.GetAmountOfDistributedCards();


            // Add null element to list
            listOfCards[UnityEngine.Random.Range( 0, listOfCards.Count - 1 )] = null;


            // Assert addition of list of cards with null element
            Assert.Throws<NullReferenceException>( () => klondikeGameModeMock.Initialize( listOfCards ),
                            "klondikeGameModeMock didn't throw NullReferenceException as expected. "
                                    + "Check if the list contains a null element."
            );


            // Assert the amount of cards from spiderGameModeMock didn't change
            Assert.AreEqual( currentAmountOfCardsBeforeInitialization,
                            klondikeGameModeMock.GetAmountOfDistributedCards(),
                            "klondikeGameModeMock amount of cards changed: it should have "
                                    + $"{currentAmountOfCardsBeforeInitialization} instead of "
                                    + $"{klondikeGameModeMock.GetAmountOfDistributedCards()}." );
        }


        [Test]
        public void WhenInitializingWithNullObject_ThenThrowNullReferenceException() {
            int currentAmountOfCardsBeforeInitialization = klondikeGameModeMock.GetAmountOfDistributedCards();

            // Assert addition of list of cards with null element
            Assert.Throws<NullReferenceException>( () => klondikeGameModeMock.Initialize( null ),
                            "klondikeGameModeMock didn't throw NullReferenceException as expected. "
                                    + "Check if the object is actually null."
            );


            // Assert the amount of cards from spiderGameModeMock didn't change
            Assert.AreEqual( currentAmountOfCardsBeforeInitialization,
                            klondikeGameModeMock.GetAmountOfDistributedCards(),
                            "spiderGameModeMock amount of cards changed: it should have "
                                    + $"{currentAmountOfCardsBeforeInitialization} instead of "
                                    + $"{klondikeGameModeMock.GetAmountOfDistributedCards()}." );
        }


        [Test]
        public void WhenCardIsFacingDown_ThenDeactivateDragging() {
            // Create facing down card
            CardFacade card = SpawnTheFollowingAmountOfCards(1)[0];

            // Set card facing down
            card.FlipCard( false );

            // Set card can bedragging to true
            card.SetCanBeDragged( true );

            // Validate card dragging
            klondikeGameModeMock.ValidateCardDragging( card );

            // Assert cannot be dragged
            Assert.IsFalse( card.gameObject.GetComponent<CardPhysics>().CanBeDragged,
                            "card shouldn't be draggable." );
        }



        #endregion


        #region Private methods
        private List<AbstractCardContainer> SpawnTheFollowingAmountOfAbstractCardContainers( int _amount ) {
            List<AbstractCardContainer> listOfCardContainersToAdd = new List<AbstractCardContainer>();

            GameObject klondikeCardContaierPrefabInstance = GameObject.Instantiate(
                            AssetDatabase.LoadAssetAtPath<GameObject>( TestConstants
                                                                    .KLONDIKECARDCONTAINER_PREFAB_PATH ) );
            listOfCardContainersToAdd.Add( klondikeCardContaierPrefabInstance.GetComponent<AbstractCardContainer>() );

            if( !klondikeCardContaierPrefabInstance )
                throw new NullReferenceException("Couldn't load prefab at "
                                                    + $"{TestConstants.KLONDIKECARDCONTAINER_PREFAB_PATH}.");

            for( int i = 0; i < _amount-1; i++ ) {
                listOfCardContainersToAdd.Add( GameObject.Instantiate( klondikeCardContaierPrefabInstance )
                                                                .GetComponent<AbstractCardContainer>() );
            }

            return listOfCardContainersToAdd;
        }

        private List<CardFacade> SpawnTheFollowingAmountOfCards( int _amountOfCards ) {
            GameObject cardFacadePrefabGameObject = GameObject.Instantiate( AssetDatabase
                                                        .LoadAssetAtPath<GameObject>( 
                                                                TestConstants.CARD_PREFAB_PATH ) );
            CardFacade cardFacadePrefab = cardFacadePrefabGameObject.GetComponent<CardFacade>();
            List<CardFacade> listOfSpawnedCards = new List<CardFacade>();
            for( int i = 0; i < _amountOfCards; i++ ) {
                listOfSpawnedCards.Add( GameObject.Instantiate( cardFacadePrefabGameObject )
                                                                .GetComponent<CardFacade>() );
            }

            return listOfSpawnedCards;
        }
        #endregion
    }
}