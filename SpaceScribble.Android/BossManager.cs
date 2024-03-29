﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace SpaceScribble
{
    /// <summary>
    /// Manages the bosses.
    /// </summary>
    class BossManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Boss> Bosses = new List<Boss>();

        public ShotManager BossShotManager;
        private PlayerManager playerManager;

        public const int SOFT_ROCKET_EXPLOSION_RADIUS = 100;
        public const float ROCKET_POWER_AT_CENTER = 200.0f;

        public const int DAMAGE_ROCKET_MIN = 30;
        public const int DAMAGE_ROCKET_MAX = 35;
        public const int DAMAGE_LASER_MIN = 10;
        public const int DAMAGE_LASER_MAX = 12;

        public int ShipsPerWave = 1;

        private List<List<Vector2>> pathWayPoints = new List<List<Vector2>>();

        private Dictionary<int, WaveInfo> waveSpawns = new Dictionary<int, WaveInfo>();

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

        private readonly Rectangle screen = new Rectangle(0, 0, 480, 800);

        private float nextShotTimer = 0.0f;
        private float nextShotMinTimer = InitialNextShotMinTimer;
        private const float InitialNextShotMinTimer = 1.25f;

        private bool bossWasKilled = false;

        private int fireCounter;

        #endregion

        #region Constructors

        public BossManager(Texture2D texture, PlayerManager playerManager,
                           Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            BossShotManager = new ShotManager(texture,
                                               new Rectangle(650, 160, 20, 20),
                                               4,
                                               2,
                                               275.0f,
                                               new Rectangle(screenBounds.X - 50, screenBounds.Y - 50,
                                                             screenBounds.Width + 100, screenBounds.Height + 100));

            setUpWayPoints();

            this.currentLevel = 1;
        }

        #endregion

        #region Methods

        private void setUpWayPoints()
        {
            List<Vector2> path0 = new List<Vector2>();
            path0.Add(new Vector2(0, -100));
            path0.Add(new Vector2(400, 400));
            path0.Add(new Vector2(400, 500));
            path0.Add(new Vector2(300, 600));
            path0.Add(new Vector2(240, 500));
            path0.Add(new Vector2(240, 200));
            path0.Add(new Vector2(340, 300));
            path0.Add(new Vector2(240, 200));
            path0.Add(new Vector2(140, 300));
            path0.Add(new Vector2(240, 200));
            path0.Add(new Vector2(240, 500));
            path0.Add(new Vector2(180, 600));
            path0.Add(new Vector2(80, 500));
            path0.Add(new Vector2(80, 400));
            path0.Add(new Vector2(400, 400));
            path0.Add(new Vector2(80, 400));
            path0.Add(new Vector2(580, -100));
            pathWayPoints.Add(path0);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path1 = new List<Vector2>();
            path1.Add(new Vector2(480, -100));
            path1.Add(new Vector2(80, 400));
            path1.Add(new Vector2(80, 500));
            path1.Add(new Vector2(180, 600));
            path1.Add(new Vector2(240, 500));
            path1.Add(new Vector2(240, 300));
            path1.Add(new Vector2(340, 400));
            path1.Add(new Vector2(240, 300));
            path1.Add(new Vector2(140, 400));
            path1.Add(new Vector2(240, 300));
            path1.Add(new Vector2(240, 500));
            path1.Add(new Vector2(300, 500));
            path1.Add(new Vector2(400, 300));
            path1.Add(new Vector2(400, 100));
            path1.Add(new Vector2(-100, -100));
            pathWayPoints.Add(path1);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path2 = new List<Vector2>();
            path2.Add(new Vector2(240, -100));
            path2.Add(new Vector2(240, 0));
            path2.Add(new Vector2(50, 100));
            path2.Add(new Vector2(430, 200));
            path2.Add(new Vector2(50, 300));
            path2.Add(new Vector2(430, 400));
            path2.Add(new Vector2(50, 500));
            path2.Add(new Vector2(430, 600));
            path2.Add(new Vector2(240, 600));
            path2.Add(new Vector2(240, 100));
            path2.Add(new Vector2(140, 400));
            path2.Add(new Vector2(240, 100));
            path2.Add(new Vector2(340, 400));
            path2.Add(new Vector2(240, 100));
            path2.Add(new Vector2(300, 500));
            path2.Add(new Vector2(180, 500));
            path2.Add(new Vector2(240, 100));
            path2.Add(new Vector2(240, -100));
            pathWayPoints.Add(path2);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path3 = new List<Vector2>();
            path3.Add(new Vector2(240, -100));
            path3.Add(new Vector2(240, 0));
            path3.Add(new Vector2(430, 100));
            path3.Add(new Vector2(50, 200));
            path3.Add(new Vector2(430, 300));
            path3.Add(new Vector2(50, 400));
            path3.Add(new Vector2(430, 500));
            path3.Add(new Vector2(50, 600));
            path3.Add(new Vector2(240, 600));
            path3.Add(new Vector2(240, 100));
            path3.Add(new Vector2(340, 400));
            path3.Add(new Vector2(240, 100));
            path3.Add(new Vector2(140, 400));
            path3.Add(new Vector2(240, 100));
            path3.Add(new Vector2(180, 500));
            path3.Add(new Vector2(300, 500));
            path3.Add(new Vector2(240, 100));
            path3.Add(new Vector2(240, -100));
            pathWayPoints.Add(path3);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path4 = new List<Vector2>();
            path4.Add(new Vector2(-100, 100));
            path4.Add(new Vector2(100, 150));
            path4.Add(new Vector2(150, 200));
            path4.Add(new Vector2(150, 300));
            path4.Add(new Vector2(300, 400));
            path4.Add(new Vector2(300, 600));
            path4.Add(new Vector2(300, 100));
            path4.Add(new Vector2(100, 700));
            path4.Add(new Vector2(240, 300));
            path4.Add(new Vector2(240, 500));
            path4.Add(new Vector2(80, 400));
            path4.Add(new Vector2(240, 500));
            path4.Add(new Vector2(400, 400));
            path4.Add(new Vector2(240, 500));
            path4.Add(new Vector2(240, 100));
            path4.Add(new Vector2(240, 400));
            path4.Add(new Vector2(240, -100));
            pathWayPoints.Add(path4);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path5 = new List<Vector2>();
            path5.Add(new Vector2(580, 100));
            path5.Add(new Vector2(380, 150));
            path5.Add(new Vector2(330, 200));
            path5.Add(new Vector2(330, 300));
            path5.Add(new Vector2(180, 400));
            path5.Add(new Vector2(180, 600));
            path5.Add(new Vector2(180, 100));
            path5.Add(new Vector2(380, 700));
            path5.Add(new Vector2(240, 300));
            path5.Add(new Vector2(240, 500));
            path5.Add(new Vector2(400, 400));
            path5.Add(new Vector2(240, 500));
            path5.Add(new Vector2(80, 400));
            path5.Add(new Vector2(240, 500));
            path5.Add(new Vector2(240, 100));
            path5.Add(new Vector2(240, 400));
            path5.Add(new Vector2(240, -100));
            pathWayPoints.Add(path5);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path6 = new List<Vector2>();
            path6.Add(new Vector2(240, -100));
            path6.Add(new Vector2(240, 200));
            path6.Add(new Vector2(50, 200));
            path6.Add(new Vector2(430, 200));
            path6.Add(new Vector2(240, 200));
            path6.Add(new Vector2(240, 300));
            path6.Add(new Vector2(50, 300));
            path6.Add(new Vector2(430, 300));
            path6.Add(new Vector2(240, 300));
            path6.Add(new Vector2(240, 400));
            path6.Add(new Vector2(50, 400));
            path6.Add(new Vector2(430, 400));
            path6.Add(new Vector2(240, 400));
            path6.Add(new Vector2(240, 500));
            path6.Add(new Vector2(50, 500));
            path6.Add(new Vector2(430, 500));
            path6.Add(new Vector2(240, 500));
            path6.Add(new Vector2(240, 600));
            path6.Add(new Vector2(50, 600));
            path6.Add(new Vector2(430, 600));
            path6.Add(new Vector2(240, 600));
            path6.Add(new Vector2(240, 200));
            path6.Add(new Vector2(240, 600));
            path6.Add(new Vector2(100, 300));
            path6.Add(new Vector2(580, 200));
            pathWayPoints.Add(path6);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path7 = new List<Vector2>();
            path7.Add(new Vector2(240, -100));
            path7.Add(new Vector2(240, 200));
            path7.Add(new Vector2(50, 200));
            path7.Add(new Vector2(430, 200));
            path7.Add(new Vector2(240, 200));
            path7.Add(new Vector2(240, 300));
            path7.Add(new Vector2(50, 300));
            path7.Add(new Vector2(430, 300));
            path7.Add(new Vector2(240, 300));
            path7.Add(new Vector2(240, 400));
            path7.Add(new Vector2(50, 400));
            path7.Add(new Vector2(430, 400));
            path7.Add(new Vector2(240, 400));
            path7.Add(new Vector2(240, 500));
            path7.Add(new Vector2(50, 500));
            path7.Add(new Vector2(430, 500));
            path7.Add(new Vector2(240, 500));
            path7.Add(new Vector2(240, 600));
            path7.Add(new Vector2(50, 600));
            path7.Add(new Vector2(430, 600));
            path7.Add(new Vector2(240, 600));
            path7.Add(new Vector2(240, 200));
            path7.Add(new Vector2(240, 600));
            path7.Add(new Vector2(380, 300));
            path7.Add(new Vector2(-100, 200));
            pathWayPoints.Add(path7);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path8 = new List<Vector2>();
            path8.Add(new Vector2(-100, 400));
            path8.Add(new Vector2(100, 200));
            path8.Add(new Vector2(200, 100));
            path8.Add(new Vector2(280, 100));
            path8.Add(new Vector2(380, 200));
            path8.Add(new Vector2(430, 300));
            path8.Add(new Vector2(430, 350));
            path8.Add(new Vector2(380, 400));
            path8.Add(new Vector2(240, 500));
            path8.Add(new Vector2(240, 600));
            path8.Add(new Vector2(240, 300));
            path8.Add(new Vector2(240, 500));
            path8.Add(new Vector2(80, 500));
            path8.Add(new Vector2(400, 500));
            path8.Add(new Vector2(240, 500));
            path8.Add(new Vector2(200, 400));
            path8.Add(new Vector2(100, 300));
            path8.Add(new Vector2(400, -100));
            pathWayPoints.Add(path8);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path9 = new List<Vector2>();
            path9.Add(new Vector2(580, 400));
            path9.Add(new Vector2(380, 200));
            path9.Add(new Vector2(280, 100));
            path9.Add(new Vector2(200, 100));
            path9.Add(new Vector2(100, 200));
            path9.Add(new Vector2(50, 300));
            path9.Add(new Vector2(50, 350));
            path9.Add(new Vector2(100, 400));
            path9.Add(new Vector2(240, 500));
            path9.Add(new Vector2(240, 600));
            path9.Add(new Vector2(240, 300));
            path9.Add(new Vector2(240, 500));
            path9.Add(new Vector2(400, 500));
            path9.Add(new Vector2(80, 500));
            path9.Add(new Vector2(240, 500));
            path9.Add(new Vector2(280, 400));
            path9.Add(new Vector2(380, 300));
            path9.Add(new Vector2(80, -100));
            pathWayPoints.Add(path9);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path10 = new List<Vector2>();
            path10.Add(new Vector2(0, -100));
            path10.Add(new Vector2(50, 600));
            path10.Add(new Vector2(150, 100));
            path10.Add(new Vector2(240, 500));
            path10.Add(new Vector2(240, 300));
            path10.Add(new Vector2(80, 300));
            path10.Add(new Vector2(400, 300));
            path10.Add(new Vector2(240, 300));
            path10.Add(new Vector2(240, 500));
            path10.Add(new Vector2(240, 300));
            path10.Add(new Vector2(240, 500));
            path10.Add(new Vector2(80, 500));
            path10.Add(new Vector2(400, 500));
            path10.Add(new Vector2(240, 500));
            path10.Add(new Vector2(330, 100));
            path10.Add(new Vector2(430, 600));
            path10.Add(new Vector2(530, -0));
            pathWayPoints.Add(path10);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path11 = new List<Vector2>();
            path11.Add(new Vector2(480, -100));
            path11.Add(new Vector2(430, 600));
            path11.Add(new Vector2(330, 100));
            path11.Add(new Vector2(240, 500));
            path11.Add(new Vector2(240, 300));
            path11.Add(new Vector2(240, 500));
            path11.Add(new Vector2(240, 300));
            path11.Add(new Vector2(240, 500));
            path11.Add(new Vector2(80, 500));
            path11.Add(new Vector2(400, 500));
            path11.Add(new Vector2(240, 500));
            path11.Add(new Vector2(150, 100));
            path11.Add(new Vector2(50, 600));
            path11.Add(new Vector2(530, -0));
            pathWayPoints.Add(path11);
            waveSpawns.Add(waveSpawns.Count, new WaveInfo(0, EnemyType.Easy));
        }

        private void spawnBoss(int path, EnemyType type)
        {
            Boss newBoss;

            switch (type)
            {
                case EnemyType.Medium:
                    newBoss = Boss.CreateMediumBoss(texture,
                                                    pathWayPoints[path][0]);
                    SoundManager.PlayBossMediumSound();
                    break;

                case EnemyType.Hard:
                    newBoss = Boss.CreateHardBoss(texture,
                                                  pathWayPoints[path][0]);
                    SoundManager.PlayBossHardSound();
                    break;

                case EnemyType.Speeder:
                    newBoss = Boss.CreateSpeederBoss(texture,
                                                  pathWayPoints[path][0]);
                    SoundManager.PlayBossSpeederSound();
                    break;

                case EnemyType.Tank:
                    newBoss = Boss.CreateTankBoss(texture,
                                                  pathWayPoints[path][0]);
                    SoundManager.PlayBossTankSound();
                    break;

                default:
                    newBoss = Boss.CreateEasyBoss(texture,
                                                  pathWayPoints[path][0]);
                    SoundManager.PlayBossEasySound();
                    break;
            }

            newBoss.SetLevel(currentLevel);

            for (int x = 0; x < pathWayPoints[path].Count; x++)
            {
                newBoss.AddWayPoint(pathWayPoints[path][x]);
            }

            Bosses.Add(newBoss);
        }

        public void SpawnRandomBoss()
        {
            EnemyType type;

            //int rnd = rand.Next(0, 5);
            int rnd = currentLevel % 5;

            switch (rnd)
            {
                case 0:
                    type = EnemyType.Tank;
                    break;

                case 1:
                    type = EnemyType.Easy;
                    break;

                case 2:
                    type = EnemyType.Medium;
                    break;

                case 3:
                    type = EnemyType.Speeder;
                    break;

                default:
                    type = EnemyType.Hard;
                    break;
            }

            int rndPath = rand.Next(pathWayPoints.Count);

            spawnBoss(rndPath, type);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            BossShotManager.Update(gameTime);

            for (int x = Bosses.Count - 1; x >= 0; --x)
            {
                Bosses[x].Update(gameTime);

                if (Bosses[x].IsDestroyed)
                {
                    BossWasKilled = true;
                }

                if (!Bosses[x].IsActive())
                {
                    Bosses.RemoveAt(x);
                }
                else
                {
                    nextShotTimer += elapsed;

                    if (nextShotTimer > nextShotMinTimer &&
                        !playerManager.IsDestroyed &&
                         screen.Contains((int)Bosses[x].BossSprite.Center.X,
                                         (int)Bosses[x].BossSprite.Center.Y))
                    {
                        ++fireCounter;

                        if (Bosses[x].HitPoints / Bosses[x].MaxHitPoints < 0.25f)
                        {
                            nextShotTimer = nextShotMinTimer / 4;
                        }
                        else if (Bosses[x].HitPoints / Bosses[x].MaxHitPoints < 0.5f)
                        {
                            nextShotTimer = nextShotMinTimer / 8;
                        }
                        else
                        {
                            nextShotTimer = 0.0f;
                        }

                        Vector2 fireLocation = Bosses[x].BossSprite.Center;

                        Vector2 shotDirection = ((playerManager.playerSprite.Center + playerManager.playerSprite.Velocity / (3.5f + (float)rand.NextDouble() * 4.5f)) - fireLocation);

                        shotDirection.Normalize();

                        Matrix m = new Matrix();
                        m.M11 = (float)Math.Cos(Math.PI / 2);
                        m.M12 = (float)-Math.Sin(Math.PI / 2);
                        m.M21 = (float)Math.Sin(Math.PI / 2);
                        m.M22 = (float)Math.Cos(Math.PI / 2);

                        Vector2 fireLocationOffset15 = Vector2.Transform(shotDirection, m) * 15;
                        Vector2 fireLocationOffset8 = Vector2.Transform(shotDirection, m) * 8;

                        if (Bosses[x].Type == EnemyType.Easy)
                        {
                            Color c = new Color(1.0f, 1.0f, 0.1f);

                            if (fireCounter % 2 == 0)
                            {
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                         shotDirection,
                                                         false,
                                                         c,
                                                         true);
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                         shotDirection,
                                                         false,
                                                         c,
                                                         true);
                            }
                            else
                            {
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                      Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(5f))),
                                                      false,
                                                      c,
                                                      true);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-5f))),
                                                          false,
                                                          c,
                                                          true);
                            }
                        }
                        else if (Bosses[x].Type == EnemyType.Medium)
                        {
                            Color c = new Color(0.1f, 1.0f, 1.0f);

                            if (fireCounter % 2 == 0)
                            {
                                BossShotManager.FireShot(fireLocation,
                                                          shotDirection,
                                                          false,
                                                          c,
                                                          true);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                          shotDirection,
                                                          false,
                                                          c,
                                                          true);
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                          shotDirection,
                                                          false,
                                                          c,
                                                          false);
                            }
                            else
                            {
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                      Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(10f))),
                                                      false,
                                                      c,
                                                      true);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-10f))),
                                                          false,
                                                          c,
                                                          true);
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                      Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(15f))),
                                                      false,
                                                      c,
                                                      false);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-15f))),
                                                          false,
                                                          c,
                                                          false);
                            }
                        }
                        else if (Bosses[x].Type == EnemyType.Hard)
                        {
                            Color c = new Color(0.8f, 0.5f, 1.0f);

                            if (fireCounter % 5 == 0)
                            {
                                BossShotManager.FireRocket(fireLocation,
                                                             shotDirection,
                                                             false,
                                                             Color.White,
                                                             true);
                            }
                            else
                            {
                                BossShotManager.FireShot(fireLocation,
                                                          shotDirection,
                                                          false,
                                                          c,
                                                          true);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset8,
                                                          shotDirection,
                                                          false,
                                                          c,
                                                          true);
                                BossShotManager.FireShot(fireLocation - fireLocationOffset8,
                                                          shotDirection,
                                                          false,
                                                          c,
                                                          false);
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(10f))),
                                                          false,
                                                          c,
                                                          false);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-10f))),
                                                          false,
                                                          c,
                                                          false);
                            }
                        }
                        else if (Bosses[x].Type == EnemyType.Speeder)
                        {
                            BossShotManager.FireShot(fireLocation,
                                                      shotDirection,
                                                      false,
                                                      new Color(1.0f, 0.1f, 1.0f),
                                                      true);
                            BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                      Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(7.5f))),
                                                      false,
                                                      new Color(1.0f, 0.1f, 1.0f),
                                                      true);
                            BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                      Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-7.5f))),
                                                      false,
                                                      new Color(1.0f, 0.1f, 1.0f),
                                                      false);


                        }
                        else if (Bosses[x].Type == EnemyType.Tank)
                        {
                            if (fireCounter % 3 == 0)
                            {
                                BossShotManager.FireShot(fireLocation,
                                                         shotDirection,
                                                         false,
                                                         Color.Black,
                                                         false);

                                BossShotManager.FireShot(fireLocation - fireLocationOffset8,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(5f))),
                                                          false,
                                                          Color.Black,
                                                          true);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset8,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-5f))),
                                                          false,
                                                          Color.Black,
                                                          false);
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(10f))),
                                                          false,
                                                          Color.Black,
                                                          true);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-10f))),
                                                          false,
                                                          Color.Black,
                                                          false);
                            }
                            else
                            {
                                BossShotManager.FireRocket(fireLocation + fireLocationOffset15,
                                                                 shotDirection,
                                                                 false,
                                                                 Color.White,
                                                                 true);
                                BossShotManager.FireRocket(fireLocation - fireLocationOffset15,
                                                                 shotDirection,
                                                                 false,
                                                                 Color.White,
                                                                 false);
                                BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(10f))),
                                                          false,
                                                          Color.Black,
                                                          true);
                                BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                          Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-10f))),
                                                          false,
                                                          Color.Black,
                                                          false);
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            BossShotManager.Draw(spriteBatch);

            foreach (var boss in Bosses)
            {
                boss.Draw(spriteBatch);
            }
        }

        public void Reset()
        {
            this.Bosses.Clear();
            this.BossShotManager.Shots.Clear();

            for (int i = 0; i < waveSpawns.Count; i++)
            {
                waveSpawns[i] = new WaveInfo(0, EnemyType.Easy);
            }

            this.IsActive = true;

            fireCounter = 0;
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            //this.nextShotMinTimer = Math.Max(InitialNextShotMinTimer - ((lvl - 1) * 0.02f), 0.5f);
            this.nextShotMinTimer = Math.Max(InitialNextShotMinTimer - (((lvl - 1) / 5) * 0.1f), 0.5f);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Bosses
            int bossesCount = Int32.Parse(reader.ReadLine());
            Bosses.Clear();

            for (int i = 0; i < bossesCount; ++i)
            {
                EnemyType type = EnemyType.Easy;
                Boss b;

                type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                switch (type)
                {
                    case EnemyType.Easy:
                        b = Boss.CreateEasyBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Medium:
                        b = Boss.CreateMediumBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Hard:
                        b = Boss.CreateHardBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Speeder:
                        b = Boss.CreateSpeederBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Tank:
                        b = Boss.CreateTankBoss(texture, Vector2.Zero);
                        break;
                    default:
                        b = Boss.CreateEasyBoss(texture, Vector2.Zero);
                        break;
                }
                b.Activated(reader);

                Bosses.Add(b);
            }

            BossShotManager.Activated(reader);

            this.ShipsPerWave = Int32.Parse(reader.ReadLine());

            // Wave spawns
            int waveSpawnsCount = Int32.Parse(reader.ReadLine());
            // Note: no list clearing here, because the list is setted up at startup.
            for (int i = 0; i < waveSpawnsCount; ++i)
            {
                int idx = Int32.Parse(reader.ReadLine());
                WaveInfo waveInfo = new WaveInfo();
                waveInfo.Activated(reader);
                waveSpawns[idx] = waveInfo;
            }

            this.IsActive = Boolean.Parse(reader.ReadLine());

            this.currentLevel = Int32.Parse(reader.ReadLine());

            this.nextShotTimer = Single.Parse(reader.ReadLine());
            this.nextShotMinTimer = Single.Parse(reader.ReadLine());

            this.bossWasKilled = Boolean.Parse(reader.ReadLine());

            this.fireCounter = Int32.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            //Bosses
            writer.WriteLine(Bosses.Count);

            for (int i = 0; i < Bosses.Count; ++i)
            {
                writer.WriteLine(Bosses[i].Type);
                Bosses[i].Deactivated(writer);
            }

            BossShotManager.Deactivated(writer);

            writer.WriteLine(ShipsPerWave);

            // Wave spawns
            writer.WriteLine(waveSpawns.Count);

            foreach (var waveSpawn in waveSpawns)
            {
                writer.WriteLine(waveSpawn.Key);
                waveSpawn.Value.Deactivated(writer);
            }

            writer.WriteLine(IsActive);

            writer.WriteLine(currentLevel);

            writer.WriteLine(nextShotTimer);
            writer.WriteLine(nextShotMinTimer);

            writer.WriteLine(bossWasKilled);

            writer.WriteLine(fireCounter);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns and resets the BossWasKilled Flag.
        /// </summary>
        public bool BossWasKilled
        {
            get
            {
                //bool temp = this.bossWasKilled;
                //this.bossWasKilled = false;
                //return temp;
                return bossWasKilled;
            }
            set
            {
                bossWasKilled = value;
            }
        }

        #endregion
    }
}