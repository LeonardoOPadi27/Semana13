/* =========================================================
   Laboratorio Semana 13 - API en capas con DTOs y AutoMapper
   Tema: Gimnasio (Membresías y Socios)
   Relación: una Membresía tiene muchos Socios (1:N)
   ========================================================= */

USE master;
GO

IF DB_ID('GimnasioDB') IS NOT NULL
BEGIN
    ALTER DATABASE GimnasioDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GimnasioDB;
END
GO

CREATE DATABASE GimnasioDB;
GO

USE GimnasioDB;
GO

/* ---------- Tabla Membresia (lado 1) ---------- */
CREATE TABLE Membresia (
    IdMembresia     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Membresia PRIMARY KEY,
    Nombre          NVARCHAR(50)      NOT NULL,
    Descripcion     NVARCHAR(200)     NULL,
    PrecioMensual   DECIMAL(8,2)      NOT NULL,
    DuracionMeses   INT               NOT NULL,
    Activo          BIT               NOT NULL CONSTRAINT DF_Membresia_Activo DEFAULT (1),
    CONSTRAINT UQ_Membresia_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Membresia_Precio CHECK (PrecioMensual > 0),
    CONSTRAINT CK_Membresia_Duracion CHECK (DuracionMeses BETWEEN 1 AND 24)
);
GO

/* ---------- Tabla Socio (lado N) ---------- */
CREATE TABLE Socio (
    IdSocio          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Socio PRIMARY KEY,
    Dni              CHAR(8)           NOT NULL,
    Nombres          NVARCHAR(60)      NOT NULL,
    Apellidos        NVARCHAR(60)      NOT NULL,
    Email            NVARCHAR(100)     NOT NULL,
    Telefono         NVARCHAR(15)      NULL,
    FechaNacimiento  DATE              NOT NULL,
    FechaInscripcion DATE              NOT NULL CONSTRAINT DF_Socio_FechaInscripcion DEFAULT (CAST(GETDATE() AS DATE)),
    Activo           BIT               NOT NULL CONSTRAINT DF_Socio_Activo DEFAULT (1),
    IdMembresia      INT               NOT NULL,
    CONSTRAINT UQ_Socio_Dni UNIQUE (Dni),
    CONSTRAINT FK_Socio_Membresia FOREIGN KEY (IdMembresia) REFERENCES Membresia (IdMembresia)
);
GO

/* ---------- Datos de ejemplo ---------- */
INSERT INTO Membresia (Nombre, Descripcion, PrecioMensual, DuracionMeses, Activo) VALUES
 (N'Básico',   N'Acceso a sala de máquinas en horario regular',            79.90,  1, 1),
 (N'Estándar', N'Sala de máquinas + clases grupales (spinning, zumba)',   119.90,  3, 1),
 (N'Premium',  N'Acceso total, clases grupales y 2 sesiones con trainer', 189.90,  6, 1),
 (N'Anual',    N'Membresía Premium con pago anual y descuento',          159.90, 12, 1),
 (N'Verano',   N'Promoción de temporada (ya no se vende)',                 69.90,  2, 0);
GO

INSERT INTO Socio (Dni, Nombres, Apellidos, Email, Telefono, FechaNacimiento, FechaInscripcion, Activo, IdMembresia) VALUES
 ('70123456', N'Carlos',   N'Ramírez Torres',  'carlos.ramirez@mail.com',  '987654321', '1998-04-12', '2026-01-10', 1, 1),
 ('71234567', N'Lucía',    N'Mendoza Paredes', 'lucia.mendoza@mail.com',   '986543210', '2001-09-25', '2026-02-03', 1, 2),
 ('72345678', N'Jorge',    N'Quispe Huamán',   'jorge.quispe@mail.com',    '985432109', '1995-12-01', '2026-02-15', 1, 3),
 ('73456789', N'Valeria',  N'Castro Rojas',    'valeria.castro@mail.com',  '984321098', '2003-06-18', '2026-03-01', 1, 2),
 ('74567890', N'Diego',    N'Flores Salazar',  'diego.flores@mail.com',    NULL,        '1990-02-28', '2026-03-20', 0, 1),
 ('75678901', N'Ana',      N'Vargas León',     'ana.vargas@mail.com',      '983210987', '1999-11-07', '2026-04-05', 1, 4),
 ('76789012', N'Miguel',   N'Chávez Ríos',     'miguel.chavez@mail.com',   '982109876', '2004-01-30', '2026-05-12', 1, 3),
 ('77890123', N'Sofía',    N'Huerta Campos',   'sofia.huerta@mail.com',    NULL,        '2000-08-14', '2026-06-22', 1, 5);
GO

SELECT * FROM Membresia;
SELECT * FROM Socio;
GO
