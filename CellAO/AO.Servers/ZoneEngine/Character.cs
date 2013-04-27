#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    public class Character : Dynel
    {
        /// <summary>
        /// Character's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Character's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Organization ID
        /// </summary>
        private int orgId;

        /// <summary>
        /// Organization name
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// Is Character attacking?
        /// </summary>
        public int Attacking { get; set; }

        /// <summary>
        /// Movement mode
        /// </summary>
        public int MovementMode { get; set; }

        /// <summary>
        /// Don't process timers when true, double use, stats aren't sent back to client(s) when true
        /// </summary>
        private bool doNotDoTimers = true;

        /// <summary>
        /// Identity of the actual target
        /// </summary>
        public Identity Target { get; set; }

        /// <summary>
        /// Identity of the fighting target
        /// </summary>
        public Identity FightingTarget { get; set; }

        /// <summary>
        /// Identity of the followed target
        /// </summary>
        public Identity FollowTarget { get; set; }

        /// <summary>
        /// Identity of the last trading target
        /// </summary>
        public Identity LastTrade { get; set; }

        /// <summary>
        /// Stats
        /// </summary>
        private CharacterStats stats;

        /// <summary>
        /// Inventory list
        /// </summary>
        private readonly List<InventoryEntries> inventory = new List<InventoryEntries>();

        /// <summary>
        /// Active Nanos list
        /// </summary>
        private readonly List<AONano> activeNanos = new List<AONano>();

        /// <summary>
        /// Active Timers list (HD/ND, Nano effect timers etc)
        /// </summary>
        private readonly List<AOTimers> timers = new List<AOTimers>();

        /// <summary>
        /// Uploaded Nanos list
        /// </summary>
        private readonly List<AOUploadedNanos> uploadedNanos = new List<AOUploadedNanos>();

        /// <summary>
        /// Bank inventory
        /// </summary>
        private readonly List<AOItem> bank = new List<AOItem>();

        /// <summary>
        /// Caching Mesh layer structure
        /// </summary>
        private MeshLayers meshLayer = new MeshLayers();

        /// <summary>
        /// Caching Mesh layer for social tab items
        /// </summary>
        private MeshLayers socialMeshLayer = new MeshLayers();

        // Not sure what that is... Need to check my notes - Algorithman
        // declare Weaponsm Bool Array
        private readonly bool[] weaponsStored = new bool[109];

        private readonly bool[] itemsStored = new bool[109];

        // KnuBot Target

        public Dynel KnuBotTarget { get; set; }

        /// <summary>
        /// Create a new Character Dynel
        /// </summary>
        /// <param name="id">ID of player</param>
        /// <param name="playfield">initial Playfield number</param>
        public Character(int id, int playfield)
            : base(id, playfield)
        {
            lock (this)
            {
                this.Id = id;
                this.PlayField = playfield;
                // We're in the character class, so set Identifier 50000
                this.Type = 50000;
                this.OurType = 0;
                if (this.Id != 0)
                {
                    this.doNotDoTimers = true;
                    this.stats = new CharacterStats(this);
                    this.ReadCoordsFromSql();
                    this.ReadHeadingFromSql();
                    this.stats.ReadStatsfromSql();
                    this.ReadSpecialStatsFromSql();
                    this.ReadTimersFromSql();
                    this.ReadInventoryFromSql();
                    this.ReadUploadedNanosFromSql();
                    this.ReadBankContentsFromSql();
                    //                    MeshLayer.AddMesh(0, (Int32)Stats.HeadMesh.StatBaseValue, 0, 8);
                    //                    SocialMeshLayer.AddMesh(0, (Int32)Stats.HeadMesh.StatBaseValue, 0, 8);
                    this.ReadSocialTab();
                    //ApplyInventory();
                    //                    CalculateNextXP();
                    this.AddHpnpTick();
                    this.Starting = false;
                }
            }
        }

        public Character()
        {
        }

        /// <summary>
        /// Dispose Character Dynel
        /// </summary>
        ~Character()
        {
            this.doNotDoTimers = true;
            lock (this)
            {
                this.WriteBankContentsToSql();
                this.SaveSocialTab();
                if (this.KnuBotTarget != null)
                {
                    ((NonPlayerCharacterClass)this.KnuBotTarget).KnuBot.TalkingTo = null;
                }
                this.Purge();
            }
        }

        #region AddTimer
        /// <summary>
        /// Add a new Timer
        /// </summary>
        /// <param name="strain">Strain number</param>
        /// <param name="time">Time to spark AOFunction aof</param>
        /// <param name="aof">The function the timer triggers</param>
        /// <param name="dolocalstats">process local stats?</param>
        public void AddTimer(int strain, DateTime time, AOFunctions aof, bool dolocalstats)
        {
            AOTimers newtimer = new AOTimers();
            newtimer.Function = aof.ShallowCopy();
            newtimer.Function.dolocalstats = dolocalstats;
            newtimer.Timestamp = time;
            newtimer.Strain = strain;
            lock (this.timers)
            {
                this.timers.Add(newtimer);
            }
        }
        #endregion

        #region PurgeTimer
        /// <summary>
        /// Remove Timer from strain
        /// </summary>
        /// <param name="strain">Strain to remove</param>
        public void PurgeTimer(int strain)
        {
            lock (this.timers)
            {
                int c = this.timers.Count() - 1;
                while (c >= 0)
                {
                    if (this.timers[c].Strain == strain)
                    {
                        this.timers.RemoveAt(c);
                    }
                    c--;
                }
            }
        }
        #endregion

        #region Process Timers
        /// <summary>
        /// Process timers
        /// </summary>
        /// <param name="now">Process all timers up to _now</param>
        public void ProcessTimers(DateTime now)
        {
            // Current Timer
            int c;
            // Current Strain
            int strain;
            // if Charachter is skipping timers Leave Function
            if (this.doNotDoTimers)
            {
                return;
            }

            lock (this)
            {
                // Backwards, easier to maintain integrity when removing something from the list
                for (c = this.timers.Count - 1; c >= 0; c--)
                {
                    strain = this.timers[c].Strain;
                    if (this.timers[c].Timestamp <= now)
                    {
                        this.ApplyFunction(this.timers[c].Function);

                        if (this.timers[c].Function.TickCount >= 0)
                        {
                            this.timers[c].Function.TickCount--;
                        }

                        if (this.timers[c].Function.TickCount == 0)
                        {
                            // Remove Timer if Ticks ran out
                            this.timers.RemoveAt(c);
                        }
                        else
                        {
                            // Reinvoke the timer after the TickInterval
                            this.timers[c].Timestamp = now
                                                       + TimeSpan.FromMilliseconds(this.timers[c].Function.TickInterval);
                        }
                    }
                }
            }
        }
        #endregion

        #region OrgId update fix
        /// <summary>
        /// This is here to prevent desync issues between this variable and the stats class.
        /// Stats are purged to the db when OrgId changes so that the ChatEngine can read
        /// the new org details.
        /// </summary>
        public uint OrgId
        {
            // Stat 5 (Clan) is org id
            // Stat 5 (ClanLevel) is org rank
            get
            {
                return this.stats.Clan.StatBaseValue;
            }
            set
            {
                string oldOrgName = this.OrgName;

                this.stats.Clan.Set(value);

                if (value == 0)
                {
                    this.stats.ClanLevel.Set(0);
                }
                lock (this)
                {
                    this.WriteStats();
                    this.WriteNames();

                    this.ReadNames();

                    if (oldOrgName != this.OrgName)
                    {
                        OrgInfo.OrgInfoPacket(this);
                    }
                }
            }
        }
        #endregion

        #region Movement prediction and update
        private DateTime predictionTime;

        /// <summary>
        /// Calculate predition Duration
        /// </summary>
        public TimeSpan PredictionDuration
        {
            get
            {
                DateTime currentTime = DateTime.Now;

                return currentTime - this.predictionTime;
            }
        }

        /// <summary>
        /// Calculate Turn time
        /// </summary>
        /// <returns>Turn time</returns>
        private double calculateTurnTime()
        {
            int turnSpeed;
            double turnTime;

            turnSpeed = this.stats.TurnSpeed.Value; // Stat #267 TurnSpeed

            if (turnSpeed == 0)
            {
                turnSpeed = 40000;
            }

            turnTime = 70000 / turnSpeed;

            return turnTime;
        }

        /// <summary>
        /// Calculate the effective run speed (run, walk, sneak etc)
        /// </summary>
        /// <returns>Effective run speed</returns>
        private int calculateEffectiveRunSpeed()
        {
            int effectiveRunSpeed;

            switch (this.moveMode)
            {
                case MoveModes.Run:
                    effectiveRunSpeed = this.stats.RunSpeed.Value; // Stat #156 = RunSpeed
                    break;

                case MoveModes.Walk:
                    effectiveRunSpeed = -500;
                    break;

                case MoveModes.Swim:
                    // Swim speed is calculated the same as Run Speed except is half as effective
                    effectiveRunSpeed = this.stats.Swim.Value >> 1; // Stat #138 = Swim
                    break;

                case MoveModes.Crawl:
                    effectiveRunSpeed = -600;
                    break;

                case MoveModes.Sneak:
                    effectiveRunSpeed = -500;
                    break;

                case MoveModes.Fly:
                    effectiveRunSpeed = 2200; // NV: TODO: Propper calc for this!
                    break;

                default:
                    // All other movement modes, sitting, sleeping, lounging, rooted, etc have a speed of 0
                    // As there is no way to 'force' that this way, we just default to 0 and hope that canMove() has been called to properly check.
                    effectiveRunSpeed = 0;
                    break;
            }

            return effectiveRunSpeed;
        }

        /// <summary>
        /// Calculate forward speed
        /// </summary>
        /// <returns>forward speed</returns>
        private double calculateForwardSpeed()
        {
            double speed;
            int effectiveRunSpeed;

            if ((this.moveDirection == MoveDirections.None) || (!this.canMove()))
            {
                return 0;
            }

            effectiveRunSpeed = this.calculateEffectiveRunSpeed();

            if (this.moveDirection == MoveDirections.Forwards)
            {
                // NV: TODO: Verify this more. Especially with uber-low runspeeds (negative)
                speed = Math.Max(0, (effectiveRunSpeed * 0.005) + 4);

                if (this.moveMode != MoveModes.Swim)
                {
                    speed = Math.Min(15, speed); // Forward speed is capped at 15 units/sec for non-swimming
                }
            }
            else
            {
                // NV: TODO: Verify this more. Especially with uber-low runspeeds (negative)
                speed = -Math.Max(0, (effectiveRunSpeed * 0.0035) + 4);

                if (this.moveMode != MoveModes.Swim)
                {
                    speed = Math.Max(-15, speed); // Backwards speed is capped at 15 units/sec for non-swimming
                }
            }

            return speed;
        }

        /// <summary>
        /// Can Character move?
        /// </summary>
        /// <returns>Can move=true</returns>
        private bool canMove()
        {
            if ((this.moveMode == MoveModes.Run) || (this.moveMode == MoveModes.Walk)
                || (this.moveMode == MoveModes.Swim) || (this.moveMode == MoveModes.Crawl)
                || (this.moveMode == MoveModes.Sneak) || (this.moveMode == MoveModes.Fly))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate strafe speed
        /// </summary>
        /// <returns>Strafe speed</returns>
        private double calculateStrafeSpeed()
        {
            double speed;
            int effectiveRunSpeed;

            // Note, you can not strafe while swimming or crawling
            if ((this.strafeDirection == SpinOrStrafeDirections.None) || (this.moveMode == MoveModes.Swim)
                || (this.moveMode == MoveModes.Crawl) || (!this.canMove()))
            {
                return 0;
            }

            effectiveRunSpeed = this.calculateEffectiveRunSpeed();

            // NV: TODO: Update this based off Forward runspeed when that is checked (strafe effective run speed = effective run speed / 2)
            speed = ((effectiveRunSpeed / 2) * 0.005) + 4;

            if (this.strafeDirection == SpinOrStrafeDirections.Left)
            {
                speed = -speed;
            }

            return speed;
        }

        /// <summary>
        /// Calculate move vector
        /// </summary>
        /// <returns>Movevector</returns>
        private Vector3 calculateMoveVector()
        {
            double forwardSpeed;
            double strafeSpeed;
            Vector3 forwardMove;
            Vector3 strafeMove;

            if (!this.canMove())
            {
                return Vector3.Origin;
            }

            forwardSpeed = this.calculateForwardSpeed();
            strafeSpeed = this.calculateStrafeSpeed();

            if ((forwardSpeed == 0) && (strafeSpeed == 0))
            {
                return Vector3.Origin;
            }

            if (forwardSpeed != 0)
            {
                forwardMove = Quaternion.RotateVector3(this.RawHeading, Vector3.AxisZ);
                forwardMove.Magnitude = Math.Abs(forwardSpeed);
                if (forwardSpeed < 0)
                {
                    forwardMove = -forwardMove;
                }
            }
            else
            {
                forwardMove = Vector3.Origin;
            }

            if (strafeSpeed != 0)
            {
                strafeMove = Quaternion.RotateVector3(this.RawHeading, Vector3.AxisX);
                strafeMove.Magnitude = Math.Abs(strafeSpeed);
                if (strafeSpeed < 0)
                {
                    strafeMove = -strafeMove;
                }
            }
            else
            {
                strafeMove = Vector3.Origin;
            }

            return forwardMove + strafeMove;
        }

        /// <summary>
        /// Calculate Turnangle
        /// </summary>
        /// <returns>Turnangle</returns>
        private double calculateTurnArcAngle()
        {
            double turnTime;
            double angle;
            double modifiedDuration;

            turnTime = this.calculateTurnTime();

            modifiedDuration = this.PredictionDuration.TotalSeconds % turnTime;

            angle = 2 * Math.PI * modifiedDuration / turnTime;

            return angle;
        }

        /// <summary>
        /// Heading as Quaternion
        /// </summary>
        public new Quaternion Heading
        {
            get
            {
                if (this.spinDirection == SpinOrStrafeDirections.None)
                {
                    return this.RawHeading;
                }
                else
                {
                    double turnArcAngle;
                    Quaternion turnQuaterion;
                    Quaternion newHeading;

                    turnArcAngle = this.calculateTurnArcAngle();
                    turnQuaterion = new Quaternion(Vector3.AxisY, turnArcAngle);

                    newHeading = Quaternion.Hamilton(turnQuaterion, this.RawHeading);
                    newHeading.Normalize();

                    return newHeading;
                }
            }
        }

        private SpinOrStrafeDirections spinDirection = SpinOrStrafeDirections.None;

        private SpinOrStrafeDirections strafeDirection = SpinOrStrafeDirections.None;

        private MoveDirections moveDirection = MoveDirections.None;

        private MoveModes moveMode = MoveModes.Run; // Run should be an appropriate default for now

        private MoveModes previousMoveMode = MoveModes.Run; // Run should be an appropriate default for now

        /// <summary>
        /// Stop movement (before teleporting for example)
        /// </summary>
        public void StopMovement()
        {
            // This should be used to stop the interpolating and save last interpolated value of movement before teleporting
            this.RawCoord = this.Coordinates;
            this.RawHeading = this.Heading;

            this.spinDirection = SpinOrStrafeDirections.None;
            this.strafeDirection = SpinOrStrafeDirections.None;
            this.moveDirection = MoveDirections.None;
        }

        /// <summary>
        /// Update move type
        /// </summary>
        /// <param name="moveType">new move type</param>
        public void UpdateMoveType(byte moveType)
        {
            this.predictionTime = DateTime.Now;

            /*
             * NV: Would be nice to have all other possible values filled out for this at some point... *Looks at Suiv*
             * More specifically, 10, 13 and 22 - 10 and 13 seem to be tied to spinning with mouse. 22 seems random (ping mabe?)
             * Also TODO: Tie this with CurrentMovementMode stat and persistance (ie, log off walking, log back on and still walking)
             * Values of CurrentMovementMode and their effects:
                0: slow moving feet not animating
                1: rooted cant sit
                2: walk
                3: run
                4: swim
                5: crawl
                6: sneak
                7: flying
                8: sitting
                9: rooted can sit
                10: same as 0
                11: sleeping
                12: lounging
                13: same as 0
                14: same as 0
                15: same as 0
                16: same as 0
             */
            switch (moveType)
            {
                case 1: // Forward Start
                    this.moveDirection = MoveDirections.Forwards;
                    break;
                case 2: // Forward Stop
                    this.moveDirection = MoveDirections.None;
                    break;

                case 3: // Reverse Start
                    this.moveDirection = MoveDirections.Backwards;
                    break;
                case 4: // Reverse Stop
                    this.moveDirection = MoveDirections.None;
                    break;

                case 5: // Strafe Right Start
                    this.strafeDirection = SpinOrStrafeDirections.Right;
                    break;
                case 6: // Strafe Stop (Right)
                    this.strafeDirection = SpinOrStrafeDirections.None;
                    break;

                case 7: // Strafe Left Start
                    this.strafeDirection = SpinOrStrafeDirections.Left;
                    break;
                case 8: // Strafe Stop (Left)
                    this.strafeDirection = SpinOrStrafeDirections.None;
                    break;

                case 9: // Turn Right Start
                    this.spinDirection = SpinOrStrafeDirections.Right;
                    break;
                case 10: // Mouse Turn Right Start
                    break;
                case 11: // Turn Stop (Right)
                    this.spinDirection = SpinOrStrafeDirections.None;
                    break;

                case 12: // Turn Left Start
                    this.spinDirection = SpinOrStrafeDirections.Left;
                    break;
                case 13: // Mouse Turn Left Start
                    break;
                case 14: // Turn Stop (Left)
                    this.spinDirection = SpinOrStrafeDirections.None;
                    break;

                case 15: // Jump Start
                    // NV: TODO: This!
                    break;
                case 16: // Jump Stop
                    break;

                case 17: // Elevate Up Start
                    break;
                case 18: // Elevate Up Stop
                    break;

                case 19: // ? 19 = 20 = 22 = 31 = 32
                    break;
                case 20: // ? 19 = 20 = 22 = 31 = 32
                    break;

                case 21: // Full Stop
                    break;

                case 22: // ? 19 = 20 = 22 = 31 = 32
                    break;

                case 23: // Switch To Frozen Mode
                    break;
                case 24: // Switch To Walk Mode
                    this.moveMode = MoveModes.Walk;
                    break;
                case 25: // Switch To Run Mode
                    this.moveMode = MoveModes.Run;
                    break;
                case 26: // Switch To Swim Mode
                    break;
                case 27: // Switch To Crawl Mode
                    this.previousMoveMode = this.moveMode;
                    this.moveMode = MoveModes.Crawl;
                    break;
                case 28: // Switch To Sneak Mode
                    this.previousMoveMode = this.moveMode;
                    this.moveMode = MoveModes.Sneak;
                    break;
                case 29: // Switch To Fly Mode
                    break;
                case 30: // Switch To Sit Ground Mode
                    this.previousMoveMode = this.moveMode;
                    this.moveMode = MoveModes.Sit;
                    this.stats.NanoDelta.CalcTrickle();
                    this.stats.HealDelta.CalcTrickle();
                    this.stats.NanoInterval.CalcTrickle();
                    this.stats.HealInterval.CalcTrickle();
                    break;

                case 31: // ? 19 = 20 = 22 = 31 = 32
                    break;
                case 32: // ? 19 = 20 = 22 = 31 = 32
                    break;

                case 33: // Switch To Sleep Mode
                    this.moveMode = MoveModes.Sleep;
                    break;
                case 34: // Switch To Lounge Mode
                    this.moveMode = MoveModes.Lounge;
                    break;

                case 35: // Leave Swim Mode
                    break;
                case 36: // Leave Sneak Mode
                    this.moveMode = this.previousMoveMode;
                    break;
                case 37: // Leave Sit Mode
                    this.moveMode = this.previousMoveMode;
                    this.stats.NanoDelta.CalcTrickle();
                    this.stats.HealDelta.CalcTrickle();
                    this.stats.NanoInterval.CalcTrickle();
                    this.stats.HealInterval.CalcTrickle();
                    break;
                case 38: // Leave Frozen Mode
                    break;
                case 39: // Leave Fly Mode
                    break;
                case 40: // Leave Crawl Mode
                    this.moveMode = this.previousMoveMode;
                    break;
                case 41: // Leave Sleep Mode
                    break;
                case 42: // Leave Lounge Mode
                    break;
                default:
                    //Console.WriteLine("Unknown MoveType: " + moveType);
                    break;
            }

            //Console.WriteLine((moveDirection != 0 ? moveMode.ToString() : "Stand") + "ing in the direction " + moveDirection.ToString() + (spinDirection != 0 ? " while spinning " + spinDirection.ToString() : "") + (strafeDirection != 0 ? " and strafing " + strafeDirection.ToString() : ""));
        }

        /// <summary>
        /// Character's coordinates
        /// </summary>
        public new AOCoord Coordinates
        {
            get
            {
                if ((this.moveDirection == MoveDirections.None) && (this.strafeDirection == SpinOrStrafeDirections.None))
                {
                    return this.RawCoord;
                }
                else if (this.spinDirection == SpinOrStrafeDirections.None)
                {
                    Vector3 moveVector = this.calculateMoveVector();

                    moveVector = moveVector * this.PredictionDuration.TotalSeconds;

                    return new AOCoord(this.RawCoord.coordinate + moveVector);
                }
                else
                {
                    Vector3 moveVector;
                    Vector3 positionFromCentreOfTurningCircle;
                    double turnArcAngle;
                    double y;
                    double duration;

                    duration = this.PredictionDuration.TotalSeconds;

                    moveVector = this.calculateMoveVector();
                    turnArcAngle = this.calculateTurnArcAngle();

                    // This is calculated seperately as height is unaffected by turning
                    y = this.RawCoord.coordinate.y + (moveVector.y * duration);

                    if (this.spinDirection == SpinOrStrafeDirections.Left)
                    {
                        positionFromCentreOfTurningCircle = new Vector3(moveVector.z, y, -moveVector.x);
                    }
                    else
                    {
                        positionFromCentreOfTurningCircle = new Vector3(-moveVector.z, y, moveVector.x);
                    }

                    return
                        new AOCoord(
                            this.RawCoord.coordinate
                            +
                            Quaternion.RotateVector3(
                                new Quaternion(Vector3.AxisY, turnArcAngle), positionFromCentreOfTurningCircle)
                            - positionFromCentreOfTurningCircle);
                }
            }
        }

        /// <summary>
        /// Active Nanos list
        /// </summary>
        public List<AONano> ActiveNanos
        {
            get
            {
                return this.activeNanos;
            }
        }

        /// <summary>
        /// Active Timers list (HD/ND, Nano effect timers etc)
        /// </summary>
        public List<AOTimers> Timers
        {
            get
            {
                return this.timers;
            }
        }

        /// <summary>
        /// Uploaded Nanos list
        /// </summary>
        public List<AOUploadedNanos> UploadedNanos
        {
            get
            {
                return this.uploadedNanos;
            }
        }

        /// <summary>
        /// Bank inventory
        /// </summary>
        public List<AOItem> Bank
        {
            get
            {
                return this.bank;
            }
        }

        /// <summary>
        /// Parent client
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Caching Mesh layer structure
        /// </summary>
        public MeshLayers MeshLayer
        {
            get
            {
                return this.meshLayer;
            }
        }

        /// <summary>
        /// Caching Mesh layer for social tab items
        /// </summary>
        public MeshLayers SocialMeshLayer
        {
            get
            {
                return this.socialMeshLayer;
            }
        }

        public bool[] WeaponsStored
        {
            get
            {
                return this.weaponsStored;
            }
        }

        public bool[] ItemsStored
        {
            get
            {
                return this.itemsStored;
            }
        }

        /// <summary>
        /// Don't process timers when true, double use, stats aren't sent back to client(s) when true
        /// </summary>
        public bool DoNotDoTimers
        {
            get
            {
                return this.doNotDoTimers;
            }
            set
            {
                this.doNotDoTimers = value;
            }
        }

        /// <summary>
        /// Stats
        /// </summary>
        public CharacterStats Stats
        {
            get
            {
                return this.stats;
            }
            set
            {
                this.stats = value;
            }
        }

        /// <summary>
        /// Inventory list
        /// </summary>
        public List<InventoryEntries> Inventory
        {
            get
            {
                return this.inventory;
            }
        }

        public MoveModes MoveMode
        {
            get
            {
                return this.moveMode;
            }
            set
            {
                this.moveMode = value;
            }
        }

        public MoveModes PreviousMoveMode
        {
            get
            {
                return this.previousMoveMode;
            }
            set
            {
                this.previousMoveMode = value;
            }
        }

        public MoveDirections MoveDirection
        {
            get
            {
                return this.moveDirection;
            }
            set
            {
                this.moveDirection = value;
            }
        }

        public SpinOrStrafeDirections StrafeDirection
        {
            get
            {
                return this.strafeDirection;
            }
            set
            {
                this.strafeDirection = value;
            }
        }

        public SpinOrStrafeDirections SpinDirection
        {
            get
            {
                return this.spinDirection;
            }
            set
            {
                this.spinDirection = value;
            }
        }

        public bool NeedPurge
        {
            get
            {
                return this.needPurge;
            }
            set
            {
                this.needPurge = value;
            }
        }
        #endregion

        /// <summary>
        /// Write stats to database
        /// </summary>
        /// <returns>true for success</returns>

        #region Write stats
        public bool WriteStats()
        {
            lock (this)
            {
                try
                {
                    this.stats.WriteStatstoSql();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        #endregion

        #region Character/Org Name(s) and ID(s)
        /// <summary>
        /// Read org and character names from DB
        /// </summary>
        public bool ReadNames()
        {
            lock (this)
            {
                SqlWrapper Sql = new SqlWrapper();
                DataTable dt =
                    Sql.ReadDatatable(
                        "SELECT `Name`, `FirstName`, `LastName` FROM " + this.GetSqlTablefromDynelType()
                        + " WHERE ID = '" + this.Id + "' LIMIT 1");
                if (dt.Rows.Count == 1)
                {
                    this.Name = (string)dt.Rows[0][0];
                    this.FirstName = (string)dt.Rows[0][1];
                    this.LastName = (string)dt.Rows[0][2];
                }

                // Read stat# 5 (Clan) - OrgID from character stats table
                dt =
                    Sql.ReadDatatable(
                        "SELECT `Value` FROM " + this.GetSqlTablefromDynelType() + "_stats WHERE ID = " + this.Id
                        + " AND Stat = 5 LIMIT 1");
                if (dt.Rows.Count == 1)
                {
                    this.orgId = (Int32)dt.Rows[0][0];
                }
                if (this.orgId == 0)
                {
                    this.OrgName = string.Empty;
                }
                else
                {
                    List<GuildEntry> m_Guild = GuildInfo.GetGuildInfo(this.orgId);

                    foreach (GuildEntry ge in m_Guild)
                    {
                        this.OrgName = ge.Name;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Write names to database
        /// </summary>
        /// <returns>true for success</returns>
        public bool WriteNames()
        {
            SqlWrapper Sql = new SqlWrapper();
            try
            {
                Sql.SqlUpdate(
                    "UPDATE " + this.GetSqlTablefromDynelType() + " SET `Name` = '" + this.Name + "', `FirstName` = '"
                    + this.FirstName + "', `LastName` = '" + this.LastName + "' WHERE `ID` = " + "'" + this.Id + "'");
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Inventory (read/write)
        /// <summary>
        /// Read inventory from database
        /// TODO: catch exceptions
        /// </summary>
        public void ReadInventoryFromSql()
        {
            lock (this.inventory)
            {
                SqlWrapper ms = new SqlWrapper();
                {
                    InventoryEntries m_inv;
                    this.inventory.Clear();
                    DataTable dt =
                        ms.ReadDatatable(
                            "SELECT * FROM " + this.GetSqlTablefromDynelType() + "inventory WHERE ID="
                            + this.Id.ToString() + " AND container=104 ORDER BY placement ASC;");
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            m_inv = new InventoryEntries();
                            m_inv.Container = (Int32)row["container"];
                            m_inv.Placement = (Int32)row["placement"];
                            m_inv.Item.HighID = (Int32)row["highid"];
                            m_inv.Item.LowID = (Int32)row["lowid"];
                            m_inv.Item.Quality = (Int32)row["quality"];
                            m_inv.Item.MultipleCount = (Int32)row["multiplecount"];
                            m_inv.Item.Type = (Int32)row["type"];
                            m_inv.Item.Instance = (Int32)row["instance"];
                            m_inv.Item.Flags = (Int32)row["flags"];
                            this.inventory.Add(m_inv);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Write inventory to database
        /// TODO: catch exceptions
        /// </summary>
        public void WriteInventoryToSql()
        {
            lock (this.inventory)
            {
                SqlWrapper ms = new SqlWrapper();
                int count;
                ms.SqlDelete(
                    "DELETE FROM " + this.GetSqlTablefromDynelType() + "inventory WHERE ID=" + this.Id.ToString()
                    + " AND container=104;");
                for (count = 0; count < this.inventory.Count; count++)
                {
                    if (this.inventory[count].Container != -1) // dont save possible trade leftovers
                    {
                        ms.SqlInsert(
                            "INSERT INTO " + this.GetSqlTablefromDynelType()
                            + "inventory (ID,placement,flags,multiplecount,lowid,highid,quality,container) values ("
                            + this.Id.ToString() + "," + this.inventory[count].Placement.ToString() + ",1,"
                            + this.inventory[count].Item.MultipleCount.ToString() + ","
                            + this.inventory[count].Item.LowID.ToString() + ","
                            + this.inventory[count].Item.HighID.ToString() + ","
                            + this.inventory[count].Item.Quality.ToString() + ",104);");
                    }
                }
            }
        }

        /// <summary>
        /// Write inventory data into packetwriter
        /// </summary>
        /// <param name="packetWriter">packet writer</param>
        public void WriteInventoryToPacket(PacketWriter packetWriter)
        {
            lock (this.inventory)
            {
                packetWriter.Push3F1Count(this.inventory.Count);
                int count;
                for (count = 0; count < this.inventory.Count; count++)
                {
                    packetWriter.PushInt(this.inventory[count].Placement);
                    packetWriter.PushShort((short)this.inventory[count].Item.Flags);
                    packetWriter.PushShort((short)this.inventory[count].Item.MultipleCount);
                    packetWriter.PushIdentity(this.inventory[count].Item.Type, this.inventory[count].Item.Instance);
                    packetWriter.PushInt(this.inventory[count].Item.LowID);
                    packetWriter.PushInt(this.inventory[count].Item.HighID);
                    packetWriter.PushInt(this.inventory[count].Item.Quality);
                    packetWriter.PushInt(this.inventory[count].Item.Nothing);
                }
            }
        }
        #endregion

        #region Timers (read/write)
        /// <summary>
        /// Write timers to database
        /// TODO: catch exceptions
        /// </summary>
        public void WriteTimersToSql()
        {
            lock (this.timers)
            {
                SqlWrapper ms = new SqlWrapper();
                int count;

                // remove HP and NP tick
                count = this.timers.Count;
                while (count > 0)
                {
                    count--;
                    if ((this.timers[count].Strain == 0) || (this.timers[count].Strain == 1))
                    {
                        this.timers.RemoveAt(count);
                    }
                }

                ms.SqlDelete("DELETE FROM " + this.GetSqlTablefromDynelType() + "timers WHERE ID=" + this.Id.ToString());
                TimeSpan ts;
                DateTime n = DateTime.Now;

                for (count = 0; count < this.timers.Count; count++)
                {
                    ts = this.timers[count].Timestamp - n;
                    ms.SqlInsert(
                        "INSERT INTO " + this.GetSqlTablefromDynelType() + "timers VALUES (" + this.Id.ToString() + ","
                        + this.timers[count].Strain + "," + this.timers[count].Timestamp.Second.ToString() + ",X'"
                        + this.timers[count].Function.Serialize() + "');");
                }
            }
        }

        /// <summary>
        /// Read timers from database
        /// TODO: catch exceptions
        /// </summary>
        public void ReadTimersFromSql()
        {
            lock (this.timers)
            {
                SqlWrapper ms = new SqlWrapper();
                TimeSpan timeSpan;
                DateTime now = DateTime.Now;
                byte[] blob = new byte[10240];
                this.timers.Clear();

                ms.SqlRead(
                    "SELECT * FROM " + this.GetSqlTablefromDynelType() + "timers WHERE ID=" + this.Id.ToString() + ";");
                DataTable dt =
                    ms.ReadDatatable(
                        "SELECT * FROM " + this.GetSqlTablefromDynelType() + "timers WHERE ID=" + this.Id.ToString()
                        + ";");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        timeSpan = TimeSpan.FromSeconds((Int32)row["timespan"]);
                        AOTimers aoTimer = new AOTimers
                            {
                                Timestamp = DateTime.Now + timeSpan,
                                Strain = (Int32)row["strain"],
                            };
                        MemoryStream memstream = new MemoryStream((byte[])row[3]);
                        BinaryFormatter bin = new BinaryFormatter();

                        aoTimer.Function = AOFunctions.Deserialize(memstream);
                    }
                }
            }
        }
        #endregion

        #region Nanos (read/write)
        /// <summary>
        /// Read nanos from database
        /// TODO: catch exceptions
        /// </summary>
        public void ReadNanosFromSql()
        {
            lock (this.activeNanos)
            {
                SqlWrapper sqlWrapper = new SqlWrapper();
                {
                    AONano m_an;

                    DataTable dt =
                        sqlWrapper.ReadDatatable(
                            "SELECT * FROM " + this.GetSqlTablefromDynelType() + "activenanos WHERE ID="
                            + this.Id.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            m_an = new AONano();
                            m_an.ID = (Int32)row["nanoID"];
                            m_an.NanoStrain = (Int32)row["strain"];
                            this.activeNanos.Add(m_an);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Write nanos to database
        /// TODO: catch exceptions
        /// </summary>
        public void WriteNanosToSql()
        {
            lock (this.activeNanos)
            {
                SqlWrapper sqlWrapper = new SqlWrapper();
                int count;

                sqlWrapper.SqlDelete(
                    "DELETE FROM " + this.GetSqlTablefromDynelType() + "activenanos WHERE ID=" + this.Id.ToString());
                for (count = 0; count < this.activeNanos.Count; count++)
                {
                    sqlWrapper.SqlInsert(
                        "INSERT INTO " + this.GetSqlTablefromDynelType() + "activenanos VALUES (" + this.Id.ToString()
                        + "," + this.activeNanos[count].ID.ToString() + ","
                        + this.activeNanos[count].NanoStrain.ToString() + ")");
                }
            }
        }
        #endregion

        #region Equip
        /// <summary>
        /// Equip an items
        /// </summary>
        /// <param name="it">Item</param>
        /// <param name="ch">Character</param>
        /// <param name="tosocialtab">should it be equipped to social tab</param>
        /// <param name="location">Location</param>
        public void EquipItem(AOItem it, Character ch, bool tosocialtab, int location)
        {
            int counter = 0;
            int f;
            lock (ch)
            {
                while (counter < it.Events.Count)
                {
                    if (it.Events[counter].EventType == Constants.EventtypeOnWear)
                    {
                        this.ExecuteEvent(
                            this, this, it.Events[counter], true, tosocialtab, location, CheckReqs.doCheckReqs);
                    }
                    counter++;
                }
                foreach (AOTimers t in this.timers)
                {
                    if (t.Strain == -1)
                    {
                        t.Timestamp = DateTime.Now;
                        break;
                    }
                }

                // Set Weaponmesh
                // Todo: another packet has to be sent, defining how to hold the weapon
                if (it.ItemType == Constants.itemtype_Weapon)
                {
                    this.stats.WeaponsStyle.Value |= it.GetWeaponStyle();
                    if (tosocialtab)
                    {
                        this.stats.WeaponsStyle.Value = it.GetWeaponStyle();
                    }

                    counter = 0;
                    f = -1;
                    while (counter < it.Stats.Count)
                    {
                        if (it.Stats[counter].Stat == 209)
                        {
                            f = counter; // found Weapon mesh attribute
                            break;
                        }
                        counter++;
                    }
                    if (f != -1)
                    {
                        if (!tosocialtab)
                        {
                            if (location == 6)
                            {
                                this.stats.WeaponMeshRight.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.meshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
                            }
                            else
                            {
                                this.stats.WeaponMeshLeft.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.meshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
                            }
                        }
                        else
                        {
                            if (location == 61)
                            {
                                if (this.SocialTab.ContainsKey(1006))
                                {
                                    this.SocialTab[1006] = it.Stats[f].Value;
                                }
                                else
                                {
                                    this.SocialTab.Add(1006, it.Stats[f].Value);
                                }
                                this.socialMeshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
                            }
                            else
                            {
                                if (this.SocialTab.ContainsKey(1007))
                                {
                                    this.SocialTab[1007] = it.Stats[f].Value;
                                }
                                else
                                {
                                    this.SocialTab.Add(1007, it.Stats[f].Value);
                                }
                                this.socialMeshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
                            }
                        }
                    }
                }

                if (!tosocialtab)
                {
                    this.CalculateSkills();
                }
            }
        }
        #endregion

        #region Fake Equip
        /// <summary>
        /// Fake equip, don't pass effects to local stats
        /// </summary>
        /// <param name="it">Item</param>
        /// <param name="ch">Character</param>
        /// <param name="tosocialtab">should it be equipped to social tab</param>
        /// <param name="location">Location</param>
        public void FakeEquipItem(AOItem it, Character ch, bool tosocialtab, int location)
        {
            int counter = 0;
            int f;
            lock (ch)
            {
                while (counter < it.Events.Count)
                {
                    if (it.Events[counter].EventType == Constants.EventtypeOnWear)
                    {
                        this.ExecuteEvent(
                            this, this, it.Events[counter], false, tosocialtab, location, CheckReqs.doEquipCheckReqs);
                    }
                    counter++;
                }

                // TODO check if still needed
                // Set Weaponmesh
                // Todo: another packet has to be sent, defining how to hold the weapon
                if (it.ItemType == Constants.itemtype_Weapon)
                {
                    this.stats.WeaponsStyle.Value |= it.GetWeaponStyle();
                    if (tosocialtab)
                    {
                        this.stats.WeaponsStyle.Value = it.GetWeaponStyle();
                    }

                    counter = 0;
                    f = -1;
                    while (counter < it.Stats.Count)
                    {
                        if (it.Stats[counter].Stat == 209)
                        {
                            f = counter; // found Weapon mesh attribute
                            break;
                        }
                        counter++;
                    }
                    if (f != -1)
                    {
                        if (!tosocialtab)
                        {
                            if (location == 6)
                            {
                                this.stats.WeaponMeshRight.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.meshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
                            }
                            else
                            {
                                this.stats.WeaponMeshLeft.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.meshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
                            }
                        }
                        else
                        {
                            if (location == 61)
                            {
                                if (this.SocialTab.ContainsKey(1006))
                                {
                                    this.SocialTab[1006] = it.Stats[f].Value;
                                }
                                else
                                {
                                    this.SocialTab.Add(1006, it.Stats[f].Value);
                                }
                                this.socialMeshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
                            }
                            else
                            {
                                if (this.SocialTab.ContainsKey(1007))
                                {
                                    this.SocialTab[1007] = it.Stats[f].Value;
                                }
                                else
                                {
                                    this.SocialTab.Add(1007, it.Stats[f].Value);
                                }
                                this.socialMeshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Unequip
        /// <summary>
        /// Unequip item
        /// </summary>
        /// <param name="it">Item</param>
        /// <param name="ch">Character</param>
        /// <param name="fromsocialtab">unequip from social tab?</param>
        /// <param name="location">Location</param>
        public void UnequipItem(AOItem it, Character ch, bool fromsocialtab, int location)
        {
            this.CalculateSkills();
            return;
            /* lock (ch)
            {
                int counter = 0;
                int f;

                // Set Weaponmesh
                if (it.ItemType == Constants.itemtype_Weapon)
                {
                    counter = 0;
                    f = -1;
                    while (counter < it.Stats.Count)
                    {
                        if (it.Stats[counter].Stat == 209)
                        {
                            f = counter;
                            break;
                        }
                        counter++;
                    }
                    if (f != -1)
                    {
                        if (!fromsocialtab)
                        {
                            if (location == 6)
                            {
                                Stats.WeaponMeshRight.Set(0); // Weaponmesh
                                ch.MeshLayer.RemoveMesh(2, it.Stats[f].Stat, 0, Misc.MeshLayers.GetLayer(location));
                            }
                            else
                            {
                                Stats.WeaponMeshLeft.Set(0);
                                ch.MeshLayer.RemoveMesh(2, it.Stats[f].Stat, 0, Misc.MeshLayers.GetLayer(location));
                            }
                        }
                        else
                        {
                            if (location == 61)
                            {
                                ch.SocialMeshLayer.RemoveMesh(2, it.Stats[f].Stat, 0, Misc.MeshLayers.GetLayer(location));
                            }
                            else
                            {
                                ch.SocialMeshLayer.RemoveMesh(2, it.Stats[f].Stat, 0, Misc.MeshLayers.GetLayer(location));
                            }
                        }
                    }
                }
            }
             */
        }
        #endregion

        #region Switch item placement
        /// <summary>
        /// Switch item placements
        /// TODO: catch exceptions
        /// </summary>
        /// <param name="cli">Client</param>
        /// <param name="fromPlacement">From location</param>
        /// <param name="toPlacement">To location</param>
        public void SwitchItems(int fromPlacement, int toPlacement)
        {
            lock (this)
            {
                SqlWrapper mySql = new SqlWrapper();
                InventoryEntries afrom = this.GetInventoryAt(fromPlacement);
                InventoryEntries ato = this.GetInventoryAt(toPlacement);
                if (afrom != null)
                {
                    afrom.Placement = toPlacement;
                }
                if (ato != null)
                {
                    ato.Placement = fromPlacement;
                }

                mySql.SqlUpdate(
                    "UPDATE " + this.GetSqlTablefromDynelType() + "inventory SET placement=255 where (ID="
                    + this.Id.ToString() + ") AND (placement=" + fromPlacement.ToString() + ")");
                mySql.SqlUpdate(
                    "UPDATE " + this.GetSqlTablefromDynelType() + "inventory SET placement=" + fromPlacement.ToString()
                    + " where (ID=" + this.Id.ToString() + ") AND (placement=" + toPlacement.ToString() + ")");
                mySql.SqlUpdate(
                    "UPDATE " + this.GetSqlTablefromDynelType() + "inventory SET placement=" + toPlacement.ToString()
                    + " where (ID=" + this.Id.ToString() + ") AND (placement=255)");
            }

            // If its a switch from or to equipment pages then recalculate the skill modifiers
            if ((fromPlacement < 64) || (toPlacement < 64))
            {
                this.CalculateSkills();
            }
        }
        #endregion

        #region Switch item placement to bank account
        /// <summary>
        /// Transfer Item to bank account
        /// </summary>
        /// <param name="fromPlacement">from inventory location</param>
        /// <param name="toPlacement">to bank location</param>
        public void TransferItemtoBank(int fromPlacement, int toPlacement)
        {
            lock (this)
            {
                InventoryEntries afrom = this.GetInventoryAt(fromPlacement);

                AOItem toBank = afrom.Item.ShallowCopy();
                toBank.Flags = this.GetNextFreeInventory(0x69);
                this.inventory.Remove(afrom);
                this.bank.Add(toBank);
                this.WriteBankContentsToSql();
                this.WriteInventoryToSql();
            }
        }
        #endregion

        #region Transfer Item from bank to inventory
        /// <summary>
        /// Transfer item from bank account to inventory
        /// </summary>
        /// <param name="fromPlacement">from bank location</param>
        /// <param name="toPlacement">to inventory location</param>
        public void TransferItemfromBank(int fromPlacement, int toPlacement)
        {
            lock (this)
            {
                int placement = this.GetNextFreeInventory(0x68);

                AOItem tempItem = null;
                foreach (AOItem item in this.bank)
                {
                    if (item.Flags == fromPlacement)
                    {
                        tempItem = item;
                        break;
                    }
                }
                if (tempItem == null)
                {
                    Console.WriteLine("Not valid item...");
                    return;
                }

                InventoryEntries mi = new InventoryEntries();
                AOItem it = ItemHandler.GetItemTemplate(tempItem.LowID);
                mi.Placement = placement;
                mi.Container = 104;
                mi.Item.LowID = tempItem.LowID;
                mi.Item.HighID = tempItem.HighID;
                mi.Item.Quality = tempItem.Quality;
                mi.Item.MultipleCount = Math.Max(1, tempItem.MultipleCount);
                this.inventory.Add(mi);
                this.bank.Remove(tempItem);
                this.WriteBankContentsToSql();
                this.WriteInventoryToSql();
            }
        }
        #endregion

        #region TransferItemfromKnuBotTrade
        public void TransferItemfromKnuBotTrade(int fromplacement, int toplacement)
        {
            // TODO: Trade it
        }
        #endregion

        #region CalculateSkills
        /// <summary>
        /// Calculate all modifiers and trickles and store them into the localstats
        /// </summary>
        public void CalculateSkills()
        {
            lock (this)
            {
                // Todo: process all item modifiers
                this.PurgeTimer(0);
                this.PurgeTimer(1);
                int c;
                int c2;
                int c3;
                int oldhealth = this.stats.Health.Value;
                int oldnano = this.stats.CurrentNano.Value;
                AOItem m_item;

                this.socialMeshLayer = new MeshLayers();
                this.Textures = new List<AOTextures>();
                this.meshLayer = new MeshLayers();
                this.SocialTab = new Dictionary<int, int>();
                this.SocialTab.Add(0, 0);
                this.SocialTab.Add(1, 0);
                this.SocialTab.Add(2, 0);
                this.SocialTab.Add(3, 0);
                this.SocialTab.Add(4, 0);
                this.SocialTab.Add(38, 0);
                this.SocialTab.Add(1004, 0);
                this.SocialTab.Add(1005, 0);
                this.SocialTab.Add(64, 0);
                this.SocialTab.Add(32, 0);
                this.SocialTab.Add(1006, 0);
                this.SocialTab.Add(1007, 0);

                // Clear Modifiers (adds and percentages)
                this.stats.ClearModifiers();
                this.meshLayer.AddMesh(0, this.stats.HeadMesh.Value, 0, 4);
                this.socialMeshLayer.AddMesh(0, this.stats.HeadMesh.Value, 0, 4);

                // Apply all modifying item functions to localstats
                for (c = 0; c < this.inventory.Count; c++)
                {
                    // only process items in the equipment pages (<64)
                    if (this.inventory[c].Placement < 64)
                    {
                        m_item = ItemHandler.interpolate(
                            this.inventory[c].Item.LowID, this.inventory[c].Item.HighID, this.inventory[c].Item.Quality);
                        for (c2 = 0; c2 < m_item.Events.Count; c2++)
                        {
                            if (m_item.Events[c2].EventType == Constants.EventtypeOnWear)
                            {
                                for (c3 = 0; c3 < m_item.Events[c2].Functions.Count; c3++)
                                {
                                    if (this.CheckRequirements(this, m_item.Events[c2].Functions[c3], false))
                                    {
                                        AOFunctions aof_withparams = m_item.Events[c2].Functions[c3].ShallowCopy();
                                        aof_withparams.Arguments.Values.Add(this.inventory[c].Placement);
                                        Program.FunctionC.CallFunction(
                                            aof_withparams.FunctionType,
                                            this,
                                            this,
                                            this,
                                            aof_withparams.Arguments.Values.ToArray());
                                    }
                                    if ((m_item.Events[c2].Functions[c3].FunctionType == Constants.FunctiontypeModify)
                                        ||
                                        (m_item.Events[c2].Functions[c3].FunctionType
                                         == Constants.FunctiontypeModifyPercentage))
                                    {
                                        // TODO ItemHandler.FunctionPack.func_do(this, m_item.ItemEvents[c2].Functions[c3], true, Inventory[c].Placement >= 49, Inventory[c].Placement);
                                    }
                                }
                            }
                        }
                    }
                }

                // Adding nano skill effects
                for (c = 0; c < this.activeNanos.Count; c++)
                {
                    // TODO: Nanohandler, similar to Itemhandler
                    // and calling the skill/attribute modifying functions
                }

                // Calculating the trickledown
                this.stats.Strength.AffectStats();
                this.stats.Agility.AffectStats();
                this.stats.Stamina.AffectStats();
                this.stats.Intelligence.AffectStats();
                this.stats.Sense.AffectStats();
                this.stats.Psychic.AffectStats();

                this.stats.Health.StatBaseValue = this.stats.Health.GetMaxValue((uint)oldhealth);
                this.stats.CurrentNano.StatBaseValue = this.stats.CurrentNano.GetMaxValue((uint)oldnano);
            }
        }
        #endregion

        #region Purge
        private bool needPurge = true;

        /// <summary>
        /// Write data to database when purging character object
        /// </summary>
        public void Purge()
        {
            lock (this)
            {
                if ((this.Id != 0) && (this.needPurge))
                {
                    this.needPurge = false;

                    this.WriteCoordinatesToSql();
                    this.WriteHeadingToSql();
                    this.WriteStats();
                    this.WriteNanosToSql();
                    this.WriteInventoryToSql();
                    this.WriteTimersToSql();
                    this.WriteUploadedNanosToSql();
                    if (this.KnuBotTarget != null)
                    {
                        ((NonPlayerCharacterClass)this.KnuBotTarget).KnuBot.TalkingTo = null;
                    }
                }
            }
        }
        #endregion

        #region Appearance Update
        /// <summary>
        /// Send Appearance update packet
        /// </summary>
        public void AppearanceUpdate()
        {
            Packets.AppearanceUpdate.AnnounceAppearanceUpdate(this);
        }
        #endregion

        #region Get Next Free Inventory spot
        /// <summary>
        /// Find next free spot in inventory
        /// </summary>
        /// <param name="container">find in container</param>
        /// <returns>Location number</returns>
        public int GetNextFreeInventory(int container)
        {
            lock (this.inventory)
            {
                int c = 0;
                if (container == 104)
                {
                    c = 64; // Starting with slot #64 if we look for character inventory
                }

                bool foundfreeslot = false;
                if (container == 0x69) // bank
                {
                    while (!foundfreeslot)
                    {
                        foundfreeslot = true;
                        foreach (AOItem i in this.bank)
                        {
                            if (i.Flags == c)
                            {
                                foundfreeslot = false;
                            }
                        }
                        if (!foundfreeslot)
                        {
                            c++;
                        }
                    }
                }
                else
                {
                    while (!foundfreeslot)
                    {
                        foundfreeslot = true;
                        foreach (InventoryEntries i in this.inventory)
                        {
                            if (i.Placement == c)
                            {
                                foundfreeslot = false;
                            }
                        }
                        if (!foundfreeslot)
                        {
                            c++;
                        }
                    }
                }
                if ((container == 104) && (c > 93))
                {
                    c = -1; // Full Inventory
                }
                return c;
            }
        }
        #endregion

        #region Get Inventory Entry At
        /// <summary>
        /// Get inventoryentry of specified location
        /// </summary>
        /// <param name="place">Location</param>
        /// <returns>InventoryEntry</returns>
        public InventoryEntries GetInventoryAt(int place)
        {
            lock (this.inventory)
            {
                foreach (InventoryEntries i in this.inventory)
                {
                    if (i.Placement == place)
                    {
                        return i;
                    }
                }
                return null;
            }
        }

        public InventoryEntries GetInventoryAt(int place, int container)
        {
            lock (this.inventory)
            {
                foreach (InventoryEntries i in this.inventory)
                {
                    if ((i.Placement == place) && (i.Container == container))
                    {
                        return i;
                    }
                }
                return null;
            }
        }
        #endregion

        #region Upload Nano (add/write/read)
        /// <summary>
        /// Upload a nano
        /// </summary>
        /// <param name="nanoId">Nano-ID</param>
        public void UploadNano(int nanoId)
        {
            lock (this.uploadedNanos)
            {
                AOUploadedNanos au = new AOUploadedNanos { Nano = nanoId };
                this.uploadedNanos.Remove(au); // In case its in already :)
                this.uploadedNanos.Add(au);
            }
            this.WriteUploadedNanosToSql();
        }

        /// <summary>
        /// Write uploaded nanos to database
        /// TODO: catch exceptions
        /// </summary>
        public void WriteUploadedNanosToSql()
        {
            lock (this.uploadedNanos)
            {
                foreach (AOUploadedNanos au in this.uploadedNanos)
                {
                    SqlWrapper sqlWrapper = new SqlWrapper();
                    sqlWrapper.SqlInsert(
                        "REPLACE INTO " + this.GetSqlTablefromDynelType() + "uploadednanos VALUES ("
                        + this.Id.ToString() + "," + au.Nano.ToString() + ")");
                }
            }
        }

        /// <summary>
        /// Read uploaded nanos from database
        /// TODO: catch exceptions
        /// </summary>
        public void ReadUploadedNanosFromSql()
        {
            lock (this.uploadedNanos)
            {
                this.uploadedNanos.Clear();
                SqlWrapper Sql = new SqlWrapper();
                DataTable dt =
                    Sql.ReadDatatable(
                        "SELECT nano FROM " + this.GetSqlTablefromDynelType() + "uploadednanos WHERE ID="
                        + this.Id.ToString());
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AOUploadedNanos au = new AOUploadedNanos { Nano = (Int32)row["Nano"] };
                        this.uploadedNanos.Add(au);
                    }
                }
            }
        }
        #endregion

        #region add HP/NP tick timers
        /// <summary>
        /// Initialize HP and NP timers (HD/ND ticks)
        /// </summary>
        public void AddHpnpTick()
        {
            lock (this.timers)
            {
                this.stats.HealInterval.CalcTrickle();
                this.stats.NanoInterval.CalcTrickle();
                AOTimers at = new AOTimers();
                at.Strain = -1;
                at.Timestamp = DateTime.Now + TimeSpan.FromMilliseconds(100);
                at.Function.Target = this.Id;
                at.Function.TickCount = -2;
                at.Function.TickInterval = 100;
                at.Function.FunctionType = 1; // MAIN stat send timer
                this.timers.Add(at);
                /*
                PurgeTimer(0);
                PurgeTimer(1);
                AOTimers at = new AOTimers();
                at.Strain = 0;
                at.Timestamp = DateTime.Now;
                at.Function.Target = this.ID;
                at.Function.TickCount = -2;
                at.Function.TickInterval = this.Stats.HealInterval.;
                at.Function.FunctionType = ItemHandler.functiontype_modify;
                at.Function.Arguments.Add(0);
                Timers.Add(at);
                at = new AOTimers();
                at.Strain = 1;
                at.Timestamp = DateTime.Now;
                at.Function.Target = this.ID;
                at.Function.TickCount = -2;
                at.Function.TickInterval = 0;
                at.Function.FunctionType = ItemHandler.functiontype_modify;
                at.Function.Arguments.Add(0);
                Timers.Add(at);
                */
            }
        }
        #endregion

        #region setTarget
        /// <summary>
        /// Set target
        /// </summary>
        /// <param name="packet">data packet</param>
        public void SetTarget(byte[] packet)
        {
            lock (this)
            {
                PacketReader packetReader = new PacketReader(packet);

                packetReader.PopHeader();
                packetReader.PopByte();
                this.Target = packetReader.PopIdentity();
                packetReader.Finish();
            }
        }
        #endregion

        #region InventoryReplaceAdd
        /// <summary>
        /// Add/Subtract stacked item or add a item to inventory
        /// </summary>
        /// <param name="ie"></param>
        public void InventoryReplaceAdd(InventoryEntries ie)
        {
            lock (this.inventory)
            {
                AOItem it = ItemHandler.GetItemTemplate(ie.Item.LowID);
                //            if (it.isStackable())
                {
                    foreach (InventoryEntries ia in this.inventory)
                    {
                        if ((ia.Item.LowID == ie.Item.LowID) && (ia.Item.HighID == ie.Item.HighID)
                            && (ia.Container == -1))
                        {
                            ia.Item.MultipleCount += ie.Item.MultipleCount;
                            if (ia.Item.MultipleCount == 0)
                            {
                                this.inventory.Remove(ia);
                            }
                            return;
                        }
                    }
                }
                this.inventory.Add(ie);
            }
        }
        #endregion

        #region BankContents
        /// <summary>
        /// Read bank account from database
        /// TODO: catch exceptions
        /// </summary>
        public void ReadBankContentsFromSql()
        {
            lock (this.bank)
            {
                SqlWrapper ms = new SqlWrapper();
                this.bank.Clear();
                DataTable dt =
                    ms.ReadDatatable(
                        "SELECT * FROM bank WHERE charID=" + this.Id.ToString() + " ORDER BY InventoryID ASC");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AOItem item = new AOItem();
                        item.Flags = (Int32)row["InventoryID"];
                        item.LowID = (Int32)row["lowID"];
                        item.HighID = (Int32)row["highID"];
                        item.MultipleCount = (Int32)row["Amount"];
                        item.Quality = (Int32)row["QL"];
                        item.Type = (Int32)row["Type"];
                        item.Instance = (Int32)row["instance"];
                        byte[] statsblob = (byte[])row[8];
                        int counter = 0;
                        long bloblen = statsblob.Length;
                        while (counter < bloblen - 1)
                        {
                            AOItemAttribute tempItemAttribute = new AOItemAttribute();
                            tempItemAttribute.Stat = BitConverter.ToInt32(statsblob, counter);
                            tempItemAttribute.Value = BitConverter.ToInt32(statsblob, counter + 4);
                            counter += 8;
                            item.Stats.Add(tempItemAttribute);
                        }
                        this.bank.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Reverse double digit hex string
        /// </summary>
        /// <param name="input"></param>
        /// <returns>reversed string</returns>
        private string reverseString(string input)
        {
            string output = "";
            for (int c = input.Length; c > 0; c -= 2)
            {
                output = output + input.Substring(c - 2, 2);
            }
            return output;
        }

        /// <summary>
        /// Write bank contents to database
        /// TODO: catch exceptions
        /// </summary>
        public void WriteBankContentsToSql()
        {
            lock (this.bank)
            {
                SqlWrapper sqlWrapper = new SqlWrapper();
                sqlWrapper.SqlDelete("DELETE FROM bank WHERE charID=" + this.Id.ToString());
                foreach (AOItem item in this.bank)
                {
                    string insert = "INSERT INTO bank VALUES(" + this.Id.ToString() + "," + item.Flags.ToString() + ","
                                    + item.LowID.ToString() + "," + item.HighID.ToString() + ","
                                    + item.MultipleCount.ToString() + "," + item.Quality.ToString() + ","
                                    + item.Type.ToString() + "," + item.Instance.ToString() + ",X'";
                    foreach (AOItemAttribute tempattr in item.Stats)
                    {
                        insert = insert + this.reverseString(tempattr.Stat.ToString("X8"))
                                 + this.reverseString(tempattr.Stat.ToString("X8"));
                    }
                    insert = insert + "');";
                    sqlWrapper.SqlInsert(insert);
                }
            }
        }
        #endregion

        #region ReadSpecialStatsfromSql
        /// <summary>
        /// Read special stats (GM, Account, Expansions etc)
        /// TODO: catch exceptions
        /// </summary>
        public void ReadSpecialStatsFromSql()
        {
            lock (this)
            {
                SqlWrapper sqlWrapper = new SqlWrapper();
                DataTable dt =
                    sqlWrapper.ReadDatatable(
                        "SELECT `ID`, `Expansions`, `AccountFlags`, `GM` FROM `login` WHERE `Username` = (SELECT `Username` FROM `characters` WHERE `ID` = "
                        + this.Id + ");");
                if (dt.Rows.Count > 0)
                {
                    // Stat 607 is PlayerID
                    this.stats.SetBaseValue(607, (UInt32)(Int32)dt.Rows[0][0]);

                    // Stat 389 is Expansion
                    this.stats.SetBaseValue(389, UInt32.Parse((string)dt.Rows[0][1]));

                    // Stat 660 is AccountFlags
                    this.stats.SetBaseValue(660, (UInt32)(Int32)dt.Rows[0][2]);

                    // Stat 215 is GmLevel
                    this.stats.SetBaseValue(215, (UInt32)(Int32)dt.Rows[0][3]);
                }
            }
        }
        #endregion

        #region ApplyInventory
        /// <summary>
        /// Set initial equipment after client logon
        /// </summary>
        public void ApplyInventory()
        {
            lock (this)
            {
                foreach (InventoryEntries ie in this.inventory)
                {
                    if (ie.Placement < 64) // only process equipped items 
                    {
                        AOItem item = ItemHandler.interpolate(ie.Item.LowID, ie.Item.HighID, ie.Item.Quality);
                        this.FakeEquipItem(item, this, ie.Placement >= 48, ie.Placement);
                    }
                }
            }
        }
        #endregion

        #region Save Social Tab
        /// <summary>
        /// Write socialtab contents to database
        /// TODO: catch exceptions
        /// </summary>
        public void SaveSocialTab()
        {
            lock (this.SocialTab)
            {
                // Note: Shouldermeshs still are same for left and right, subject to change in the future (as well as weaponmesh)
                SqlWrapper ms = new SqlWrapper();
                ms.SqlInsert(
                    "REPLACE INTO socialtab VALUES (" + this.Id + ", "
                    + (this.SocialTab.ContainsKey(0) ? this.SocialTab[0] : 0) + ", "
                    + (this.SocialTab.ContainsKey(1) ? this.SocialTab[1] : 0) + ", "
                    + (this.SocialTab.ContainsKey(2) ? this.SocialTab[2] : 0) + ", "
                    + (this.SocialTab.ContainsKey(3) ? this.SocialTab[3] : 0) + ", "
                    + (this.SocialTab.ContainsKey(4) ? this.SocialTab[4] : 0) + ", "
                    + (this.SocialTab.ContainsKey(38) ? this.SocialTab[38] : 0) + ", "
                    + (this.SocialTab.ContainsKey(1004) ? this.SocialTab[1004] : 0) + ", "
                    + (this.SocialTab.ContainsKey(1005) ? this.SocialTab[1005] : 0) + ", "
                    + (this.SocialTab.ContainsKey(64) ? this.SocialTab[64] : 0) + ", "
                    + (this.SocialTab.ContainsKey(32) ? this.SocialTab[32] : 0) + ", "
                    + (this.SocialTab.ContainsKey(1006) ? this.SocialTab[1006] : 0) + ", "
                    + (this.SocialTab.ContainsKey(1007) ? this.SocialTab[1007] : 0) + ")");
            }
        }
        #endregion

        #region Read Social Tab
        /// <summary>
        /// Read social tab from database
        /// TODO: catch exceptions
        /// </summary>
        public void ReadSocialTab()
        {
            lock (this.SocialTab)
            {
                SqlWrapper ms = new SqlWrapper();
                DataTable dt = ms.ReadDatatable("SELECT * FROM socialtab WHERE charid=" + this.Id);
                if (dt.Rows.Count > 0)
                {
                    this.SocialTab.Clear();
                    this.SocialTab.Add(0, (Int32)dt.Rows[0][1]);
                    this.SocialTab.Add(1, (Int32)dt.Rows[0][2]);
                    this.SocialTab.Add(2, (Int32)dt.Rows[0][3]);
                    this.SocialTab.Add(3, (Int32)dt.Rows[0][4]);
                    this.SocialTab.Add(4, (Int32)dt.Rows[0][5]);
                    this.SocialTab.Add(38, (Int32)dt.Rows[0][6]);
                    this.SocialTab.Add(1004, (Int32)dt.Rows[0][7]);
                    this.SocialTab.Add(1005, (Int32)dt.Rows[0][8]);
                    this.SocialTab.Add(64, (Int32)dt.Rows[0][9]);
                    this.SocialTab.Add(32, (Int32)dt.Rows[0][10]);
                    this.SocialTab.Add(1006, (Int32)dt.Rows[0][11]);
                    this.SocialTab.Add(1007, (Int32)dt.Rows[0][12]);
                }
                else
                {
                    // Nothing loaded, lets go with zeros
                    this.SocialTab.Add(0, 0);
                    this.SocialTab.Add(1, 0);
                    this.SocialTab.Add(2, 0);
                    this.SocialTab.Add(3, 0);
                    this.SocialTab.Add(4, 0);
                    this.SocialTab.Add(38, 0);
                    this.SocialTab.Add(1004, 0);
                    this.SocialTab.Add(1005, 0);
                    this.SocialTab.Add(64, 0);
                    this.SocialTab.Add(32, 0);
                    this.SocialTab.Add(1006, 0);
                    this.SocialTab.Add(1007, 0);
                }
            }
        }
        #endregion

        #region CalculateNextXP
        /// <summary>
        /// Calculate XP needed for next character level
        /// </summary>
        public void CalculateNextXP()
        {
            int level = this.stats.Level.Value;
            int ailevel = this.stats.AlienLevel.Value;

            double needaixp = XPTable.TableAlienXP[ailevel, 2];
            this.stats.AlienNextXP.StatBaseValue = Convert.ToUInt32(needaixp);

            if (level < 200) //we get XP
            {
                double needrkxp = XPTable.TableRKXP[level - 1, 2];
                this.stats.NextXP.StatBaseValue = Convert.ToUInt32(needrkxp);
            }
            if (level > 199) //we get SK
            {
                level -= 200;
                double needslsk = XPTable.TableShadowLandsSK[level, 2];
                this.stats.NextSK.StatBaseValue = Convert.ToUInt32(needslsk);
            }
        }
        #endregion

        #region Get Characters Target
        public Character TargetCharacter()
        {
            return (Character)FindDynel.FindDynelById(this.Target.Type, this.Target.Instance);
        }
        #endregion

        #region Get Characters fighting target
        public Character FightingCharacter()
        {
            return (Character)FindDynel.FindDynelById(this.FightingTarget.Type, this.FightingTarget.Instance);
        }
        #endregion

        #region Requirement Check
        public bool CheckRequirements(Character ch, AOFunctions aof, bool checkAll)
        {
            Character reqtarget = null;
            bool requirementsMet = true;
            int childOperator = -1; // Starting value
            bool foundCharRelated = (aof.FunctionType == Constants.FunctiontypeHairMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeBackMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeTexture)
                                    || (aof.FunctionType == Constants.FunctiontypeAttractorMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeCatMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeChangeBodyMesh)
                                    || (aof.FunctionType == Constants.functiontype_shouldermesh)
                                    || (aof.FunctionType == Constants.FunctiontypeHeadMesh);

            foreach (AORequirements aor in aof.Requirements)
            {
                switch (aor.Target)
                {
                    case Constants.ItemtargetUser:
                        {
                            reqtarget = ch;
                            break;
                        }
                    case Constants.ItemtargetWearer:
                        {
                            reqtarget = ch;
                            break;
                        }
                    case Constants.ItemtargetTarget:
                        {
                            // TODO: pass on target 
                            break;
                        }
                    case Constants.ItemtargetFightingtarget:
                        {
                            reqtarget = ch.FightingCharacter();
                            break;
                        }
                    case Constants.ItemtargetSelectedtarget:
                        {
                            reqtarget = ch.TargetCharacter();
                            break;
                        }
                    case Constants.ItemtargetSelf:
                        {
                            reqtarget = ch;
                            break;
                        }
                    default:
                        {
                            reqtarget = null;
                            break;
                        }
                }

                if (reqtarget == null)
                {
                    return false;
                    // Target not found, cant check reqs -> FALSE
                }

                int statval = reqtarget.stats.StatValueByName(aor.Statnumber);
                bool reqresult = true;
                switch (aor.Operator)
                {
                    case Constants.OperatorAnd:
                        {
                            reqresult = ((statval & aor.Value) != 0);
                            break;
                        }
                    case Constants.OperatorOr:
                        {
                            reqresult = ((statval | aor.Value) != 0);
                            break;
                        }
                    case Constants.OperatorEqualTo:
                        {
                            reqresult = (statval == aor.Value);
                            break;
                        }
                    case Constants.OperatorLessThan:
                        {
                            reqresult = (statval < aor.Value);
                            break;
                        }
                    case Constants.OperatorGreaterThan:
                        {
                            reqresult = (statval > aor.Value);
                            break;
                        }
                    case Constants.OperatorUnequal:
                        {
                            reqresult = (statval != aor.Value);
                            break;
                        }
                    case Constants.OperatorTrue:
                        {
                            reqresult = true;
                            break;
                        }
                    case Constants.OperatorFalse:
                        {
                            reqresult = false;
                            break;
                        }
                    case Constants.OperatorBitAnd:
                        {
                            reqresult = ((statval & aor.Value) != 0);
                            break;
                        }
                    case Constants.OperatorBitOr:
                        {
                            reqresult = ((statval | aor.Value) != 0);
                            break;
                        }

                    default:
                        {
                            // TRUE for now
                            reqresult = true;
                            break;
                        }
                }

                switch (childOperator)
                {
                    case Constants.OperatorAnd:
                        {
                            requirementsMet &= reqresult;
                            break;
                        }
                    case Constants.OperatorOr:
                        {
                            requirementsMet &= reqresult;
                            break;
                        }
                    case -1:
                        {
                            requirementsMet = reqresult;
                            break;
                        }
                    default:
                        break;
                }
                childOperator = aor.ChildOperator;
            }

            if (!checkAll)
            {
                if (foundCharRelated)
                {
                    requirementsMet &= foundCharRelated;
                }
                else
                {
                    requirementsMet = true;
                }
            }

            return requirementsMet;
        }
        #endregion

        #region Event execution
        public bool ExecuteEvent(
            Character ch,
            Character caller,
            AOEvents eve,
            bool dolocalstats,
            bool tosocialtab,
            int location,
            CheckReqs doreqs)
        {
            Character chartarget = (Character)FindDynel.FindDynelById(ch.Target.Type, ch.Target.Instance);
            Boolean reqs_met;
            if (ch != null)
            {
                foreach (AOFunctions aof in eve.Functions)
                {
                    if (doreqs == CheckReqs.doCheckReqs)
                    {
                        reqs_met = this.CheckRequirements(ch, aof, true);
                    }
                    else if (doreqs == CheckReqs.doEquipCheckReqs)
                    {
                        reqs_met = this.CheckRequirements(ch, aof, false);
                        ch.AddTimer(-9, DateTime.Now, aof, true);
                        continue;
                    }
                    else
                    {
                        reqs_met = true;
                    }

                    if (reqs_met)
                    {
                        if (eve.EventType == Constants.EventtypeOnWear)
                        {
                            AOFunctions aofcopy = aof.ShallowCopy();
                            aofcopy.Arguments.Values.Add(location);
                            ch.AddTimer(-9, DateTime.Now, aofcopy, dolocalstats);
                        }
                        else
                        {
                            ch.AddTimer(-9, DateTime.Now, aof, dolocalstats);
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        #region Apply Function on character (called mainly from timers)
        public void ApplyFunction(AOFunctions aof)
        {
            Program.FunctionC.CallFunction(aof.FunctionType, this, this, this, aof.Arguments.Values.ToArray());
        }
        #endregion

        #region ReApply Inventory
        public void REApplyEquip()
        {
            // Prevent server to send values back to client, its just recalculating here
            this.doNotDoTimers = true;

            // Clear all stat modifiers/Percentagemodifiers
            foreach (ClassStat cs in this.stats.All)
            {
                cs.StatModifier = 0;
                cs.StatPercentageModifier = 100;
            }
            this.ApplyInventory();
        }
        #endregion

        #region Add Item to Inventory
        public void AddItemToInventory(AOItem item)
        {
            // TODO: Check for full inventory/open overflow
            int nextfreespot = this.GetNextFreeInventory(0x68); // Main inventory
            InventoryEntries ie = new InventoryEntries();
            ie.Container = 0x68;
            ie.Item = item.ShallowCopy();
            ie.Placement = nextfreespot;
            this.inventory.Add(ie);
        }
        #endregion
    }

    /// <summary>
    /// Enumeration of Spin or Strafe directions
    /// </summary>
    public enum SpinOrStrafeDirections
    {
        None,

        Left,

        Right
    }

    /// <summary>
    /// Enumeration of Move directions
    /// </summary>
    public enum MoveDirections
    {
        None,

        Forwards,

        Backwards
    }

    /// <summary>
    /// Enumeration of Move modes
    /// </summary>
    public enum MoveModes
    {
        None,

        Rooted,

        Walk,

        Run,

        Swim,

        Crawl,

        Sneak,

        Fly,

        Sit,

        SocialTemp, // NV: What is this again exactly?
        Nothing,

        Sleep,

        Lounge
    }
}