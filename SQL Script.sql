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

CREATE TABLE EventTypes (
    EventTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(150) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    AdminID INT NOT NULL,
    EventTypeID INT NOT NULL,
    Title VARCHAR(150) NOT NULL,
    Description VARCHAR(500) NOT NULL,
    EventDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    Location VARCHAR(150) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL,
    RegistrationOpens DATE NOT NULL,
    RegistrationCloses DATE NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    StatusOfEvent VARCHAR(30) NOT NULL DEFAULT 'UPCOMING!',

    CONSTRAINT FK_Events_Admins
        FOREIGN KEY (AdminID)
        REFERENCES Admins(AdminID),

    CONSTRAINT FK_Events_EventTypes
        FOREIGN KEY (EventTypeID)
        REFERENCES EventTypes(EventTypeID)
);