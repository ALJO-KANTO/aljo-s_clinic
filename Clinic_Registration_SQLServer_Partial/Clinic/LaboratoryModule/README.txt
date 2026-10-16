LABORATORY MODULE - WinForms .NET Framework 4.7.2

1. Add ClinicDatabase.Laboratory.cs to Clinic_DataAccess project (existing partial class).
2. Add frmLaboratory.cs, frmLaboratory.Designer.cs, frmLabDialogs.cs to Clinic project.
3. Ensure Clinic project references Clinic_DataAccess and MySql.Data is installed in DataAccess.
4. Add using Clinic.Medical_Services.Laboratory; to frmMain.cs.
5. Replace the Laboratory placeholder MessageBox with:
   OpenIfAuthorized(() => { using (var frm = new frmLaboratory()) frm.ShowDialog(this); },
       clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse);
   Add appropriate laboratory technician role later if required.
6. On first opening, EnsureLaboratoryTables creates labrequests (requires laboratorytests, visits, patients, persons tables).
7. For production, move the CREATE TABLE statement into a NEW versioned DatabaseMigration and call ApplyMigrations on startup. Do not edit already-applied migration versions.

IMPORTANT:
- This is a starter workflow: visit ID is entered manually, result is free text.
- It does not yet implement result ranges, units, approvals, PDFs, audit logs, or billing integration.
- Completed results cannot be overwritten; test deactivation retains history.
- Verify migration/schema and test against a backup before deployment.
