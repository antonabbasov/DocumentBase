CREATE PROCEDURE [dbo].[FileSaveProcedure]
@authorId bigint,
@changedate date,
@Name varchar(50),
@BinaryFile varchar(max)
AS
INSERT INTO Document(AuthorId, ChangeDate, Name, BinaryFile)
VALUES(@authorId, @changedate, @Name,@BinaryFile)
SELECT SCOPE_IDENTITY() GO 
