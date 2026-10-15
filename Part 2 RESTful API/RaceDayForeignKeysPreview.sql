BEGIN TRANSACTION;
ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Admins_AdminID] FOREIGN KEY ([AdminID]) REFERENCES [Admins] ([AdminID]);

ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Participants_ParticipantID] FOREIGN KEY ([ParticipantID]) REFERENCES [Participants] ([ParticipantID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261015155224_AddIdentityProfileForeignKeys', N'10.0.0');

COMMIT;
GO

