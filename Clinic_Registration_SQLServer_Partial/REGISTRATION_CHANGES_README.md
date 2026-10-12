# Clinic registration changes (SQL Server-compatible phase)

This is a partial update, not a completed MySQL migration.

## Implemented
- Login screen has a Register button (created in frmLogin.cs).
- New WinForms registration screen in Clinic/Login/frmRegister.cs.
- Clinic_DataAccess/clsRegistrationData.cs inserts a Person and an inactive User atomically, using the ORIGINAL ClinicDB SQL Server database and original Roles/Countries tables.
- The existing login, patient, doctor, appointment and other modules remain unchanged and continue to use SQL Server.
- Registration uses the existing clsGlobal.ComputeHash SHA-256 format so it matches the original login. This is compatibility only; production systems should upgrade to a salted password hashing scheme and avoid storing plaintext passwords in the Windows registry.

## Setup
1. Restore/import Database/ClinicDB.sql in Microsoft SQL Server, not HeidiSQL/MySQL.
2. Set the correct `MyConn` SQL Server connection string in Clinic/App.config.
3. Confirm the `Roles` table contains a row named `Receptionist` and `Countries` contains data.
4. Open Clinic/Clinic.slnx in Visual Studio 2022 and rebuild.
5. Click Register on the login screen. New accounts are inactive by default.
6. An administrator must activate an approved account: `UPDATE Users SET IsActive = 1 WHERE UserName = 'approved_username';` Run only after reviewing the request.

## NOT IMPLEMENTED
- Full MySQL schema migration and conversion of 24 SQL Server data-access modules.
- Connecting every existing page to the previously discussed MySQL ClinicDatabase class.
- Runtime build testing (Visual Studio/.NET Framework build tools unavailable here).

The existing SQL Server schema uses SQL Server-only syntax (TOP, SCOPE_IDENTITY, views, etc.). Do not point the existing application at MySQL without migrating those queries and schema first.
