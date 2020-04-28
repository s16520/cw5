CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN


	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name=@Studies);
	IF @IdStudies IS NULL
	BEGIN
		--RAISEERROR
		RAISERROR ('Studies NOT FOUND!',10,1); 
		RETURN;
	END

	DECLARE @IdEnrollment INT = (SELECT IdEnrollment 
									FROM Enrollment 
									INNER JOIN Studies ON (Enrollment.IdStudy=Studies.IdStudy) 
									WHERE Enrollment.Semester=(@Semester+1) AND Studies.Name=@Studies
								);

	IF @IdEnrollment IS NULL
	BEGIN
		SET @IdEnrollment = (SELECT MAX(IdEnrollment)+1 FROM Enrollment);
		INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (@IdEnrollment, @Semester+1,@IdStudies,CURRENT_TIMESTAMP)
	END

	UPDATE Student
	SET IdEnrollment = @IdEnrollment
	WHERE IdEnrollment = (SELECT IdEnrollment 
							FROM Enrollment INNER JOIN Studies ON (Enrollment.IdStudy=Studies.IdStudy) 
							WHERE Enrollment.Semester=@Semester AND Studies.Name=@Studies
							);

	COMMIT
END



select * from Enrollment inner join Studies on Enrollment.IdStudy=Studies.IdStudy


select * from Student


EXEC	[dbo].[PromoteStudents]
		@Studies = 'Informatic',
		@Semester = 1

GO

rollup