CREATE TABLE [dbo].[Flights]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NULL, 
    [TotalSeatCount] INT NULL, 
    [BookingStartDate] DATETIMEOFFSET NULL, 
    [BookingEndDate] DATETIMEOFFSET NULL, 
    [AvailableSeatCount] INT NULL
)
