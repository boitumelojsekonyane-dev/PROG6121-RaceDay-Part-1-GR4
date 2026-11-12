USE RaceDayActivitiesDB;

 SELECT 'Admins' AS TableName, COUNT(*) AS RecordCount FROM Admins
UNION ALL
SELECT 'Participants', COUNT(*) FROM Participants
UNION ALL
SELECT 'EventTypes', COUNT(*) FROM EventTypes
UNION ALL
SELECT 'Events', COUNT(*) FROM Events
UNION ALL
SELECT 'EventCategories', COUNT(*) FROM EventCategories
UNION ALL
SELECT 'Enrolments', COUNT(*) FROM Enrolments
UNION ALL
SELECT 'Routes', COUNT(*) FROM Routes
UNION ALL
SELECT 'Weather', COUNT(*) FROM Weather
UNION ALL
SELECT 'Results', COUNT(*) FROM Results;

SELECT
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS ChildTable,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ChildColumn,
    OBJECT_NAME(fk.referenced_object_id) AS ParentTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ParentColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc
    ON fk.object_id = fkc.constraint_object_id
ORDER BY ChildTable, ForeignKeyName;