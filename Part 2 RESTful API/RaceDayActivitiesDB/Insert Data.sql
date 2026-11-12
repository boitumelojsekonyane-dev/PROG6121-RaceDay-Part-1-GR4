USE RaceDayActivitiesDB;

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

     INSERT INTO Results
    (EnrolmentID, PositionCategory, OverallPosition,
     PaceMinKm, StartTime, FinishTime, CapturedByAdminID)
VALUES
    (2, 2, 2, 5.10, '07:00:00', '08:47:00', 1),
    (3, 1, 1, 4.95, '07:00:00', '08:44:00', 1),
    (4, 1, 1, 7.20, '08:00:00', '09:12:00', 2),
    (5, 2, 2, 7.50, '08:00:00', '09:15:00', 2),
    (6, 2, 2, 4.85, '06:30:00', '09:53:00', 3),
    (7, 1, 1, 4.60, '06:30:00', '09:44:00', 3),
    (8, 3, 3, 5.35, '07:00:00', '08:52:00', 1),
    (9, 3, 4, 5.55, '07:00:00', '08:55:00', 1);

