﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceScribble.Inputs;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;
using System.IO;
using SpaceScribble.Extensions;
using Microsoft.Phone.Applications.Common;
using SpaceScribble.Android;

namespace SpaceScribble
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class SpaceshipManager
    {
        private Texture2D menuTexture;
        private Texture2D spriteSheet;
        private SpriteFont font;

        private readonly Rectangle SelectShipTitleSource = new Rectangle(240, 800,
                                                                       240, 80);
        private readonly Vector2 TitlePosition = new Vector2(120.0f, 100.0f);

        private readonly Rectangle SpaceshipGreenHornetSource = new Rectangle(600, 450,
                                                                       100, 100);
        private readonly Rectangle SpaceshipEasySource = new Rectangle(0, 450,
                                                              100, 100);
        private readonly Rectangle SpaceshipMediumSource = new Rectangle(0, 250,
                                                              100, 100);
        private readonly Rectangle SpaceshipHardSource = new Rectangle(600, 250,
                                                                100, 100);
        private readonly Rectangle SpaceshipTankSource = new Rectangle(0, 350,
                                                              100, 100);
        private readonly Rectangle SpaceshipSpeederSource = new Rectangle(600, 350,
                                                              100, 100);
        private readonly Rectangle SpaceshipRaiderSource = new Rectangle(1100, 150,
                                                              100, 100);

        private readonly Rectangle SpaceshipDestination = new Rectangle(190, 235,
                                                                        100, 100);

        private readonly Rectangle LockSource = new Rectangle(500, 1450,
                                                              50, 50);
        private readonly Rectangle LockDestination = new Rectangle(215, 260,
                                                                   50, 50);

        public const long CREDITS_TO_UNLOCK_GREENHORNET = 1000000;
        public const long CREDITS_TO_UNLOCK_MEDIUM = 150000;
        public const long CREDITS_TO_UNLOCK_SPEEDER = 300000;
        public const long CREDITS_TO_UNLOCK_HARD = 500000;
        public const long CREDITS_TO_UNLOCK_TANK = 750000;
        public const long CREDITS_TO_UNLOCK_RAIDER = 2500000;

        private const int PriceTextPositionY = 385;
        public const string PRICE_TITLE_TEXT = "Price:";
        public const string CREDITS_TO_UNLOCK_GREENHORNET_TEXT = "1000000 $";
        public const string CREDITS_TO_UNLOCK_MEDIUM_TEXT = "150000 $";
        public const string CREDITS_TO_UNLOCK_SPEEDER_TEXT = "300000 $";
        public const string CREDITS_TO_UNLOCK_HARD_TEXT = "500000 $";
        public const string CREDITS_TO_UNLOCK_TANK_TEXT = "750000 $";
        public const string CREDITS_TO_UNLOCK_RAIDER_TEXT = "2500000 $";

        private readonly Rectangle ArrowRightSource = new Rectangle(340, 400,
                                                                  100, 100);
        private readonly Rectangle ArrowRightDestination = new Rectangle(330, 235,
                                                                       100, 100);
        private readonly Rectangle ArrowLeftDestination = new Rectangle(50, 235,
                                                                       100, 100);

        private readonly Rectangle cancelSource = new Rectangle(0, 800,
                                                                240, 80);
        private readonly Rectangle cancelDestination = new Rectangle(245, 710,
                                                                     230, 77);

        private readonly Rectangle goSource = new Rectangle(0, 720,
                                                            240, 80);
        private readonly Rectangle goDestination = new Rectangle(5, 710,
                                                                 230, 77);

        private readonly Rectangle buySource = new Rectangle(240, 880,
                                                            240, 80);
        private readonly Rectangle buyDestination = new Rectangle(5, 710,
                                                                 230, 77);

        public static GameInput GameInput;
        private const string BuyAction = "BuyShip";
        private const string LeftAction = "SelectLeftShip";
        private const string RightAction = "SelectRightShip";
        private const string GoAction = "Go";
        private const string CancelAction = "CancelShipSelection";

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private PlayerManager playerManager;
        private CreditsManager creditsManager;

        private bool cancelClicked = false;
        private bool goClicked = false;

        private float switchShipTimer = 0.0f;
        private const float SwitchShipMinTimer = 0.25f;

        private string descriptionGreenHornet = null;
        private string descriptionMedium = null;
        private string descriptionHard = null;
        private string descriptionSpeeder = null;
        private string descriptionTank = null;
        private string descriptionRaider = null;

        private readonly Vector2 SmallBarOverlayStart = new Vector2(600, 950);
        private const int BarWidth = 150;
        private const int BarHeight = 10;

        private Vector2 maxHitPointLocation = new Vector2(240, 355);
        private Vector2 agilityLocation = new Vector2(240, 388);
        private Vector2 firePowerLocation = new Vector2(240, 421);
        private Vector2 fireSpeedLocation = new Vector2(240, 454);
        private Vector2 accuracyLocation = new Vector2(240, 487);

        private const string HP_TEXT = "HP";
        private const string AGILITY_TEXT = "Agility";
        private const string FIREPOWER_TEXT = "Laser Power";
        private const string FIRESPEED_TEXT = "Laser Speed";
        private const string ACCURACY_TEXT = "Accuracy";

        private Vector2 maxHitPointTextLocation = new Vector2(90, 348);
        private Vector2 agilityTextLocation = new Vector2(90, 381);
        private Vector2 firePowerTextLocation = new Vector2(90, 414);
        private Vector2 fireSpeedTextLocation = new Vector2(90, 447);
        private Vector2 accuracyTextLocation = new Vector2(90, 480);

        private const string SHIPDATA_FILE = "spaceship.txt";

        private bool isMediumUnlocked;
        private bool isHardUnlocked;
        private bool isSpeederUnlocked;
        private bool isTankUnlocked;
        private bool isGreenHornetUnlocked;
        private bool isRaiderUnlocked;

        private const string CREDITS_TEXT = "Credits:";
        private readonly Vector2 creditsTextPosition = new Vector2(130, 194);
        private readonly Vector2 creditsPosition = new Vector2(238, 194);

        private readonly Rectangle[] PhoneSource = {new Rectangle(480, 0, 200, 256),
                                                    new Rectangle(680, 0, 200, 256),
                                                    new Rectangle(480, 300, 200, 256),
                                                     new Rectangle(680, 300, 200, 256),
                                                    new Rectangle(480, 600, 200, 256),
                                                   new Rectangle(680, 600, 200, 256),
                                                   new Rectangle(680, 1050, 200, 256)};
        private readonly Rectangle PhoneUnknownSource = new Rectangle(480, 1050, 200, 256);
        private readonly Rectangle PhoneTouchSource = new Rectangle(0, 1540, 256, 256);

        private readonly Rectangle PhoneSelectedDestination = new Rectangle(195, 560, 110, 140);
        private readonly Rectangle PhoneTouchDestination = new Rectangle(180, 560, 140, 140);

        private readonly Rectangle PhoneRight1Destination = new Rectangle(295, 580, 78, 100);
        private readonly Rectangle PhoneRight2Destination = new Rectangle(340, 590, 62, 80);
        private readonly Rectangle PhoneLeft1Destination = new Rectangle(117, 580, 78, 100);
        private readonly Rectangle PhoneLeft2Destination = new Rectangle(72, 590, 62, 80);

        private readonly SettingsManager settingsManager;

        private bool canStart;

        // input selection
        private readonly Rectangle InputTouchSource = new Rectangle(580, 1450, 300, 50);
        private readonly Rectangle InputSensorSource = new Rectangle(580, 1500, 300, 50);
        private readonly Rectangle InputTitleDestionation = new Rectangle(90, 515, 300, 50);
        private readonly Rectangle InputTouchArea = new Rectangle(0, 515, 480, 175);
        private readonly Rectangle InputArrowRightDestination = new Rectangle(380, 595,
                                                                       60, 60);
        private readonly Rectangle InputArrowLeftDestination = new Rectangle(30, 595,
                                                                       60, 60);
        private const string ActionInput = "ChangeInput";

        public SpaceshipManager(Texture2D mtex, Texture2D spriteSheet, SpriteFont font,
                                PlayerManager player, CreditsManager creditsManager)
        {
            this.menuTexture = mtex;
            this.spriteSheet = spriteSheet;
            this.font = font;
            this.playerManager = player;
            this.creditsManager = creditsManager;

            PrepareStrings();

            loadSpaceshipData();

            settingsManager = SettingsManager.GetInstance();

            AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
            AccelerometerHelper.Instance.Active = true;
        }

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(LeftAction,
                                           GestureType.Tap,
                                           ArrowLeftDestination);
            GameInput.AddTouchGestureInput(RightAction,
                                           GestureType.Tap,
                                           ArrowRightDestination);

            GameInput.AddTouchGestureInput(GoAction,
                                           GestureType.Tap,
                                           goDestination);
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);
            GameInput.AddTouchGestureInput(BuyAction,
                                           GestureType.Tap,
                                           buyDestination);

            GameInput.AddTouchGestureInput(ActionInput,
                                           GestureType.Tap,
                                           InputTouchArea);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isActive)
            {
                if (PlayerManager.IsSensorInput)
                {
                    if (settingsManager.GetNeutralPositionIndex() >= 0)
                    {
                        canStart = true;
                    }
                    else
                    {
                        canStart = false;
                    }
                }
                else
                {
                    canStart = true;
                }

                switchShipTimer += elapsed;

                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        private bool isCurrentSelectionUnlocked()
        {
            switch (playerManager.ShipType)
            {
                case PlayerManager.PlayerType.GreenHornet:
                    return isGreenHornetUnlocked;

                case PlayerManager.PlayerType.Medium:
                    return isMediumUnlocked;

                case PlayerManager.PlayerType.Hard:
                    return isHardUnlocked;

                case PlayerManager.PlayerType.Tank:
                    return isTankUnlocked;

                case PlayerManager.PlayerType.Speeder:
                    return isSpeederUnlocked;

                case PlayerManager.PlayerType.Raider:
                    return isRaiderUnlocked;

                default:
                    return true;
            }
        }

        private bool canUnlockCurrentSelection()
        {
            switch (playerManager.ShipType)
            {
                case PlayerManager.PlayerType.GreenHornet:
                    return creditsManager.TotalCredits >= CREDITS_TO_UNLOCK_GREENHORNET;

                case PlayerManager.PlayerType.Medium:
                    return creditsManager.TotalCredits >= CREDITS_TO_UNLOCK_MEDIUM;

                case PlayerManager.PlayerType.Hard:
                    return creditsManager.TotalCredits >= CREDITS_TO_UNLOCK_HARD;

                case PlayerManager.PlayerType.Tank:
                    return creditsManager.TotalCredits >= CREDITS_TO_UNLOCK_TANK;

                case PlayerManager.PlayerType.Speeder:
                    return creditsManager.TotalCredits >= CREDITS_TO_UNLOCK_SPEEDER;

                case PlayerManager.PlayerType.Raider:
                    return creditsManager.TotalCredits >= CREDITS_TO_UNLOCK_RAIDER;

                default:
                    return true;
            }
        }

        private void unlockCurrentSelection()
        {
            switch (playerManager.ShipType)
            {
                case PlayerManager.PlayerType.GreenHornet:
                    creditsManager.DecreaseTotalCredits(CREDITS_TO_UNLOCK_GREENHORNET);
                    ZoomTextManager.ShowBuyPrice(CREDITS_TO_UNLOCK_GREENHORNET_TEXT);
                    this.isGreenHornetUnlocked = true;
                    break;

                case PlayerManager.PlayerType.Medium:
                    creditsManager.DecreaseTotalCredits(CREDITS_TO_UNLOCK_MEDIUM);
                    ZoomTextManager.ShowBuyPrice(CREDITS_TO_UNLOCK_MEDIUM_TEXT);
                    this.isMediumUnlocked = true;
                    break;

                case PlayerManager.PlayerType.Hard:
                    creditsManager.DecreaseTotalCredits(CREDITS_TO_UNLOCK_HARD);
                    ZoomTextManager.ShowBuyPrice(CREDITS_TO_UNLOCK_HARD_TEXT);
                    this.isHardUnlocked = true;
                    break;

                case PlayerManager.PlayerType.Tank:
                    creditsManager.DecreaseTotalCredits(CREDITS_TO_UNLOCK_TANK);
                    ZoomTextManager.ShowBuyPrice(CREDITS_TO_UNLOCK_TANK_TEXT);
                    this.isTankUnlocked = true;
                    break;

                case PlayerManager.PlayerType.Speeder:
                    creditsManager.DecreaseTotalCredits(CREDITS_TO_UNLOCK_SPEEDER);
                    ZoomTextManager.ShowBuyPrice(CREDITS_TO_UNLOCK_SPEEDER_TEXT);
                    this.isSpeederUnlocked = true;
                    break;

                case PlayerManager.PlayerType.Raider:
                    creditsManager.DecreaseTotalCredits(CREDITS_TO_UNLOCK_RAIDER);
                    ZoomTextManager.ShowBuyPrice(CREDITS_TO_UNLOCK_RAIDER_TEXT);
                    this.isRaiderUnlocked = true;
                    break;
            }

            SoundManager.PlayCoinSound();
            creditsManager.Save();
            this.saveSpaceshipData();
        }

        private void handleTouchInputs()
        {
            // Left
            if (GameInput.IsPressed(LeftAction) && switchShipTimer > SwitchShipMinTimer)
            {
                switchShipTimer = 0.0f;

                toggleSpaceshipLeft();

                SoundManager.PlayPaperSound();
            }
            // Right
            if (GameInput.IsPressed(RightAction) && switchShipTimer > SwitchShipMinTimer)
            {
                switchShipTimer = 0.0f;

                toggleSpaceshipRight();

                SoundManager.PlayPaperSound();
            }

            // Toogle input
            if (GameInput.IsPressed(ActionInput))
            {
                PlayerManager.IsSensorInput = !PlayerManager.IsSensorInput;
            }

            // Go/Buy
            if (isCurrentSelectionUnlocked())
            {
                if (GameInput.IsPressed(GoAction))
                {
                    goClicked = true;
                }
            }
            else
            {
                if (GameInput.IsPressed(BuyAction))
                {
                    if (canUnlockCurrentSelection())
                        unlockCurrentSelection();
                }
            }

            // Cancel
            if (GameInput.IsPressed(CancelAction))
            {
                cancelClicked = true;
            }
        }

        private void toggleSpaceshipRight()
        {
            switch (playerManager.ShipType)
            {
                case PlayerManager.PlayerType.Easy:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Medium);
                    break;
                case PlayerManager.PlayerType.Medium:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Speeder);
                    break;
                case PlayerManager.PlayerType.Speeder:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Hard);
                    break;
                case PlayerManager.PlayerType.Hard:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Tank);
                    break;
                case PlayerManager.PlayerType.Tank:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.GreenHornet);
                    break;
                case PlayerManager.PlayerType.GreenHornet:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Raider);
                    break;
                case PlayerManager.PlayerType.Raider:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Easy);
                    break;
            }
        }

        private void toggleSpaceshipLeft()
        {
            switch (playerManager.ShipType)
            {
                case PlayerManager.PlayerType.Easy:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Raider);
                    break;
                case PlayerManager.PlayerType.Medium:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Easy);
                    break;
                case PlayerManager.PlayerType.Speeder:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Medium);
                    break;
                case PlayerManager.PlayerType.Hard:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Speeder);
                    break;
                case PlayerManager.PlayerType.Tank:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Hard);
                    break;
                case PlayerManager.PlayerType.GreenHornet:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.Tank);
                    break;
                case PlayerManager.PlayerType.Raider:
                    playerManager.SelectPlayerType(PlayerManager.PlayerType.GreenHornet);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuTexture,
                             TitlePosition,
                             SelectShipTitleSource,
                             Color.White * opacity);

            drawSpaceship(spriteBatch);

            // Arrow left
            spriteBatch.Draw(menuTexture,
                             ArrowLeftDestination,
                             ArrowRightSource,
                             Color.White * opacity,
                             0.0f,
                             Vector2.Zero,
                             SpriteEffects.FlipHorizontally,
                             0.0f);

            // Arrow right
            spriteBatch.Draw(menuTexture,
                             ArrowRightDestination,
                             ArrowRightSource,
                             Color.White * opacity,
                             0.0f,
                             Vector2.Zero,
                             SpriteEffects.None,
                             0.0f);

            // Button select
            Color startColor;

            if (canStart)
            {
                startColor = Color.White * opacity;
            }
            else
            {
                startColor = Color.White * opacity * 0.5f;
            }

            if (isCurrentSelectionUnlocked())
            {
                spriteBatch.Draw(menuTexture,
                                 goDestination,
                                 goSource,
                                 startColor);

                drawMaxHitPoints(spriteBatch);
                drawAgility(spriteBatch);
                drawFirePower(spriteBatch);
                drawFireSpeed(spriteBatch);
                drawAccuracy(spriteBatch);
            }
            else
            {
                if (canUnlockCurrentSelection())
                    spriteBatch.Draw(menuTexture,
                                 buyDestination,
                                 buySource,
                                 Color.White * opacity);
                else
                    spriteBatch.Draw(menuTexture,
                                 buyDestination,
                                 buySource,
                                 Color.White * opacity * 0.5f);

                drawShipPrice(spriteBatch);
            }

            // Button cancel
            spriteBatch.Draw(menuTexture,
                             cancelDestination,
                             cancelSource,
                             Color.White * opacity);

            spriteBatch.DrawString(font,
                                   CREDITS_TEXT,
                                   creditsTextPosition,
                                   Color.Black * opacity);

            Vector2 pos = spriteBatch.DrawInt64WithZeros(font,
                                                creditsManager.TotalCredits,
                                                creditsPosition,
                                                Color.Black * opacity,
                                                8);
            spriteBatch.DrawString(font,
                                   " $",
                                   pos,
                                   Color.Black * opacity);

            // input
            drawInput(spriteBatch);
        }

        private void drawInput(SpriteBatch spriteBatch)
        {
            Rectangle inputTitleSrc;

            if (PlayerManager.IsSensorInput)
                inputTitleSrc = InputSensorSource;
            else
                inputTitleSrc = InputTouchSource;

            spriteBatch.Draw(menuTexture,
                InputTitleDestionation,
                inputTitleSrc,
                Color.White);

            // Phone position
            drawPhonePosition(spriteBatch);

            // Arrow left
            spriteBatch.Draw(menuTexture,
                             InputArrowLeftDestination,
                             ArrowRightSource,
                             Color.White * opacity,
                             0.0f,
                             Vector2.Zero,
                             SpriteEffects.FlipHorizontally,
                             0.0f);

            // Arrow right
            spriteBatch.Draw(menuTexture,
                             InputArrowRightDestination,
                             ArrowRightSource,
                             Color.White * opacity,
                             0.0f,
                             Vector2.Zero,
                             SpriteEffects.None,
                             0.0f);
        }

        private void drawPhonePosition(SpriteBatch spriteBatch)
        {
            if (PlayerManager.IsSensorInput)
            {
                int phoneIndex = settingsManager.GetNeutralPositionIndex();

                if (phoneIndex >= 0 && phoneIndex < PhoneSource.Length)
                {
                    spriteBatch.Draw(
                        menuTexture,
                        PhoneSelectedDestination,
                        PhoneSource[phoneIndex],
                        Color.White);
                }
                else
                {
                    spriteBatch.Draw(
                        menuTexture,
                        PhoneSelectedDestination,
                        PhoneUnknownSource,
                        Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(
                        menuTexture,
                        PhoneTouchDestination,
                        PhoneTouchSource,
                        Color.White);
            }
        }

        private void drawShipPrice(SpriteBatch spriteBatch)
        {
            string priceText;

            switch (playerManager.ShipType)
            {
                case PlayerManager.PlayerType.GreenHornet:
                    priceText = CREDITS_TO_UNLOCK_GREENHORNET_TEXT;
                    break;
                case PlayerManager.PlayerType.Medium:
                    priceText = CREDITS_TO_UNLOCK_MEDIUM_TEXT;
                    break;
                case PlayerManager.PlayerType.Hard:
                    priceText = CREDITS_TO_UNLOCK_HARD_TEXT;
                    break;
                case PlayerManager.PlayerType.Tank:
                    priceText = CREDITS_TO_UNLOCK_TANK_TEXT;
                    break;
                case PlayerManager.PlayerType.Speeder:
                    priceText = CREDITS_TO_UNLOCK_SPEEDER_TEXT;
                    break;
                case PlayerManager.PlayerType.Raider:
                    priceText = CREDITS_TO_UNLOCK_RAIDER_TEXT;
                    break;
                default:
                    priceText = string.Empty;
                    break;
            }

            spriteBatch.DrawString(font,
                                   PRICE_TITLE_TEXT,
                                   new Vector2(240.0f - font.MeasureString(PRICE_TITLE_TEXT).X / 2,
                                               PriceTextPositionY),
                                   Color.Black);

            spriteBatch.DrawString(font,
                                   priceText,
                                   new Vector2(240.0f - font.MeasureString(priceText).X / 2,
                                               PriceTextPositionY + 35),
                                   Color.Black);
        }

        /// <summary>
        /// Prepares the variable strings (called each time navigating to the select spaceship screen).
        /// </summary>
        public void PrepareStrings()
        {
            descriptionGreenHornet = string.Format("Gain {0} XP! [{1}%]",
                                                   CREDITS_TO_UNLOCK_GREENHORNET,
                                                   (int)(100.0f * (creditsManager.TotalCredits / (float)CREDITS_TO_UNLOCK_GREENHORNET)));

            descriptionMedium = string.Format("Gain {0} XP! [{1}%]",
                                              CREDITS_TO_UNLOCK_MEDIUM,
                                              (int)(100.0f * (creditsManager.TotalCredits / (float)CREDITS_TO_UNLOCK_MEDIUM)));

            descriptionHard = string.Format("Gain {0} XP! [{1}%]",
                                            CREDITS_TO_UNLOCK_HARD,
                                            (int)(100.0f * (creditsManager.TotalCredits / (float)CREDITS_TO_UNLOCK_HARD)));

            descriptionSpeeder = string.Format("Gain {0} XP! [{1}%]",
                                               CREDITS_TO_UNLOCK_SPEEDER,
                                               (int)(100.0f * (creditsManager.TotalCredits / (float)CREDITS_TO_UNLOCK_SPEEDER)));

            descriptionTank = string.Format("Gain {0} XP! [{1}%]",
                                            CREDITS_TO_UNLOCK_TANK,
                                            (int)(100.0f * (creditsManager.TotalCredits / (float)CREDITS_TO_UNLOCK_TANK)));

            descriptionRaider = string.Format("Gain {0} XP! [{1}%]",
                                            CREDITS_TO_UNLOCK_RAIDER,
                                            (int)(100.0f * (creditsManager.TotalCredits / (float)CREDITS_TO_UNLOCK_RAIDER)));
        }

        private void drawSpaceship(SpriteBatch spriteBatch)
        {
            Rectangle src;

            switch (playerManager.ShipType)
            {
                case PlayerManager.PlayerType.GreenHornet:
                    src = SpaceshipGreenHornetSource;
                    break;
                case PlayerManager.PlayerType.Medium:
                    src = SpaceshipMediumSource;
                    break;
                case PlayerManager.PlayerType.Hard:
                    src = SpaceshipHardSource;
                    break;
                case PlayerManager.PlayerType.Speeder:
                    src = SpaceshipSpeederSource;
                    break;
                case PlayerManager.PlayerType.Tank:
                    src = SpaceshipTankSource;
                    break;
                case PlayerManager.PlayerType.Raider:
                    src = SpaceshipRaiderSource;
                    break;
                default:
                    src = SpaceshipEasySource;
                    break;
            }

            if (isCurrentSelectionUnlocked())
            {
                spriteBatch.Draw(spriteSheet,
                                 new Rectangle(SpaceshipDestination.X,
                                             SpaceshipDestination.Y + 100,
                                             SpaceshipDestination.Width,
                                             SpaceshipDestination.Height),
                                 src,
                                 Color.White * opacity,
                                 MathHelper.PiOver2 * 3,
                                 Vector2.Zero,
                                 SpriteEffects.None,
                                 0.0f);
            }
            else
            {
                spriteBatch.Draw(spriteSheet,
                                 new Rectangle(SpaceshipDestination.X,
                                             SpaceshipDestination.Y + 100,
                                             SpaceshipDestination.Width,
                                             SpaceshipDestination.Height),
                                 src,
                                 Color.White * (opacity * 0.5f),
                                 MathHelper.PiOver2 * 3,
                                 Vector2.Zero,
                                 SpriteEffects.None,
                                 0.0f);

                spriteBatch.Draw(menuTexture,
                             LockDestination,
                             LockSource,
                             Color.Black * opacity);
            }
        }

        private void drawMaxHitPoints(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   HP_TEXT,
                                   maxHitPointTextLocation,
                                   Color.Black);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)maxHitPointLocation.X,
                        (int)maxHitPointLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)maxHitPointLocation.X,
                        (int)maxHitPointLocation.Y,
                        (int)(playerManager.InitMaxHitPoints - 200),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(playerManager.InitMaxHitPoints - 200),
                        BarHeight),
                    Color.Black * 0.75f);
        }

        private void drawAgility(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   AGILITY_TEXT,
                                   agilityTextLocation,
                                   Color.Black);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)agilityLocation.X,
                        (int)agilityLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)agilityLocation.X,
                        (int)agilityLocation.Y,
                        (int)(playerManager.InitPlayerSpeed - 100),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(playerManager.InitPlayerSpeed - 100),
                        BarHeight),
                    Color.Black * 0.75f);
        }

        private void drawFirePower(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   FIREPOWER_TEXT,
                                   firePowerTextLocation,
                                   Color.Black);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)firePowerLocation.X,
                        (int)firePowerLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)firePowerLocation.X,
                        (int)firePowerLocation.Y,
                        (int)(playerManager.InitShotPower * 8 - 210),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(playerManager.InitShotPower * 8 - 210),
                        BarHeight),
                    Color.Black * 0.75f);
        }

        private void drawFireSpeed(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   FIRESPEED_TEXT,
                                   fireSpeedTextLocation,
                                   Color.Black);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)fireSpeedLocation.X,
                        (int)fireSpeedLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)fireSpeedLocation.X,
                        (int)fireSpeedLocation.Y,
                        (int)(playerManager.InitShotSpeed - 200),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(playerManager.InitShotSpeed - 200),
                        BarHeight),
                    Color.Black * 0.75f);
        }

        private void drawAccuracy(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   ACCURACY_TEXT,
                                   accuracyTextLocation,
                                   Color.Black);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)accuracyLocation.X,
                        (int)accuracyLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarWidth,
                        BarHeight),
                    Color.Black * 0.3f);

            spriteBatch.Draw(spriteSheet,
                    new Rectangle(
                        (int)accuracyLocation.X,
                        (int)accuracyLocation.Y,
                        (int)(150 - (playerManager.InitInaccuracy - 7) * 37.5),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(150 - (playerManager.InitInaccuracy - 7) * 37.5),
                        BarHeight),
                    Color.Black * 0.75f);
        }

        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            if (isActive)
            {
                Vector3 currentAccValue = new Vector3((float)e.AverageAcceleration.X,
                                          (float)e.AverageAcceleration.Y,
                                          (float)e.AverageAcceleration.Z);

                if (currentAccValue.Z < 0.001f || Math.Abs(currentAccValue.X) > 0.5f)
                {
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Unsupported);
                    return;
                }

                float val = (float)Math.Asin(currentAccValue.Y);

                if (val >= settingsManager.GetNeutralPositionRadianValue(-10.0f) && val < settingsManager.GetNeutralPositionRadianValue(5.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle0);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(5.0f) && val < settingsManager.GetNeutralPositionRadianValue(15.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle10);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(15.0f) && val < settingsManager.GetNeutralPositionRadianValue(25.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle20);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(25.0f) && val < settingsManager.GetNeutralPositionRadianValue(35.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle30);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(35.0f) && val < settingsManager.GetNeutralPositionRadianValue(45.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle40);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(45.0f) && val < settingsManager.GetNeutralPositionRadianValue(55.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle50);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(55.0f) && val < settingsManager.GetNeutralPositionRadianValue(70.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle60);
                else
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Unsupported);
            }

        }

        #region Load/Save

        private void saveSpaceshipData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(SHIPDATA_FILE, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.isMediumUnlocked);
                        sw.WriteLine(this.isHardUnlocked);
                        sw.WriteLine(this.isSpeederUnlocked);
                        sw.WriteLine(this.isTankUnlocked);
                        sw.WriteLine(this.isGreenHornetUnlocked);
                        sw.WriteLine(this.isRaiderUnlocked);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        private void loadSpaceshipData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(SHIPDATA_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(SHIPDATA_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            this.isMediumUnlocked = Boolean.Parse(sr.ReadLine());
                            this.isHardUnlocked = Boolean.Parse(sr.ReadLine());
                            this.isSpeederUnlocked = Boolean.Parse(sr.ReadLine());
                            this.isTankUnlocked = Boolean.Parse(sr.ReadLine());
                            this.isGreenHornetUnlocked = Boolean.Parse(sr.ReadLine());

                            // check for data because this property didn't exist before version 1.9
                            try
                            {
                                this.isRaiderUnlocked = Boolean.Parse(sr.ReadLine());
                            }
                            catch (Exception)
                            {
                                this.isRaiderUnlocked = false;
                            }
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.isMediumUnlocked);
                            sw.WriteLine(this.isHardUnlocked);
                            sw.WriteLine(this.isSpeederUnlocked);
                            sw.WriteLine(this.isTankUnlocked);
                            sw.WriteLine(this.isGreenHornetUnlocked);
                            sw.WriteLine(this.isRaiderUnlocked);

                            // ... ? 
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Properties

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                    this.goClicked = false;
                    this.cancelClicked = false;
                }
            }
        }

        public bool CancelClicked
        {
            get
            {
                return this.cancelClicked;
            }
        }

        public bool GoClicked
        {
            get
            {
                return this.goClicked;
            }
        }

        #endregion
    }
}