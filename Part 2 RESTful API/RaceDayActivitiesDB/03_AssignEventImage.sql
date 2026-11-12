USE RaceDayActivitiesDb;
GO

UPDATE dbo.Events
SET ImageBlobName = '42abdd3bff3d44b49088d946eaf2580e.jpg'
WHERE EventID = 4;

SELECT
    EventID,
    Title,
    ImageBlobName
FROM dbo.Events
WHERE EventID = 4;