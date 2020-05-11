CREATE TABLE [dbo].[user_profile]
(
	[UserId] INT NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(50) NULL, 
    [LastName] NVARCHAR(50) NULL, 
    [Company] NVARCHAR(150) NULL, 
    [Notes] NVARCHAR(MAX) NULL
    CONSTRAINT [FK_user_profile_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [users]([UserId]), 
    [ProfileImageId] INT NULL
)
