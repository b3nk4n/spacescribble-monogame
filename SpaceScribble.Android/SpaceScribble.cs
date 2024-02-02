using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SpaceScribble.Inputs;

namespace SpaceScribble.Android;

public class SpaceScribble : Game, IBackButtonPressedCallback
{
    /*
     * The game's fixed width and heigth of the screen.
     * Do NOT change this number, because other related code uses hard
     * coded values similar to this one and assumes that this is the screen
     * dimension. This value values was fixed in Windows Phone,
     * but there are very different screen out there in Android.
     */
    const int WIDTH = 480;
    const int HEIGHT = 800;
    Matrix screenScaleMatrix;
    Vector2 screenScaleVector;

    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    private const string GameOverText = "GAME OVER!";

    private const string ContinueText = "Push to continue...";
    private string VersionText;
    private const string MusicByText = "Music by";
    private const string MusicCreatorText = "PLSQMPRFKT";
    private const string CreatorText = "by Benjamin Kan";

    enum GameStates
    {
        TitleScreen, MainMenu, Instructions, Settings, Playing, AsteroidShower, BossDuell, Paused, PlayerDead, GameOver, Submittion,
        SelectShipAndPhonePosition
    };

    GameStates gameState = GameStates.TitleScreen;
    GameStates stateBeforePaused;
    Texture2D spriteSheet;
    Texture2D menuSheet;
    Texture2D planetSheet;
    Texture2D paperSheet;
    Texture2D handSheet;

    StarFieldManager starFieldManager1;

    AsteroidManager asteroidManager;

    PlayerManager playerManager;

    EnemyManager enemyManager;
    BossManager bossManager;

    CollisionManager collisionManager;

    SpriteFont pericles16;
    SpriteFont pericles18;
    SpriteFont pericles20;
    SpriteFont pericles32;

    ZoomTextManager zoomTextManager;

    private float playerDeathTimer = 0.0f;
    private const float playerDeathDelayTime = 6.0f;
    private const float playerGameOverDelayTime = 5.0f;

    private float titleScreenTimer = 0.0f;
    private const float titleScreenDelayTime = 1.0f;

    private Vector2 playerStartLocation = new Vector2(215, 625);

    Hud hud;

    CreditsManager creditsManager;
    SubmissionManager submissionManager;

    MainMenuManager mainMenuManager;

    private float backButtonTimer = 0.0f;
    private const float backButtonDelayTime = 0.25f;

    LevelManager levelManager;

    InstructionManager instructionManager;

    PowerUpManager powerUpManager;
    Texture2D powerUpSheet;

    SettingsManager settingsManager;

    private bool bossDirectKill = true;
    private const long InitialBossBonusScore = 5000;
    private long bossBonusScore = InitialBossBonusScore;

    GameInput gameInput = new GameInput();
    private const string TitleAction = "Title";
    private const string BackToGameAction = "BackToGame";
    private const string BackToMainAction = "BackToMain";

    private readonly Rectangle cancelSource = new Rectangle(0, 960,
                                                            240, 80);
    private readonly Rectangle cancelDestination = new Rectangle(120, 620,
                                                                 240, 80);

    private readonly Rectangle continueSource = new Rectangle(0, 880,
                                                              240, 80);
    private readonly Rectangle continueDestination = new Rectangle(120, 510,
                                                                   240, 80);

    SpaceshipManager spaceshipAndPhonePositionManager;

    HandManager handManager;

    private bool backButtonPressed = false;

    public SpaceScribble()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

        Content.RootDirectory = "Content";

        // Frame rate is 60 fps
        TargetElapsedTime = TimeSpan.FromTicks(166667);
    }

    void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
    {
        e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        graphics.IsFullScreen = true;
        graphics.PreferredBackBufferHeight = HEIGHT;
        graphics.PreferredBackBufferWidth = WIDTH;
        graphics.SupportedOrientations = DisplayOrientation.Portrait;

        // Aplly the gfx changes
        graphics.ApplyChanges();

        // calculate scaling matrix/vector to fit everything to the assumed screen bounds
        var bw = GraphicsDevice.PresentationParameters.BackBufferWidth;
        var bh = GraphicsDevice.PresentationParameters.BackBufferHeight;
        screenScaleVector = new Vector2((float)bw / WIDTH, (float)bh / HEIGHT);
        screenScaleMatrix = Matrix.Identity * Matrix.CreateScale(screenScaleVector.X, screenScaleVector.Y, 0f);

        // Because we are using a different virtual scale compared to the
        // physical resolution of the screen, using a transformation matrix
        // of the SpriteBatch, we need to change the display for the touch
        // panel the same way
        TouchPanel.DisplayOrientation = DisplayOrientation.Portrait;
        TouchPanel.DisplayHeight = HEIGHT;
        TouchPanel.DisplayWidth = WIDTH;
        TouchPanel.EnabledGestures = GestureType.Tap;

        loadVersion();

        base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = new SpriteBatch(GraphicsDevice);

        spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
        menuSheet = Content.Load<Texture2D>(@"Textures\MenuSheet");
        powerUpSheet = Content.Load<Texture2D>(@"Textures\PowerUpSheet");
        planetSheet = Content.Load<Texture2D>(@"Textures\PlanetSheet");
        paperSheet = Content.Load<Texture2D>(@"Textures\PaperSheet");
        handSheet = Content.Load<Texture2D>(@"Textures\HandSheet");

        starFieldManager1 = new StarFieldManager(WIDTH,
                                                 HEIGHT,
                                                 100,
                                                 50,
                                                 new Vector2(0, 25.0f),
                                                 new Vector2(0, 50.0f),
                                                 spriteSheet,
                                                 new Rectangle(0, 550, 2, 2),
                                                 new Rectangle(0, 550, 3, 3),
                                                 planetSheet,
                                                 new Rectangle(0, 0, 400, 314),
                                                 2,
                                                 3);

        asteroidManager = new AsteroidManager(2,
                                              spriteSheet,
                                              new Rectangle(0, 0, 50, 50),
                                              20,
                                              WIDTH,
                                              HEIGHT);

        playerManager = new PlayerManager(spriteSheet,
                                          new Rectangle(0, 100, 50, 50),
                                          6,
                                          new Rectangle(0, 0, WIDTH,HEIGHT),
                                          playerStartLocation,
                                          gameInput);

        enemyManager = new EnemyManager(spriteSheet,
                                        playerManager,
                                        new Rectangle(0, 0, WIDTH, HEIGHT));

        bossManager = new BossManager(spriteSheet,
                                      playerManager,
                                      new Rectangle(0, 0, WIDTH, HEIGHT));
        Boss.Player = playerManager;

        EffectManager.Initialize(spriteSheet,
                                 new Rectangle(0, 550, 2, 2),
                                 new Rectangle(0, 50, 50, 50),
                                 5);

        powerUpManager = new PowerUpManager(powerUpSheet);

        collisionManager = new CollisionManager(asteroidManager,
                                                playerManager,
                                                enemyManager,
                                                bossManager,
                                                powerUpManager);

        SoundManager.Initialize(Content);

        pericles16 = Content.Load<SpriteFont>(@"Fonts\Pericles16");
        pericles18 = Content.Load<SpriteFont>(@"Fonts\Pericles18");
        pericles20 = Content.Load<SpriteFont>(@"Fonts\Pericles20");
        pericles32 = Content.Load<SpriteFont>(@"Fonts\Pericles32");

        zoomTextManager = new ZoomTextManager(new Vector2(WIDTH / 2,
                                                          HEIGHT / 2),
                                                          pericles18,
                                                          pericles32);

        hud = Hud.GetInstance(new Rectangle(0, 0, WIDTH, HEIGHT),
                              spriteSheet,
                              pericles20,
                              pericles16,
                              0,
                              3,
                              100.0f,
                              100.0f,
                              0.0f,
                              0.0f,
                              5,
                              10.0f,
                              10.0f,
                              1,
                              0,
                              bossManager,
                              playerManager);

        submissionManager = SubmissionManager.GetInstance();
        SubmissionManager.FontSmall = pericles20;
        SubmissionManager.FontBig = pericles32;
        SubmissionManager.Texture = menuSheet;
        SubmissionManager.GameInput = gameInput;

        mainMenuManager = new MainMenuManager(menuSheet, gameInput);

        levelManager = new LevelManager();
        levelManager.Register(asteroidManager);
        levelManager.Register(enemyManager);
        levelManager.Register(bossManager);
        levelManager.Register(playerManager);

        instructionManager = new InstructionManager(spriteSheet,
                                                    pericles20,
                                                    new Rectangle(0, 0, WIDTH, HEIGHT),
                                                    asteroidManager,
                                                    playerManager,
                                                    enemyManager,
                                                    bossManager,
                                                    powerUpManager);

        SoundManager.PlayBackgroundSound();


        settingsManager = SettingsManager.GetInstance();
        settingsManager.Initialize(menuSheet, pericles20);
        SettingsManager.GameInput = gameInput;

        this.creditsManager = CreditsManager.GetInstance();
        spaceshipAndPhonePositionManager = new SpaceshipManager(menuSheet,
                                                spriteSheet,
                                                pericles20,
                                                playerManager,
                                                creditsManager);
        SpaceshipManager.GameInput = gameInput;

        handManager = new HandManager(handSheet);

        setupInputs();
    }

    private void setupInputs()
    {
        gameInput.AddTouchGestureInput(TitleAction, GestureType.Tap, new Rectangle(0, 0, WIDTH, HEIGHT));
        gameInput.AddTouchGestureInput(BackToGameAction, GestureType.Tap, continueDestination);
        gameInput.AddTouchGestureInput(BackToMainAction, GestureType.Tap, cancelDestination);
        mainMenuManager.SetupInputs();
        playerManager.SetupInputs();
        submissionManager.SetupInputs();
        settingsManager.SetupInputs();
        spaceshipAndPhonePositionManager.SetupInputs();
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
    }

    /// <summary>
    /// Pauses the game when a call is incoming.
    /// Attention: Also called for GUID !!!
    /// </summary>
    protected override void OnDeactivated(object sender, EventArgs args)
    {
        base.OnDeactivated(sender, args);

        if (gameState == GameStates.Playing
            || gameState == GameStates.PlayerDead
            || gameState == GameStates.Instructions
            || gameState == GameStates.BossDuell
            || gameState == GameStates.AsteroidShower)
        {
            stateBeforePaused = gameState;
            gameState = GameStates.Paused;
        }
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        TouchPanel.DisplayHeight = HEIGHT;
        TouchPanel.DisplayWidth = WIDTH;

        SoundManager.Update(gameTime);

        handManager.Update(gameTime);

        gameInput.BeginUpdate();

        backButtonTimer += elapsed;

        if (backButtonTimer >= backButtonDelayTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                backButtonPressed = true;
                backButtonTimer = 0.0f;
            }
        }

        switch (gameState)
        {
            case GameStates.TitleScreen:

                titleScreenTimer += elapsed;

                updateBackground(gameTime);

                EffectManager.Update(gameTime);

                if (titleScreenTimer >= titleScreenDelayTime)
                {
                    if (gameInput.IsPressed(TitleAction))
                    {
                        gameState = GameStates.MainMenu;
                        SoundManager.PlayPaperSound();
                    }
                }

                if (backButtonPressed)
                    this.Exit();

                break;

            case GameStates.MainMenu:

                updateBackground(gameTime);
                asteroidManager.IsActive = true;

                EffectManager.Update(gameTime);

                mainMenuManager.IsActive = true;
                mainMenuManager.Update(gameTime);

                handManager.ShowHands();

                switch (mainMenuManager.LastPressedMenuItem)
                {
                    case MainMenuManager.MenuItems.Start:
                        gameState = GameStates.SelectShipAndPhonePosition;
                        break;

                    case MainMenuManager.MenuItems.Highscores:
                        // TODO GPGS leaderboards
                        break;

                    case MainMenuManager.MenuItems.Instructions:
                        resetGame();
                        settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle20);
                        asteroidManager.Reset();
                        playerManager.SelectPlayerType(PlayerManager.PlayerType.Easy);
                        instructionManager.Reset();
                        instructionManager.IsAutostarted = false;
                        updateHud();
                        gameState = GameStates.Instructions;
                        break;

                    case MainMenuManager.MenuItems.Settings:
                        gameState = GameStates.Settings;
                        break;

                    case MainMenuManager.MenuItems.None:
                        // do nothing
                        break;
                }

                if (gameState != GameStates.MainMenu)
                {
                    mainMenuManager.IsActive = false;
                    if (gameState == GameStates.Instructions)
                        handManager.HideHandsAndScribble();
                    else
                        handManager.HideHands();
                }

                if (backButtonPressed)
                    this.Exit();

                break;

            case GameStates.SelectShipAndPhonePosition:
                updateBackground(gameTime);

                EffectManager.Update(gameTime);

                spaceshipAndPhonePositionManager.IsActive = true;
                spaceshipAndPhonePositionManager.Update(gameTime);

                zoomTextManager.Update();

                if (spaceshipAndPhonePositionManager.CancelClicked || backButtonPressed)
                {
                    spaceshipAndPhonePositionManager.IsActive = false;
                    gameState = GameStates.MainMenu;
                    SoundManager.PlayPaperSound();
                }
                else if (spaceshipAndPhonePositionManager.GoClicked)
                {
                    spaceshipAndPhonePositionManager.IsActive = false;

                    resetGame();
                    updateHud();

                    handManager.HideHandsAndScribble();
                    if (instructionManager.HasDoneInstructions)
                    {
                        gameState = GameStates.Playing;
                    }
                    else
                    {
                        instructionManager.Reset();
                        instructionManager.IsAutostarted = true;
                        gameState = GameStates.Instructions;
                    }
                    SoundManager.PlayPaperSound();
                }

                break;

            case GameStates.Submittion:

                updateBackground(gameTime);

                EffectManager.Update(gameTime);

                submissionManager.IsActive = true;
                submissionManager.Update(gameTime);

                zoomTextManager.Reset();

                if (submissionManager.CancelClicked || backButtonPressed)
                {
                    submissionManager.IsActive = false;
                    gameState = GameStates.MainMenu;
                    SoundManager.PlayPaperSound();
                }
                else if (submissionManager.RetryClicked)
                {
                    submissionManager.IsActive = false;
                    resetGame();
                    updateHud();
                    handManager.HideHandsAndScribble();
                    gameState = GameStates.Playing;
                }
                break;

            case GameStates.Instructions:

                starFieldManager1.Update(gameTime);

                levelManager.SetLevel(1);

                EffectManager.Update(gameTime);
                playerManager.CanUpgrade = true;
                instructionManager.Update(gameTime);
                collisionManager.Update();
                EffectManager.Update(gameTime);
                updateHud();

                zoomTextManager.Update();

                if (backButtonPressed)
                {
                    if (!instructionManager.HasDoneInstructions && instructionManager.EnougthInstructionsDone)
                    {
                        instructionManager.InstructionsDone();
                        instructionManager.SaveHasDoneInstructions();
                    }

                    EffectManager.Reset();
                    if (instructionManager.IsAutostarted)
                    {
                        resetGame();
                        updateHud();
                        handManager.HideHandsAndScribble();
                        gameState = GameStates.Playing;
                    }
                    else
                    {
                        gameState = GameStates.MainMenu;
                        SoundManager.PlayPaperSound();
                    }
                }

                break;

            case GameStates.Settings:

                updateBackground(gameTime);

                EffectManager.Update(gameTime);

                settingsManager.IsActive = true;
                settingsManager.Update(gameTime);

                if (settingsManager.CancelClicked || backButtonPressed)
                {
                    settingsManager.IsActive = false;
                    gameState = GameStates.MainMenu;
                    SoundManager.PlayPaperSound();
                }

                break;

            case GameStates.Playing:

                updateBackground(gameTime);

                levelManager.Update(gameTime);

                playerManager.Update(gameTime);
                playerManager.CanUpgrade = true;

                enemyManager.Update(gameTime);
                enemyManager.IsActive = true;

                bossManager.Update(gameTime);

                EffectManager.Update(gameTime);

                collisionManager.Update();

                powerUpManager.Update(gameTime);

                zoomTextManager.Update();

                updateHud();

                if (levelManager.HasChanged)
                {
                    enemyManager.IsActive = false;

                    if (enemyManager.Enemies.Count == 0)
                    {
                        if (levelManager.LevelState == LevelManager.LevelStates.Enemies1)
                        {
                            asteroidManager.AcivateShower();
                            gameState = GameStates.AsteroidShower;
                        }
                        else if (levelManager.LevelState == LevelManager.LevelStates.Enemies2)
                        {
                            bossManager.SpawnRandomBoss();
                            gameState = GameStates.BossDuell;
                        }

                        levelManager.GoToNextState();
                    }
                }

                if (playerManager.IsDestroyed)
                {
                    playerDeathTimer = 0.0f;
                    enemyManager.IsActive = false;
                    bossManager.IsActive = false;
                    playerManager.DecreaseLives();

                    if (playerManager.LivesRemaining < 0)
                    {
                        //levelManager.ResetLevelTimer();
                        levelManager.GoToLastEnemyState();
                        gameState = GameStates.GameOver;
                        zoomTextManager.ShowText(GameOverText);
                    }
                    else
                    {
                        //levelManager.ResetLevelTimer();
                        levelManager.GoToLastEnemyState();
                        gameState = GameStates.PlayerDead;
                    }
                }

                if (backButtonPressed)
                {
                    stateBeforePaused = GameStates.Playing;
                    gameState = GameStates.Paused;
                    SoundManager.PlayPaperSound();
                }

                break;

            case GameStates.AsteroidShower:

                updateBackground(gameTime);

                levelManager.Update(gameTime);

                playerManager.Update(gameTime);
                playerManager.CanUpgrade = true;

                enemyManager.Update(gameTime);
                enemyManager.IsActive = false;

                bossManager.Update(gameTime);

                EffectManager.Update(gameTime);

                collisionManager.Update();

                powerUpManager.Update(gameTime);

                zoomTextManager.Update();

                updateHud();

                if (levelManager.HasChanged)
                {
                    enemyManager.IsActive = true;

                    asteroidManager.DeactivateShower();
                    levelManager.GoToNextState();

                    gameState = GameStates.Playing;
                }

                if (playerManager.IsDestroyed)
                {
                    playerDeathTimer = 0.0f;
                    enemyManager.IsActive = false;
                    bossManager.IsActive = false;
                    playerManager.DecreaseLives();

                    if (playerManager.LivesRemaining < 0)
                    {
                        levelManager.GoToLastEnemyState();
                        gameState = GameStates.GameOver;
                        zoomTextManager.ShowText(GameOverText);
                    }
                    else
                    {
                        levelManager.GoToLastEnemyState();
                        gameState = GameStates.PlayerDead;
                    }
                }

                if (backButtonPressed)
                {
                    stateBeforePaused = GameStates.AsteroidShower;
                    gameState = GameStates.Paused;
                    SoundManager.PlayPaperSound();
                }

                break;

            case GameStates.BossDuell:

                updateBackground(gameTime);

                levelManager.Update(gameTime);

                playerManager.Update(gameTime);
                playerManager.CanUpgrade = false;

                enemyManager.Update(gameTime);

                bossManager.Update(gameTime);

                EffectManager.Update(gameTime);

                collisionManager.Update();

                powerUpManager.Update(gameTime);

                zoomTextManager.Update();

                updateHud();

                if (bossManager.Bosses.Count == 0 && levelManager.HasChanged)
                {
                    if (bossManager.BossWasKilled)
                    {
                        bossManager.BossWasKilled = false;

                        if (bossDirectKill)
                        {
                            zoomTextManager.ShowText("+" + bossBonusScore + " Bonus");
                            playerManager.IncreasePlayerScore(bossBonusScore);
                            bossBonusScore += InitialBossBonusScore;
                        }
                        else
                        {
                            bossBonusScore = InitialBossBonusScore;
                        }

                        levelManager.GoToNextState();

                        bossDirectKill = true;
                    }
                    else
                    {
                        bossDirectKill = false;

                        levelManager.GoToLastEnemyState();
                    }

                    gameState = GameStates.Playing;
                }

                if (playerManager.IsDestroyed)
                {
                    playerDeathTimer = 0.0f;
                    enemyManager.IsActive = false;
                    bossManager.IsActive = false;
                    playerManager.DecreaseLives();

                    if (playerManager.LivesRemaining < 0)
                    {
                        gameState = GameStates.GameOver;
                        zoomTextManager.ShowText(GameOverText);
                    }
                    else
                    {
                        gameState = GameStates.PlayerDead;
                        bossDirectKill = false;
                    }

                    levelManager.GoToLastEnemyState();
                }

                if (backButtonPressed)
                {
                    stateBeforePaused = GameStates.BossDuell;
                    gameState = GameStates.Paused;
                    SoundManager.PlayPaperSound();
                }

                break;

            case GameStates.Paused:

                if (gameInput.IsPressed(BackToGameAction) || backButtonPressed)
                {
                    gameState = stateBeforePaused;
                    SoundManager.PlayPaperSound();
                }

                if (gameInput.IsPressed(BackToMainAction))
                {
                    SoundManager.PlayPaperSound();

                    if (playerManager.PlayerScore > 0)
                    {
                        gameState = GameStates.Submittion;
                        creditsManager.IncreaseTotalCredits(playerManager.Credits);
                        submissionManager.SetUp(playerManager.PlayerScore, levelManager.CurrentLevel, playerManager.Credits);
                    }
                    else
                    {
                        gameState = GameStates.MainMenu;
                    }

                    asteroidManager.DeactivateShower();
                }

                break;

            case GameStates.PlayerDead:

                playerDeathTimer += elapsed;

                updateBackground(gameTime);
                asteroidManager.Update(gameTime);
                asteroidManager.IsActive = false;
                starFieldManager1.SpeedFactor = 3.0f;

                playerManager.PlayerShotManager.Update(gameTime);
                playerManager.PlayerShotManager.Update(gameTime);

                powerUpManager.Update(gameTime);

                enemyManager.Update(gameTime);
                enemyManager.Update(gameTime);
                bossManager.Update(gameTime);
                bossManager.Update(gameTime);
                EffectManager.Update(gameTime);
                EffectManager.Update(gameTime);
                collisionManager.Update();
                zoomTextManager.Update();
                updateHud();

                if (playerDeathTimer >= playerDeathDelayTime)
                {
                    starFieldManager1.SpeedFactor = 1.0f;
                    asteroidManager.IsActive = true;
                    resetRound();
                    gameState = GameStates.Playing;
                    asteroidManager.DeactivateShower();
                }

                if (backButtonPressed)
                {
                    starFieldManager1.SpeedFactor = 1.0f;
                    asteroidManager.IsActive = true;
                    stateBeforePaused = GameStates.PlayerDead;
                    gameState = GameStates.Paused;
                    SoundManager.PlayPaperSound();
                }

                break;

            case GameStates.GameOver:

                playerDeathTimer += elapsed;

                updateBackground(gameTime);

                playerManager.PlayerShotManager.Update(gameTime);
                powerUpManager.Update(gameTime);
                enemyManager.Update(gameTime);
                bossManager.Update(gameTime);
                EffectManager.Update(gameTime);
                collisionManager.Update();
                zoomTextManager.Update();
                updateHud();

                if (playerDeathTimer >= playerGameOverDelayTime)
                {
                    asteroidManager.DeactivateShower();

                    playerDeathTimer = 0.0f;
                    titleScreenTimer = 0.0f;

                    if (playerManager.PlayerScore > 0)
                    {
                        gameState = GameStates.Submittion;
                        creditsManager.IncreaseTotalCredits(playerManager.Credits);
                        submissionManager.SetUp(playerManager.PlayerScore, levelManager.CurrentLevel, playerManager.Credits);
                    }
                    else
                    {
                        gameState = GameStates.MainMenu;
                    }
                }

                if (backButtonPressed)
                {
                    stateBeforePaused = GameStates.GameOver;
                    gameState = GameStates.Paused;
                    SoundManager.PlayPaperSound();
                }

                break;
        }

        // Reset Back-Button flag
        backButtonPressed = false;

        gameInput.EndUpdate();

        base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin(transformMatrix: screenScaleMatrix);

        if (gameState == GameStates.TitleScreen)
        {
            drawBackground(spriteBatch);

            EffectManager.Draw(spriteBatch);

            // title
            spriteBatch.Draw(menuSheet,
                             new Vector2(0.0f, 200.0f),
                             new Rectangle(0, 0, WIDTH, 200),
                             Color.White);

            spriteBatch.DrawString(pericles20,
                                   ContinueText,
                                   new Vector2(WIDTH / 2 - pericles20.MeasureString(ContinueText).X / 2,
                                               455),
                                   Color.Black * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

            spriteBatch.DrawString(pericles20,
                                   MusicByText,
                                   new Vector2(WIDTH / 2 - pericles20.MeasureString(MusicByText).X / 2,
                                               634),
                                   Color.Black);
            spriteBatch.DrawString(pericles20,
                                   MusicCreatorText,
                                   new Vector2(WIDTH / 2 - pericles20.MeasureString(MusicCreatorText).X / 2,
                                               663),
                                   Color.Black);

            spriteBatch.DrawString(pericles18,
                                   VersionText,
                                   new Vector2(WIDTH - (pericles18.MeasureString(VersionText).X + 15),
                                               HEIGHT - (pericles18.MeasureString(VersionText).Y + 10)),
                                   Color.Black);

            spriteBatch.DrawString(pericles18,
                                   CreatorText,
                                   new Vector2(15, HEIGHT - (pericles18.MeasureString(CreatorText).Y + 10)),
                                   Color.Black);
        }

        if (gameState == GameStates.MainMenu)
        {
            drawBackground(spriteBatch);

            EffectManager.Draw(spriteBatch);

            mainMenuManager.Draw(spriteBatch);
        }

        if (gameState == GameStates.SelectShipAndPhonePosition)
        {
            drawBackground(spriteBatch);

            EffectManager.Draw(spriteBatch);

            spaceshipAndPhonePositionManager.Draw(spriteBatch);

            zoomTextManager.Draw(spriteBatch);
        }

        if (gameState == GameStates.Submittion)
        {
            drawBackground(spriteBatch);

            EffectManager.Draw(spriteBatch);

            submissionManager.Draw(spriteBatch);
        }

        if (gameState == GameStates.Instructions)
        {
            drawPaper(spriteBatch);

            starFieldManager1.Draw(spriteBatch);

            instructionManager.Draw(spriteBatch);

            EffectManager.Draw(spriteBatch);

            zoomTextManager.Draw(spriteBatch);

            hud.Draw(spriteBatch);
        }

        if (gameState == GameStates.Settings)
        {
            drawBackground(spriteBatch);

            EffectManager.Draw(spriteBatch);

            settingsManager.Draw(spriteBatch);
        }

        if (gameState == GameStates.Paused)
        {
            drawBackground(spriteBatch);

            powerUpManager.Draw(spriteBatch);

            playerManager.Draw(spriteBatch);

            enemyManager.Draw(spriteBatch);

            bossManager.Draw(spriteBatch);

            EffectManager.Draw(spriteBatch);

            // Pause title

            spriteBatch.Draw(spriteSheet,
                             new Rectangle(0, 0, 480, 800),
                             new Rectangle(0, 550, 1, 1),
                             Color.White * 0.5f);

            spriteBatch.Draw(menuSheet,
                             new Vector2(0.0f, 250.0f),
                             new Rectangle(0, 200,
                                           480,
                                           100),
                             Color.White * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

            spriteBatch.Draw(menuSheet,
                             cancelDestination,
                             cancelSource,
                             Color.White);

            spriteBatch.Draw(menuSheet,
                             continueDestination,
                             continueSource,
                             Color.White);
        }

        if (gameState == GameStates.Playing ||
            gameState == GameStates.PlayerDead ||
            gameState == GameStates.GameOver ||
            gameState == GameStates.BossDuell ||
            gameState == GameStates.AsteroidShower)
        {
            drawBackground(spriteBatch);

            powerUpManager.Draw(spriteBatch);

            playerManager.Draw(spriteBatch);

            enemyManager.Draw(spriteBatch);

            bossManager.Draw(spriteBatch);

            EffectManager.Draw(spriteBatch);

            zoomTextManager.Draw(spriteBatch);

            hud.Draw(spriteBatch);
        }

        handManager.Draw(spriteBatch);

        spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Helper method to reduce update redundace.
    /// </summary>
    private void updateBackground(GameTime gameTime)
    {
        starFieldManager1.Update(gameTime);

        asteroidManager.Update(gameTime);
    }

    /// <summary>
    /// Helper method to reduce draw redundace.
    /// </summary>
    private void drawBackground(SpriteBatch spriteBatch)
    {
        drawPaper(spriteBatch);

        starFieldManager1.Draw(spriteBatch);

        asteroidManager.Draw(spriteBatch);
    }

    private void drawPaper(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(paperSheet,
                            new Rectangle(0, 0, WIDTH, HEIGHT),
                            new Rectangle(0, 0, WIDTH, HEIGHT),
                            Color.White);

        // vertical
        for (int x = 15; x < 480; x += 25)
        {
            spriteBatch.Draw(spriteSheet,
                                new Rectangle(x, 0, 2, HEIGHT),
                                new Rectangle(0, 550, 1, 1),
                                Color.Black * 0.1f);
        }

        // horizontal
        for (int y = 15; y < 800; y += 25)
        {
            spriteBatch.Draw(spriteSheet,
                                new Rectangle(0, y, WIDTH, 2),
                                new Rectangle(0, 550, 1, 1),
                                Color.Black * 0.1f);
        }
    }

    private void resetRound()
    {
        asteroidManager.Reset();
        enemyManager.Reset();
        bossManager.Reset();
        playerManager.Reset();
        EffectManager.Reset();
        powerUpManager.Reset();

        zoomTextManager.Reset();
    }

    private void resetGame()
    {
        resetRound();

        levelManager.Reset();

        playerManager.ResetPlayerScore();
        playerManager.ResetRemainingLives();
        playerManager.ResetSpecialWeapons();
        playerManager.resetUpgradeLevels();

        bossDirectKill = true;
        bossBonusScore = InitialBossBonusScore;

        GC.Collect();
    }

    /// <summary>
    /// Loads the current version from assembly.
    /// </summary>
    private void loadVersion()
    {
        System.Reflection.AssemblyName an = new System.Reflection.AssemblyName(System.Reflection.Assembly
                                                                               .GetExecutingAssembly()
                                                                               .FullName);
        this.VersionText = new StringBuilder().Append("v ")
                                              .Append(an.Version.Major)
                                              .Append('.')
                                              .Append(an.Version.Minor)
                                              .ToString();
    }

    private void updateHud()
    {
        hud.Update(playerManager.PlayerScore,
                   playerManager.LivesRemaining,
                   playerManager.HitPoints,
                   playerManager.InitMaxHitPoints,
                   playerManager.ShieldPoints,
                   playerManager.InitMaxShieldPoints,
                   playerManager.SpecialShotsRemaining,
                   playerManager.SpecialShotReloadTimeMax,
                   playerManager.SpecialShotReloadTime,
                   levelManager.CurrentLevel,
                   playerManager.Upgrades);
    }

    public void BackButtonPressed()
    {
        backButtonPressed = true;
    }
}

