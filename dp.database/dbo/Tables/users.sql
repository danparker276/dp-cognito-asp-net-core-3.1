CREATE TABLE [dbo].[users]
(
	[UserId]     INT            IDENTITY (1, 1) NOT NULL,
    [CognitoUserName] NVARCHAR(250) NOT NULL, 
	[Email]      NVARCHAR (350) NULL,
    [Created]    DATETIME       DEFAULT (GETDATE()) NOT NULL,
    [Updated]    DATETIME       NULL,
    [UserTypeId]   INT            DEFAULT ((1)) NOT NULL,
    [TeamId] INT NULL, 
    [IsActive]   BIT            DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
)
