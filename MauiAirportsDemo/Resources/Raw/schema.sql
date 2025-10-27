-- Drop obsolete schema.

DROP INDEX IF EXISTS IX_Airports_Name;
DROP TABLE IF EXISTS Airports_rtree;
DROP TABLE IF EXISTS Airports;

-- Recreate the schema.

CREATE TABLE IF NOT EXISTS Airports
(
    Id INTEGER PRIMARY KEY,
    Name TEXT NOT NULL,
    Properties TEXT
);

CREATE VIRTUAL TABLE Airports_rtree USING rtree(Id, MinX, MaxX, MinY, MaxY);

-- Add indexes.
CREATE INDEX IF NOT EXISTS IX_Airports_Id ON Airports (Id);
CREATE INDEX IF NOT EXISTS IX_Airports_Name ON Airports (Name);
CREATE INDEX IF NOT EXISTS IX_Airports_IATA_Name ON Airports (
    JsonProperty(Properties, 'iata_code'),
    Name);
CREATE INDEX IF NOT EXISTS IX_Airports_Longitude ON Airports (JsonProperty(Properties, 'longitude_deg'));
CREATE INDEX IF NOT EXISTS IX_Airports_Latitude ON Airports (JsonProperty(Properties, 'latitude_deg'));

-- Add triggers to keep the RTree index in sync.
CREATE TRIGGER IF NOT EXISTS Airports_AfterInsert
AFTER INSERT ON Airports
BEGIN
    INSERT INTO Airports_rtree (Id, MinX, MaxX, MinY, MaxY)
    VALUES (
        NEW.Id,
        JsonProperty(NEW.Properties, 'longitude_deg'),
        JsonProperty(NEW.Properties, 'longitude_deg'),
        JsonProperty(NEW.Properties, 'latitude_deg'),
        JsonProperty(NEW.Properties, 'latitude_deg'));
END;

CREATE TRIGGER IF NOT EXISTS Airports_AfterUpdate
AFTER UPDATE ON Airports
BEGIN
    UPDATE Airports_rtree
    SET MinX = JsonProperty(NEW.Properties, 'longitude_deg'),
        MaxX = JsonProperty(NEW.Properties, 'longitude_deg'),
        MinY = JsonProperty(NEW.Properties, 'latitude_deg'),
        MaxY = JsonProperty(NEW.Properties, 'latitude_deg')
    WHERE Id = NEW.Id;
END;

CREATE TRIGGER IF NOT EXISTS Airports_AfterDelete
AFTER DELETE ON Airports
BEGIN
    DELETE FROM Airports_rtree WHERE Id = OLD.Id;
END;
