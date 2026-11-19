DECLARE @Correo NVARCHAR(100) = N'recepcionista.s14@gimnasio.test';

UPDATE dbo.Usuario
SET Rol = N'Recepcionista'
WHERE Correo = @Correo AND Rol = N'Socio';

IF @@ROWCOUNT <> 1
    THROW 50001, 'Registre primero el correo por /api/auth/registro y confirme que aun tiene el rol Socio.', 1;