CREATE TABLE  `login` (
  `ID` int(32) NOT NULL AUTO_INCREMENT,
  `CreationDate` datetime NOT NULL,
  `Email` varchar(64) NOT NULL,
  `FirstName` varchar (32) NOT NULL,
  `LastName` varchar (32) NOT NULL,
  `Username` varchar(32) NOT NULL,
  `Password` varchar(37) NOT NULL,
  `Allowed_Characters` int(32) NOT NULL DEFAULT '6' COMMENT 'You can change this to whatever you want 0 is disabled.. no characters allowed',
  `Flags` int(32) NOT NULL DEFAULT '0',
  `AccountFlags` int(32) NOT NULL DEFAULT '0',
  `Expansions` varchar(16) NOT NULL DEFAULT '127',
  `GM` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;