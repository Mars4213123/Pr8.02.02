CREATE DATABASE  IF NOT EXISTS `weather_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `weather_db`;
-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: MySQL-8.2    Database: weather_db
-- ------------------------------------------------------
-- Server version	8.2.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `DailyRequests`
--

DROP TABLE IF EXISTS `DailyRequests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DailyRequests` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RequestDate` date NOT NULL,
  `RequestCount` int DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_date` (`RequestDate`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DailyRequests`
--

LOCK TABLES `DailyRequests` WRITE;
/*!40000 ALTER TABLE `DailyRequests` DISABLE KEYS */;
INSERT INTO `DailyRequests` VALUES (1,'2025-12-09',9);
/*!40000 ALTER TABLE `DailyRequests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WeatherCache`
--

DROP TABLE IF EXISTS `WeatherCache`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WeatherCache` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `City` varchar(100) NOT NULL,
  `Latitude` decimal(10,6) NOT NULL DEFAULT '0.000000',
  `Longitude` decimal(10,6) NOT NULL DEFAULT '0.000000',
  `WeatherData` text NOT NULL,
  `Timestamp` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_city` (`City`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WeatherCache`
--

LOCK TABLES `WeatherCache` WRITE;
/*!40000 ALTER TABLE `WeatherCache` DISABLE KEYS */;
INSERT INTO `WeatherCache` VALUES (1,'Пермь',58.010456,56.229443,'{\"forecasts\":[{\"date\":\"2025-12-09\",\"hours\":[{\"hour\":\"0\",\"condition\":\"clear\",\"humidity\":75,\"prec_type\":0,\"temp\":-18},{\"hour\":\"1\",\"condition\":\"clear\",\"humidity\":76,\"prec_type\":0,\"temp\":-19},{\"hour\":\"2\",\"condition\":\"partly-cloudy\",\"humidity\":76,\"prec_type\":0,\"temp\":-19},{\"hour\":\"3\",\"condition\":\"partly-cloudy\",\"humidity\":78,\"prec_type\":0,\"temp\":-20},{\"hour\":\"4\",\"condition\":\"partly-cloudy\",\"humidity\":78,\"prec_type\":0,\"temp\":-20},{\"hour\":\"5\",\"condition\":\"partly-cloudy\",\"humidity\":79,\"prec_type\":0,\"temp\":-20},{\"hour\":\"6\",\"condition\":\"clear\",\"humidity\":78,\"prec_type\":0,\"temp\":-20},{\"hour\":\"7\",\"condition\":\"clear\",\"humidity\":79,\"prec_type\":0,\"temp\":-20},{\"hour\":\"8\",\"condition\":\"cloudy\",\"humidity\":79,\"prec_type\":0,\"temp\":-20},{\"hour\":\"9\",\"condition\":\"overcast\",\"humidity\":79,\"prec_type\":0,\"temp\":-21},{\"hour\":\"10\",\"condition\":\"overcast\",\"humidity\":78,\"prec_type\":0,\"temp\":-20},{\"hour\":\"11\",\"condition\":\"overcast\",\"humidity\":72,\"prec_type\":0,\"temp\":-19},{\"hour\":\"12\",\"condition\":\"overcast\",\"humidity\":73,\"prec_type\":0,\"temp\":-18},{\"hour\":\"13\",\"condition\":\"overcast\",\"humidity\":71,\"prec_type\":0,\"temp\":-17},{\"hour\":\"14\",\"condition\":\"overcast\",\"humidity\":71,\"prec_type\":0,\"temp\":-17},{\"hour\":\"15\",\"condition\":\"overcast\",\"humidity\":72,\"prec_type\":0,\"temp\":-17},{\"hour\":\"16\",\"condition\":\"overcast\",\"humidity\":73,\"prec_type\":0,\"temp\":-16},{\"hour\":\"17\",\"condition\":\"overcast\",\"humidity\":73,\"prec_type\":0,\"temp\":-16},{\"hour\":\"18\",\"condition\":\"overcast\",\"humidity\":76,\"prec_type\":0,\"temp\":-16},{\"hour\":\"19\",\"condition\":\"overcast\",\"humidity\":77,\"prec_type\":0,\"temp\":-16},{\"hour\":\"20\",\"condition\":\"cloudy\",\"humidity\":77,\"prec_type\":0,\"temp\":-15},{\"hour\":\"21\",\"condition\":\"overcast\",\"humidity\":77,\"prec_type\":0,\"temp\":-15},{\"hour\":\"22\",\"condition\":\"overcast\",\"humidity\":78,\"prec_type\":0,\"temp\":-16},{\"hour\":\"23\",\"condition\":\"overcast\",\"humidity\":75,\"prec_type\":0,\"temp\":-14}]},{\"date\":\"2025-12-10\",\"hours\":[{\"hour\":\"0\",\"condition\":\"overcast\",\"humidity\":75,\"prec_type\":0,\"temp\":-12},{\"hour\":\"1\",\"condition\":\"overcast\",\"humidity\":74,\"prec_type\":0,\"temp\":-10},{\"hour\":\"2\",\"condition\":\"overcast\",\"humidity\":73,\"prec_type\":0,\"temp\":-9},{\"hour\":\"3\",\"condition\":\"overcast\",\"humidity\":71,\"prec_type\":0,\"temp\":-8},{\"hour\":\"4\",\"condition\":\"overcast\",\"humidity\":71,\"prec_type\":0,\"temp\":-8},{\"hour\":\"5\",\"condition\":\"overcast\",\"humidity\":68,\"prec_type\":0,\"temp\":-8},{\"hour\":\"6\",\"condition\":\"overcast\",\"humidity\":69,\"prec_type\":0,\"temp\":-8},{\"hour\":\"7\",\"condition\":\"overcast\",\"humidity\":69,\"prec_type\":0,\"temp\":-8},{\"hour\":\"8\",\"condition\":\"overcast\",\"humidity\":68,\"prec_type\":0,\"temp\":-7},{\"hour\":\"9\",\"condition\":\"overcast\",\"humidity\":68,\"prec_type\":0,\"temp\":-7},{\"hour\":\"10\",\"condition\":\"overcast\",\"humidity\":68,\"prec_type\":0,\"temp\":-7},{\"hour\":\"11\",\"condition\":\"overcast\",\"humidity\":66,\"prec_type\":0,\"temp\":-6},{\"hour\":\"12\",\"condition\":\"overcast\",\"humidity\":67,\"prec_type\":0,\"temp\":-6},{\"hour\":\"13\",\"condition\":\"overcast\",\"humidity\":65,\"prec_type\":0,\"temp\":-6},{\"hour\":\"14\",\"condition\":\"overcast\",\"humidity\":64,\"prec_type\":0,\"temp\":-6},{\"hour\":\"15\",\"condition\":\"overcast\",\"humidity\":64,\"prec_type\":0,\"temp\":-6},{\"hour\":\"16\",\"condition\":\"overcast\",\"humidity\":66,\"prec_type\":0,\"temp\":-6},{\"hour\":\"17\",\"condition\":\"overcast\",\"humidity\":65,\"prec_type\":0,\"temp\":-6},{\"hour\":\"18\",\"condition\":\"overcast\",\"humidity\":67,\"prec_type\":0,\"temp\":-6},{\"hour\":\"19\",\"condition\":\"overcast\",\"humidity\":67,\"prec_type\":0,\"temp\":-6},{\"hour\":\"20\",\"condition\":\"overcast\",\"humidity\":68,\"prec_type\":0,\"temp\":-6},{\"hour\":\"21\",\"condition\":\"overcast\",\"humidity\":69,\"prec_type\":0,\"temp\":-6},{\"hour\":\"22\",\"condition\":\"overcast\",\"humidity\":70,\"prec_type\":0,\"temp\":-6},{\"hour\":\"23\",\"condition\":\"light-snow\",\"humidity\":74,\"prec_type\":3,\"temp\":-6}]},{\"date\":\"2025-12-11\",\"hours\":[{\"hour\":\"0\",\"condition\":\"light-snow\",\"humidity\":75,\"prec_type\":3,\"temp\":-6},{\"hour\":\"1\",\"condition\":\"snow\",\"humidity\":72,\"prec_type\":3,\"temp\":-6},{\"hour\":\"2\",\"condition\":\"snow\",\"humidity\":71,\"prec_type\":3,\"temp\":-6},{\"hour\":\"3\",\"condition\":\"snow\",\"humidity\":77,\"prec_type\":3,\"temp\":-6},{\"hour\":\"4\",\"condition\":\"snow\",\"humidity\":78,\"prec_type\":3,\"temp\":-6},{\"hour\":\"5\",\"condition\":\"snow\",\"humidity\":80,\"prec_type\":3,\"temp\":-6},{\"hour\":\"6\",\"condition\":\"snow\",\"humidity\":81,\"prec_type\":3,\"temp\":-7},{\"hour\":\"7\",\"condition\":\"snow\",\"humidity\":81,\"prec_type\":3,\"temp\":-7},{\"hour\":\"8\",\"condition\":\"light-snow\",\"humidity\":80,\"prec_type\":3,\"temp\":-7},{\"hour\":\"9\",\"condition\":\"light-snow\",\"humidity\":81,\"prec_type\":3,\"temp\":-7},{\"hour\":\"10\",\"condition\":\"light-snow\",\"humidity\":80,\"prec_type\":3,\"temp\":-6},{\"hour\":\"11\",\"condition\":\"overcast\",\"humidity\":78,\"prec_type\":0,\"temp\":-6},{\"hour\":\"12\",\"condition\":\"light-snow\",\"humidity\":79,\"prec_type\":3,\"temp\":-6},{\"hour\":\"13\",\"condition\":\"overcast\",\"humidity\":78,\"prec_type\":0,\"temp\":-6},{\"hour\":\"14\",\"condition\":\"light-snow\",\"humidity\":77,\"prec_type\":3,\"temp\":-6},{\"hour\":\"15\",\"condition\":\"overcast\",\"humidity\":80,\"prec_type\":0,\"temp\":-6},{\"hour\":\"16\",\"condition\":\"light-snow\",\"humidity\":79,\"prec_type\":3,\"temp\":-6},{\"hour\":\"17\",\"condition\":\"light-snow\",\"humidity\":81,\"prec_type\":3,\"temp\":-6},{\"hour\":\"18\",\"condition\":\"light-snow\",\"humidity\":81,\"prec_type\":3,\"temp\":-5},{\"hour\":\"19\",\"condition\":\"snow\",\"humidity\":82,\"prec_type\":3,\"temp\":-5},{\"hour\":\"20\",\"condition\":\"snow\",\"humidity\":82,\"prec_type\":3,\"temp\":-5},{\"hour\":\"21\",\"condition\":\"snow\",\"humidity\":82,\"prec_type\":3,\"temp\":-5},{\"hour\":\"22\",\"condition\":\"light-snow\",\"humidity\":82,\"prec_type\":3,\"temp\":-5},{\"hour\":\"23\",\"condition\":\"light-snow\",\"humidity\":82,\"prec_type\":3,\"temp\":-5}]},{\"date\":\"2025-12-12\",\"hours\":[{\"hour\":\"0\",\"condition\":\"light-snow\",\"humidity\":83,\"prec_type\":3,\"temp\":-5},{\"hour\":\"1\",\"condition\":\"light-snow\",\"humidity\":83,\"prec_type\":3,\"temp\":-4},{\"hour\":\"2\",\"condition\":\"light-snow\",\"humidity\":82,\"prec_type\":3,\"temp\":-4},{\"hour\":\"3\",\"condition\":\"overcast\",\"humidity\":83,\"prec_type\":0,\"temp\":-4},{\"hour\":\"4\",\"condition\":\"light-snow\",\"humidity\":83,\"prec_type\":3,\"temp\":-4},{\"hour\":\"5\",\"condition\":\"light-snow\",\"humidity\":83,\"prec_type\":3,\"temp\":-4},{\"hour\":\"6\",\"condition\":\"snow\",\"humidity\":85,\"prec_type\":3,\"temp\":-4},{\"hour\":\"7\",\"condition\":\"snow\",\"humidity\":86,\"prec_type\":3,\"temp\":-4},{\"hour\":\"8\",\"condition\":\"snow\",\"humidity\":87,\"prec_type\":3,\"temp\":-4},{\"hour\":\"9\",\"condition\":\"snow\",\"humidity\":88,\"prec_type\":3,\"temp\":-4},{\"hour\":\"10\",\"condition\":\"snow\",\"humidity\":88,\"prec_type\":3,\"temp\":-4},{\"hour\":\"11\",\"condition\":\"snow\",\"humidity\":88,\"prec_type\":3,\"temp\":-3},{\"hour\":\"12\",\"condition\":\"snow\",\"humidity\":89,\"prec_type\":3,\"temp\":-3},{\"hour\":\"13\",\"condition\":\"light-snow\",\"humidity\":88,\"prec_type\":3,\"temp\":-3},{\"hour\":\"14\",\"condition\":\"light-snow\",\"humidity\":88,\"prec_type\":3,\"temp\":-2},{\"hour\":\"15\",\"condition\":\"light-snow\",\"humidity\":89,\"prec_type\":3,\"temp\":-2},{\"hour\":\"16\",\"condition\":\"overcast\",\"humidity\":89,\"prec_type\":0,\"temp\":-2},{\"hour\":\"17\",\"condition\":\"light-snow\",\"humidity\":89,\"prec_type\":3,\"temp\":-2},{\"hour\":\"18\",\"condition\":\"light-snow\",\"humidity\":90,\"prec_type\":3,\"temp\":-2},{\"hour\":\"19\",\"condition\":\"wet-snow\",\"humidity\":90,\"prec_type\":2,\"temp\":-2},{\"hour\":\"20\",\"condition\":\"light-snow\",\"humidity\":90,\"prec_type\":3,\"temp\":-2},{\"hour\":\"21\",\"condition\":\"wet-snow\",\"humidity\":89,\"prec_type\":2,\"temp\":-2}]},{\"date\":\"2025-12-13\",\"hours\":[]},{\"date\":\"2025-12-14\",\"hours\":[]},{\"date\":\"2025-12-15\",\"hours\":[]},{\"date\":\"2025-12-16\",\"hours\":[]},{\"date\":\"2025-12-17\",\"hours\":[]},{\"date\":\"2025-12-18\",\"hours\":[]},{\"date\":\"2025-12-19\",\"hours\":[]}]}','2025-12-09 16:53:36');
/*!40000 ALTER TABLE `WeatherCache` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-09 22:03:08
