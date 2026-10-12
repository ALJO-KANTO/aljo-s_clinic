Clinic registration changes (2026-10-10)

- Existing Login and Register .Designer.cs and .resx are UNCHANGED.
- Register now creates an active SQL Server ClinicDB Persons + Users pair in one SQL transaction.
- No admin approval step for sign-in. Existing login uses clsUser normally.
- Data access centralized in Clinic_DataAccess/clsRegistrationData.cs.
- Selected MySQL country is mapped by CountryName to SQL Server Countries.
- The existing SQL Server Roles table must contain Receptionist; all new self-registered accounts get that low-privilege role. A trusted admin must separately grant doctor/nurse permissions; do not grant them via a public registration form.
- Email address required. Registration sends username + confirmation, NOT the plaintext password (emailing passwords is insecure).
- Configure RegistrationSmtpHost/Port/From/User/Password in Clinic/App.config to enable email. Account creation succeeds even if email cannot be sent; an informative warning is shown.
- IMPORTANT: This project still depends on BOTH MySQL clinic_management (country list) and SQL Server ClinicDB (Users, Persons, Roles, Countries). SQL Server must be configured using MyConn in Clinic/App.config.
- Previously approved MySQL-only registrations are not automatically migrated; those users need an administrator-assisted migration or a new registration with the same username in SQL Server.
- No Windows/.NET build performed in this Linux environment. Open Clinic.slnx in Visual Studio, restore packages, build, and test against your databases.
- Email configuration secrets should be stored securely outside source control in production.
