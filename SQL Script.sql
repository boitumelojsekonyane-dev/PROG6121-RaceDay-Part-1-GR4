CREATE DATABASE RaceDayActivitiesDB;

CREATE TABLE Admins (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    Surname VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(20) NOT NULL
);

CREATE TABLE Participants (
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    Surname VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    UserName VARCHAR(50) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(20) NOT NULL,
    EmergencyContact VARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    IdentityNumber VARCHAR(20) NULL UNIQUE,
    PassportNumber VARCHAR(20) NULL UNIQUE,
    Gender VARCHAR(20) NOT NULL,
    Province VARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT CK_Participants_Identification
    CHECK (IdentityNumber IS NOT NULL OR PassportNumber IS NOT NULL)
);