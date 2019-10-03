-- --------------------------------------------------------
-- Hôte :                        mvinet.fr
-- Version du serveur:           10.1.38-MariaDB-0+deb9u1 - Debian 9.8
-- SE du serveur:                debian-linux-gnu
-- HeidiSQL Version:             10.1.0.5464
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Listage de la structure de la base pour BookYourCar
CREATE DATABASE IF NOT EXISTS `BookYourCar` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;
USE `BookYourCar`;

-- Listage de la structure de la table BookYourCar. COMMENTS
CREATE TABLE IF NOT EXISTS `COMMENTS` (
  `COMMENT_ID` int(11) NOT NULL AUTO_INCREMENT,
  `COMMENT_DATE` datetime DEFAULT NULL,
  `COMMENT_LOC_ID` int(11) DEFAULT NULL,
  `COMMENT_TEXT` text,
  `COMMENT_USER_ID` int(11) DEFAULT NULL,
  `COMMENT_VEH_ID` int(11) DEFAULT NULL,
  PRIMARY KEY (`COMMENT_ID`),
  KEY `FK_COMMENTS_VEHICLE` (`COMMENT_VEH_ID`),
  KEY `FK_COMMENTS_USER` (`COMMENT_USER_ID`),
  KEY `FK_COMMENTS_LOCATION` (`COMMENT_LOC_ID`),
  CONSTRAINT `FK_COMMENTS_LOCATION` FOREIGN KEY (`COMMENT_LOC_ID`) REFERENCES `LOCATION` (`LOC_ID`),
  CONSTRAINT `FK_COMMENTS_USER` FOREIGN KEY (`COMMENT_USER_ID`) REFERENCES `USER` (`USER_ID`),
  CONSTRAINT `FK_COMMENTS_VEHICLE` FOREIGN KEY (`COMMENT_VEH_ID`) REFERENCES `VEHICLE` (`VEH_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=116 DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la procédure BookYourCar. getAvailableVehicle
DELIMITER //
CREATE DEFINER=`a5d`@`%` PROCEDURE `getAvailableVehicle`(
	IN `DATEDEBUT` datetime,
	IN `DATEFIN` datetime,
	IN `POLESTART` int,
	IN `POLEEND` int
)
BEGIN

  -- -- -- -- -- -- -- -- -- -- -- --  --
  --      DÉCLARATION DES VARIABLES    --
  -- -- -- -- -- -- -- -- -- -- -- --  --

  DECLARE vehicleId INT;
  DECLARE isGoodLocation BOOLEAN DEFAULT FALSE;

  -- On déclare fin comme un BOOLEAN, avec FALSE pour défaut
  -- pour détecter quand la boucle est finie
  DECLARE fin BOOLEAN DEFAULT FALSE;

  -- Curseur qui retourne la liste des identifiants de vehicle disponible
  DECLARE curs_vehicle CURSOR FOR
    SELECT DISTINCT V.VEH_ID
    FROM VEHICLE V
           LEFT JOIN LOCATION L ON V.VEH_ID = L.LOC_VEH_ID
    WHERE
       -- Récupération des véhicules qui n'ont pas de location
        L.LOC_ID IS NULL

        -- récupération du véhicule au pole ou il est
        AND V.VEH_POLE_ID = POLESTART

       -- Récupération des identifiants de véhicule qui ne sont pas en location sur une période données
       OR V.VEH_ID NOT IN (
      SELECT DISTINCT LOC_VEH_ID
      FROM LOCATION
      WHERE LOC_VEH_ID IS NOT NULL
        AND (
          DATEDEBUT between LOC_DATESTARTLOCATION and LOC_DATEENDLOCATION
          OR DATEFIN between LOC_DATESTARTLOCATION AND LOC_DATEENDLOCATION
          OR LOC_DATESTARTLOCATION between DATEDEBUT AND DATEFIN
          OR LOC_DATEENDLOCATION between DATEDEBUT AND DATEFIN
        )
    );

  -- Curseur qui permet de voir si la prochaine location est disponible grâce a un id vehicule
  DECLARE curs_check_next_location CURSOR FOR
    SELECT LOC_POLE_IDSTART = POLEEND
    FROM VEHICLE V
           LEFT JOIN LOCATION L ON V.VEH_ID = L.LOC_VEH_ID
    WHERE L.LOC_VEH_ID = vehicleId
      AND L.LOC_DATESTARTLOCATION > DATEFIN -- date de fin
    ORDER BY LOC_DATESTARTLOCATION
    LIMIT 1;

  DECLARE CONTINUE HANDLER FOR NOT FOUND SET fin = TRUE;

  -- -- -- -- -- -- -- -- -- -- --
  --     DÉBUT DU TRAITEMENT    --
  -- -- -- -- -- -- -- -- -- -- --

  DROP TABLE IF EXISTS result;

  -- Création d'une table temp vide a partir de la table vehicule
  -- 1 = 2 pour créer une table vide (car condition toujours fausse)
  CREATE TEMPORARY TABLE result
  SELECT * FROM VEHICLE WHERE 1 = 2;

  OPEN curs_vehicle;

  REPEAT
    FETCH curs_vehicle into vehicleId;

    OPEN curs_check_next_location;
    FETCH curs_check_next_location INTO isGoodLocation;
    IF isGoodLocation THEN
      -- Si bonne location, on ajoute a la liste des resultats
      INSERT INTO result SELECT * FROM VEHICLE where VEH_ID = vehicleId;
    END IF;
    CLOSE curs_check_next_location;

  UNTIL fin END REPEAT;

  CLOSE curs_vehicle;

  SELECT * FROM result;

END//
DELIMITER ;

-- Listage de la structure de la table BookYourCar. HISTORYMAINTENANCE
CREATE TABLE IF NOT EXISTS `HISTORYMAINTENANCE` (
  `HIST_ID` int(11) NOT NULL AUTO_INCREMENT,
  `HIST_CITYGARAGE` varchar(100) DEFAULT NULL,
  `HIST_CPGARAGE` varchar(5) DEFAULT NULL,
  `HIST_DATEENDMAINTENANCE` datetime DEFAULT NULL,
  `HIST_DATESTARTMAINTENANCE` datetime DEFAULT NULL,
  `HIST_REFFACTURE` varchar(100) DEFAULT NULL,
  `HIST_VEH_ID` int(11) DEFAULT NULL,
  PRIMARY KEY (`HIST_ID`),
  KEY `FK_HISTORYMAINTENANCE_VEHICLE` (`HIST_VEH_ID`),
  CONSTRAINT `FK_HISTORYMAINTENANCE_VEHICLE` FOREIGN KEY (`HIST_VEH_ID`) REFERENCES `VEHICLE` (`VEH_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. IMAGES
CREATE TABLE IF NOT EXISTS `IMAGES` (
  `IMAGE_ID` int(11) NOT NULL AUTO_INCREMENT,
  `IMAGE_URI` varchar(255) DEFAULT NULL,
  `IMAGE_VEH_ID` int(11) DEFAULT NULL,
  `IMAGE_USER_ID` int(11) DEFAULT NULL,
  PRIMARY KEY (`IMAGE_ID`),
  KEY `FK_IMAGES_VEHICLE` (`IMAGE_VEH_ID`),
  KEY `FK_IMAGES_USER` (`IMAGE_USER_ID`),
  CONSTRAINT `FK_IMAGES_USER` FOREIGN KEY (`IMAGE_USER_ID`) REFERENCES `USER` (`USER_ID`),
  CONSTRAINT `FK_IMAGES_VEHICLE` FOREIGN KEY (`IMAGE_VEH_ID`) REFERENCES `VEHICLE` (`VEH_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. KEY
CREATE TABLE IF NOT EXISTS `KEY` (
  `KEY_ID` int(11) NOT NULL AUTO_INCREMENT,
  `KEY_AVAILABLE` tinyint(1) DEFAULT NULL,
  `KEY_CAR_ID` int(11) DEFAULT NULL,
  `KEY_LOCALISATION` varchar(100) DEFAULT NULL,
  `KEY_STATUS` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`KEY_ID`),
  KEY `FK_KEY_VEHICLE` (`KEY_CAR_ID`),
  CONSTRAINT `FK_KEY_VEHICLE` FOREIGN KEY (`KEY_CAR_ID`) REFERENCES `VEHICLE` (`VEH_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. LOCATION
CREATE TABLE IF NOT EXISTS `LOCATION` (
  `LOC_ID` int(11) NOT NULL AUTO_INCREMENT,
  `LOC_DATEENDLOCATION` datetime NOT NULL,
  `LOC_DATESTARTLOCATION` datetime NOT NULL,
  `LOC_POLE_IDEND` int(11),
  `LOC_POLE_IDSTART` int(11),
  `LOC_STATE` tinyint(4) NOT NULL DEFAULT '0',
  `LOC_USER_ID` int(11) DEFAULT NULL,
  `LOC_VEH_ID` int(11) DEFAULT NULL,
  PRIMARY KEY (`LOC_ID`),
  KEY `FK_LOCATION_VEHICLE` (`LOC_VEH_ID`),
  KEY `FK_LOCATION_USER` (`LOC_USER_ID`),
  KEY `FK_LOCATION_POLE` (`LOC_POLE_IDEND`),
  KEY `FK_LOCATION_POLE_2` (`LOC_POLE_IDSTART`),
  CONSTRAINT `FK_LOCATION_POLE` FOREIGN KEY (`LOC_POLE_IDEND`) REFERENCES `POLE` (`POLE_ID`),
  CONSTRAINT `FK_LOCATION_POLE_2` FOREIGN KEY (`LOC_POLE_IDSTART`) REFERENCES `POLE` (`POLE_ID`),
  CONSTRAINT `FK_LOCATION_USER` FOREIGN KEY (`LOC_USER_ID`) REFERENCES `USER` (`USER_ID`),
  CONSTRAINT `FK_LOCATION_VEHICLE` FOREIGN KEY (`LOC_VEH_ID`) REFERENCES `VEHICLE` (`VEH_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=132 DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. POLE
CREATE TABLE IF NOT EXISTS `POLE` (
  `POLE_ID` int(11) NOT NULL AUTO_INCREMENT,
  `POLE_ADDRESS` varchar(255) DEFAULT NULL,
  `POLE_CITY` varchar(100) DEFAULT NULL,
  `POLE_CP` varchar(5) DEFAULT NULL,
  `POLE_NAME` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`POLE_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. RIDE
CREATE TABLE IF NOT EXISTS `RIDE` (
  `RIDE_ID` int(11) NOT NULL AUTO_INCREMENT,
  `RIDE_HOURSTART` datetime DEFAULT NULL,
  `RIDE_LOC_ID` int(11) DEFAULT NULL,
  `RIDE_POLE_IDEND` int(11) DEFAULT NULL,
  `RIDE_POLE_IDSTART` int(11) DEFAULT NULL,
  PRIMARY KEY (`RIDE_ID`),
  KEY `FK_RIDE_POLE` (`RIDE_POLE_IDEND`),
  KEY `FK_RIDE_POLE_2` (`RIDE_POLE_IDSTART`),
  KEY `FK_RIDE_LOCATION` (`RIDE_LOC_ID`),
  CONSTRAINT `FK_RIDE_LOCATION` FOREIGN KEY (`RIDE_LOC_ID`) REFERENCES `LOCATION` (`LOC_ID`),
  CONSTRAINT `FK_RIDE_POLE` FOREIGN KEY (`RIDE_POLE_IDEND`) REFERENCES `POLE` (`POLE_ID`),
  CONSTRAINT `FK_RIDE_POLE_2` FOREIGN KEY (`RIDE_POLE_IDSTART`) REFERENCES `POLE` (`POLE_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. RIDE_USER
CREATE TABLE IF NOT EXISTS `RIDE_USER` (
  `RIDE_ID` int(11) NOT NULL,
  `USER_ID` int(11) NOT NULL,
  PRIMARY KEY (`RIDE_ID`,`USER_ID`),
  KEY `FK_RIDE_USER_USER` (`USER_ID`),
  CONSTRAINT `FK_RIDE_USER_RIDE` FOREIGN KEY (`RIDE_ID`) REFERENCES `RIDE` (`RIDE_ID`),
  CONSTRAINT `FK_RIDE_USER_USER` FOREIGN KEY (`USER_ID`) REFERENCES `USER` (`USER_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. RIGHT
CREATE TABLE IF NOT EXISTS `RIGHT` (
  `RIGHT_ID` int(11) NOT NULL AUTO_INCREMENT,
  `RIGHT_LABEL` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`RIGHT_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. USER
CREATE TABLE IF NOT EXISTS `USER` (
  `USER_ID` int(11) NOT NULL AUTO_INCREMENT,
  `USER_EMAIL` varchar(100) NOT NULL,
  `USER_FIRSTNAME` varchar(25) DEFAULT NULL,
  `USER_NAME` varchar(25) DEFAULT NULL,
  `USER_NUMPERMIS` varchar(25) DEFAULT NULL,
  `USER_PASSWORD` varchar(255) NOT NULL,
  `USER_PHONE` varchar(10) DEFAULT NULL,
  `USER_POLE_ID` int(11) DEFAULT NULL,
  `USER_RIGHT_ID` int(11) DEFAULT NULL,
  `USER_STATE` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`USER_ID`),
  UNIQUE KEY `USER_EMAIL` (`USER_EMAIL`),
  UNIQUE KEY `USER_PHONE` (`USER_PHONE`),
  KEY `FK_USER_RIGHT` (`USER_RIGHT_ID`),
  KEY `FK_USER_POLE` (`USER_POLE_ID`),
  CONSTRAINT `FK_USER_POLE` FOREIGN KEY (`USER_POLE_ID`) REFERENCES `POLE` (`POLE_ID`),
  CONSTRAINT `FK_USER_RIGHT` FOREIGN KEY (`USER_RIGHT_ID`) REFERENCES `RIGHT` (`RIGHT_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=66 DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
-- Listage de la structure de la table BookYourCar. VEHICLE
CREATE TABLE IF NOT EXISTS `VEHICLE` (
  `VEH_ID` int(11) NOT NULL AUTO_INCREMENT,
  `VEH_BRAND` varchar(100) DEFAULT NULL,
  `VEH_COLOR` varchar(100) DEFAULT NULL,
  `VEH_DATEMEC` datetime NOT NULL,
  `VEH_ISACTIVE` bit(1) NOT NULL DEFAULT b'0',
  `VEH_KM` int(11) NOT NULL DEFAULT '0',
  `VEH_MODEL` varchar(100) DEFAULT NULL,
  `VEH_NUMBERPLACE` int(11) NOT NULL DEFAULT '5',
  `VEH_POLE_ID` int(11) DEFAULT NULL,
  `VEH_REGISTRATION` varchar(20) DEFAULT NULL,
  `VEH_STATE` tinyint(4) NOT NULL DEFAULT '0',
  `VEH_TYPE_ESSENCE` varchar(100) NOT NULL,
  PRIMARY KEY (`VEH_ID`),
  KEY `FK_VEHICLE_POLE` (`VEH_POLE_ID`),
  CONSTRAINT `FK_VEHICLE_POLE` FOREIGN KEY (`VEH_POLE_ID`) REFERENCES `POLE` (`POLE_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=utf8mb4;

-- Les données exportées n'étaient pas sélectionnées.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;


-- Insert admin user with password 
INSERT INTO `BookYourCar`.`USER` (`USER_EMAIL`, `USER_FIRSTNAME`, `USER_NAME`, `USER_NUMPERMIS`, `USER_PASSWORD`, `USER_PHONE`, `USER_RIGHT_ID`, `USER_STATE)
 VALUES ('admin@admin.com', 'admin', 'admin', '00000000000','AQAAAAEAACcQAAAAEEMoOsrjn+ftA4oZ8xHkeNkscoAnZ1YPKO9UvNeI3QN8MMb5DXIjNJunTdkLrm60eg==','0000000001', '2', '1');

