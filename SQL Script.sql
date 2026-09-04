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

CREATE TABLE EventCategories (
    EventCategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(200) NULL,
    MinAge INT NULL,
    MaxAge INT NULL,
    DistanceKm DECIMAL(6,2) NULL,

    CONSTRAINT FK_EventCategories_Events
        FOREIGN KEY (EventID)
        REFERENCES Events(EventID)
);

CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventCategoryID INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    EventStatus VARCHAR(30) NOT NULL DEFAULT 'Registered',
    PaymentStatus VARCHAR(30) NOT NULL DEFAULT 'Pending',
    AmountPaid DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    PaymentReference VARCHAR(100) NULL,
    ConfirmationCode VARCHAR(50) NOT NULL UNIQUE,

    CONSTRAINT FK_Enrolments_Participants
        FOREIGN KEY (ParticipantID)
        REFERENCES Participants(ParticipantID),

    CONSTRAINT FK_Enrolments_EventCategories
        FOREIGN KEY (EventCategoryID)
        REFERENCES EventCategories(EventCategoryID)
);

CREATE TABLE Routes (
    RouteID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    AdminID INT NOT NULL,
    RouteName VARCHAR(100) NOT NULL,
    Description VARCHAR(500) NULL,
    RouteStartKm DECIMAL(6,2) NOT NULL,
    RouteFinishKm DECIMAL(6,2) NOT NULL,

    CONSTRAINT FK_Routes_Events
        FOREIGN KEY (EventID)
        REFERENCES Events(EventID),

    CONSTRAINT FK_Routes_Admins
        FOREIGN KEY (AdminID)
        REFERENCES Admins(AdminID)
);

CREATE TABLE Weather (
    WeatherID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    ForecastDate DATE NOT NULL,
    ForecastTime TIME NOT NULL,
    TemperatureC DECIMAL(5,2) NOT NULL,
    RainProbability INT NOT NULL,
    Source VARCHAR(150) NOT NULL,
    Notes VARCHAR(500) NULL,
    CapturedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Weather_Events
        FOREIGN KEY (EventID)
        REFERENCES Events(EventID)
);

CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL,
    PositionCategory INT NULL,
    OverallPosition INT NULL,
    PaceMinKm DECIMAL(6,2) NULL,
    StartTime TIME NULL,
    FinishTime TIME NOT NULL,
    CapturedByAdminID INT NOT NULL,
    CapturedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Results_Enrolments
        FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolments(EnrolmentID),

    CONSTRAINT FK_Results_Admins
        FOREIGN KEY (CapturedByAdminID)
        REFERENCES Admins(AdminID)
);

SELECT * FROM Participants;

SELECT * FROM Events;

SELECT * FROM Enrolments;

SELECT * FROM Results;

INSERT INTO EventTypes
    (TypeName, Description)
VALUES
    ('Run', 'Road running event'),
    ('Walk', 'Road walking event'),
    ('Cycle', 'Road cycling event');