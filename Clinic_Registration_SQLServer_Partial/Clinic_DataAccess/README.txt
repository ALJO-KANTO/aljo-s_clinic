Clinic registration form replacement

Files:
frmRegister.cs - code-behind (business logic)
frmRegister.Designer.cs - visual WinForms design
ClinicDatabase.Registration.cs - MySQL database operations; merge methods into existing ClinicDatabase class (or use partial class as explained)
registration.sql - SQL schema

The new form uses MySQL through ClinicDatabase, not clsRegistrationData SQL Server.
Existing clsGlobal.ComputeHash() is used for password hashing.

Setup:
1. Run registration.sql against clinic_management in MySQL Workbench.
2. In your existing ClinicDatabase.cs, change:
   public static class ClinicDatabase
   to:
   public static partial class ClinicDatabase
3. Add ClinicDatabase.Registration.cs to the Clinic_DataAccess project.
4. Add frmRegister.cs and frmRegister.Designer.cs to the Clinic project.
5. Keep your existing frmLogin registration-button click that opens frmRegister.
6. Ensure the Clinic project references Clinic_DataAccess and Clinic_Business as before.
7. MySql.Data must be installed in Clinic_DataAccess.

IMPORTANT: The provided login uses clsUser.FindByUsernameAndPassword, which may
read different legacy user/person tables or SQL Server. Registration here writes
only tbl_registration_account in MySQL. Approval/login integration with your
existing authentication schema is NOT implemented. Do not expect newly approved
registrations to log in until that mapping is added.

The brand logo is a typographic placeholder to avoid inventing a resource name.
You can replace it with your existing Clinic logo PictureBox resource.
