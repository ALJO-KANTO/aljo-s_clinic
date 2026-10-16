
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Clinic_DataAccess
{
    public static partial class ClinicDatabase
    {
        private class DatabaseMigration
        {
            public string Version { get; set; }
            public string Description { get; set; }
            public string Sql { get; set; }
        }

        public static void ApplyMigrations()
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                Debug.WriteLine(
                    "Migration connected to: " +
                    connection.DataSource + " / " +
                    connection.Database);

                // STEP 1: Create migration tracking table
                string createMigrationTable = @"
                    CREATE TABLE IF NOT EXISTS schema_migrations
                    (
                        version VARCHAR(100) NOT NULL PRIMARY KEY,
                        description VARCHAR(255),
                        applied_at DATETIME NOT NULL
                            DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB;";

                using (MySqlCommand command =
                    new MySqlCommand(createMigrationTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // STEP 2: Verify tracking table was created
                using (MySqlCommand command = new MySqlCommand(@"
                        SELECT COUNT(*)
                        FROM information_schema.tables
                        WHERE table_schema = DATABASE()
                          AND table_name = 'schema_migrations';",
                    connection))
                {
                    long count = Convert.ToInt64(
                        command.ExecuteScalar());

                    if (count == 0)
                    {
                        throw new Exception(
                            "schema_migrations table was not created.");
                    }

                    Debug.WriteLine(
                        "schema_migrations exists: True");
                }

                // STEP 3: Define database migrations
                // Add new migrations to this list.
                List<DatabaseMigration> migrations =
                  new List<DatabaseMigration>
                  {
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_001",
                            Description = "Initial schema: countries",
                            Sql = @"CREATE TABLE IF NOT EXISTS `countries` (
                                      `CountryID` int NOT NULL AUTO_INCREMENT,
                                      `CountryName` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                                      PRIMARY KEY (`CountryID`),
                                      UNIQUE KEY `CountryName` (`CountryName`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_002",
                            Description = "Initial schema: tbl_country",
                            Sql = @"CREATE TABLE IF NOT EXISTS `tbl_country` (
                                      `CountryID` int NOT NULL AUTO_INCREMENT,
                                      `CountryName` varchar(120) NOT NULL,
                                      PRIMARY KEY (`CountryID`),
                                      UNIQUE KEY `CountryName` (`CountryName`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_003",
                            Description = "Initial schema: bloodtypes",
                            Sql = @"CREATE TABLE IF NOT EXISTS `bloodtypes` (
                                      `BloodTypeID` int NOT NULL AUTO_INCREMENT,
                                      `BloodTypeName` varchar(20) NOT NULL,
                                      PRIMARY KEY (`BloodTypeID`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_004",
                            Description = "Initial schema: days",
                            Sql = @"CREATE TABLE IF NOT EXISTS `days` (
                                      `DayID` int NOT NULL,
                                      `DayName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                                      PRIMARY KEY (`DayID`)
                                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_005",
                            Description = "Initial schema: roles",
                            Sql = @"CREATE TABLE IF NOT EXISTS `roles` (
                                      `RoleID` int NOT NULL AUTO_INCREMENT,
                                      `RoleName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                                      PRIMARY KEY (`RoleID`),
                                      UNIQUE KEY `RoleName` (`RoleName`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_006",
                            Description = "Initial schema: specializations",
                            Sql = @"CREATE TABLE IF NOT EXISTS `specializations` (
                                      `SpecializationID` int NOT NULL AUTO_INCREMENT,
                                      `SpecializationName` varchar(150) NOT NULL,
                                      PRIMARY KEY (`SpecializationID`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_007",
                            Description = "Initial schema: appointmenttypes",
                            Sql = @"CREATE TABLE IF NOT EXISTS `appointmenttypes` (
                                      `AppointmentTypeID` int NOT NULL AUTO_INCREMENT,
                                      `AppointmentTypeName` varchar(150) NOT NULL,
                                      `Description` varchar(500) DEFAULT NULL,
                                      PRIMARY KEY (`AppointmentTypeID`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_008",
                            Description = "Initial schema: discounts",
                            Sql = @"CREATE TABLE IF NOT EXISTS `discounts` (
                                      `DiscountID` int NOT NULL AUTO_INCREMENT,
                                      `DiscountName` varchar(150) NOT NULL,
                                      `DiscountPercentage` decimal(5,2) NOT NULL DEFAULT '0.00',
                                      `IsActive` tinyint(1) NOT NULL DEFAULT '1',
                                      PRIMARY KEY (`DiscountID`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_009",
                            Description = "Initial schema: services",
                            Sql = @"CREATE TABLE IF NOT EXISTS `services` (
                                      `ServiceID` int NOT NULL AUTO_INCREMENT,
                                      `ServiceName` varchar(200) NOT NULL,
                                      `ServicePrice` decimal(12,2) NOT NULL DEFAULT '0.00',
                                      `IsActive` tinyint(1) NOT NULL DEFAULT '1',
                                      PRIMARY KEY (`ServiceID`)
                                    ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_010",
                            Description = "Initial schema: medicines",
                            Sql = @"CREATE TABLE IF NOT EXISTS `medicines` (
                                  `MedicineID` int NOT NULL AUTO_INCREMENT,
                                  `MedicineName` varchar(200) NOT NULL,
                                  `MedicinePrice` decimal(12,2) NOT NULL DEFAULT '0.00',
                                  `CurrentStock` decimal(14,3) NOT NULL DEFAULT '0.000',
                                  `ReorderLevel` decimal(14,3) NOT NULL DEFAULT '0.000',
                                  `TaxRate` decimal(6,3) NOT NULL DEFAULT '0.000',
                                  PRIMARY KEY (`MedicineID`)
                                ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_011",
                            Description = "Initial schema: persons",
                            Sql = @"CREATE TABLE IF NOT EXISTS `persons` (
                              `PersonID` int NOT NULL AUTO_INCREMENT,
                              `FullName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                              `DateOfBirth` date NOT NULL,
                              `Gender` smallint NOT NULL,
                              `PhoneNumber` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
                              `Email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
                              `Address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
                              `ImagePath` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
                              `NationalityCountryID` int DEFAULT NULL,
                              `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
                              PRIMARY KEY (`PersonID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_012",
                            Description = "Initial schema: systemlogs",
                            Sql = @"CREATE TABLE IF NOT EXISTS `systemlogs` (
                              `LogID` int NOT NULL AUTO_INCREMENT,
                              `LogLevel` varchar(30) NOT NULL,
                              `Message` text NOT NULL,
                              `StackTrace` longtext,
                              `Source` varchar(255) DEFAULT NULL,
                              `UserID` int DEFAULT NULL,
                              `MachineName` varchar(150) DEFAULT NULL,
                              `Timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              PRIMARY KEY (`LogID`),
                              KEY `ix_systemlogs_timestamp` (`Timestamp`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=132 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_013",
                            Description = "Initial schema: users",
                            Sql = @"CREATE TABLE IF NOT EXISTS `users` (
                              `UserID` int NOT NULL AUTO_INCREMENT,
                              `PersonID` int NOT NULL,
                              `UserName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                              `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                              `IsActive` tinyint(1) NOT NULL DEFAULT '1',
                              `RoleID` int NOT NULL,
                              `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
                              PRIMARY KEY (`UserID`),
                              UNIQUE KEY `UserName` (`UserName`),
                              KEY `FK_Users_Persons` (`PersonID`),
                              KEY `FK_Users_Roles` (`RoleID`),
                              CONSTRAINT `FK_Users_Persons` FOREIGN KEY (`PersonID`) REFERENCES `persons` (`PersonID`),
                              CONSTRAINT `FK_Users_Roles` FOREIGN KEY (`RoleID`) REFERENCES `roles` (`RoleID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_014",
                            Description = "Initial schema: tbl_registration_account",
                            Sql = @"CREATE TABLE IF NOT EXISTS `tbl_registration_account` (
                              `RegistrationID` int NOT NULL AUTO_INCREMENT,
                              `FullName` varchar(200) NOT NULL,
                              `DateOfBirth` date NOT NULL,
                              `Gender` tinyint unsigned NOT NULL,
                              `Phone` varchar(20) NOT NULL,
                              `Email` varchar(150) DEFAULT NULL,
                              `Address` varchar(500) NOT NULL,
                              `CountryID` int NOT NULL,
                              `Username` varchar(20) NOT NULL,
                              `PasswordHash` varchar(255) NOT NULL,
                              `IsActive` tinyint(1) NOT NULL DEFAULT '0',
                              `ApprovalStatus` varchar(20) NOT NULL DEFAULT 'Pending',
                              `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `ApprovedAt` datetime DEFAULT NULL,
                              `ApprovedBy` varchar(100) DEFAULT NULL,
                              PRIMARY KEY (`RegistrationID`),
                              UNIQUE KEY `Username` (`Username`),
                              KEY `fk_registration_country` (`CountryID`),
                              CONSTRAINT `fk_registration_country` FOREIGN KEY (`CountryID`) REFERENCES `countries` (`CountryID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_015",
                            Description = "Initial schema: patients",
                            Sql = @"CREATE TABLE IF NOT EXISTS `patients` (
                              `PatientID` int NOT NULL AUTO_INCREMENT,
                              `PersonID` int NOT NULL,
                              `EmergencyContact` varchar(100) DEFAULT NULL,
                              `BloodTypeID` int DEFAULT NULL,
                              `MedicalHistory` text,
                              `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `CreatedByUserID` int DEFAULT NULL,
                              `IsActive` tinyint(1) NOT NULL DEFAULT '1',
                              PRIMARY KEY (`PatientID`),
                              KEY `ix_patients_person` (`PersonID`),
                              KEY `fk_patients_blood` (`BloodTypeID`),
                              KEY `fk_patients_creator` (`CreatedByUserID`),
                              CONSTRAINT `fk_patients_blood` FOREIGN KEY (`BloodTypeID`) REFERENCES `bloodtypes` (`BloodTypeID`),
                              CONSTRAINT `fk_patients_creator` FOREIGN KEY (`CreatedByUserID`) REFERENCES `users` (`UserID`),
                              CONSTRAINT `fk_patients_person` FOREIGN KEY (`PersonID`) REFERENCES `persons` (`PersonID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_016",
                            Description = "Initial schema: doctors",
                            Sql = @"CREATE TABLE IF NOT EXISTS `doctors` (
                              `DoctorID` int NOT NULL AUTO_INCREMENT,
                              `PersonID` int NOT NULL,
                              `SpecializationID` int DEFAULT NULL,
                              `ConsultationFees` decimal(12,2) NOT NULL DEFAULT '0.00',
                              `LicenseNumber` varchar(100) DEFAULT NULL,
                              `CreatedByUserID` int DEFAULT NULL,
                              `IsActive` tinyint(1) NOT NULL DEFAULT '1',
                              PRIMARY KEY (`DoctorID`),
                              KEY `ix_doctors_person` (`PersonID`),
                              KEY `ix_doctors_specialization` (`SpecializationID`),
                              KEY `fk_doctors_creator` (`CreatedByUserID`),
                              CONSTRAINT `fk_doctors_creator` FOREIGN KEY (`CreatedByUserID`) REFERENCES `users` (`UserID`),
                              CONSTRAINT `fk_doctors_person` FOREIGN KEY (`PersonID`) REFERENCES `persons` (`PersonID`),
                              CONSTRAINT `fk_doctors_specialization` FOREIGN KEY (`SpecializationID`) REFERENCES `specializations` (`SpecializationID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_017",
                            Description = "Initial schema: doctorspecialties",
                            Sql = @"CREATE TABLE IF NOT EXISTS `doctorspecialties` (
                              `DoctorID` int NOT NULL,
                              `SpecializationID` int NOT NULL,
                              PRIMARY KEY (`DoctorID`,`SpecializationID`),
                              KEY `fk_ds_specialization` (`SpecializationID`),
                              CONSTRAINT `fk_ds_doctor` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`),
                              CONSTRAINT `fk_ds_specialization` FOREIGN KEY (`SpecializationID`) REFERENCES `specializations` (`SpecializationID`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_018",
                            Description = "Initial schema: doctorworkingdays",
                            Sql = @"CREATE TABLE IF NOT EXISTS `doctorworkingdays` (
                              `DoctorID` int NOT NULL,
                              `DayID` int NOT NULL,
                              `StartTime` time NOT NULL,
                              `EndTime` time NOT NULL,
                              PRIMARY KEY (`DoctorID`,`DayID`),
                              KEY `fk_dwd_day` (`DayID`),
                              CONSTRAINT `fk_dwd_day` FOREIGN KEY (`DayID`) REFERENCES `days` (`DayID`),
                              CONSTRAINT `fk_dwd_doctor` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_019",
                            Description = "Initial schema: appointments",
                            Sql = @"CREATE TABLE IF NOT EXISTS `appointments` (
                              `AppointmentID` int NOT NULL AUTO_INCREMENT,
                              `PatientID` int NOT NULL,
                              `DoctorID` int NOT NULL,
                              `CreatedByUserID` int DEFAULT NULL,
                              `AppointmentStatus` varchar(40) NOT NULL DEFAULT 'Scheduled',
                              `AppointmentTypeID` int DEFAULT NULL,
                              `AppointmentFees` decimal(12,2) NOT NULL DEFAULT '0.00',
                              `AppointmentDate` datetime NOT NULL,
                              PRIMARY KEY (`AppointmentID`),
                              KEY `ix_appointments_date` (`AppointmentDate`),
                              KEY `fk_appointments_patient` (`PatientID`),
                              KEY `fk_appointments_doctor` (`DoctorID`),
                              KEY `fk_appointments_creator` (`CreatedByUserID`),
                              KEY `fk_appointments_type` (`AppointmentTypeID`),
                              CONSTRAINT `fk_appointments_creator` FOREIGN KEY (`CreatedByUserID`) REFERENCES `users` (`UserID`),
                              CONSTRAINT `fk_appointments_doctor` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`),
                              CONSTRAINT `fk_appointments_patient` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`),
                              CONSTRAINT `fk_appointments_type` FOREIGN KEY (`AppointmentTypeID`) REFERENCES `appointmenttypes` (`AppointmentTypeID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_020",
                            Description = "Initial schema: visits",
                            Sql = @"CREATE TABLE IF NOT EXISTS `visits` (
                              `VisitID` int NOT NULL AUTO_INCREMENT,
                              `PatientID` int NOT NULL,
                              `AppointmentID` int DEFAULT NULL,
                              `DoctorID` int NOT NULL,
                              `VisitDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `VisitStatus` varchar(40) NOT NULL DEFAULT 'Open',
                              `CreatedByUserID` int DEFAULT NULL,
                              PRIMARY KEY (`VisitID`),
                              KEY `fk_visits_patient` (`PatientID`),
                              KEY `fk_visits_appointment` (`AppointmentID`),
                              KEY `fk_visits_doctor` (`DoctorID`),
                              KEY `fk_visits_creator` (`CreatedByUserID`),
                              CONSTRAINT `fk_visits_appointment` FOREIGN KEY (`AppointmentID`) REFERENCES `appointments` (`AppointmentID`),
                              CONSTRAINT `fk_visits_creator` FOREIGN KEY (`CreatedByUserID`) REFERENCES `users` (`UserID`),
                              CONSTRAINT `fk_visits_doctor` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`),
                              CONSTRAINT `fk_visits_patient` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_021",
                            Description = "Initial schema: vitals",
                            Sql = @"CREATE TABLE IF NOT EXISTS `vitals` (
                              `VitalID` int NOT NULL AUTO_INCREMENT,
                              `VisitID` int NOT NULL,
                              `AppointmentID` int DEFAULT NULL,
                              `BloodPressure` varchar(30) DEFAULT NULL,
                              `Temperature` decimal(5,2) DEFAULT NULL,
                              `Weight` decimal(7,2) DEFAULT NULL,
                              `Pulse` int DEFAULT NULL,
                              `RecordedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `CreatedByUserID` int DEFAULT NULL,
                              PRIMARY KEY (`VitalID`),
                              KEY `fk_vitals_visit` (`VisitID`),
                              KEY `fk_vitals_appointment` (`AppointmentID`),
                              KEY `fk_vitals_creator` (`CreatedByUserID`),
                              CONSTRAINT `fk_vitals_appointment` FOREIGN KEY (`AppointmentID`) REFERENCES `appointments` (`AppointmentID`),
                              CONSTRAINT `fk_vitals_creator` FOREIGN KEY (`CreatedByUserID`) REFERENCES `users` (`UserID`),
                              CONSTRAINT `fk_vitals_visit` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_022",
                            Description = "Initial schema: bills",
                            Sql = @"CREATE TABLE IF NOT EXISTS `bills` (
                              `BillID` int NOT NULL AUTO_INCREMENT,
                              `VisitID` int NOT NULL,
                              `PaymentStatus` varchar(40) NOT NULL DEFAULT 'Unpaid',
                              `CreatedByUserID` int DEFAULT NULL,
                              `BillDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `TotalAmount` decimal(12,2) NOT NULL DEFAULT '0.00',
                              PRIMARY KEY (`BillID`),
                              KEY `fk_bills_visit` (`VisitID`),
                              KEY `fk_bills_creator` (`CreatedByUserID`),
                              CONSTRAINT `fk_bills_creator` FOREIGN KEY (`CreatedByUserID`) REFERENCES `users` (`UserID`),
                              CONSTRAINT `fk_bills_visit` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_023",
                            Description = "Initial schema: billservices",
                            Sql = @"CREATE TABLE IF NOT EXISTS `billservices` (
                              `BillServiceID` int NOT NULL AUTO_INCREMENT,
                              `BillID` int NOT NULL,
                              `ServiceID` int NOT NULL,
                              `Quantity` decimal(12,3) NOT NULL DEFAULT '1.000',
                              `UnitPrice` decimal(12,2) NOT NULL DEFAULT '0.00',
                              `TotalAmount` decimal(12,2) NOT NULL DEFAULT '0.00',
                              PRIMARY KEY (`BillServiceID`),
                              KEY `fk_billservices_bill` (`BillID`),
                              KEY `fk_billservices_service` (`ServiceID`),
                              CONSTRAINT `fk_billservices_bill` FOREIGN KEY (`BillID`) REFERENCES `bills` (`BillID`),
                              CONSTRAINT `fk_billservices_service` FOREIGN KEY (`ServiceID`) REFERENCES `services` (`ServiceID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_024",
                            Description = "Initial schema: payments",
                            Sql = @"CREATE TABLE IF NOT EXISTS `payments` (
                              `PaymentID` int NOT NULL AUTO_INCREMENT,
                              `BillID` int NOT NULL,
                              `PaymentAmount` decimal(12,2) NOT NULL,
                              `PaymentDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `PaymentMethod` varchar(40) NOT NULL,
                              `CreatedByUserID` int DEFAULT NULL,
                              PRIMARY KEY (`PaymentID`),
                              KEY `fk_payments_bill` (`BillID`),
                              KEY `fk_payments_creator` (`CreatedByUserID`),
                              CONSTRAINT `fk_payments_bill` FOREIGN KEY (`BillID`) REFERENCES `bills` (`BillID`),
                              CONSTRAINT `fk_payments_creator` FOREIGN KEY (`CreatedByUserID`) REFERENCES `users` (`UserID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_025",
                            Description = "Initial schema: invoices",
                            Sql = @"CREATE TABLE IF NOT EXISTS `invoices` (
                              `InvoiceID` int NOT NULL AUTO_INCREMENT,
                              `BillID` int DEFAULT NULL,
                              `InvoiceDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `TotalAmount` decimal(12,2) NOT NULL DEFAULT '0.00',
                              `InvoiceStatus` varchar(40) NOT NULL DEFAULT 'Open',
                              PRIMARY KEY (`InvoiceID`),
                              KEY `fk_invoices_bill` (`BillID`),
                              CONSTRAINT `fk_invoices_bill` FOREIGN KEY (`BillID`) REFERENCES `bills` (`BillID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_026",
                            Description = "Initial schema: salesreturns",
                            Sql = @"CREATE TABLE IF NOT EXISTS `salesreturns` (
                              `SalesReturnID` int NOT NULL AUTO_INCREMENT,
                              `BillID` int NOT NULL,
                              `ReturnDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `TotalRefund` decimal(12,2) NOT NULL DEFAULT '0.00',
                              `CashierID` int DEFAULT NULL,
                              PRIMARY KEY (`SalesReturnID`),
                              KEY `fk_salesreturns_bill` (`BillID`),
                              KEY `fk_salesreturns_cashier` (`CashierID`),
                              CONSTRAINT `fk_salesreturns_bill` FOREIGN KEY (`BillID`) REFERENCES `bills` (`BillID`),
                              CONSTRAINT `fk_salesreturns_cashier` FOREIGN KEY (`CashierID`) REFERENCES `users` (`UserID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_027",
                            Description = "Initial schema: prescriptions",
                            Sql = @"CREATE TABLE IF NOT EXISTS `prescriptions` (
                              `PrescriptionID` int NOT NULL AUTO_INCREMENT,
                              `VisitID` int NOT NULL,
                              `PrescriptionDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `PrescriptionNotes` text,
                              `PrescriptionStatus` varchar(40) DEFAULT NULL,
                              `Prescriptiontype` varchar(40) DEFAULT NULL,
                              PRIMARY KEY (`PrescriptionID`),
                              KEY `fk_prescriptions_visit` (`VisitID`),
                              CONSTRAINT `fk_prescriptions_visit` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_028",
                            Description = "Initial schema: prescriptiondetails",
                            Sql = @"CREATE TABLE IF NOT EXISTS `prescriptiondetails` (
                              `PrescriptionDetailID` int NOT NULL AUTO_INCREMENT,
                              `PrescriptionID` int NOT NULL,
                              `MedicineID` int DEFAULT NULL,
                              `Quantity` decimal(12,3) NOT NULL DEFAULT '0.000',
                              `Dosage` varchar(150) DEFAULT NULL,
                              `Instructions` text,
                              `SavedMedicineName` varchar(200) DEFAULT NULL,
                              `SavedMedicinePrice` decimal(12,2) DEFAULT NULL,
                              `Frequency` varchar(150) DEFAULT NULL,
                              `DiscountAmount` decimal(12,2) NOT NULL DEFAULT '0.00',
                              PRIMARY KEY (`PrescriptionDetailID`),
                              KEY `fk_prescriptiondetails_master` (`PrescriptionID`),
                              KEY `fk_prescriptiondetails_medicine` (`MedicineID`),
                              CONSTRAINT `fk_prescriptiondetails_master` FOREIGN KEY (`PrescriptionID`) REFERENCES `prescriptions` (`PrescriptionID`),
                              CONSTRAINT `fk_prescriptiondetails_medicine` FOREIGN KEY (`MedicineID`) REFERENCES `medicines` (`MedicineID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_BASE_029",
                            Description = "Initial schema: inventorytransactions",
                            Sql = @"CREATE TABLE IF NOT EXISTS `inventorytransactions` (
                              `InventoryTransactionID` int NOT NULL AUTO_INCREMENT,
                              `MedicineID` int NOT NULL,
                              `QuantityChange` decimal(14,3) NOT NULL,
                              `ReferenceID` int DEFAULT NULL,
                              `TransactionDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `UserID` int DEFAULT NULL,
                              PRIMARY KEY (`InventoryTransactionID`),
                              KEY `fk_inventory_medicine` (`MedicineID`),
                              KEY `fk_inventory_user` (`UserID`),
                              CONSTRAINT `fk_inventory_medicine` FOREIGN KEY (`MedicineID`) REFERENCES `medicines` (`MedicineID`),
                              CONSTRAINT `fk_inventory_user` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;"
                        },
                        new DatabaseMigration
{
    Version = "20261016_LAB_001",
    Description = "Create laboratory requests table",
    Sql = @"
        CREATE TABLE IF NOT EXISTS labrequests
        (
            LabRequestID INT NOT NULL AUTO_INCREMENT,
            VisitID INT NOT NULL,
            TestID INT NOT NULL,
            RequestedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            Status VARCHAR(30) NOT NULL DEFAULT 'Ordered',
            ResultValue TEXT NULL,
            ResultNotes TEXT NULL,
            ResultedAt DATETIME NULL,

            PRIMARY KEY (LabRequestID),

            INDEX IX_LabRequests_Visit (VisitID),
            INDEX IX_LabRequests_Test (TestID),
            INDEX IX_LabRequests_Status (Status),

            CONSTRAINT FK_LabRequests_Visit
                FOREIGN KEY (VisitID)
                REFERENCES visits(VisitID),

            CONSTRAINT FK_LabRequests_Test
                FOREIGN KEY (TestID)
                REFERENCES laboratorytests(TestID)

        ) ENGINE=InnoDB;"
},
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_001",
                            Description = "Initial reference data: appointmenttypes",
                            Sql = @"INSERT IGNORE INTO `appointmenttypes` (`AppointmentTypeID`, `AppointmentTypeName`, `Description`) VALUES
	                        (1, 'Consultation', 'Standard doctor consultation'),
	                        (2, 'Follow-up', 'Review after consultation'),
	                        (3, 'Emergency', 'Urgent assessment');"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_002",
                            Description = "Initial reference data: bloodtypes",
                            Sql = @"INSERT IGNORE INTO `bloodtypes` (`BloodTypeID`, `BloodTypeName`) VALUES
	                        (1, 'O+'),
	                        (2, 'O-'),
	                        (3, 'A+'),
	                        (4, 'A-'),
	                        (5, 'B+'),
	                        (6, 'B-'),
	                        (7, 'AB+'),
	                        (8, 'AB-');"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_003",
                            Description = "Initial reference data: countries",
                            Sql = @"INSERT IGNORE INTO `countries` (`CountryID`, `CountryName`) VALUES
	                        (1, 'Afghanistan'),
	                        (2, 'Albania'),
	                        (3, 'Algeria'),
	                        (4, 'Andorra'),
	                        (5, 'Angola'),
	                        (6, 'Antigua and Barbuda'),
	                        (7, 'Argentina'),
	                        (8, 'Armenia'),
	                        (9, 'Australia'),
	                        (10, 'Austria'),
	                        (11, 'Azerbaijan'),
	                        (12, 'Bahamas'),
	                        (13, 'Bahrain'),
	                        (14, 'Bangladesh'),
	                        (15, 'Barbados'),
	                        (16, 'Belarus'),
	                        (17, 'Belgium'),
	                        (18, 'Belize'),
	                        (19, 'Benin'),
	                        (20, 'Bhutan'),
	                        (21, 'Bolivia'),
	                        (22, 'Bosnia and Herzegovina'),
	                        (23, 'Botswana'),
	                        (24, 'Brazil'),
	                        (25, 'Brunei'),
	                        (26, 'Bulgaria'),
	                        (27, 'Burkina Faso'),
	                        (28, 'Burundi'),
	                        (29, 'Cabo Verde'),
	                        (30, 'Cambodia'),
	                        (31, 'Cameroon'),
	                        (32, 'Canada'),
	                        (33, 'Central African Republic'),
	                        (34, 'Chad'),
	                        (35, 'Chile'),
	                        (36, 'China'),
	                        (37, 'Colombia'),
	                        (38, 'Comoros'),
	                        (39, 'Congo'),
	                        (40, 'Costa Rica'),
	                        (41, 'Croatia'),
	                        (42, 'Cuba'),
	                        (43, 'Cyprus'),
	                        (44, 'Czechia'),
	                        (45, 'Denmark'),
	                        (46, 'Djibouti'),
	                        (47, 'Dominica'),
	                        (48, 'Dominican Republic'),
	                        (49, 'Ecuador'),
	                        (50, 'Egypt'),
	                        (51, 'El Salvador'),
	                        (52, 'Equatorial Guinea'),
	                        (53, 'Eritrea'),
	                        (54, 'Estonia'),
	                        (55, 'Eswatini'),
	                        (56, 'Ethiopia'),
	                        (57, 'Fiji'),
	                        (58, 'Finland'),
	                        (59, 'France'),
	                        (60, 'Gabon'),
	                        (61, 'Gambia'),
	                        (62, 'Georgia'),
	                        (63, 'Germany'),
	                        (64, 'Ghana'),
	                        (65, 'Greece'),
	                        (66, 'Grenada'),
	                        (67, 'Guatemala'),
	                        (68, 'Guinea'),
	                        (69, 'Guinea-Bissau'),
	                        (70, 'Guyana'),
	                        (71, 'Haiti'),
	                        (72, 'Honduras'),
	                        (73, 'Hungary'),
	                        (74, 'Iceland'),
	                        (75, 'India'),
	                        (76, 'Indonesia'),
	                        (77, 'Iran'),
	                        (78, 'Iraq'),
	                        (79, 'Ireland'),
	                        (80, 'Israel'),
	                        (81, 'Italy'),
	                        (82, 'Jamaica'),
	                        (83, 'Japan'),
	                        (84, 'Jordan'),
	                        (85, 'Kazakhstan'),
	                        (86, 'Kenya'),
	                        (87, 'Kiribati'),
	                        (88, 'Kuwait'),
	                        (89, 'Kyrgyzstan'),
	                        (90, 'Laos'),
	                        (91, 'Latvia'),
	                        (92, 'Lebanon'),
	                        (93, 'Lesotho'),
	                        (94, 'Liberia'),
	                        (95, 'Libya'),
	                        (96, 'Liechtenstein'),
	                        (97, 'Lithuania'),
	                        (98, 'Luxembourg'),
	                        (99, 'Madagascar'),
	                        (100, 'Malawi'),
	                        (101, 'Malaysia'),
	                        (102, 'Maldives'),
	                        (103, 'Mali'),
	                        (104, 'Malta'),
	                        (105, 'Marshall Islands'),
	                        (106, 'Mauritania'),
	                        (107, 'Mauritius'),
	                        (108, 'Mexico'),
	                        (109, 'Micronesia'),
	                        (110, 'Moldova'),
	                        (111, 'Monaco'),
	                        (112, 'Mongolia'),
	                        (113, 'Montenegro'),
	                        (114, 'Morocco'),
	                        (115, 'Mozambique'),
	                        (116, 'Myanmar'),
	                        (117, 'Namibia'),
	                        (118, 'Nauru'),
	                        (119, 'Nepal'),
	                        (120, 'Netherlands'),
	                        (121, 'New Zealand'),
	                        (122, 'Nicaragua'),
	                        (123, 'Niger'),
	                        (124, 'Nigeria'),
	                        (125, 'North Korea'),
	                        (126, 'North Macedonia'),
	                        (127, 'Norway'),
	                        (128, 'Oman'),
	                        (129, 'Pakistan'),
	                        (130, 'Palau'),
	                        (131, 'Palestine'),
	                        (132, 'Panama'),
	                        (133, 'Papua New Guinea'),
	                        (134, 'Paraguay'),
	                        (135, 'Peru'),
	                        (136, 'Philippines'),
	                        (137, 'Poland'),
	                        (138, 'Portugal'),
	                        (139, 'Qatar'),
	                        (140, 'Romania'),
	                        (141, 'Russia'),
	                        (142, 'Rwanda'),
	                        (143, 'Saint Kitts and Nevis'),
	                        (144, 'Saint Lucia'),
	                        (145, 'Saint Vincent and the Grenadines'),
	                        (146, 'Samoa'),
	                        (147, 'San Marino'),
	                        (148, 'Sao Tome and Principe'),
	                        (149, 'Saudi Arabia'),
	                        (150, 'Senegal'),
	                        (151, 'Serbia'),
	                        (152, 'Seychelles'),
	                        (153, 'Sierra Leone'),
	                        (154, 'Singapore'),
	                        (155, 'Slovakia'),
	                        (156, 'Slovenia'),
	                        (157, 'Solomon Islands'),
	                        (158, 'Somalia'),
	                        (159, 'South Africa'),
	                        (160, 'South Korea'),
	                        (161, 'South Sudan'),
	                        (162, 'Spain'),
	                        (163, 'Sri Lanka'),
	                        (164, 'Sudan'),
	                        (165, 'Suriname'),
	                        (166, 'Sweden'),
	                        (167, 'Switzerland'),
	                        (168, 'Syria'),
	                        (169, 'Taiwan'),
	                        (170, 'Tajikistan'),
	                        (171, 'Tanzania'),
	                        (172, 'Thailand'),
	                        (173, 'Togo'),
	                        (174, 'Tonga'),
	                        (175, 'Trinidad and Tobago'),
	                        (176, 'Tunisia'),
	                        (177, 'Turkey'),
	                        (178, 'Turkmenistan'),
	                        (179, 'Tuvalu'),
	                        (180, 'Uganda'),
	                        (181, 'Ukraine'),
	                        (182, 'United Arab Emirates'),
	                        (183, 'United Kingdom'),
	                        (184, 'United States'),
	                        (185, 'Uruguay'),
	                        (186, 'Uzbekistan'),
	                        (187, 'Vanuatu'),
	                        (188, 'Vatican City'),
	                        (189, 'Venezuela'),
	                        (190, 'Vietnam'),
	                        (191, 'Yemen'),
	                        (192, 'Zambia'),
	                        (193, 'Zimbabwe');"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_004",
                            Description = "Initial reference data: days",
                            Sql = @"INSERT IGNORE INTO `days` (`DayID`, `DayName`) VALUES
	                        (1, 'Saturday'),
	                        (2, 'Sunday'),
	                        (3, 'Monday'),
	                        (4, 'Tuesday'),
	                        (5, 'Wednesday'),
	                        (6, 'Thursday'),
	                        (7, 'Friday');"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_005",
                            Description = "Initial reference data: discounts",
                            Sql = @"INSERT IGNORE INTO `discounts` (`DiscountID`, `DiscountName`, `DiscountPercentage`, `IsActive`) VALUES
	                        (1, 'Senior Citizen', 10.00, 1),
	                        (2, 'Staff Benefit', 15.00, 1),
	                        (3, 'Promotional', 5.00, 1);"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_006",
                            Description = "Initial reference data: roles",
                            Sql = @"INSERT IGNORE INTO `roles` (`RoleID`, `RoleName`) VALUES
	                        (1, 'Administrator'),
	                        (2, 'Doctor'),
	                        (3, 'Nurse'),
	                        (4, 'Receptionist'),
	                        (5, 'Staff'),
	                        (6, 'Pharmacist'),
	                        (7, 'Cashier');"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_007",
                            Description = "Initial reference data: specializations",
                            Sql = @"INSERT IGNORE INTO `specializations` (`SpecializationID`, `SpecializationName`) VALUES
	                        (1, 'General Medicine'),
	                        (2, 'Cardiology'),
	                        (3, 'Pediatrics'),
	                        (4, 'Dermatology'),
	                        (5, 'Orthopedics');"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_SEED_008",
                            Description = "Initial reference data: tbl_country",
                            Sql = @"INSERT IGNORE INTO `tbl_country` (`CountryID`, `CountryName`) VALUES
	                        (1, 'Afghanistan'),
	                        (2, 'Albania'),
	                        (3, 'Algeria'),
	                        (4, 'Andorra'),
	                        (5, 'Angola'),
	                        (6, 'Antigua and Barbuda'),
	                        (7, 'Argentina'),
	                        (8, 'Armenia'),
	                        (9, 'Australia'),
	                        (10, 'Austria'),
	                        (11, 'Azerbaijan'),
	                        (12, 'Bahamas'),
	                        (13, 'Bahrain'),
	                        (14, 'Bangladesh'),
	                        (15, 'Barbados'),
	                        (16, 'Belarus'),
	                        (17, 'Belgium'),
	                        (18, 'Belize'),
	                        (19, 'Benin'),
	                        (20, 'Bhutan'),
	                        (21, 'Bolivia'),
	                        (22, 'Bosnia and Herzegovina'),
	                        (23, 'Botswana'),
	                        (24, 'Brazil'),
	                        (25, 'Brunei'),
	                        (26, 'Bulgaria'),
	                        (27, 'Burkina Faso'),
	                        (28, 'Burundi'),
	                        (29, 'Cabo Verde'),
	                        (30, 'Cambodia'),
	                        (31, 'Cameroon'),
	                        (32, 'Canada'),
	                        (33, 'Central African Republic'),
	                        (34, 'Chad'),
	                        (35, 'Chile'),
	                        (36, 'China'),
	                        (37, 'Colombia'),
	                        (38, 'Comoros'),
	                        (39, 'Congo'),
	                        (40, 'Costa Rica'),
	                        (41, 'Croatia'),
	                        (42, 'Cuba'),
	                        (43, 'Cyprus'),
	                        (44, 'Czechia'),
	                        (45, 'Denmark'),
	                        (46, 'Djibouti'),
	                        (47, 'Dominica'),
	                        (48, 'Dominican Republic'),
	                        (49, 'Ecuador'),
	                        (50, 'Egypt'),
	                        (51, 'El Salvador'),
	                        (52, 'Equatorial Guinea'),
	                        (53, 'Eritrea'),
	                        (54, 'Estonia'),
	                        (55, 'Eswatini'),
	                        (56, 'Ethiopia'),
	                        (57, 'Fiji'),
	                        (58, 'Finland'),
	                        (59, 'France'),
	                        (60, 'Gabon'),
	                        (61, 'Gambia'),
	                        (62, 'Georgia'),
	                        (63, 'Germany'),
	                        (64, 'Ghana'),
	                        (65, 'Greece'),
	                        (66, 'Grenada'),
	                        (67, 'Guatemala'),
	                        (68, 'Guinea'),
	                        (69, 'Guinea-Bissau'),
	                        (70, 'Guyana'),
	                        (71, 'Haiti'),
	                        (72, 'Honduras'),
	                        (73, 'Hungary'),
	                        (74, 'Iceland'),
	                        (75, 'India'),
	                        (76, 'Indonesia'),
	                        (77, 'Iran'),
	                        (78, 'Iraq'),
	                        (79, 'Ireland'),
	                        (80, 'Israel'),
	                        (81, 'Italy'),
	                        (82, 'Jamaica'),
	                        (83, 'Japan'),
	                        (84, 'Jordan'),
	                        (85, 'Kazakhstan'),
	                        (86, 'Kenya'),
	                        (87, 'Kiribati'),
	                        (88, 'Kuwait'),
	                        (89, 'Kyrgyzstan'),
	                        (90, 'Laos'),
	                        (91, 'Latvia'),
	                        (92, 'Lebanon'),
	                        (93, 'Lesotho'),
	                        (94, 'Liberia'),
	                        (95, 'Libya'),
	                        (96, 'Liechtenstein'),
	                        (97, 'Lithuania'),
	                        (98, 'Luxembourg'),
	                        (99, 'Madagascar'),
	                        (100, 'Malawi'),
	                        (101, 'Malaysia'),
	                        (102, 'Maldives'),
	                        (103, 'Mali'),
	                        (104, 'Malta'),
	                        (105, 'Marshall Islands'),
	                        (106, 'Mauritania'),
	                        (107, 'Mauritius'),
	                        (108, 'Mexico'),
	                        (109, 'Micronesia'),
	                        (110, 'Moldova'),
	                        (111, 'Monaco'),
	                        (112, 'Mongolia'),
	                        (113, 'Montenegro'),
	                        (114, 'Morocco'),
	                        (115, 'Mozambique'),
	                        (116, 'Myanmar'),
	                        (117, 'Namibia'),
	                        (118, 'Nauru'),
	                        (119, 'Nepal'),
	                        (120, 'Netherlands'),
	                        (121, 'New Zealand'),
	                        (122, 'Nicaragua'),
	                        (123, 'Niger'),
	                        (124, 'Nigeria'),
	                        (125, 'North Korea'),
	                        (126, 'North Macedonia'),
	                        (127, 'Norway'),
	                        (128, 'Oman'),
	                        (129, 'Pakistan'),
	                        (130, 'Palau'),
	                        (131, 'Palestine'),
	                        (132, 'Panama'),
	                        (133, 'Papua New Guinea'),
	                        (134, 'Paraguay'),
	                        (135, 'Peru'),
	                        (136, 'Philippines'),
	                        (137, 'Poland'),
	                        (138, 'Portugal'),
	                        (139, 'Qatar'),
	                        (140, 'Romania'),
	                        (141, 'Russia'),
	                        (142, 'Rwanda'),
	                        (143, 'Saint Kitts and Nevis'),
	                        (144, 'Saint Lucia'),
	                        (145, 'Saint Vincent and the Grenadines'),
	                        (146, 'Samoa'),
	                        (147, 'San Marino'),
	                        (148, 'Sao Tome and Principe'),
	                        (149, 'Saudi Arabia'),
	                        (150, 'Senegal'),
	                        (151, 'Serbia'),
	                        (152, 'Seychelles'),
	                        (153, 'Sierra Leone'),
	                        (154, 'Singapore'),
	                        (155, 'Slovakia'),
	                        (156, 'Slovenia'),
	                        (157, 'Solomon Islands'),
	                        (158, 'Somalia'),
	                        (159, 'South Africa'),
	                        (160, 'South Korea'),
	                        (161, 'South Sudan'),
	                        (162, 'Spain'),
	                        (163, 'Sri Lanka'),
	                        (164, 'Sudan'),
	                        (165, 'Suriname'),
	                        (166, 'Sweden'),
	                        (167, 'Switzerland'),
	                        (168, 'Syria'),
	                        (169, 'Taiwan'),
	                        (170, 'Tajikistan'),
	                        (171, 'Tanzania'),
	                        (172, 'Thailand'),
	                        (173, 'Togo'),
	                        (174, 'Tonga'),
	                        (175, 'Trinidad and Tobago'),
	                        (176, 'Tunisia'),
	                        (177, 'Turkey'),
	                        (178, 'Turkmenistan'),
	                        (179, 'Tuvalu'),
	                        (180, 'Uganda'),
	                        (181, 'Ukraine'),
	                        (182, 'United Arab Emirates'),
	                        (183, 'United Kingdom'),
	                        (184, 'United States'),
	                        (185, 'Uruguay'),
	                        (186, 'Uzbekistan'),
	                        (187, 'Vanuatu'),
	                        (188, 'Vatican City'),
	                        (189, 'Venezuela'),
	                        (190, 'Vietnam'),
	                        (191, 'Yemen'),
	                        (192, 'Zambia'),
	                        (193, 'Zimbabwe');"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_VIEW_001",
                            Description = "Initial view: view_doctorsdetails",
                            Sql = @"CREATE OR REPLACE VIEW `view_doctorsdetails` AS select `d`.`DoctorID` AS `DoctorID`,`d`.`PersonID` AS `PersonID`,`p`.`FullName` AS `FullName`,`d`.`LicenseNumber` AS `LicenseNumber`,`d`.`ConsultationFees` AS `ConsultationFees`,coalesce((select group_concat(distinct `s`.`SpecializationName` order by `s`.`SpecializationName` ASC separator ', ') from (`doctorspecialties` `ds` join `specializations` `s` on((`s`.`SpecializationID` = `ds`.`SpecializationID`))) where (`ds`.`DoctorID` = `d`.`DoctorID`)),'') AS `Specializations`,coalesce((select group_concat(`dy`.`DayName` order by `dw`.`DayID` ASC separator ', ') from (`doctorworkingdays` `dw` join `days` `dy` on((`dy`.`DayID` = `dw`.`DayID`))) where (`dw`.`DoctorID` = `d`.`DoctorID`)),'') AS `WorkingDays`,coalesce(`c`.`CountryName`,'') AS `CountryName`,`d`.`IsActive` AS `IsActive` from ((`doctors` `d` left join `persons` `p` on((`p`.`PersonID` = `d`.`PersonID`))) left join `countries` `c` on((`c`.`CountryID` = `p`.`NationalityCountryID`)));"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_VIEW_002",
                            Description = "Initial view: view_visitdetails",
                            Sql = @"CREATE OR REPLACE VIEW `view_visitdetails` AS select `v`.`VisitID` AS `VisitID`,`v`.`PatientID` AS `PatientID`,`v`.`DoctorID` AS `DoctorID`,`v`.`AppointmentID` AS `AppointmentID`,`pp`.`FullName` AS `PatientName`,`dp`.`FullName` AS `DoctorName`,`v`.`VisitDate` AS `VisitDate`,`v`.`VisitStatus` AS `VisitStatus`,'' AS `Diagnosis`,(case when (`v`.`VisitStatus` = 'Open') then 'Open' when (`v`.`VisitStatus` = 'Completed') then 'Completed' else coalesce(`v`.`VisitStatus`,'') end) AS `StatusText`,`v`.`CreatedByUserID` AS `CreatedByUserID` from ((((`visits` `v` left join `patients` `p` on((`p`.`PatientID` = `v`.`PatientID`))) left join `persons` `pp` on((`pp`.`PersonID` = `p`.`PersonID`))) left join `doctors` `d` on((`d`.`DoctorID` = `v`.`DoctorID`))) left join `persons` `dp` on((`dp`.`PersonID` = `d`.`PersonID`)));"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_VIEW_003",
                            Description = "Initial view: view_patientswaitingfordoctors",
                            Sql = @"CREATE OR REPLACE VIEW `view_patientswaitingfordoctors` AS select `v`.`VisitID` AS `VisitID`,`v`.`AppointmentID` AS `AppointmentID`,`p`.`FullName` AS `PatientName`,`a`.`AppointmentDate` AS `CheckInTime`,`a`.`AppointmentStatus` AS `StatusText`,0 AS `IsCalled`,`d`.`PersonID` AS `DoctorPersonID` from ((((`visits` `v` join `appointments` `a` on((`a`.`AppointmentID` = `v`.`AppointmentID`))) join `patients` `pt` on((`pt`.`PatientID` = `v`.`PatientID`))) join `persons` `p` on((`p`.`PersonID` = `pt`.`PersonID`))) join `doctors` `d` on((`d`.`DoctorID` = `v`.`DoctorID`)));"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261013_001",
                            Description = "Create laboratory tests table",
                            Sql = @"
                                CREATE TABLE IF NOT EXISTS laboratorytests
                                (
                                    TestID INT AUTO_INCREMENT PRIMARY KEY,
                                    TestName VARCHAR(200) NOT NULL,
                                    TestPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
                                    IsActive TINYINT(1) NOT NULL DEFAULT 1
                                ) ENGINE=InnoDB;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261013_002",
                            Description = "Create visit services table",
                            Sql = @"
                                CREATE TABLE IF NOT EXISTS visitservices
                                (
                                    VisitServiceID INT AUTO_INCREMENT PRIMARY KEY,
                                    VisitID INT NOT NULL,
                                    ServiceID INT NOT NULL,
                                    SavedServicePrice DECIMAL(18,2) NOT NULL DEFAULT 0,
                                    Quantity DECIMAL(18,3) NOT NULL DEFAULT 1,
                                    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,

                                    INDEX IX_VisitServices_VisitID (VisitID),
                                    INDEX IX_VisitServices_ServiceID (ServiceID)
                                ) ENGINE=InnoDB;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261014_003",
                            Description = "Create billing list view",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_Bills AS
                                SELECT
                                    B.BillID,
                                    CONCAT(
                                        'INV-',
                                        YEAR(B.BillDate),
                                        '-',
                                        LPAD(B.BillID, 4, '0')
                                    ) AS BillNumber,
                                    B.VisitID,
                                    V.PatientID,
                                    P.PersonID,
                                    PE.FullName AS PatientName,
                                    V.DoctorID,
                                    V.AppointmentID,
                                    B.BillDate,
                                    B.TotalAmount,
                                    B.PaymentStatus,
                                    B.CreatedByUserID,
                                    COALESCE(PAY.PaymentAmount, 0) AS PaidAmount,
                                    GREATEST(
                                        B.TotalAmount - COALESCE(PAY.PaymentAmount, 0),
                                        0
                                    ) AS BalanceDue
                                FROM bills B
                                INNER JOIN visits V
                                    ON B.VisitID = V.VisitID
                                INNER JOIN patients P
                                    ON V.PatientID = P.PatientID
                                INNER JOIN persons PE
                                    ON P.PersonID = PE.PersonID
                                LEFT JOIN
                                (
                                    SELECT
                                        BillID,
                                        SUM(PaymentAmount) AS PaymentAmount
                                    FROM payments
                                    GROUP BY BillID
                                ) PAY
                                    ON B.BillID = PAY.BillID;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261014_004",
                            Description = "Synchronize fully paid bills with visit status",
                            Sql = @"
                                UPDATE visits V
                                INNER JOIN bills B
                                    ON B.VisitID = V.VisitID
                                LEFT JOIN
                                (
                                    SELECT
                                        BillID,
                                        SUM(PaymentAmount) AS PaidAmount
                                    FROM payments
                                    GROUP BY BillID
                                ) PAY
                                    ON PAY.BillID = B.BillID
                                SET V.VisitStatus = 'Completed'
                                WHERE V.VisitStatus = 'Pending_Billing'
                                  AND B.PaymentStatus = 'Paid'
                                  AND COALESCE(PAY.PaidAmount, 0)
                                      >= B.TotalAmount;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261014_005",
                            Description = "Create or repair appointments details view",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_AppointmentsDetails AS
                                SELECT
                                    A.AppointmentID,
                                    A.PatientID,
                                    PE.FullName AS PatientName,
                                    A.DoctorID,
                                    DPE.FullName AS DoctorName,
                                    A.AppointmentDate,
                                    A.AppointmentStatus,
                                    A.AppointmentStatus AS AppointmentStatusText,
                                    A.AppointmentTypeID,
                                    AT.AppointmentTypeName AS TypeName,
                                    A.AppointmentFees,
                                    A.CreatedByUserID

                                FROM appointments A

                                LEFT JOIN patients P
                                    ON A.PatientID = P.PatientID

                                LEFT JOIN persons PE
                                    ON P.PersonID = PE.PersonID

                                LEFT JOIN doctors D
                                    ON A.DoctorID = D.DoctorID

                                LEFT JOIN persons DPE
                                    ON D.PersonID = DPE.PersonID

                                LEFT JOIN appointmenttypes AT
                                    ON A.AppointmentTypeID = AT.AppointmentTypeID;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261014_006",
                            Description = "Configure working days for Doctor ID 1",
                            Sql = @"
                                INSERT INTO doctorworkingdays
                                (
                                    DoctorID,
                                    DayID,
                                    StartTime,
                                    EndTime
                                )
                                SELECT
                                    1,
                                    D.DayID,
                                    '09:00:00',
                                    '17:00:00'
                                FROM days D
                                WHERE D.DayID IN (3, 4, 5, 6, 7)
                                  AND EXISTS
                                  (
                                      SELECT 1
                                      FROM doctors DR
                                      WHERE DR.DoctorID = 1
                                  )
                                  AND NOT EXISTS
                                  (
                                      SELECT 1
                                      FROM doctorworkingdays DW
                                      WHERE DW.DoctorID = 1
                                        AND DW.DayID = D.DayID
                                  );"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261014_007",
                            Description = "Configure working days for Doctor ID 2",
                            Sql = @"
                                INSERT INTO doctorworkingdays
                                (
                                    DoctorID,
                                    DayID,
                                    StartTime,
                                    EndTime
                                )
                                SELECT
                                    2,
                                    D.DayID,
                                    '09:00:00',
                                    '17:00:00'
                                FROM days D
                                WHERE D.DayID IN (3, 4, 5, 6, 7)
                                  AND EXISTS
                                  (
                                      SELECT 1
                                      FROM doctors DR
                                      WHERE DR.DoctorID = 2
                                  )
                                  AND NOT EXISTS
                                  (
                                      SELECT 1
                                      FROM doctorworkingdays DW
                                      WHERE DW.DoctorID = 2
                                        AND DW.DayID = D.DayID
                                  );"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261015_008",
                            Description = "Fix appointment status text in registry",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_AppointmentsDetails AS
                                SELECT
                                    A.AppointmentID,
                                    A.PatientID,
                                    PE.FullName AS PatientName,
                                    A.DoctorID,
                                    DPE.FullName AS DoctorName,
                                    A.AppointmentDate,

                                    A.AppointmentStatus,

                                    CASE CAST(A.AppointmentStatus AS CHAR)
                                        WHEN '1' THEN 'Scheduled'
                                        WHEN '2' THEN 'InQueue'
                                        WHEN '3' THEN 'Progress'
                                        WHEN '4' THEN 'Postponed'
                                        WHEN '5' THEN 'Completed'
                                        WHEN '6' THEN 'Cancelled'
                                        WHEN '7' THEN 'Waiting_For_Vitals'
                                        WHEN '8' THEN 'Ready_For_Doctor'
                                        WHEN '9' THEN 'In_Billing'
                                        ELSE CAST(A.AppointmentStatus AS CHAR)
                                    END AS AppointmentStatusText,

                                    A.AppointmentTypeID,
                                    AT.AppointmentTypeName AS TypeName,
                                    A.AppointmentFees,
                                    A.CreatedByUserID

                                FROM appointments A

                                LEFT JOIN patients P
                                    ON A.PatientID = P.PatientID

                                LEFT JOIN persons PE
                                    ON P.PersonID = PE.PersonID

                                LEFT JOIN doctors D
                                    ON A.DoctorID = D.DoctorID

                                LEFT JOIN persons DPE
                                    ON D.PersonID = DPE.PersonID

                                LEFT JOIN appointmenttypes AT
                                    ON A.AppointmentTypeID = AT.AppointmentTypeID;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_009",
                            Description = "Add last modified tracking columns to appointments",
                            Sql = @"
                                ALTER TABLE appointments
                                ADD COLUMN LastModifiedDate DATETIME NULL,
                                ADD COLUMN LastModifiedByUserID INT NULL;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_014",
                            Description = "Add IsDeleted column to persons",
                            Sql = @"
                                ALTER TABLE persons
                                ADD COLUMN IsDeleted TINYINT(1) NOT NULL DEFAULT 0;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_019",
                            Description = "Add waiting queue columns to appointments",
                            Sql = @"
                                ALTER TABLE appointments
                                ADD COLUMN CallType INT NOT NULL DEFAULT 0,
                                ADD COLUMN IsCalled TINYINT(1) NOT NULL DEFAULT 0,
                                ADD COLUMN CheckInTime DATETIME NULL;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_010",
                            Description = "Create View_UserDetails",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_UserDetails AS
                                SELECT
                                    U.UserID,
                                    P.PersonID,
                                    P.FullName,
                                    U.UserName,
                                    R.RoleName,
                                    U.IsActive
                                FROM users U
                                INNER JOIN persons P
                                    ON U.PersonID = P.PersonID
                                INNER JOIN roles R
                                    ON U.RoleID = R.RoleID
                                WHERE P.IsDeleted = 0;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_011",
                            Description = "Create View_PrescriptionDetails",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_PrescriptionDetails AS
                                SELECT
                                    P.PrescriptionID,

                                    DATE_FORMAT(
                                        P.PrescriptionDate,
                                        '%Y-%m-%d'
                                    ) AS PrescriptionDate,

                                    CASE
                                        WHEN P.PrescriptionType = 2
                                            THEN 'Pharmacy Sale'
                                        ELSE COALESCE(
                                            PP.FullName,
                                            'Unknown Patient'
                                        )
                                    END AS PatientName,

                                    CASE
                                        WHEN P.PrescriptionType = 2
                                            THEN 'Pharmacy System'
                                        ELSE COALESCE(
                                            PD.FullName,
                                            'Unknown Doctor'
                                        )
                                    END AS DoctorName,

                                    P.VisitID,

                                    CASE P.PrescriptionStatus
                                        WHEN 1 THEN 'Pending'
                                        WHEN 2 THEN 'Waiting For Payment'
                                        WHEN 3 THEN 'Ready For Dispensing'
                                        WHEN 4 THEN 'Dispensed'
                                        WHEN 5 THEN 'Partially Dispensed'
                                        WHEN 6 THEN 'Cancelled'
                                        ELSE 'Unknown'
                                    END AS Status,

                                    P.PrescriptionNotes AS Notes

                                FROM prescriptions P

                                LEFT JOIN visits V
                                    ON P.VisitID = V.VisitID

                                LEFT JOIN patients Pat
                                    ON V.PatientID = Pat.PatientID

                                LEFT JOIN persons PP
                                    ON Pat.PersonID = PP.PersonID

                                LEFT JOIN doctors D
                                    ON V.DoctorID = D.DoctorID

                                LEFT JOIN persons PD
                                    ON D.PersonID = PD.PersonID;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_012",
                            Description = "Create View_PatientsDetails",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_PatientsDetails AS
                                SELECT
                                    Pat.PatientID,
                                    P.PersonID,
                                    P.FullName,
                                    Pat.MedicalHistory,
                                    Pat.CreatedDate,
                                    C.CountryName,
                                    B.BloodTypeName,
                                    Pat.EmergencyContact
                                FROM persons P
                                INNER JOIN patients Pat
                                    ON P.PersonID = Pat.PersonID
                                INNER JOIN countries C
                                    ON P.NationalityCountryID = C.CountryID
                                INNER JOIN bloodtypes B
                                    ON Pat.BloodTypeID = B.BloodTypeID
                                WHERE P.IsDeleted = 0;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_013",
                            Description = "Create View_PersonDetails",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_PersonDetails AS
                                SELECT
                                    P.PersonID,
                                    P.FullName,
                                    P.DateOfBirth,

                                    CASE P.Gender
                                        WHEN 0 THEN 'Male'
                                        ELSE 'Female'
                                    END AS GenderCaption,

                                    P.PhoneNumber,
                                    C.CountryName,
                                    P.Address,
                                    P.Email

                                FROM persons P

                                INNER JOIN countries C
                                    ON P.NationalityCountryID = C.CountryID
                                WHERE P.IsDeleted = 0;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_015",
                            Description = "Create View_PersonTrashDetails",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_PersonTrashDetails AS
                                SELECT
                                    P.PersonID,
                                    P.FullName,
                                    P.DateOfBirth,

                                    CASE P.Gender
                                        WHEN 0 THEN 'Male'
                                        ELSE 'Female'
                                    END AS GenderCaption,

                                    P.PhoneNumber,
                                    C.CountryName,
                                    P.Address,
                                    P.Email

                                FROM persons P

                                INNER JOIN countries C
                                    ON P.NationalityCountryID = C.CountryID

                                WHERE P.IsDeleted = 1;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_016",
                            Description = "Create View_BillSummaries",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_BillSummaries AS
                                SELECT
                                    CONCAT(
                                        'INV-',
                                        YEAR(B.BillDate),
                                        '-',
                                        LPAD(B.BillID, 4, '0')
                                    ) AS BillNumber,

                                    B.BillID,

                                    COALESCE(P.FullName, 'Pharmacy Sale') AS PatientName,

                                    CASE
                                        WHEN B.VisitID IS NULL THEN
                                            CONCAT(
                                                COALESCE(M.PrescriptionCount, 0),
                                                ' Pharmacy Sale'
                                            )
                                        ELSE
                                            CONCAT(
                                                COALESCE(S.ServiceCount, 0),
                                                ' Servs, ',
                                                COALESCE(M.PrescriptionCount, 0),
                                                ' Presc'
                                            )
                                    END AS ItemsTotal,

                                    COALESCE(A.AppointmentFees, 0) AS AppointmentFees,

                                    COALESCE(M.TotalMedicinesOnly, 0) AS MedicinesTotal,

                                    COALESCE(S.TotalServicesOnly, 0) AS ServicesTotal,

                                    (
                                        COALESCE(S.ServiceDiscount, 0)
                                        + COALESCE(M.MedsDiscount, 0)
                                    ) AS TotalDiscount,

                                    COALESCE(M.TotalTax, 0) AS TotalTax,

                                    (
                                        COALESCE(A.AppointmentFees, 0)
                                        + COALESCE(M.TotalMedicinesNet, 0)
                                        + COALESCE(S.TotalServicesNet, 0)
                                    ) AS Subtotal,

                                    COALESCE(Pay.TotalPaid, 0) AS PaymentAmount,

                                    (
                                        COALESCE(A.AppointmentFees, 0)
                                        + COALESCE(M.TotalMedicinesNet, 0)
                                        + COALESCE(S.TotalServicesNet, 0)
                                        - COALESCE(Pay.TotalPaid, 0)
                                    ) AS BalanceDue

                                FROM bills B

                                LEFT JOIN visits V
                                    ON B.VisitID = V.VisitID

                                LEFT JOIN appointments A
                                    ON V.AppointmentID = A.AppointmentID

                                LEFT JOIN patients PA
                                    ON V.PatientID = PA.PatientID

                                LEFT JOIN persons P
                                    ON PA.PersonID = P.PersonID

                                LEFT JOIN
                                (
                                    SELECT
                                        BillID,
                                        SUM(PaymentAmount) AS TotalPaid
                                    FROM payments
                                    GROUP BY BillID
                                ) Pay
                                    ON B.BillID = Pay.BillID

                                LEFT JOIN
                                (
                                    SELECT
                                        VisitID,

                                        COUNT(*) AS ServiceCount,

                                        SUM(
                                            SavedServicePrice * Quantity
                                        ) AS TotalServicesOnly,

                                        SUM(
                                            Discount * Quantity
                                        ) AS ServiceDiscount,

                                        SUM(
                                            (SavedServicePrice * Quantity)
                                            - (Discount * Quantity)
                                        ) AS TotalServicesNet

                                    FROM visitservices
                                    GROUP BY VisitID
                                ) S
                                    ON B.VisitID = S.VisitID

                                LEFT JOIN
                                (
                                    SELECT
                                        PR.VisitID,

                                        COUNT(DISTINCT PR.PrescriptionID)
                                            AS PrescriptionCount,

                                        COALESCE(
                                            SUM(
                                                PD.SavedMedicinePrice
                                                * PD.Quantity
                                            ), 0
                                        ) AS TotalMedicinesOnly,

                                        COALESCE(
                                            SUM(
                                                PD.DiscountAmount
                                                * PD.Quantity
                                            ), 0
                                        ) AS MedsDiscount,

                                        CAST(
                                            ROUND(
                                                COALESCE(
                                                    SUM(
                                                        CASE
                                                            WHEN PD.Quantity > 0 THEN
                                                                (
                                                                    (
                                                                        PD.SavedMedicinePrice
                                                                        * PD.Quantity
                                                                    )
                                                                    -
                                                                    (
                                                                        PD.DiscountAmount
                                                                        * PD.Quantity
                                                                    )
                                                                )
                                                                *
                                                                (
                                                                    COALESCE(MED.TaxRate, 0)
                                                                    / 100.0
                                                                )
                                                            ELSE 0
                                                        END
                                                    ), 0
                                                ),
                                                2
                                            ) AS DECIMAL(18,2)
                                        ) AS TotalTax,

                                        COALESCE(
                                            SUM(
                                                CASE
                                                    WHEN PD.Quantity > 0 THEN
                                                        (
                                                            (
                                                                PD.SavedMedicinePrice
                                                                * PD.Quantity
                                                            )
                                                            -
                                                            (
                                                                PD.DiscountAmount
                                                                * PD.Quantity
                                                            )
                                                        )
                                                        *
                                                        (
                                                            1 +
                                                            (
                                                                COALESCE(MED.TaxRate, 0)
                                                                / 100.0
                                                            )
                                                        )
                                                    ELSE 0
                                                END
                                            ), 0
                                        ) AS TotalMedicinesNet

                                    FROM prescriptions PR

                                    LEFT JOIN prescriptiondetails PD
                                        ON PR.PrescriptionID = PD.PrescriptionID

                                    LEFT JOIN medicines MED
                                        ON PD.MedicineID = MED.MedicineID

                                    GROUP BY PR.VisitID
                                ) M
                                    ON B.VisitID = M.VisitID;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_017",
                            Description = "Create View_BillItems",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_BillItems AS

                                -- 1. Medicine Items
                                SELECT
                                    B.BillID,
                                    'Medicine' AS ItemType,
                                    PD.SavedMedicineName AS ItemName,
                                    PD.Quantity AS Quantity,
                                    PD.SavedMedicinePrice AS Price,
                                    (
                                        PD.SavedMedicinePrice * PD.Quantity
                                    ) AS Total

                                FROM prescriptiondetails PD

                                INNER JOIN prescriptions PR
                                    ON PD.PrescriptionID = PR.PrescriptionID

                                INNER JOIN bills B
                                    ON PR.VisitID = B.VisitID

                                UNION ALL

                                -- 2. Service Items
                                SELECT
                                    B.BillID,
                                    'Service' AS ItemType,
                                    S.ServiceName AS ItemName,
                                    VS.Quantity AS Quantity,
                                    VS.SavedServicePrice AS Price,
                                    (
                                        VS.SavedServicePrice * VS.Quantity
                                    ) AS Total

                                FROM visitservices VS

                                INNER JOIN services S
                                    ON VS.ServiceID = S.ServiceID

                                INNER JOIN bills B
                                    ON VS.VisitID = B.VisitID

                                UNION ALL

                                -- 3. Appointment Fees
                                SELECT
                                    B.BillID,
                                    'Appointment' AS ItemType,
                                    'Appointment Fees' AS ItemName,
                                    1 AS Quantity,
                                    A.AppointmentFees AS Price,
                                    A.AppointmentFees AS Total

                                FROM appointments A

                                INNER JOIN visits V
                                    ON A.AppointmentID = V.AppointmentID

                                INNER JOIN bills B
                                    ON V.VisitID = B.VisitID

                                WHERE A.AppointmentFees > 0;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_018",
                            Description = "Create View_PatientsWaitingForVitals",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_PatientsWaitingForVitals AS
                                SELECT
                                    V.VisitID,
                                    A.AppointmentID,
                                    P.FullName AS PatientName,
                                    A.CheckInTime,

                                    CASE A.AppointmentStatus
                                        WHEN 2 THEN 'In-Queue'
                                        WHEN 7 THEN 'Waiting_For_Vitals'
                                        ELSE 'Unknown'
                                    END AS StatusText,

                                    A.IsCalled

                                FROM appointments A

                                INNER JOIN visits V
                                    ON A.AppointmentID = V.AppointmentID

                                INNER JOIN patients PT
                                    ON A.PatientID = PT.PatientID

                                INNER JOIN persons P
                                    ON PT.PersonID = P.PersonID;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261016_019",
                            Description = "Create View_WaitingPatients",
                            Sql = @"
                                CREATE OR REPLACE VIEW View_WaitingPatients AS
                                SELECT
                                    A.AppointmentID,
                                    Pe.FullName AS PatientName,
                                    A.CallType,
                                    A.IsCalled,
                                    A.CheckInTime,
                                    A.AppointmentStatus,
                                    D.DoctorID,

                                    CASE A.AppointmentStatus
                                        WHEN 2 THEN 'In-Queue'
                                        WHEN 7 THEN 'Waiting_For_Vitals'
                                        WHEN 8 THEN 'Ready_For_Doctor'
                                        ELSE 'Unknown'
                                    END AS StatusText

                                FROM appointments A

                                INNER JOIN patients P
                                    ON A.PatientID = P.PatientID

                                INNER JOIN persons Pe
                                    ON P.PersonID = Pe.PersonID

                                INNER JOIN doctors D
                                    ON A.DoctorID = D.DoctorID;
                            "
                        },
                  };

                // STEP 4: Apply pending migrations
                foreach (DatabaseMigration migration in migrations)
                {
                    // Check if this migration was already applied
                    using (MySqlCommand command = new MySqlCommand(@"
                        SELECT COUNT(*)
                        FROM schema_migrations
                        WHERE version = @version;",
                        connection))
                    {
                        command.Parameters.AddWithValue(
                            "@version", migration.Version);

                        long count = Convert.ToInt64(
                            command.ExecuteScalar());

                        if (count > 0)
                        {
                            Debug.WriteLine(
                                "Already applied: " +
                                migration.Version);

                            continue;
                        }
                    }

                    try
                    {
                        Debug.WriteLine(
                            "Executing migration: " + migration.Version);

                        using (MySqlCommand command =
                            new MySqlCommand(migration.Sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new InvalidOperationException(
                            "Database migration failed: " +
                            migration.Version + Environment.NewLine +
                            migration.Description + Environment.NewLine +
                            ex.Message,
                            ex);
                    }

                    // Save migration history after successful execution
                    using (MySqlCommand command = new MySqlCommand(@"
                        INSERT INTO schema_migrations
                            (version, description)
                        VALUES
                            (@version, @description);",
                        connection))
                    {
                        command.Parameters.AddWithValue(
                            "@version", migration.Version);

                        command.Parameters.AddWithValue(
                            "@description", migration.Description);

                        command.ExecuteNonQuery();
                    }

                    Debug.WriteLine(
                        "Migration applied: " +
                        migration.Version);
                }

                Debug.WriteLine(
                    "All database migrations completed.");
            }
        }
    }
}
