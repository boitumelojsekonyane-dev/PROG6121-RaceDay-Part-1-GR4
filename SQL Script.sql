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

    INSERT INTO Admins
    (FirstName, Surname, Email, PhoneNumber)
VALUES
    ('Thabo', 'Mokoena', 'thabo.mokoena@raceday.co.za', '0712345678'),
    ('Rayno', 'Alberts', 'rayno.alberts@raceday.co.za', '0685247889'),
    ('Trudy', 'Van Der Merwe', 'TrudyVDM@raceday.co.za', '0651239874'),
    ('Naledi', 'Molefe', 'naledi.molefe@raceday.co.za', '0723456789'),
    ('Kabelo', 'Dlamini', 'kabelo.dlamini@raceday.co.za', '0734567890'),
    ('Lerato', 'Nkosi', 'lerato.nkosi@raceday.co.za', '0745678901'),
    ('Mpho', 'Mahlangu', 'mpho.mahlangu@raceday.co.za', '0756789012'),
    ('Jacobie', 'Jansen', 'jac.jansen35@raceday.co.za', '0748523699');

    INSERT INTO Participants
    (FirstName, Surname, Email, UserName, PhoneNumber,
     EmergencyContact, DateOfBirth, IdentityNumber, PassportNumber,
     Gender, Province)
VALUES
    ('Sipho', 'Ndlovu', 'sipho.ndlovu@Yahoo.com', 'sipho.ndlovu',
     '0819874563', 'Ayanda Ndlovu - 0821453695', '1998-05-14',
     '9805145123087', NULL, 'Male', 'Gauteng'),

    ('Anele', 'Mthembu', 'anele.mthembu@gmail.com', 'anele.mthembu',
     '0835289654', 'Lindiwe Mthembu - 0832123658', '2001-08-21',
     '0108215123088', NULL, 'Female', 'KwaZulu-Natal'),

    ('Tshepo', 'Molefe', 'tshepo.molefe@myunisa.edu.za', 'tshepo.molefe',
     '0837896354', 'Neo Molefe - 0843735896', '1995-02-10',
     '9502105123089', NULL, 'Male', 'Free State'),

    ('Palesa', 'Mokoena', 'palesa.mokoena@fitworld.co.za', 'palesa.mokoena',
     '0864785112', 'Karabo Mokoena - 0854115999', '1999-11-03',
     '9911035123090', NULL, 'Female', 'Gauteng'),

    ('Bongani', 'Zulu', 'bongani.zulu@Yahoo.com', 'bongani.zulu',
     '0856942314', 'Sibusiso Zulu - 0745823366', '1992-06-17',
     '9206175123091', NULL, 'Male', 'KwaZulu-Natal'),

    ('Jenny', 'Wilders', 'jenwilders@gmail.com', 'fitjen',
     '0851236699', 'August Wilders - 0748523366', '1987-05-14',
     '8714050683086', NULL, 'Female', 'Western Cape'),

    ('Karabo', 'Mahlangu', 'karabo.mahlangu@Yahoo.com', 'karabo.mahlangu',
     '0864789663', 'Refilwe Mahlangu - 0876225912', '2003-01-25',
     '0301255123092', NULL, 'Female', 'Mpumalanga'),

    ('Lerato', 'Seboko', 'lerato.seboko@gmail.com', 'lerato.seboko',
     '0878147553', 'Mpho Seboko - 0886982134', '1997-09-12',
     '9709125123093', NULL, 'Female', 'North West'),

    ('Andile', 'Dube', 'andile.dube@example.com', 'andile.dube',
     '0826547123', 'Nokuthula Dube - 0898905663', '1990-03-29',
     '9003295123094', NULL, 'Male', 'Gauteng');

       INSERT INTO Events
    (AdminID, EventTypeID, Title, Description, EventDate,
     StartTime, Location, DistanceKm, RegistrationOpens,
     RegistrationCloses, EntryFee, StatusOfEvent)
VALUES
    (1, 1, 'Johannesburg City Run',
     'Annual road running event through Johannesburg.',
     '2026-10-18', '07:00:00', 'Johannesburg', 21.10,
     '2026-08-01', '2026-10-10', 250.00, 'Upcoming'),

    (2, 2, 'Pretoria Charity Walk',
     'Community walking event supporting local charities.',
     '2026-11-07', '08:00:00', 'Pretoria', 10.00,
     '2026-08-15', '2026-10-31', 100.00, 'Upcoming'),

    (3, 3, 'Cape Town Cycle Challenge',
     'Road cycling event for recreational and competitive cyclists.',
     '2026-11-22', '06:30:00', 'Cape Town', 42.00,
     '2026-08-20', '2026-11-15', 350.00, 'Upcoming');

      INSERT INTO EventCategories
    (EventID, CategoryName, Description, MinAge, MaxAge, DistanceKm)
VALUES
    (1, '21KM Open', 'Open 21 kilometre race', 18, 60, 21.10),
    (1, '21KM Women', 'Women''s 21 kilometre race', 18, 60, 21.10),
    (1, '21KM Men', 'Men''s 21 kilometre race', 18, 60, 21.10),

    (2, '10KM Open', 'Open 10 kilometre charity walk', 18, 70, 10.00),
    (2, '10KM Women', 'Women''s 10 kilometre charity walk', 18, 70, 10.00),
    (2, '10KM Men', 'Men''s 10 kilometre charity walk', 18, 70, 10.00),

    (3, '42KM Open', 'Open 42 kilometre cycle challenge', 18, 60, 42.00),
    (3, '42KM Women', 'Women''s 42 kilometre cycle challenge', 18, 60, 42.00),
    (3, '42KM Men', 'Men''s 42 kilometre cycle challenge', 18, 60, 42.00);

     INSERT INTO Enrolments
    (ParticipantID, EventCategoryID, EventStatus,
     PaymentStatus, AmountPaid, PaymentReference, ConfirmationCode)
VALUES
    (3, 2, 'Registered', 'Paid', 250.00, 'PAY10001', 'RD10001'),
    (4, 2, 'Registered', 'Paid', 250.00, 'PAY10002', 'RD10002'),
    (5, 4, 'Registered', 'Paid', 100.00, 'PAY10003', 'RD10003'),
    (6, 5, 'Registered', 'Pending', 0.00, NULL, 'RD10004'),
    (7, 8, 'Registered', 'Paid', 350.00, 'PAY10005', 'RD10005'),
    (8, 9, 'Registered', 'Paid', 350.00, 'PAY10006', 'RD10006'),
    (9, 1, 'Registered', 'Paid', 250.00, 'PAY10007', 'RD10007'),
    (10, 3, 'Registered', 'Paid', 250.00, 'PAY10008', 'RD10008');

       INSERT INTO Routes
    (EventID, AdminID, RouteName, Description,
     RouteStartKm, RouteFinishKm)
VALUES
    (1, 1, 'Johannesburg 21KM Route',
     'Road route for the Johannesburg City Run.',
     0.00, 21.10),

    (2, 2, 'Pretoria 10KM Route',
     'Road route for the Pretoria Charity Walk.',
     0.00, 10.00),

    (3, 3, 'Cape Town 42KM Route',
     'Road route for the Cape Town Cycle Challenge.',
     0.00, 42.00);

       INSERT INTO Weather
    (EventID, ForecastDate, ForecastTime, TemperatureC,
     RainProbability, Source, Notes)
VALUES
    (1, '2026-10-18', '06:30:00', 18.50, 20,
     'RaceDay Weather Service',
     'Cool morning with low chance of rain.'),

    (2, '2026-11-07', '07:30:00', 17.00, 15,
     'RaceDay Weather Service',
     'Clear morning expected.'),

    (3, '2026-11-22', '06:00:00', 16.50, 25,
     'RaceDay Weather Service',
     'Mild morning with a moderate coastal breeze.');