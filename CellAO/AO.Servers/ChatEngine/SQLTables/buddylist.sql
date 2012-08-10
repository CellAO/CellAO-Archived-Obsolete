CREATE TABLE IF NOT EXISTS `buddylist` (
  `PlayerID` int(32) NOT NULL,
  `BuddyID` int(32) NOT NULL,
  PRIMARY KEY  (`PlayerID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
