-- Pokreni OVO samo ako baza već ima iste tabele kao što ih kreira migracija InitialCreate,
-- a EF ne zna da je migracija već "primijenjena" (npr. posle prelaska sa starog monolita).
-- Zameni MigrationId ako promeniš ime migracije u folderu Migrations.

IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324210321_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260324210321_InitialCreate', N'10.0.5');
END
