CREATE TABLE  `instanced_items` (
  `InstanceID` int(32) NOT NULL,
  `Location` int(32) NOT NULL,
  `Item ID` int(32) NOT NULL,
  `Item LowID` int(32) NOT NULL,
  `Item HighID` int(32) NOT NULL,
  `Quality` int(32) NOT NULL,
  `Playfield` int(32) NOT NULL,
  `X` float NOT NULL,
  `Y` float NOT NULL,
  `Z` float NOT NULL,
  `HeadingX` float NOT NULL,
  `HeadingY` float NOT NULL,
  `HeadingZ` float NOT NULL,
  `HeadingW` float NOT NULL,
  `Container` int(32) NOT NULL,
  `Placement` int(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;