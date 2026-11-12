USE RaceDayActivitiesDb;


IF COL_LENGTH('dbo.Events', 'ImageBlobName') IS NULL
BEGIN
    ALTER TABLE dbo.Events
    ADD ImageBlobName VARCHAR(255) NULL;
END;
