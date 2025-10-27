-- Drop obsolete schema.

DROP INDEX IF EXISTS IX_Airports_Name;
DROP TABLE IF EXISTS Airports;

-- Recreate the schema.

CREATE TABLE IF NOT EXISTS Airports
(
    Id INTEGER PRIMARY KEY,
    Name TEXT NOT NULL,
    Properties TEXT
);

-- Add indexes.
CREATE INDEX IF NOT EXISTS IX_Airports_Name ON Airports (Name);
