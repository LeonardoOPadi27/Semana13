IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario (
        IdUsuario INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
        Correo NVARCHAR(100) NOT NULL,
        ClaveHash NVARCHAR(100) NOT NULL,
        Rol NVARCHAR(20) NOT NULL,
        CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo),
        CONSTRAINT CK_Usuario_Rol CHECK (Rol IN (N'Socio', N'Recepcionista'))
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'socio.demo@gimnasio.test')
    INSERT INTO dbo.Usuario (Correo, ClaveHash, Rol)
    VALUES (N'socio.demo@gimnasio.test', N'$2a$12$L1J2IVWLLpQcAUOYp7EqJOxz/E7Ou2mHxmisK58eQktZHZeoOW8qy', N'Socio');

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'recepcionista.demo@gimnasio.test')
    INSERT INTO dbo.Usuario (Correo, ClaveHash, Rol)
    VALUES (N'recepcionista.demo@gimnasio.test', N'$2a$12$uOORf5QA5lNccCCgnbNCCeoKSDOaSXy1undeSLqqxKFOxWDkPAeWm', N'Recepcionista');