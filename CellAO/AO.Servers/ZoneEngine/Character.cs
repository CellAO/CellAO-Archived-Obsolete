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
        public string FirstName;

        /// <summary>
        /// Character's last name
        /// </summary>
        public string LastName;

        /// <summary>
        /// Organization ID
        /// </summary>
        private int orgId;

        /// <summary>
        /// Organization name
        /// </summary>
        public string OrgName;

        /// <summary>
        /// Is Character attacking?
        /// </summary>
        public int Attacking;

        /// <summary>
        /// Movement mode
        /// </summary>
        public int MovementMode;

        /// <summary>
        /// Don't process timers when true, double use, stats aren't sent back to client(s) when true
        /// </summary>
        public bool dontdotimers = true;

        /// <summary>
        /// Identity of the actual target
        /// </summary>
        public Identity Target;

        /// <summary>
        /// Identity of the fighting target
        /// </summary>
        public Identity FightingTarget;

        /// <summary>
        /// Identity of the followed target
        /// </summary>
        public Identity FollowTarget;

        /// <summary>
        /// Identity of the last trading target
        /// </summary>
        public Identity LastTrade;

        /// <summary>
        /// Stats
        /// </summary>
        public CharacterStats Stats;

        /// <summary>
        /// Inventory list
        /// </summary>
        public List<InventoryEntries> Inventory = new List<InventoryEntries>();

        /// <summary>
        /// Active Nanos list
        /// </summary>
        public List<AONano> ActiveNanos = new List<AONano>();

        /// <summary>
        /// Active Timers list (HD/ND, Nano effect timers etc)
        /// </summary>
        public List<AOTimers> Timers = new List<AOTimers>();

        /// <summary>
        /// Uploaded Nanos list
        /// </summary>
        public List<AOUploadedNanos> UploadedNanos = new List<AOUploadedNanos>();

        /// <summary>
        /// Bank inventory
        /// </summary>
        public List<AOItem> Bank = new List<AOItem>();

        /// <summary>
        /// Parent client
        /// </summary>
        public Client client;

        /// <summary>
        /// Caching Mesh layer structure
        /// </summary>
        public MeshLayers MeshLayer = new MeshLayers();

        /// <summary>
        /// Caching Mesh layer for social tab items
        /// </summary>
        public MeshLayers SocialMeshLayer = new MeshLayers();

        // Not sure what that is... Need to check my notes - Algorithman
        // declare Weaponsm Bool Array
        public bool[] WeaponsStored = new bool[109];

        public bool[] ItemsStored = new bool[109];

        // KnuBot Target
        protected Dynel _kbt;

        public Dynel KnuBotTarget
        {
            get
            {
                return this._kbt;
            }
            set
            {
                this._kbt = value;
            }
        }

        /// <summary>
        /// Create a new Character Dynel
        /// </summary>
        /// <param name="_id">ID of player</param>
        /// <param name="_playfield">initial Playfield number</param>
        public Character(int _id, int _playfield)
            : base(_id, _playfield)
        {
            lock (this)
            {
                this.ID = _id;
                this.PlayField = _playfield;
                // We're in the character class, so set Identifier 50000
                this.Type = 50000;
                this.ourType = 0;
                if (this.ID != 0)
                {
                    this.dontdotimers = true;
                    this.Stats = new CharacterStats(this);
                    this.readCoordsfromSQL();
                    this.readHeadingfromSQL();
                    this.Stats.ReadStatsfromSQL();
                    this.ReadSpecialStatsfromSQL();
                    this.readTimersfromSQL();
                    this.readInventoryfromSQL();
                    this.readUploadedNanosfromSQL();
                    this.readBankContentsfromSQL();
                    //                    MeshLayer.AddMesh(0, (Int32)Stats.HeadMesh.StatBaseValue, 0, 8);
                    //                    SocialMeshLayer.AddMesh(0, (Int32)Stats.HeadMesh.StatBaseValue, 0, 8);
                    this.ReadSocialTab();
                    //ApplyInventory();
                    //                    CalculateNextXP();
                    this.addHPNPtick();
                    this.startup = false;
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
            this.dontdotimers = true;
            lock (this)
            {
                this.writeBankContentstoSQL();
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
            lock (this.Timers)
            {
                this.Timers.Add(newtimer);
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
            lock (this.Timers)
            {
                int c = this.Timers.Count() - 1;
                while (c >= 0)
                {
                    if (this.Timers[c].Strain == strain)
                    {
                        this.Timers.RemoveAt(c);
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
        /// <param name="_now">Process all timers up to _now</param>
        public void processTimers(DateTime _now)
        {
            // Current Timer
            int c;
            // Current Strain
            int strain;
            // if Charachter is skipping timers Leave Function
            if (this.dontdotimers)
            {
                return;
            }

            lock (this)
            {
                // Backwards, easier to maintain integrity when removing something from the list
                for (c = this.Timers.Count - 1; c >= 0; c--)
                {
                    strain = this.Timers[c].Strain;
                    if (this.Timers[c].Timestamp <= _now)
                    {
                        this.ApplyFunction(this.Timers[c].Function);

                        if (this.Timers[c].Function.TickCount >= 0)
                        {
                            this.Timers[c].Function.TickCount--;
                        }

                        if (this.Timers[c].Function.TickCount == 0)
                        {
                            // Remove Timer if Ticks ran out
                            this.Timers.RemoveAt(c);
                        }
                        else
                        {
                            // Reinvoke the timer after the TickInterval
                            this.Timers[c].Timestamp = _now
                                                       + TimeSpan.FromMilliseconds(this.Timers[c].Function.TickInterval);
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
                return this.Stats.Clan.StatBaseValue;
            }
            set
            {
                string oldOrgName = this.OrgName;

                this.Stats.Clan.Set(value);

                if (value == 0)
                {
                    this.Stats.ClanLevel.Set(0);
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
        public TimeSpan predictionDuration
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

            turnSpeed = this.Stats.TurnSpeed.Value; // Stat #267 TurnSpeed

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
                case MoveMode.Run:
                    effectiveRunSpeed = this.Stats.RunSpeed.Value; // Stat #156 = RunSpeed
                    break;

                case MoveMode.Walk:
                    effectiveRunSpeed = -500;
                    break;

                case MoveMode.Swim:
                    // Swim speed is calculated the same as Run Speed except is half as effective
                    effectiveRunSpeed = this.Stats.Swim.Value >> 1; // Stat #138 = Swim
                    break;

                case MoveMode.Crawl:
                    effectiveRunSpeed = -600;
                    break;

                case MoveMode.Sneak:
                    effectiveRunSpeed = -500;
                    break;

                case MoveMode.Fly:
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

            if ((this.moveDirection == MoveDirection.None) || (!this.canMove()))
            {
                return 0;
            }

            effectiveRunSpeed = this.calculateEffectiveRunSpeed();

            if (this.moveDirection == MoveDirection.Forwards)
            {
                // NV: TODO: Verify this more. Especially with uber-low runspeeds (negative)
                speed = Math.Max(0, (effectiveRunSpeed * 0.005) + 4);

                if (this.moveMode != MoveMode.Swim)
                {
                    speed = Math.Min(15, speed); // Forward speed is capped at 15 units/sec for non-swimming
                }
            }
            else
            {
                // NV: TODO: Verify this more. Especially with uber-low runspeeds (negative)
                speed = -Math.Max(0, (effectiveRunSpeed * 0.0035) + 4);

                if (this.moveMode != MoveMode.Swim)
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
            if ((this.moveMode == MoveMode.Run) || (this.moveMode == MoveMode.Walk) || (this.moveMode == MoveMode.Swim)
                || (this.moveMode == MoveMode.Crawl) || (this.moveMode == MoveMode.Sneak)
                || (this.moveMode == MoveMode.Fly))
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
            if ((this.strafeDirection == SpinOrStrafeDirection.None) || (this.moveMode == MoveMode.Swim)
                || (this.moveMode == MoveMode.Crawl) || (!this.canMove()))
            {
                return 0;
            }

            effectiveRunSpeed = this.calculateEffectiveRunSpeed();

            // NV: TODO: Update this based off Forward runspeed when that is checked (strafe effective run speed = effective run speed / 2)
            speed = ((effectiveRunSpeed / 2) * 0.005) + 4;

            if (this.strafeDirection == SpinOrStrafeDirection.Left)
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
                forwardMove = Quaternion.RotateVector3(this.rawHeading, Vector3.AxisZ);
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
                strafeMove = Quaternion.RotateVector3(this.rawHeading, Vector3.AxisX);
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

            modifiedDuration = this.predictionDuration.TotalSeconds % turnTime;

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
                if (this.spinDirection == SpinOrStrafeDirection.None)
                {
                    return this.rawHeading;
                }
                else
                {
                    double turnArcAngle;
                    Quaternion turnQuaterion;
                    Quaternion newHeading;

                    turnArcAngle = this.calculateTurnArcAngle();
                    turnQuaterion = new Quaternion(Vector3.AxisY, turnArcAngle);

                    newHeading = Quaternion.Hamilton(turnQuaterion, this.rawHeading);
                    newHeading.Normalize();

                    return newHeading;
                }
            }
        }

        /// <summary>
        /// Enumeration of Spin or Strafe directions
        /// </summary>
        public enum SpinOrStrafeDirection
        {
            None,

            Left,

            Right
        }

        /// <summary>
        /// Enumeration of Move directions
        /// </summary>
        public enum MoveDirection
        {
            None,

            Forwards,

            Backwards
        }

        /// <summary>
        /// Enumeration of Move modes
        /// </summary>
        public enum MoveMode
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

        public SpinOrStrafeDirection spinDirection = SpinOrStrafeDirection.None;

        public SpinOrStrafeDirection strafeDirection = SpinOrStrafeDirection.None;

        public MoveDirection moveDirection = MoveDirection.None;

        public MoveMode moveMode = MoveMode.Run; // Run should be an appropriate default for now

        public MoveMode prevMoveMode = MoveMode.Run; // Run should be an appropriate default for now

        /// <summary>
        /// Stop movement (before teleporting for example)
        /// </summary>
        public void stopMovement()
        {
            // This should be used to stop the interpolating and save last interpolated value of movement before teleporting
            this.rawCoord = this.Coordinates;
            this.rawHeading = this.Heading;

            this.spinDirection = SpinOrStrafeDirection.None;
            this.strafeDirection = SpinOrStrafeDirection.None;
            this.moveDirection = MoveDirection.None;
        }

        /// <summary>
        /// Update move type
        /// </summary>
        /// <param name="moveType">new move type</param>
        public void updateMoveType(byte moveType)
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
                    this.moveDirection = MoveDirection.Forwards;
                    break;
                case 2: // Forward Stop
                    this.moveDirection = MoveDirection.None;
                    break;

                case 3: // Reverse Start
                    this.moveDirection = MoveDirection.Backwards;
                    break;
                case 4: // Reverse Stop
                    this.moveDirection = MoveDirection.None;
                    break;

                case 5: // Strafe Right Start
                    this.strafeDirection = SpinOrStrafeDirection.Right;
                    break;
                case 6: // Strafe Stop (Right)
                    this.strafeDirection = SpinOrStrafeDirection.None;
                    break;

                case 7: // Strafe Left Start
                    this.strafeDirection = SpinOrStrafeDirection.Left;
                    break;
                case 8: // Strafe Stop (Left)
                    this.strafeDirection = SpinOrStrafeDirection.None;
                    break;

                case 9: // Turn Right Start
                    this.spinDirection = SpinOrStrafeDirection.Right;
                    break;
                case 10: // Mouse Turn Right Start
                    break;
                case 11: // Turn Stop (Right)
                    this.spinDirection = SpinOrStrafeDirection.None;
                    break;

                case 12: // Turn Left Start
                    this.spinDirection = SpinOrStrafeDirection.Left;
                    break;
                case 13: // Mouse Turn Left Start
                    break;
                case 14: // Turn Stop (Left)
                    this.spinDirection = SpinOrStrafeDirection.None;
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
                    this.moveMode = MoveMode.Walk;
                    break;
                case 25: // Switch To Run Mode
                    this.moveMode = MoveMode.Run;
                    break;
                case 26: // Switch To Swim Mode
                    break;
                case 27: // Switch To Crawl Mode
                    this.prevMoveMode = this.moveMode;
                    this.moveMode = MoveMode.Crawl;
                    break;
                case 28: // Switch To Sneak Mode
                    this.prevMoveMode = this.moveMode;
                    this.moveMode = MoveMode.Sneak;
                    break;
                case 29: // Switch To Fly Mode
                    break;
                case 30: // Switch To Sit Ground Mode
                    this.prevMoveMode = this.moveMode;
                    this.moveMode = MoveMode.Sit;
                    this.Stats.NanoDelta.CalcTrickle();
                    this.Stats.HealDelta.CalcTrickle();
                    this.Stats.NanoInterval.CalcTrickle();
                    this.Stats.HealInterval.CalcTrickle();
                    break;

                case 31: // ? 19 = 20 = 22 = 31 = 32
                    break;
                case 32: // ? 19 = 20 = 22 = 31 = 32
                    break;

                case 33: // Switch To Sleep Mode
                    this.moveMode = MoveMode.Sleep;
                    break;
                case 34: // Switch To Lounge Mode
                    this.moveMode = MoveMode.Lounge;
                    break;

                case 35: // Leave Swim Mode
                    break;
                case 36: // Leave Sneak Mode
                    this.moveMode = this.prevMoveMode;
                    break;
                case 37: // Leave Sit Mode
                    this.moveMode = this.prevMoveMode;
                    this.Stats.NanoDelta.CalcTrickle();
                    this.Stats.HealDelta.CalcTrickle();
                    this.Stats.NanoInterval.CalcTrickle();
                    this.Stats.HealInterval.CalcTrickle();
                    break;
                case 38: // Leave Frozen Mode
                    break;
                case 39: // Leave Fly Mode
                    break;
                case 40: // Leave Crawl Mode
                    this.moveMode = this.prevMoveMode;
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
                if ((this.moveDirection == MoveDirection.None) && (this.strafeDirection == SpinOrStrafeDirection.None))
                {
                    return this.rawCoord;
                }
                else if (this.spinDirection == SpinOrStrafeDirection.None)
                {
                    Vector3 moveVector = this.calculateMoveVector();

                    moveVector = moveVector * this.predictionDuration.TotalSeconds;

                    return new AOCoord(this.rawCoord.coordinate + moveVector);
                }
                else
                {
                    Vector3 moveVector;
                    Vector3 positionFromCentreOfTurningCircle;
                    double turnArcAngle;
                    double y;
                    double duration;

                    duration = this.predictionDuration.TotalSeconds;

                    moveVector = this.calculateMoveVector();
                    turnArcAngle = this.calculateTurnArcAngle();

                    // This is calculated seperately as height is unaffected by turning
                    y = this.rawCoord.coordinate.y + (moveVector.y * duration);

                    if (this.spinDirection == SpinOrStrafeDirection.Left)
                    {
                        positionFromCentreOfTurningCircle = new Vector3(moveVector.z, y, -moveVector.x);
                    }
                    else
                    {
                        positionFromCentreOfTurningCircle = new Vector3(-moveVector.z, y, moveVector.x);
                    }

                    return
                        new AOCoord(
                            this.rawCoord.coordinate
                            +
                            Quaternion.RotateVector3(
                                new Quaternion(Vector3.AxisY, turnArcAngle), positionFromCentreOfTurningCircle)
                            - positionFromCentreOfTurningCircle);
                }
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
                    this.Stats.WriteStatstoSQL();
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
                        "SELECT `Name`, `FirstName`, `LastName` FROM " + this.getSQLTablefromDynelType()
                        + " WHERE ID = '" + this.ID + "' LIMIT 1");
                if (dt.Rows.Count == 1)
                {
                    this.Name = (string)dt.Rows[0][0];
                    this.FirstName = (string)dt.Rows[0][1];
                    this.LastName = (string)dt.Rows[0][2];
                }

                // Read stat# 5 (Clan) - OrgID from character stats table
                dt =
                    Sql.ReadDatatable(
                        "SELECT `Value` FROM " + this.getSQLTablefromDynelType() + "_stats WHERE ID = " + this.ID
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
                    "UPDATE " + this.getSQLTablefromDynelType() + " SET `Name` = '" + this.Name + "', `FirstName` = '"
                    + this.FirstName + "', `LastName` = '" + this.LastName + "' WHERE `ID` = " + "'" + this.ID + "'");
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
        public void readInventoryfromSQL()
        {
            lock (this.Inventory)
            {
                SqlWrapper ms = new SqlWrapper();
                {
                    InventoryEntries m_inv;
                    this.Inventory.Clear();
                    DataTable dt =
                        ms.ReadDatatable(
                            "SELECT * FROM " + this.getSQLTablefromDynelType() + "inventory WHERE ID="
                            + this.ID.ToString() + " AND container=104 ORDER BY placement ASC;");
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            m_inv = new InventoryEntries();
                            m_inv.Container = (Int32)row["container"];
                            m_inv.Placement = (Int32)row["placement"];
                            m_inv.Item.highID = (Int32)row["highid"];
                            m_inv.Item.lowID = (Int32)row["lowid"];
                            m_inv.Item.Quality = (Int32)row["quality"];
                            m_inv.Item.multiplecount = (Int32)row["multiplecount"];
                            m_inv.Item.Type = (Int32)row["type"];
                            m_inv.Item.Instance = (Int32)row["instance"];
                            m_inv.Item.flags = (Int32)row["flags"];
                            this.Inventory.Add(m_inv);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Write inventory to database
        /// TODO: catch exceptions
        /// </summary>
        public void writeInventorytoSQL()
        {
            lock (this.Inventory)
            {
                SqlWrapper ms = new SqlWrapper();
                int count;
                ms.SqlDelete(
                    "DELETE FROM " + this.getSQLTablefromDynelType() + "inventory WHERE ID=" + this.ID.ToString()
                    + " AND container=104;");
                for (count = 0; count < this.Inventory.Count; count++)
                {
                    if (this.Inventory[count].Container != -1) // dont save possible trade leftovers
                    {
                        ms.SqlInsert(
                            "INSERT INTO " + this.getSQLTablefromDynelType()
                            + "inventory (ID,placement,flags,multiplecount,lowid,highid,quality,container) values ("
                            + this.ID.ToString() + "," + this.Inventory[count].Placement.ToString() + ",1,"
                            + this.Inventory[count].Item.multiplecount.ToString() + ","
                            + this.Inventory[count].Item.lowID.ToString() + ","
                            + this.Inventory[count].Item.highID.ToString() + ","
                            + this.Inventory[count].Item.Quality.ToString() + ",104);");
                    }
                }
            }
        }

        /// <summary>
        /// Write inventory data into packetwriter
        /// </summary>
        /// <param name="writer">ref of the packetwriter</param>
        public void writeInventorytoPacket(ref PacketWriter writer)
        {
            int count;
            lock (this.Inventory)
            {
                writer.Push3F1Count(this.Inventory.Count);
                for (count = 0; count < this.Inventory.Count; count++)
                {
                    writer.PushInt(this.Inventory[count].Placement);
                    writer.PushShort((short)this.Inventory[count].Item.flags);
                    writer.PushShort((short)this.Inventory[count].Item.multiplecount);
                    writer.PushIdentity(this.Inventory[count].Item.Type, this.Inventory[count].Item.Instance);
                    writer.PushInt(this.Inventory[count].Item.lowID);
                    writer.PushInt(this.Inventory[count].Item.highID);
                    writer.PushInt(this.Inventory[count].Item.Quality);
                    writer.PushInt(this.Inventory[count].Item.Nothing);
                }
            }
        }
        #endregion

        #region Timers (read/write)
        /// <summary>
        /// Write timers to database
        /// TODO: catch exceptions
        /// </summary>
        public void writeTimerstoSQL()
        {
            lock (this.Timers)
            {
                SqlWrapper ms = new SqlWrapper();
                int count;

                // remove HP and NP tick
                count = this.Timers.Count;
                while (count > 0)
                {
                    count--;
                    if ((this.Timers[count].Strain == 0) || (this.Timers[count].Strain == 1))
                    {
                        this.Timers.RemoveAt(count);
                    }
                }

                ms.SqlDelete("DELETE FROM " + this.getSQLTablefromDynelType() + "timers WHERE ID=" + this.ID.ToString());
                TimeSpan ts;
                DateTime n = DateTime.Now;

                for (count = 0; count < this.Timers.Count; count++)
                {
                    ts = this.Timers[count].Timestamp - n;
                    ms.SqlInsert(
                        "INSERT INTO " + this.getSQLTablefromDynelType() + "timers VALUES (" + this.ID.ToString() + ","
                        + this.Timers[count].Strain + "," + this.Timers[count].Timestamp.Second.ToString() + ",X'"
                        + this.Timers[count].Function.ToBlob() + "');");
                }
            }
        }

        /// <summary>
        /// Read timers from database
        /// TODO: catch exceptions
        /// </summary>
        public void readTimersfromSQL()
        {
            lock (this.Timers)
            {
                SqlWrapper ms = new SqlWrapper();
                TimeSpan ts;
                DateTime n = DateTime.Now;
                AOTimers m_timer;
                byte[] blob = new byte[10240];
                this.Timers.Clear();

                ms.SqlRead(
                    "SELECT * FROM " + this.getSQLTablefromDynelType() + "timers WHERE ID=" + this.ID.ToString() + ";");
                DataTable dt =
                    ms.ReadDatatable(
                        "SELECT * FROM " + this.getSQLTablefromDynelType() + "timers WHERE ID=" + this.ID.ToString()
                        + ";");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ts = TimeSpan.FromSeconds((Int32)row["timespan"]);
                        m_timer = new AOTimers();
                        m_timer.Timestamp = DateTime.Now + ts;
                        m_timer.Strain = (Int32)row["strain"];
                        m_timer.Function = new AOFunctions();
                        MemoryStream memstream = new MemoryStream((byte[])row[3]);
                        BinaryFormatter bin = new BinaryFormatter();

                        m_timer.Function = (AOFunctions)bin.Deserialize(memstream);
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
        public void readNanosfromSQL()
        {
            lock (this.ActiveNanos)
            {
                SqlWrapper ms = new SqlWrapper();
                {
                    AONano m_an;

                    DataTable dt =
                        ms.ReadDatatable(
                            "SELECT * FROM " + this.getSQLTablefromDynelType() + "activenanos WHERE ID="
                            + this.ID.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            m_an = new AONano();
                            m_an.ID = (Int32)row["nanoID"];
                            m_an.NanoStrain = (Int32)row["strain"];
                            this.ActiveNanos.Add(m_an);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Write nanos to database
        /// TODO: catch exceptions
        /// </summary>
        public void writeNanostoSQL()
        {
            lock (this.ActiveNanos)
            {
                SqlWrapper ms = new SqlWrapper();
                int count;

                ms.SqlDelete(
                    "DELETE FROM " + this.getSQLTablefromDynelType() + "activenanos WHERE ID=" + this.ID.ToString());
                for (count = 0; count < this.ActiveNanos.Count; count++)
                {
                    ms.SqlInsert(
                        "INSERT INTO " + this.getSQLTablefromDynelType() + "activenanos VALUES (" + this.ID.ToString()
                        + "," + this.ActiveNanos[count].ID.ToString() + ","
                        + this.ActiveNanos[count].NanoStrain.ToString() + ")");
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
                    if (it.Events[counter].EventType == Constants.eventtype_onwear)
                    {
                        this.ExecuteEvent(
                            this, this, it.Events[counter], true, tosocialtab, location, CheckReqs.doCheckReqs);
                    }
                    counter++;
                }
                foreach (AOTimers t in this.Timers)
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
                    this.Stats.WeaponsStyle.Value |= it.GetWeaponStyle();
                    if (tosocialtab)
                    {
                        this.Stats.WeaponsStyle.Value = it.GetWeaponStyle();
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
                                this.Stats.WeaponMeshRight.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.MeshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
                            }
                            else
                            {
                                this.Stats.WeaponMeshLeft.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.MeshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
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
                                this.SocialMeshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
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
                                this.SocialMeshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
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
                    if (it.Events[counter].EventType == Constants.eventtype_onwear)
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
                    this.Stats.WeaponsStyle.Value |= it.GetWeaponStyle();
                    if (tosocialtab)
                    {
                        this.Stats.WeaponsStyle.Value = it.GetWeaponStyle();
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
                                this.Stats.WeaponMeshRight.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.MeshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
                            }
                            else
                            {
                                this.Stats.WeaponMeshLeft.Set((uint)it.Stats[f].Value); // Weaponmesh
                                this.MeshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
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
                                this.SocialMeshLayer.AddMesh(1, it.Stats[f].Value, 0, 4);
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
                                this.SocialMeshLayer.AddMesh(2, it.Stats[f].Value, 0, 4);
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
        /// <param name="_from">From location</param>
        /// <param name="_to">To location</param>
        public void switchItems(Client cli, int _from, int _to)
        {
            lock (cli)
            {
                SqlWrapper mySql = new SqlWrapper();
                InventoryEntries afrom = this.getInventoryAt(_from);
                InventoryEntries ato = this.getInventoryAt(_to);
                if (afrom != null)
                {
                    afrom.Placement = _to;
                }
                if (ato != null)
                {
                    ato.Placement = _from;
                }

                mySql.SqlUpdate(
                    "UPDATE " + this.getSQLTablefromDynelType() + "inventory SET placement=255 where (ID="
                    + cli.Character.ID.ToString() + ") AND (placement=" + _from.ToString() + ")");
                mySql.SqlUpdate(
                    "UPDATE " + this.getSQLTablefromDynelType() + "inventory SET placement=" + _from.ToString()
                    + " where (ID=" + cli.Character.ID.ToString() + ") AND (placement=" + _to.ToString() + ")");
                mySql.SqlUpdate(
                    "UPDATE " + this.getSQLTablefromDynelType() + "inventory SET placement=" + _to.ToString()
                    + " where (ID=" + cli.Character.ID.ToString() + ") AND (placement=255)");
            }

            // If its a switch from or to equipment pages then recalculate the skill modifiers
            if ((_from < 64) || (_to < 64))
            {
                this.CalculateSkills();
            }
        }
        #endregion

        #region Switch item placement to bank account
        /// <summary>
        /// Transfer Item to bank account
        /// </summary>
        /// <param name="_from">from inventory location</param>
        /// <param name="_to">to bank location</param>
        public void TransferItemtoBank(int _from, int _to)
        {
            lock (this)
            {
                InventoryEntries afrom = this.getInventoryAt(_from);

                AOItem tobank = new AOItem();
                tobank = afrom.Item.ShallowCopy();
                tobank.flags = this.GetNextFreeInventory(0x69);
                this.Inventory.Remove(afrom);
                this.Bank.Add(tobank);
                this.writeBankContentstoSQL();
                this.writeInventorytoSQL();
            }
        }
        #endregion

        #region Transfer Item from bank to inventory
        /// <summary>
        /// Transfer item from bank account to inventory
        /// </summary>
        /// <param name="_from">from bank location</param>
        /// <param name="_to">to inventory location</param>
        public void TransferItemfromBank(int _from, int _to)
        {
            lock (this)
            {
                int placement = this.GetNextFreeInventory(0x68);

                AOItem tempitem = null;
                foreach (AOItem aoi in this.Bank)
                {
                    if (aoi.flags == _from)
                    {
                        tempitem = aoi;
                        break;
                    }
                }
                if (tempitem == null)
                {
                    Console.WriteLine("Not valid item...");
                    return;
                }

                InventoryEntries mi = new InventoryEntries();
                AOItem it = ItemHandler.GetItemTemplate(tempitem.lowID);
                mi.Placement = placement;
                mi.Container = 104;
                mi.Item.lowID = tempitem.lowID;
                mi.Item.highID = tempitem.highID;
                mi.Item.Quality = tempitem.Quality;
                mi.Item.multiplecount = Math.Max(1, tempitem.multiplecount);
                this.Inventory.Add(mi);
                this.Bank.Remove(tempitem);
                this.writeBankContentstoSQL();
                this.writeInventorytoSQL();
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
                int oldhealth = this.Stats.Health.Value;
                int oldnano = this.Stats.CurrentNano.Value;
                AOItem m_item;

                this.SocialMeshLayer = new MeshLayers();
                this.Textures = new List<AOTextures>();
                this.MeshLayer = new MeshLayers();
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
                this.Stats.ClearModifiers();
                this.MeshLayer.AddMesh(0, this.Stats.HeadMesh.Value, 0, 4);
                this.SocialMeshLayer.AddMesh(0, this.Stats.HeadMesh.Value, 0, 4);

                // Apply all modifying item functions to localstats
                for (c = 0; c < this.Inventory.Count; c++)
                {
                    // only process items in the equipment pages (<64)
                    if (this.Inventory[c].Placement < 64)
                    {
                        m_item = ItemHandler.interpolate(
                            this.Inventory[c].Item.lowID, this.Inventory[c].Item.highID, this.Inventory[c].Item.Quality);
                        for (c2 = 0; c2 < m_item.Events.Count; c2++)
                        {
                            if (m_item.Events[c2].EventType == Constants.eventtype_onwear)
                            {
                                for (c3 = 0; c3 < m_item.Events[c2].Functions.Count; c3++)
                                {
                                    if (this.CheckRequirements(this, m_item.Events[c2].Functions[c3], false))
                                    {
                                        AOFunctions aof_withparams = m_item.Events[c2].Functions[c3].ShallowCopy();
                                        aof_withparams.Arguments.Add(this.Inventory[c].Placement);
                                        Program.FunctionC.CallFunction(
                                            aof_withparams.FunctionType,
                                            this,
                                            this,
                                            this,
                                            aof_withparams.Arguments.ToArray());
                                    }
                                    if ((m_item.Events[c2].Functions[c3].FunctionType == Constants.functiontype_modify)
                                        ||
                                        (m_item.Events[c2].Functions[c3].FunctionType
                                         == Constants.functiontype_modifypercentage))
                                    {
                                        // TODO ItemHandler.FunctionPack.func_do(this, m_item.ItemEvents[c2].Functions[c3], true, Inventory[c].Placement >= 49, Inventory[c].Placement);
                                    }
                                }
                            }
                        }
                    }
                }

                // Adding nano skill effects
                for (c = 0; c < this.ActiveNanos.Count; c++)
                {
                    // TODO: Nanohandler, similar to Itemhandler
                    // and calling the skill/attribute modifying functions
                }

                // Calculating the trickledown
                this.Stats.Strength.AffectStats();
                this.Stats.Agility.AffectStats();
                this.Stats.Stamina.AffectStats();
                this.Stats.Intelligence.AffectStats();
                this.Stats.Sense.AffectStats();
                this.Stats.Psychic.AffectStats();

                this.Stats.Health.StatBaseValue = this.Stats.Health.GetMaxValue((uint)oldhealth);
                this.Stats.CurrentNano.StatBaseValue = this.Stats.CurrentNano.GetMaxValue((uint)oldnano);
            }
        }
        #endregion

        #region Purge
        public bool needpurge = true;

        /// <summary>
        /// Write data to database when purging character object
        /// </summary>
        public void Purge()
        {
            lock (this)
            {
                if ((this.ID != 0) && (this.needpurge))
                {
                    this.needpurge = false;

                    this.writeCoordinatestoSQL();
                    this.writeHeadingtoSQL();
                    this.WriteStats();
                    this.writeNanostoSQL();
                    this.writeInventorytoSQL();
                    this.writeTimerstoSQL();
                    this.writeUploadedNanostoSQL();
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
            Packets.AppearanceUpdate.Appearance_Update(this);
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
            lock (this.Inventory)
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
                        foreach (AOItem i in this.Bank)
                        {
                            if (i.flags == c)
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
                        foreach (InventoryEntries i in this.Inventory)
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
        public InventoryEntries getInventoryAt(int place)
        {
            lock (this.Inventory)
            {
                foreach (InventoryEntries i in this.Inventory)
                {
                    if (i.Placement == place)
                    {
                        return i;
                    }
                }
                return null;
            }
        }

        public InventoryEntries getInventoryAt(int place, int container)
        {
            lock (this.Inventory)
            {
                foreach (InventoryEntries i in this.Inventory)
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
        /// <param name="_id">Nano-ID</param>
        public void UploadNano(int _id)
        {
            lock (this.UploadedNanos)
            {
                AOUploadedNanos au = new AOUploadedNanos();
                au.Nano = _id;
                this.UploadedNanos.Remove(au); // In case its in already :)
                this.UploadedNanos.Add(au);
            }
        }

        /// <summary>
        /// Write uploaded nanos to database
        /// TODO: catch exceptions
        /// </summary>
        public void writeUploadedNanostoSQL()
        {
            lock (this.UploadedNanos)
            {
                foreach (AOUploadedNanos au in this.UploadedNanos)
                {
                    SqlWrapper Sql = new SqlWrapper();
                    Sql.SqlInsert(
                        "REPLACE INTO " + this.getSQLTablefromDynelType() + "uploadednanos VALUES ("
                        + this.ID.ToString() + "," + au.Nano.ToString() + ")");
                }
            }
        }

        /// <summary>
        /// Read uploaded nanos from database
        /// TODO: catch exceptions
        /// </summary>
        public void readUploadedNanosfromSQL()
        {
            lock (this.UploadedNanos)
            {
                this.UploadedNanos.Clear();
                SqlWrapper Sql = new SqlWrapper();
                DataTable dt =
                    Sql.ReadDatatable(
                        "SELECT nano FROM " + this.getSQLTablefromDynelType() + "uploadednanos WHERE ID="
                        + this.ID.ToString());
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AOUploadedNanos au = new AOUploadedNanos();
                        au.Nano = (Int32)row["Nano"];
                        this.UploadedNanos.Add(au);
                    }
                }
            }
        }
        #endregion

        #region add HP/NP tick timers
        /// <summary>
        /// Initialize HP and NP timers (HD/ND ticks)
        /// </summary>
        public void addHPNPtick()
        {
            lock (this.Timers)
            {
                this.Stats.HealInterval.CalcTrickle();
                this.Stats.NanoInterval.CalcTrickle();
                AOTimers at = new AOTimers();
                at.Strain = -1;
                at.Timestamp = DateTime.Now + TimeSpan.FromMilliseconds(100);
                at.Function.Target = this.ID;
                at.Function.TickCount = -2;
                at.Function.TickInterval = 100;
                at.Function.FunctionType = 1; // MAIN stat send timer
                this.Timers.Add(at);
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
        /// <param name="packet">ref to datapacket</param>
        public void setTarget(ref byte[] packet)
        {
            lock (this)
            {
                PacketReader pr = new PacketReader(ref packet);

                Header head = pr.PopHeader();
                pr.PopByte();
                this.Target = pr.PopIdentity();
                pr.Finish();
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
            lock (this.Inventory)
            {
                AOItem it = ItemHandler.GetItemTemplate(ie.Item.lowID);
                //            if (it.isStackable())
                {
                    foreach (InventoryEntries ia in this.Inventory)
                    {
                        if ((ia.Item.lowID == ie.Item.lowID) && (ia.Item.highID == ie.Item.highID)
                            && (ia.Container == -1))
                        {
                            ia.Item.multiplecount += ie.Item.multiplecount;
                            if (ia.Item.multiplecount == 0)
                            {
                                this.Inventory.Remove(ia);
                            }
                            return;
                        }
                    }
                }
                this.Inventory.Add(ie);
            }
        }
        #endregion

        #region BankContents
        /// <summary>
        /// Read bank account from database
        /// TODO: catch exceptions
        /// </summary>
        public void readBankContentsfromSQL()
        {
            lock (this.Bank)
            {
                SqlWrapper ms = new SqlWrapper();
                this.Bank.Clear();
                DataTable dt =
                    ms.ReadDatatable("SELECT * FROM bank WHERE charID=" + this.ID.ToString() + " ORDER BY InventoryID ASC");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AOItem item = new AOItem();
                        item.flags = (Int32)row["InventoryID"];
                        item.lowID = (Int32)row["lowID"];
                        item.highID = (Int32)row["highID"];
                        item.multiplecount = (Int32)row["Amount"];
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
                        this.Bank.Add(item);
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
        public void writeBankContentstoSQL()
        {
            SqlWrapper ms = new SqlWrapper();
            ms.SqlDelete("DELETE FROM bank WHERE charID=" + this.ID.ToString());
            foreach (AOItem temp in this.Bank)
            {
                string insert = "INSERT INTO bank VALUES(" + this.ID.ToString() + "," + temp.flags.ToString() + ","
                                + temp.lowID.ToString() + "," + temp.highID.ToString() + ","
                                + temp.multiplecount.ToString() + "," + temp.Quality.ToString() + ","
                                + temp.Type.ToString() + "," + temp.Instance.ToString() + ",X'";
                foreach (AOItemAttribute tempattr in temp.Stats)
                {
                    insert = insert + this.reverseString(tempattr.Stat.ToString("X8"))
                             + this.reverseString(tempattr.Stat.ToString("X8"));
                }
                insert = insert + "');";
                ms.SqlInsert(insert);
            }
        }
        #endregion

        #region ReadSpecialStatsfromSQL
        /// <summary>
        /// Read special stats (GM, Account, Expansions etc)
        /// TODO: catch exceptions
        /// </summary>
        public void ReadSpecialStatsfromSQL()
        {
            lock (this)
            {
                SqlWrapper sql = new SqlWrapper();
                DataTable dt =
                    sql.ReadDatatable(
                        "SELECT `ID`, `Expansions`, `AccountFlags`, `GM` FROM `login` WHERE `Username` = (SELECT `Username` FROM `characters` WHERE `ID` = "
                        + this.ID + ");");
                if (dt.Rows.Count > 0)
                {
                    // Stat 607 is PlayerID
                    this.Stats.SetBaseValue(607, (UInt32)(Int32)dt.Rows[0][0]);

                    // Stat 389 is Expansion
                    this.Stats.SetBaseValue(389, UInt32.Parse((string)dt.Rows[0][1]));

                    // Stat 660 is AccountFlags
                    this.Stats.SetBaseValue(660, (UInt32)(Int32)dt.Rows[0][2]);

                    // Stat 215 is GmLevel
                    this.Stats.SetBaseValue(215, (UInt32)(Int32)dt.Rows[0][3]);
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
                AOItem it;
                foreach (InventoryEntries ie in this.Inventory)
                {
                    if (ie.Placement < 64) // only process equipped items 
                    {
                        it = ItemHandler.interpolate(ie.Item.lowID, ie.Item.highID, ie.Item.Quality);
                        this.FakeEquipItem(it, this, ie.Placement >= 48, ie.Placement);
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
                    "REPLACE INTO socialtab VALUES (" + this.ID + ", "
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
                DataTable dt = ms.ReadDatatable("SELECT * FROM socialtab WHERE charid=" + this.ID);
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
            int level = this.Stats.Level.Value;
            int ailevel = this.Stats.AlienLevel.Value;

            double needaixp = Program.zoneServer.XPproLevel.tableAIXP[ailevel, 2];
            this.Stats.AlienNextXP.StatBaseValue = Convert.ToUInt32(needaixp);

            if (level < 200) //we get XP
            {
                double needrkxp = Program.zoneServer.XPproLevel.tableRKXP[level - 1, 2];
                this.Stats.NextXP.StatBaseValue = Convert.ToUInt32(needrkxp);
            }
            if (level > 199) //we get SK
            {
                level -= 200;
                double needslsk = Program.zoneServer.XPproLevel.tableSLSK[level, 2];
                this.Stats.NextSK.StatBaseValue = Convert.ToUInt32(needslsk);
            }
        }
        #endregion

        #region Get Characters Target
        public Character GetTarget()
        {
            return (Character)FindDynel.FindDynelByID(this.Target.Type, this.Target.Instance);
        }
        #endregion

        #region Get Characters fighting target
        public Character GetFightingTarget()
        {
            return (Character)FindDynel.FindDynelByID(this.FightingTarget.Type, this.FightingTarget.Instance);
        }
        #endregion

        #region Requirement Check
        public bool CheckRequirements(Character ch, AOFunctions aof, bool CheckAll)
        {
            Character reqtarget = null;
            bool reqs_met = true;
            int childop = -1; // Starting value
            bool foundCharRelated = false;
            if ((aof.FunctionType == Constants.functiontype_hairmesh)
                || (aof.FunctionType == Constants.functiontype_backmesh)
                || (aof.FunctionType == Constants.functiontype_texture)
                || (aof.FunctionType == Constants.functiontype_attractormesh)
                || (aof.FunctionType == Constants.functiontype_catmesh)
                || (aof.FunctionType == Constants.functiontype_changebodymesh)
                || (aof.FunctionType == Constants.functiontype_shouldermesh)
                || (aof.FunctionType == Constants.functiontype_headmesh)) // I hope i got them all
            {
                foundCharRelated = true;
            }

            foreach (AORequirements aor in aof.Requirements)
            {
                switch (aor.Target)
                {
                    case Constants.itemtarget_user:
                        {
                            reqtarget = ch;
                            break;
                        }
                    case Constants.itemtarget_wearer:
                        {
                            reqtarget = ch;
                            break;
                        }
                    case Constants.itemtarget_target:
                        {
                            // TODO: pass on target 
                            break;
                        }
                    case Constants.itemtarget_fightingtarget:
                        {
                            reqtarget = ch.GetFightingTarget();
                            break;
                        }
                    case Constants.itemtarget_selectedtarget:
                        {
                            reqtarget = ch.GetTarget();
                            break;
                        }
                    case Constants.itemtarget_self:
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

                int statval = reqtarget.Stats.Get(aor.Statnumber);
                bool reqresult = true;
                switch (aor.Operator)
                {
                    case Constants.operator_and:
                        {
                            reqresult = ((statval & aor.Value) != 0);
                            break;
                        }
                    case Constants.operator_or:
                        {
                            reqresult = ((statval | aor.Value) != 0);
                            break;
                        }
                    case Constants.operator_equalto:
                        {
                            reqresult = (statval == aor.Value);
                            break;
                        }
                    case Constants.operator_lessthan:
                        {
                            reqresult = (statval < aor.Value);
                            break;
                        }
                    case Constants.operator_greaterthan:
                        {
                            reqresult = (statval > aor.Value);
                            break;
                        }
                    case Constants.operator_unequal:
                        {
                            reqresult = (statval != aor.Value);
                            break;
                        }
                    case Constants.operator_true:
                        {
                            reqresult = true;
                            break;
                        }
                    case Constants.operator_false:
                        {
                            reqresult = false;
                            break;
                        }
                    case Constants.operator_bitand:
                        {
                            reqresult = ((statval & aor.Value) != 0);
                            break;
                        }
                    case Constants.operator_bitor:
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

                switch (childop)
                {
                    case Constants.operator_and:
                        {
                            reqs_met &= reqresult;
                            break;
                        }
                    case Constants.operator_or:
                        {
                            reqs_met &= reqresult;
                            break;
                        }
                    case -1:
                        {
                            reqs_met = reqresult;
                            break;
                        }
                    default:
                        break;
                }
                childop = aor.ChildOperator;
            }

            if (!CheckAll)
            {
                if (foundCharRelated)
                {
                    reqs_met &= foundCharRelated;
                }
                else
                {
                    reqs_met = true;
                }
            }

            return reqs_met;
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
            Character chartarget = (Character)FindDynel.FindDynelByID(ch.Target.Type, ch.Target.Instance);
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
                        if (eve.EventType == Constants.eventtype_onwear)
                        {
                            AOFunctions aofcopy = aof.ShallowCopy();
                            aofcopy.Arguments.Add(location);
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
            Program.FunctionC.CallFunction(aof.FunctionType, this, this, this, aof.Arguments.ToArray());
        }
        #endregion

        #region ReApply Inventory
        public void ReEquip()
        {
            // Prevent server to send values back to client, its just recalculating here
            this.dontdotimers = true;

            // Clear all stat modifiers/Percentagemodifiers
            foreach (ClassStat cs in this.Stats.all)
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
            this.Inventory.Add(ie);
        }
        #endregion
    }
}