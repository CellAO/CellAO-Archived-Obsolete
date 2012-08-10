CREATE TABLE  `characters_stats` (
  `ID` int(32) NOT NULL,
  `Stat` int(32) NOT NULL,
  `Value` int(32) NOT NULL,
  PRIMARY KEY (`ID`,`Stat`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;