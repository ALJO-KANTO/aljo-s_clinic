
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
                            Version = "20261016_021",
                            Description = "Add BillNumber column to Bills",
                            Sql = @"
                                ALTER TABLE bills
                                ADD COLUMN BillNumber VARCHAR(30) NULL;
                            "
                        },
                        new DatabaseMigration
                        {
                            Version = "20261019_0022",
                            Description = "Create XRayTests and XRayRequests tables",
                            Sql = @"
                                CREATE TABLE IF NOT EXISTS xraytests (
                                    XRayTestID INT NOT NULL AUTO_INCREMENT,
                                    TestName VARCHAR(200) NOT NULL,
                                    TestPrice DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                                    IsActive TINYINT(1) NOT NULL DEFAULT 1,
                                    PRIMARY KEY (XRayTestID),
                                    UNIQUE KEY UQ_XRayTests_Name (TestName)
                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
                                  COLLATE=utf8mb4_0900_ai_ci;

                                CREATE TABLE IF NOT EXISTS xrayrequests (
                                    XRayRequestID INT NOT NULL AUTO_INCREMENT,
                                    VisitID INT NOT NULL,
                                    XRayTestID INT NOT NULL,
                                    RequestedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                    Status VARCHAR(30) NOT NULL DEFAULT 'Ordered',
                                    ClinicalNotes TEXT,
                                    Findings TEXT,
                                    Impression TEXT,
                                    ImagePath VARCHAR(1000) DEFAULT NULL,
                                    ResultedAt DATETIME DEFAULT NULL,

                                    PRIMARY KEY (XRayRequestID),

                                    KEY IX_XRayRequests_Visit (VisitID),
                                    KEY IX_XRayRequests_Test (XRayTestID),
                                    KEY IX_XRayRequests_Status (Status),

                                    CONSTRAINT FK_XRayRequests_Test
                                        FOREIGN KEY (XRayTestID)
                                        REFERENCES xraytests (XRayTestID),

                                    CONSTRAINT FK_XRayRequests_Visit
                                        FOREIGN KEY (VisitID)
                                        REFERENCES visits (VisitID)

                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
                                  COLLATE=utf8mb4_0900_ai_ci;
                            "
                        },
                        //Views  
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
                        // Paste these entries into your existing List<DatabaseMigration>.
                        // Generated from the supplied ERP createDataBase source.
                        // Schema only; seed data provided separately.
                        // Verify against a test database before production.

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_001",
                            Description = "Create tbl_coa_level_1",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_coa_level_1` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                                  `code` int DEFAULT NULL,
                                                  `category_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_002",
                            Description = "Create tbl_coa_level_2",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_coa_level_2` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                                  `code` int NOT NULL,
                                                  `main_id` int NOT NULL,
                                                  `SyncStatus` tinyint NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_003",
                            Description = "Create tbl_coa_level_3",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_coa_level_3` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                                  `code` int NOT NULL,
                                                  `main_id` int NOT NULL,
                                                  `SyncStatus` tinyint NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_004",
                            Description = "Create tbl_coa_level_4",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_coa_level_4` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                                  `code` int NOT NULL,
                                                  `main_id` int NOT NULL,
                                                  `debit` decimal(20,3) DEFAULT '0.000',
                                                  `credit` decimal(20,3) DEFAULT '0.000',
                                                  `date` datetime DEFAULT NULL,
                                                  `SyncStatus` tinyint NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_005",
                            Description = "Create tbl_cost_center",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_cost_center` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` int DEFAULT NULL,
                                                  `name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_006",
                            Description = "Create tbl_sub_cost_center",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_sub_cost_center` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` int DEFAULT NULL,
                                                  `name` varchar(350) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `main_id` int DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_007",
                            Description = "Create tbl_fixed_assets",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_fixed_assets` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `brand` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `category_id` int DEFAULT NULL,
                                                  `model` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `supplier` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `status` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `invoice_number` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `purchase_date` date DEFAULT NULL,
                                                  `end_date` date DEFAULT NULL,
                                                  `depreciation_life` int DEFAULT NULL,
                                                  `purchase_price` decimal(20,6) DEFAULT NULL,
                                                  `debit_account_id` int DEFAULT NULL,
                                                  `credit_account_id` int DEFAULT NULL,
                                                  `expence_account_id` int DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `modified_by` int DEFAULT NULL,
                                                  `modified_date` date DEFAULT NULL,
                                                  `state` int DEFAULT '0',
                                                  `manufacture` int DEFAULT '0',
                                                  `manufactureStatus` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_008",
                            Description = "Create tbl_fixed_assets_category",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_fixed_assets_category` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `category_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
	                                              `assets_account_id` INT(10) NOT NULL DEFAULT '0',
	                                              `depreciation_account_id` INT(10) NOT NULL DEFAULT '0',
	                                              `expence_account_id` INT(10) NOT NULL DEFAULT '0',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_009",
                            Description = "Create tbl_deleted_records",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS tbl_deleted_records (
                                                    id INT AUTO_INCREMENT PRIMARY KEY,
                                                    table_name VARCHAR(100),
                                                    record_data TEXT,
                                                    deleted_by INT,
                                                    deleted_at DATETIME DEFAULT CURRENT_TIMESTAMP
                                                );
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_010",
                            Description = "Create tbl_journal_voucher_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_journal_voucher_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date NOT NULL,
                                                  `debit` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `credit` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `inv_id` int NOT NULL DEFAULT '0',
                                                  `description` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `partner` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
                                                  `account_id` int NOT NULL DEFAULT '0',
                                                  `project_id` INT NOT NULL DEFAULT 0, 
                          `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,

                                                  PRIMARY KEY (`id`)

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_011",
                            Description = "Create tbl_journal_voucher",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_journal_voucher` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `debit` decimal(20,4) DEFAULT NULL,
                                                  `credit` decimal(20,4) DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `modified_by` int DEFAULT NULL,
                                                  `modified_date` date DEFAULT NULL,
                                                  `state` int DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                         `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,

                                                  PRIMARY KEY (`id`) USING BTREE

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_012",
                            Description = "Create tbl_check_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_check_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `check_id` int DEFAULT NULL,
                                                  `check_no` int DEFAULT NULL,
                                                  `check_date` date DEFAULT NULL,
                                                  `check_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `pvc_no` int DEFAULT NULL,
                                                  `check_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `amount` decimal(20,6) DEFAULT NULL,
                                                  `pass_date` date DEFAULT NULL,
                                                  `return_date` date DEFAULT NULL,
                                                  `hold_date` date DEFAULT NULL,
                                                  `cancel_date` date DEFAULT NULL,
                                                  `state` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_013",
                            Description = "Create tbl_payment_voucher_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_payment_voucher_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `payment_id` int DEFAULT NULL,
                                                  `hum_id` int DEFAULT NULL,
                                                  `inv_id` int DEFAULT NULL,
                                                  `inv_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `total` decimal(20,6) DEFAULT NULL,
                                                  `payment` decimal(20,6) DEFAULT NULL,
                                                  `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `voucher_type` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '',
                                                  `cost_center_id` int DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                        `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,

                                                  PRIMARY KEY (`id`)

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_014",
                            Description = "Create tbl_payment_voucher",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_payment_voucher` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `method` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `amount` decimal(20,4) DEFAULT NULL,
                                                  `debit_account_id` int DEFAULT NULL,
                                                  `debit_cost_center_id` int DEFAULT NULL,
                                                  `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `credit_account_id` int DEFAULT NULL,
                                                  `credit_cost_center_id` int DEFAULT NULL,
                                                  `bank_id` int DEFAULT NULL,
                                                  `bank_account_id` int DEFAULT NULL,
                                                  `book_no` int DEFAULT NULL,
                                                  `check_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `check_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `check_date` date DEFAULT NULL,
                                                  `trans_date` date DEFAULT NULL,
                                                  `trans_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `trans_ref` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `modified_by` int DEFAULT NULL,
                                                  `modified_date` date DEFAULT NULL,
                                                  `state` int DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,`GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,

                                                  PRIMARY KEY (`id`)

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_015",
                            Description = "Create tbl_advance_payment_voucher_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_advance_payment_voucher_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `payment_id` int NOT NULL DEFAULT '0',
                                                  `name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `bank_name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `check_name` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `check_no` int DEFAULT NULL,
                                                  `check_date` date DEFAULT NULL,
                                                  `bank_account_name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `book_no` int DEFAULT NULL,
                                                  `trans_date` date DEFAULT NULL,
                                                  `trans_name` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `trans_ref` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `description` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `amount` decimal(20,6) DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,

                            -- Sync columns (IMPORTANT)
                            `GlobalId` VARCHAR(36) NOT NULL,
                            `DeviceId` VARCHAR(50) DEFAULT NULL,
                            `LastModified` DATETIME DEFAULT CURRENT_TIMESTAMP,
                            `SyncStatus` TINYINT(1) NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_016",
                            Description = "Create tbl_advance_payment_voucher",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_advance_payment_voucher` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `pv_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `method` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `amount` decimal(20,4) DEFAULT NULL,
                                                  `debit_account_id` int DEFAULT NULL,
                                                  `debit_cost_center_id` int DEFAULT NULL,
                                                  `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `credit_account_id` int DEFAULT NULL,
                                                  `credit_cost_center_id` int DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `modified_by` int DEFAULT NULL,
                                                  `modified_date` date DEFAULT NULL,
                                                  `state` int DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                                                  `DeviceId` varchar(50) DEFAULT '',
                                                  `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                                                  `SyncStatus` tinyint NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_017",
                            Description = "Create tbl_petty_cash_card",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_petty_cash_card` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `name` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `account_id` int DEFAULT NULL,
                                                  `mobile` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `whatsapp_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `email` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_018",
                            Description = "Create tbl_petty_cash_submition_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_petty_cash_submition_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `petty_id` int NOT NULL DEFAULT '0',
                                                  `date` date DEFAULT NULL,
                                                  `account_id` int DEFAULT NULL,
                                                  `category` int DEFAULT NULL,
                                                  `cost_center_id` int DEFAULT NULL,
                                                  `amount` decimal(20,6) DEFAULT NULL,
                                                  `vat` decimal(20,6) DEFAULT NULL,
                                                  `total` decimal(20,6) DEFAULT NULL,
                                                  `note` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `state` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_019",
                            Description = "Create tbl_petty_cash_submition",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_petty_cash_submition` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `amount` decimal(20,6) DEFAULT NULL,
                                                  `total_before_vat` decimal(20,6) DEFAULT NULL,
                                                  `total_vat` decimal(20,6) DEFAULT NULL,
                                                  `net_amount` decimal(20,6) DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_020",
                            Description = "Create tbl_petty_cash_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS tbl_petty_cash_details(
                                                    id int NOT NULL AUTO_INCREMENT,
                                                    petty_cash_id INT NOT NULL,
                                                    entry_date DATE NOT NULL,
                                                    ref_id VARCHAR(100) NULL,
                                                    hum_id INT NOT NULL,
                                                    hum_name VARCHAR(300) NOT NULL DEFAULT '',
                                                    category INT(10) NOT NULL DEFAULT '0',
                                                    cost_center_id INT NOT NULL,
                                                    description TEXT NULL,
                                                    amount DECIMAL(18,2) NOT NULL DEFAULT 0,
                                                    vatP DECIMAL(18,2) NOT NULL DEFAULT 0,
                                                    vat int DEFAULT NULL,
                                                    total DECIMAL(18,2) NOT NULL DEFAULT 0,
                                                    project_id INT(10) NOT NULL DEFAULT '0',
                                                    note TEXT NULL,
                                                    PRIMARY KEY (`id`)
                                                );
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_021",
                            Description = "Create tbl_petty_cash",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS tbl_petty_cash(
                                                    id int NOT NULL AUTO_INCREMENT,
                                                    code VARCHAR(50) NOT NULL,
                                                    voucher_date DATE NOT NULL,
                                                    cash_account_id INT NOT NULL,
                                                    employee_id INT NOT NULL,
                                                    amount decimal(20,6) DEFAULT 0,
                                                    vat decimal(20,6) DEFAULT 0,
                                                    total decimal(20,6) DEFAULT 0,
                                                    notes TEXT NULL,
                                                    created_by INT NULL,
                                                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                                    project_id INT NOT NULL DEFAULT '0',
                                                    status INT NOT NULL DEFAULT '0',
                                                    PRIMARY KEY (`id`)
                                                );
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_023",
                            Description = "Create tbl_petty_cash_request",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_petty_cash_request` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `request_date` date DEFAULT NULL,
                                                  `request_ref` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Petty_cash_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `amount` decimal(20,6) DEFAULT NULL,
                                                  `description` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `debit_account_id` int DEFAULT NULL,
                                                  `credit_account_id` int DEFAULT NULL,
                                                  `approved_date` date DEFAULT NULL,
                                                  `state` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `pay` decimal(20,6) DEFAULT NULL,
                                                  `change` decimal(20,6) DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_024",
                            Description = "Create tbl_city",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_city` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `country_id` int DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_025",
                            Description = "Create tbl_salary_adjustments",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_salary_adjustments` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `name` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `adjustment_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `description` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `amount` decimal(10,6) DEFAULT NULL,
                                                  `date` date DEFAULT NULL,
                                                  `ref_id` int NOT NULL DEFAULT '0',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_026",
                            Description = "Create tbl_attendance_salary",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_attendance_salary` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `emp_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `absence_days` int DEFAULT NULL,
                                                  `total_absence` decimal(20,6) DEFAULT NULL,
                                                  `delay_minutes` decimal(20,6) DEFAULT NULL,
                                                  `total_delay` decimal(20,6) DEFAULT NULL,
                                                  `total_loan` decimal(20,6) DEFAULT NULL,
                                                  `net_salary` decimal(20,6) DEFAULT NULL,
                                                  `pay` decimal(20,6) DEFAULT NULL,
                                                  `change` decimal(20,6) DEFAULT NULL,
                                                  `ss_no` int DEFAULT '0',
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_027",
                            Description = "Create tbl_attendancesheet",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_attendancesheet` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `attendance_salary_id` int NOT NULL DEFAULT '0',
                                                  `code` int NOT NULL,
                                                  `WorkDate` date NOT NULL,
                                                  `TimeIn` time DEFAULT NULL,
                                                  `TimeOut` time DEFAULT NULL,
                                                  `DayOfWeek` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Reference` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Ref_Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_028",
                            Description = "Create tbl_end_of_service",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_end_of_service` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `code` int DEFAULT NULL,
                                                  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Reference` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `description` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `debit` decimal(20,6) DEFAULT 0,
                                                  `leave_days` decimal(20,6) DEFAULT 0,
                                                  `credit` decimal(20,6) DEFAULT 0,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_029",
                            Description = "Create tbl_leave_salary",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_leave_salary` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `code` int DEFAULT NULL,
                                                  `name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Reference` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `debit` decimal(20,6) DEFAULT '0.000000',
                                                  `leave_days` decimal(20,6) DEFAULT NULL,
                                                  `credit` decimal(20,6) DEFAULT '0.000000',
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_030",
                            Description = "Create tbl_employee",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_employee` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` int NOT NULL DEFAULT '0',
                                                  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `city_id` int DEFAULT 0,
                                                  `address` varchar(350) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `phone` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `birth_day` date DEFAULT NULL,
                                                  `Social_Status` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Social_Insurance_Number` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `EmergencyName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `EmergencyAddress` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `EmergencyPhone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Relation` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `BasicSalary` decimal(10,2) DEFAULT 0,
                                                  `HousingAllowance` decimal(10,2) DEFAULT 0,
                                                  `TransportationAllowance` decimal(10,2) DEFAULT 0,
                                                  `Other` decimal(10,2) DEFAULT 0,
                                                  `bank_id` int DEFAULT 0,
                                                  `Iban_Number` varchar(70) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Bank_account_Number` varchar(70) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `EmiratesIDFileNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `EmiratesIDIssuingAuthority` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `EmiratesIDIssueDate` date DEFAULT NULL,
                                                  `EmiratesIDExpiryDate` date DEFAULT NULL,
                                                  `PassportNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `CountryOfIssue` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `PassportIssueDate` date DEFAULT NULL,
                                                  `PassportExpiryDate` date DEFAULT NULL,
                                                  `WorkContractNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `WorkContractType` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Position_id` int DEFAULT 0,
                                                  `Department_id` int DEFAULT 0,
                                                  `WorkDays` int DEFAULT 0,
                                                  `workinghours` int DEFAULT 0,
                                                  `ContractIssueDate` date DEFAULT NULL,
                                                  `ContractExpiryDate` date DEFAULT NULL,
                                                  `ResidencyFileNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `ResidencyIssuingAuthority` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `ResidencyIssueDate` date DEFAULT NULL,
                                                  `ResidencyExpiryDate` date DEFAULT NULL,
                                                  `account_id` int DEFAULT 0,
                                                  `Accrued_Salaries_id` int DEFAULT 0,
                                                  `Employee_Recivable_id` int DEFAULT 0,
                                                  `Acroal_Leave_Salary_id` int DEFAULT 0,
                                                  `Gratuit_id` int DEFAULT 0,
                                                  `Petty_Cash_id` int DEFAULT 0,
                                                  `active` int NOT NULL DEFAULT '0',
                                                  `state` int NOT NULL DEFAULT '0',
                                                  `sRole` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_031",
                            Description = "Create tbl_final_settlement",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_final_settlement` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Date` date DEFAULT NULL,
                                                  `emp_id` int DEFAULT NULL,
                                                  `DateCommencement` date DEFAULT NULL,
                                                  `DateLastWork` date DEFAULT NULL,
                                                  `TotalSalary` decimal(20,2) DEFAULT NULL,
                                                  `OtherAdditions` decimal(20,2) DEFAULT NULL,
                                                  `TotalAdditions` decimal(20,2) DEFAULT NULL,
                                                  `Payments` decimal(20,2) DEFAULT NULL,
                                                  `OtherDeductions` decimal(20,2) DEFAULT NULL,
                                                  `TotalDeductions` decimal(20,2) DEFAULT NULL,
                                                  `NetAccruals` decimal(20,2) DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_032",
                            Description = "Create tbl_loan",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_loan` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `LoanDate` date DEFAULT NULL,
                                                  `EmployeeID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `EmployeeName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `RequestAmount` decimal(20,6) DEFAULT NULL,
                                                  `Installments` int DEFAULT NULL,
                                                  `StartDate` date DEFAULT NULL,
                                                  `EndDate` date DEFAULT NULL,
                                                  `loanDates` date DEFAULT NULL,
                                                  `Months` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Amount` decimal(20,6) DEFAULT NULL,
                                                  `debit_account_id` int DEFAULT NULL,
                                                  `credit_account_id` int DEFAULT NULL,
                                                  `pay` decimal(20,6) DEFAULT '0.000000',
                                                  `change` decimal(20,6) DEFAULT '0.000000',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_033",
                            Description = "Create tbl_damage_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_damage_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `damage_id` int NOT NULL,
                                                  `item_id` int NOT NULL,
                                                  `qty` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `cost_price` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `total` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_034",
                            Description = "Create tbl_items",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_items` (
	                                            `id` INT(10) NOT NULL AUTO_INCREMENT,
	                                            `code` VARCHAR(50) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_general_ci',
	                                            `type` VARCHAR(50) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_general_ci',
	                                            `warehouse_id` INT(10) NOT NULL DEFAULT '0',
	                                            `name` VARCHAR(500) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_general_ci',
	                                            `unit_id` INT(10) NOT NULL DEFAULT '0',
	                                            `barcode` VARCHAR(70) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_general_ci',
	                                            `cost_price` DECIMAL(20,4) NOT NULL DEFAULT '0.000',
	                                            `cogs_account_id` INT(10) NOT NULL DEFAULT '0',
	                                            `vendor_id` INT(10) NOT NULL DEFAULT '0',
	                                            `sales_price` DECIMAL(20,4) NOT NULL DEFAULT '0.000',
	                                            `tax_code_id` INT(10) NOT NULL DEFAULT '0',
	                                            `income_account_id` INT(10) NOT NULL DEFAULT '0',
	                                            `asset_account_id` INT(10) NOT NULL DEFAULT '0',
	                                            `min_amount` DECIMAL(20,4) NOT NULL DEFAULT '0.000',
	                                            `max_amount` DECIMAL(20,4) NOT NULL DEFAULT '0.000',
	                                            `on_hand` DECIMAL(20,4) NOT NULL DEFAULT '0.000',
	                                            `total_value` DECIMAL(20,6) NOT NULL DEFAULT '0.000000',
	                                            `date` DATE NOT NULL,
	                                            `img` BLOB NULL DEFAULT NULL,
                                                `ItemImg` LONGBLOB NULL DEFAULT NULL,
	                                            `active` INT(10) NOT NULL DEFAULT '0',
	                                            `method` VARCHAR(50) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_general_ci',
	                                            `state` INT(10) NOT NULL DEFAULT '0',
	                                            `created_By` INT(10) NOT NULL DEFAULT '0',
	                                            `created_date` DATE NOT NULL,
	                                            `deleted_by` INT(10) NOT NULL DEFAULT '0',
	                                            `category_id` INT(10) NULL DEFAULT NULL,
	                                            `posItem` INT(10) NULL DEFAULT NULL,
                                                `item_type` VARCHAR(50) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_general_ci',
                        `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,
	                                            PRIMARY KEY (`id`) USING BTREE
                                            )
                                            COLLATE='utf8mb4_general_ci' ENGINE=InnoDB ;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_035",
                            Description = "Create tbl_item_category",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_item_category` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '0',
                                                  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_036",
                            Description = "Create tbl_unit",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_unit` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_037",
                            Description = "Create tbl_warehouse",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_warehouse` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `emp_id` int DEFAULT NULL,
                                                  `city` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `building_name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `account_id` int DEFAULT NULL,
                                                  `state` int DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_042",
                            Description = "Create tbl_item_warehouse_transaction",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_item_warehouse_transaction` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `warehouse_from` int DEFAULT NULL,
                                                  `warehouse_to` int DEFAULT NULL,
                                                  `item_id` int DEFAULT NULL,
                                                  `qty` decimal(20,6) DEFAULT NULL,
                                                  `description` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_043",
                            Description = "Create tbl_items_warehouse",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_items_warehouse` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `warehouse_id` int DEFAULT NULL,
                                                  `item_id` int DEFAULT NULL,
                                                  `qty` decimal(20,6) DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_044",
                            Description = "Create tbl_item_stock_settlement_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_item_stock_settlement_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `settle_id` int DEFAULT NULL,
                                                  `item_id` int DEFAULT NULL,
                                                  `on_hand` decimal(20,4) DEFAULT NULL,
                                                  `price` decimal(20,4) DEFAULT NULL,
                                                  `new_on_hand` decimal(20,4) DEFAULT NULL,
                                                  `qty` decimal(20,4) DEFAULT NULL,
                                                  `minusamount` decimal(20,4) DEFAULT NULL,
                                                  `plusamount` decimal(20,4) DEFAULT NULL,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_045",
                            Description = "Create tbl_item_stock_settlement",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_item_stock_settlement` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `date` date DEFAULT NULL,
                                                  `warehouse_id` int DEFAULT NULL,
                                                  `total_plus` decimal(20,6) DEFAULT NULL,
                                                  `total_minus` decimal(20,6) DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `modified_by` int DEFAULT NULL,
                                                  `modified_date` date DEFAULT NULL,
                                                  `state` int DEFAULT '0',
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_046",
                            Description = "Create tbl_item_transaction",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_item_transaction` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `reference` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `item_id` int DEFAULT NULL,
                                                  `cost_price` decimal(20,8) DEFAULT NULL,
                                                  `qty_in` decimal(20,4) DEFAULT NULL,
                                                  `sales_price` decimal(20,4) DEFAULT NULL,
                                                  `qty_out` decimal(20,4) DEFAULT NULL,
                                                  `qty_inc` decimal(20,4) DEFAULT NULL,
                                                  `description` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `warehouse_id` INT NOT NULL DEFAULT 0,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                        `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,

                                                  PRIMARY KEY (`id`)

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_047",
                            Description = "Create tbl_stock_reconciliation",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS tbl_stock_reconciliation
                                                (
                                                    reconciliation_id INT AUTO_INCREMENT PRIMARY KEY,
                                                    reconciliation_no VARCHAR(30) NOT NULL,
                                                    reconciliation_date DATE NOT NULL,
                                                    warehouse_id INT NOT NULL,
                                                    project_id INT NULL,
                                                    reconciliation_type VARCHAR(30) DEFAULT 'Periodic',
                                                    checked_by INT NULL,
                                                    approved_by INT NULL,
                                                    status VARCHAR(30) DEFAULT 'Draft',
                                                    remarks VARCHAR(500),
                                                    created_by INT,
                                                    created_date DATETIME DEFAULT CURRENT_TIMESTAMP
                                                );
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_048",
                            Description = "Create tbl_stock_reconciliation_item",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS tbl_stock_reconciliation_item
                                                (
                                                    id INT AUTO_INCREMENT PRIMARY KEY,
                                                    reconciliation_id INT NOT NULL,
                                                    item_id INT NOT NULL,
                                                    system_qty DECIMAL(18,4) DEFAULT 0,
                                                    physical_qty DECIMAL(18,4) DEFAULT 0,
                                                    variance_qty DECIMAL(18,4) DEFAULT 0,
                                                    adjustment_type VARCHAR(20),
                                                    uom VARCHAR(30),
                                                    unit_cost DECIMAL(18,4) DEFAULT 0,
                                                    variance_value DECIMAL(18,4) DEFAULT 0,
                                                    remarks VARCHAR(250)
                                                );
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_049",
                            Description = "Create tbl_tax",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_tax` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `value` int DEFAULT NULL,
                                                  `description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `state` int DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_050",
                            Description = "Create tbl_purchase_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_purchase_details` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `purchase_id` int DEFAULT NULL,
                                                  `item_id` int DEFAULT NULL,
                                                  `qty` decimal(20,4) DEFAULT NULL,
                                                  `cost_price` decimal(20,4) DEFAULT NULL,
                                                  `price` decimal(20,4) DEFAULT NULL,
                                                  `vatp` decimal(20,4) DEFAULT '0.000',
                                                  `vat` int DEFAULT NULL,
                                                  `total` decimal(20,4) DEFAULT NULL,
                                                  `discount` decimal(20,4) DEFAULT NULL,
                                                  `cost_center_id` int DEFAULT NULL,
                                                  `project_id` INT NOT NULL DEFAULT 0,`GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_051",
                            Description = "Create tbl_purchase",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_purchase` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date NOT NULL,
                                                  `vendor_id` int NOT NULL DEFAULT '0',
                                                  `invoice_id` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `warehouse_id` int NOT NULL DEFAULT '0',
                                                  `po_num` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `bill_to` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `city` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `sales_man` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
                                                  `ship_date` date DEFAULT NULL,
                                                  `ship_via` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `ship_to` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `payment_method` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `account_cash_id` int NOT NULL DEFAULT '0',
                                                  `payment_terms` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
                                                  `payment_date` date NOT NULL,
                                                  `total` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `vat` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `net` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `pay` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `change` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `retention_percentage` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `retention_amount` decimal(20,4) NOT NULL DEFAULT '0.000',
                                                  `created_by` int NOT NULL DEFAULT '0',
                                                  `created_date` date NOT NULL,
                                                  `modified_by` int DEFAULT NULL,
                                                  `modified_date` date DEFAULT NULL,
                                                  `state` int NOT NULL DEFAULT '0',
                                                  `purchase_type` VARCHAR(50) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_general_ci',
                                                  `fixed_asset_category_id` int NOT NULL DEFAULT '0',
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  `description` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '',
                                                  `reference_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '',
                        `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,

                                                  PRIMARY KEY (`id`)

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_052",
                            Description = "Create tbl_vendor",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_vendor` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `code` int NOT NULL DEFAULT '0',
                                                  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `Cat_id` int DEFAULT NULL,
                                                  `Balance` decimal(20,4) DEFAULT NULL,
                                                  `date` date DEFAULT NULL,
                                                  `main_phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `work_phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `mobile` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `email` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `ccemail` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `website` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `country` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `city` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `region` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `building_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `account_id` int DEFAULT NULL,
                                                  `trn` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `facilty_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `active` int DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `state` int DEFAULT NULL,
                                                  `type` VARCHAR(150) NULL DEFAULT 'Vendor' COLLATE 'utf8mb4_general_ci',
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  `project_site` VARCHAR(150) DEFAULT '' COLLATE 'utf8mb4_general_ci',
                        `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,

                                                  PRIMARY KEY (`id`)

                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_053",
                            Description = "Create tbl_cost_center_transaction",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_cost_center_transaction` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `type` varchar(50) COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
                                                  `date` datetime DEFAULT NULL,
                                                  `ref_id` int DEFAULT '0',
                                                  `debit` decimal(20,4) DEFAULT '0.000',
                                                  `credit` decimal(20,4) DEFAULT '0.000',
                                                  `description` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '',
                                                  `cost_center_id` int DEFAULT '0',
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_054",
                            Description = "Create tbl_transaction",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_transaction` (
                                                  `id` int NOT NULL AUTO_INCREMENT,
                                                  `date` date DEFAULT NULL,
                                                  `account_id` int DEFAULT NULL,
                                                  `debit` decimal(20,4) DEFAULT NULL,
                                                  `credit` decimal(20,4) DEFAULT NULL,
                                                  `transaction_id` INT NOT NULL DEFAULT 0,
                                                  `hum_id` int DEFAULT '0',
                                                  `type` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `t_type` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
                                                  `created_by` int DEFAULT NULL,
                                                  `created_date` date DEFAULT NULL,
                                                  `modified_by` int DEFAULT NULL,
                                                  `modified_date` date DEFAULT NULL,
                                                  `state` int DEFAULT NULL,
                                                  `voucher_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '',
                        `GlobalId` varchar(36) NOT NULL DEFAULT '0',
                          `DeviceId` varchar(50) DEFAULT NULL,
                          `LastModified` datetime DEFAULT CURRENT_TIMESTAMP,
                          `SyncStatus` tinyint NOT NULL DEFAULT 0,
                                                  `project_id` INT NOT NULL DEFAULT 0,
                                                  PRIMARY KEY (`id`)
                                                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_055",
                            Description = "Create tbl_item_card_details",
                            Sql = @"
                        CREATE TABLE IF NOT EXISTS `tbl_item_card_details` (
	                                                `id` INT(10) NOT NULL AUTO_INCREMENT,
	                                                `itemId` INT(10) NOT NULL DEFAULT '0',
	                                                `date` DATE NULL DEFAULT NULL,
	                                                `wharehouse_id` INT(10) NOT NULL DEFAULT '0',
	                                                `inv_no` VARCHAR(50) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	                                                `trans_no` INT(10) NOT NULL DEFAULT '0',
	                                                `trans_type` VARCHAR(50) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	                                                `description` VARCHAR(500) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	                                                `price` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `qty_in` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `qty_out` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `qty_balance` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `debit` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `credit` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `balance` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `fifo_qty` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                `fifo_cost` DECIMAL(20,4) NOT NULL DEFAULT '0.0000',
	                                                PRIMARY KEY (`id`) USING BTREE
                                                )
                                                COLLATE='utf8mb4_general_ci'
                                                ENGINE=InnoDB
                                                ;
                            "
                        },
                        // Optional reference seed migrations. Run only after schema migrations.
                        // tbl_city has no unique (country_id,name) constraint; do not rerun under a new version.

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_SEED_001",
                            Description = "Seed tbl_coa_level_1",
                            Sql = @"
                        INSERT IGNORE INTO `tbl_coa_level_1` (`id`, `name`, `code`, `category_code`) VALUES
	                                                (1, 'Assets', 1,'ASSET'),
	                                                (2, 'Liabilities', 2,'LIABILITY'),
	                                                (3, 'Equity', 3,'EQUITY'),
	                                                (4, 'Income', 4,'INCOME'),
	                                                (5, 'Cost', 5,'COST'),
	                                                (6, 'General & Direct Expenses', 6,'EXPENSE');
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_SEED_002",
                            Description = "Seed tbl_coa_level_2",
                            Sql = @"
                        INSERT IGNORE INTO `tbl_coa_level_2` (`id`, `name`, `code`, `main_id`,`SyncStatus`) VALUES
	                                                (1, 'Current Assets', 11, 1, 1),
	                                                (2, 'Long-Term Assets', 12, 1, 1),
	                                                (3, 'Current Liabilities', 21, 2, 1),
	                                                (4, 'Long-Term Liabilities', 22, 2,1),
	                                                (5, 'Equity & Capital', 31, 3,1),
	                                                (6, 'Direct Income', 41, 4,1),
	                                                (7, 'Indirect Income', 42, 4,1),
	                                                (8, 'Cost', 51, 5,1),
	                                                (9, 'Direct Expenses', 61, 6,1),
	                                                (10, 'Operating Expenses', 62, 6,1),
	                                                (11, 'Inventory Adjustments', 63, 6,1),
	                                                (12, 'Other Income', 465, 4,1);
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_SEED_003",
                            Description = "Seed tbl_coa_level_3",
                            Sql = @"
                        INSERT IGNORE INTO `tbl_coa_level_3` (`id`, `name`, `code`, `main_id`,`SyncStatus`) VALUES
	                                                (1, 'Banks', 1101, 1,1),
	                                                (2, 'Petty Cash', 1102, 1,1),
	                                                (3, 'Accounts Receivable', 1103, 1,1),
	                                                (4, 'Inventory', 1104, 1,1),
	                                                (5, 'Prepayment', 1105, 1,1),
	                                                (6, 'Other\'s Receivable', 1106, 1,1),
	                                                (7, 'Related parties Receivable', 1107, 1,1),
	                                                (8, 'Fixed Assets', 1201, 2,1),
	                                                (9, 'Suppliers', 2101, 3,1),
	                                                (10, 'Accrued Employee benefits Expense', 2102, 3,1),
	                                                (11, 'Other Current Liabilities', 2103, 3,1),
	                                                (12, 'Long-term employee benefits', 2201, 4,1),
	                                                (13, 'Capital', 3101, 5,1),
	                                                (14, 'Members Draw', 3102, 5,1),
	                                                (15, 'Opening Balance Equity', 3201, 5,1),
	                                                (16, 'Reserve', 3301, 5,1),
	                                                (17, 'Retained earnings', 3401, 5,1),
	                                                (18, 'Sales', 4101, 6,1),
	                                                (19, 'Sales Return', 4102, 6,1),
	                                                (20, 'Maintenance Income', 4201, 7,1),
	                                                (21, 'Cost of Goods Sold', 5101, 8,1),
	                                                (22, 'Freight Shipping Cost', 5102, 8,1),
	                                                (23, 'Customs & Clearance', 5103, 8,1),
	                                                (24, 'Other Cost', 5104, 8,1),
	                                                (25, 'General Expenses', 6101, 9,1),
                                                    (26, 'Payroll Liabilities', 2104, 3,1),
	                                                (27, 'Employee Receivable', 1108, 1,1),
	                                                (28, 'Short Term Investment', 1109, 1,1),
	                                                (29, 'Other Payable', 2105, 3,1),
	                                                (30, 'Cash & Cash Equivalents', 1110, 1,1),
	                                                (31, 'Losses', 6201, 16,1),
	                                                (32, 'Stock Write-offs', 6387, 17,1),
	                                                (33, 'Stock Adjustments', 46501, 18,1),
	                                                (34, 'Accounts Payable', 1111, 1,1),
	                                                (35, 'Other Income', 46502, 12,1);
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_SEED_004",
                            Description = "Seed tbl_coa_level_4",
                            Sql = @"
                        INSERT IGNORE INTO `tbl_coa_level_4` (`id`, `name`, `code`, `main_id`, `debit`, `credit`, `date`,`SyncStatus`) VALUES
	                                                (1, 'Bank Account', 1101001, 1, 0.000, 0.000, NULL,1),
	                                                (2, 'Petty Cash', 1102001, 2, 0.000, 0.000, NULL,1),
	                                                (3, 'Accounts Receivable', 1103001, 3, 0.000, 0.000, NULL,1),
	                                                (4, 'Raw Material Inventory', 1104001, 4, 0.000, 0.000, NULL,1),
	                                                (5, 'Finish Goods Inventory', 1104002, 4, 0.000, 0.000, NULL,1),
	                                                (6, 'Prepayment Government', 1105001, 5, 0.000, 0.000, NULL,1),
	                                                (7, 'Prepayment Rent', 1105002, 5, 0.000, 0.000, NULL,1),
	                                                (8, 'Prepayment Other\'s', 1105003, 5, 0.000, 0.000, NULL,1),
	                                                (9, 'Deposits', 1106001, 6, 0.000, 0.000, NULL,1),
	                                                (10, 'Taxes Paid (Input)', 1106002, 6, 0.000, 0.000, NULL,1),
	                                                (11, 'Advance payments to employees', 1106003, 6, 0.000, 0.000, NULL,1),
	                                                (12, 'Fixed Assets Net', 1201001, 8, 0.000, 0.000, NULL,1),
	                                                (13, 'Suppliers (Creditors)  Net', 2101001, 9, 0.000, 0.000, NULL,1),
	                                                (14, 'Accrued Salaries', 2102001, 10, 0.000, 0.000, NULL,1),
	                                                (15, 'Accrued Sales commission', 2102002, 10, 0.000, 0.000, NULL,1),
	                                                (16, 'Accrued Leave Salary', 2102003, 10, 0.000, 0.000, NULL,1),
	                                                (17, 'Sales Tax Payable', 2103001, 11, 0.000, 0.000, NULL,1),
	                                                (18, 'PDC CHQ', 2103002, 11, 0.000, 0.000, NULL,1),
	                                                (19, 'Advance from Customers', 2103004, 11, 0.000, 0.000, NULL,1),
	                                                (20, 'Gratuity Provision', 2201001, 12, 0.000, 0.000, NULL,1),
	                                                (21, 'Capital - Opening Balance', 3101001, 13, 0.000, 0.000, NULL,1),
	                                                (22, 'Opening Balance Equity', 3201001, 15, 0.000, 0.000, NULL,1),
	                                                (23, 'Sales - Local', 4101001, 18, 0.000, 0.000, NULL,1),
	                                                (24, 'Sales - GCC', 4101002, 18, 0.000, 0.000, NULL,1),
	                                                (25, 'Materials Purchased', 5101001, 21, 0.000, 0.000, NULL,1),
	                                                (26, 'Employee Salary Advance', 1108001, 27, 0.000, 0.000, NULL,1),
	                                                (27, 'Employee Loans', 1108002, 27, 0.000, 0.000, NULL,1),
	                                                (28, 'Other Employee Receivables', 1101002, 27, 0.000, 0.000, NULL,1),
	                                                (29, 'Accrued Salary', 2102004, 10, 0.000, 0.000, NULL,1),
	                                                (30, 'Employee Payables', 2105001, 29, 0.000, 0.000, NULL,1),
	                                                (31, 'Stock Settlement Gain', 46501001, 35, 0.000, 0.000, NULL,1),
	                                                (32, 'Accumulated Dep Fur', 1201002, 25, 0.000, 0.000, NULL,1),
	                                                (33, 'PDC Receivable', 1103002, 3, 0.000, 0.000, NULL,1),
	                                                (34, 'PDC Receivable Return', 1103003, 3, 0.000, 0.000, NULL,1),
	                                                (35, 'PDC Receivable Hold', 1103004, 3, 0.000, 0.000, NULL,1),
	                                                (36, 'PDC Receivable Cancel', 1103005, 3, 0.000, 0.000, NULL,1),
	                                                (37, 'PDC Payable', 1111001, 34, 0.000, 0.000, NULL,1),
	                                                (38, 'PDC Payable Return', 1111002, 34, 0.000, 0.000, NULL,1),
	                                                (39, 'PDC Payable Hold', 1111003, 34, 0.000, 0.000, NULL,1),
	                                                (40, 'PDC Payable Cancel', 1111003, 34, 0.000, 0.000, NULL,1),
	                                                (41, 'Sales Discount', 6101004, 25, 0.000, 0.000, NULL,1),
	                                                (42, 'Cash On Hand', 1103006, 3, 0.000, 0.000, NULL,1),
	                                                (43, 'Sales Return', 4101003, 19, 0.000, 0.000, NULL,1),
	                                                (44, 'Customer Advances', 2105002, 29, 0.000, 0.000, NULL,1),
	                                                (45, 'Retention Receivable', 1103007, 3, 0.000, 0.000, NULL,1),
	                                                (46, 'Retention Payable', 2105003, 29, 0.000, 0.000, NULL,1);
                            "
                        },

                        new DatabaseMigration
                        {
                            Version = "20261019_ERP_SEED_005",
                            Description = "Seed tbl_city",
                            Sql = @"
                        INSERT INTO `tbl_city` ( `country_id`, `name`) VALUES
                                                    (1,'Kabul'),
                                                    (2,'Tirana'),
                                                    (3,'Algiers'),
                                                    (4,'Andorra la Vella'),
                                                    (5,'Luanda'),
                                                    (6,'Saint John''s'),
                                                    (7,'Buenos Aires'),
                                                    (8,'Yerevan'),
                                                    (9,'Canberra'),
                                                    (10,'Vienna'),
                                                    (11,'Baku'),
                                                    (12,'Nassau'),
                                                    (13,'Manama'),
                                                    (14,'Dhaka'),
                                                    (15,'Bridgetown'),
                                                    (16,'Minsk'),
                                                    (17,'Brussels'),
                                                    (18,'Belmopan'),
                                                    (19,'Porto-Novo'),
                                                    (20,'Thimphu'),
                                                    (21,'Sucre'),
                                                    (22,'Sarajevo'),
                                                    (23,'Gaborone'),
                                                    (24,'Brasilia'),
                                                    (25,'Bandar Seri Begawan'),
                                                    (26,'Sofia'),
                                                    (27,'Ouagadougou'),
                                                    (28,'Gitega'),
                                                    (29,'Praia'),
                                                    (30,'Phnom Penh'),
                                                    (31,'Yaounde'),
                                                    (32,'Ottawa'),
                                                    (33,'Bangui'),
                                                    (34,'N''Djamena'),
                                                    (35,'Santiago'),
                                                    (36,'Beijing'),
                                                    (37,'Bogota'),
                                                    (38,'Moroni'),
                                                    (39,'Brazzaville'),
                                                    (40,'San Jose'),
                                                    (41,'Zagreb'),
                                                    (42,'Havana'),
                                                    (43,'Nicosia'),
                                                    (44,'Prague'),
                                                    (45,'Copenhagen'),
                                                    (46,'Djibouti'),
                                                    (47,'Roseau'),
                                                    (48,'Santo Domingo'),
                                                    (49,'Quito'),
                                                    (50,'Cairo'),
                                                    (51,'San Salvador'),
                                                    (52,'Malabo'),
                                                    (53,'Asmara'),
                                                    (54,'Tallinn'),
                                                    (55,'Mbabane'),
                                                    (56,'Addis Ababa'),
                                                    (57,'Suva'),
                                                    (58,'Helsinki'),
                                                    (59,'Paris'),
                                                    (60,'Libreville'),
                                                    (61,'Banjul'),
                                                    (62,'Tbilisi'),
                                                    (63,'Berlin'),
                                                    (64,'Accra'),
                                                    (65,'Athens'),
                                                    (66,'St George''s'),
                                                    (67,'Guatemala City'),
                                                    (68,'Conakry'),
                                                    (69,'Bissau'),
                                                    (70,'Georgetown'),
                                                    (71,'Port-au-Prince'),
                                                    (72,'Tegucigalpa'),
                                                    (73,'Budapest'),
                                                    (74,'Reykjavik'),
                                                    (75,'New Delhi'),
                                                    (76,'Jakarta'),
                                                    (77,'Tehran'),
                                                    (78,'Baghdad'),
                                                    (79,'Dublin'),
                                                    (80,'Jerusalem'),
                                                    (81,'Rome'),
                                                    (82,'Kingston'),
                                                    (83,'Tokyo'),
                                                    (84,'Amman'),
                                                    (85,'Astana'),
                                                    (86,'Nairobi'),
                                                    (87,'Tarawa'),
                                                    (88,'Kuwait City'),
                                                    (89,'Bishkek'),
                                                    (90,'Vientiane'),
                                                    (91,'Riga'),
                                                    (92,'Beirut'),
                                                    (93,'Maseru'),
                                                    (94,'Monrovia'),
                                                    (95,'Tripoli'),
                                                    (96,'Vaduz'),
                                                    (97,'Vilnius'),
                                                    (98,'Luxembourg'),
                                                    (99,'Antananarivo'),
                                                    (100,'Lilongwe'),
                                                    (101,'Kuala Lumpur'),
                                                    (102,'Male'),
                                                    (103,'Bamako'),
                                                    (104,'Valletta'),
                                                    (105,'Majuro'),
                                                    (106,'Nouakchott'),
                                                    (107,'Port Louis'),
                                                    (108,'Mexico City'),
                                                    (109,'Palikir'),
                                                    (110,'Chișinău'),
                                                    (111,'Monaco'),
                                                    (112,'Ulaanbaatar'),
                                                    (113,'Podgorica'),
                                                    (114,'Rabat'),
                                                    (115,'Maputo'),
                                                    (116,'Nay Pyi Taw'),
                                                    (117,'Windhoek'),
                                                    (118,'Nauru'),
                                                    (119,'Kathmandu'),
                                                    (120,'Amsterdam'),
                                                    (121,'Wellington'),
                                                    (122,'Managua'),
                                                    (123,'Niamey'),
                                                    (124,'Abuja'),
                                                    (125,'Pyongyang'),
                                                    (126,'Skopje'),
                                                    (127,'Oslo'),
                                                    (128,'Muscat'),
                                                    (129,'Islamabad'),
                                                    (130,'Ngerulmud'),
                                                    (131,'Jerusalem'),
                                                    (132,'Panama City'),
                                                    (133,'Port Moresby'),
                                                    (134,'Asuncion'),
                                                    (135,'Lima'),
                                                    (136,'Manila'),
                                                    (137,'Warsaw'),
                                                    (138,'Lisbon'),
                                                    (139,'Doha'),
                                                    (140,'Bucharest'),
                                                    (141,'Moscow'),
                                                    (142,'Kigali'),
                                                    (143,'Basseterre'),
                                                    (144,'Castries'),
                                                    (145,'Kingstown'),
                                                    (146,'Apia'),
                                                    (147,'San Marino'),
                                                    (148,'Sao Tome'),
                                                    (149,'Riyadh'),
                                                    (150,'Dakar'),
                                                    (151,'Belgrade'),
                                                    (152,'Victoria'),
                                                    (153,'Freetown'),
                                                    (154,'Singapore'),
                                                    (155,'Bratislava'),
                                                    (156,'Ljubljana'),
                                                    (157,'Honiara'),
                                                    (158,'Mogadishu'),
                                                    (159,'Pretoria'),
                                                    (160,'Seoul'),
                                                    (161,'Juba'),
                                                    (162,'Madrid'),
                                                    (163,'Sri Jayawardenapura Kotte'),
                                                    (164,'Khartoum'),
                                                    (165,'Paramaribo'),
                                                    (166,'Stockholm'),
                                                    (167,'Bern'),
                                                    (168,'Damascus'),
                                                    (169,'Dushanbe'),
                                                    (170,'Dodoma'),
                                                    (171,'Bangkok'),
                                                    (172,'Taipei'),
                                                    (173,'Lome'),
                                                    (174,'Nuku''alofa'),
                                                    (175,'Port of Spain'),
                                                    (176,'Tunis'),
                                                    (177,'Ankara'),
                                                    (178,'Ashgabat'),
                                                    (179,'Funafuti'),
                                                    (180,'Kampala'),
                                                    (181,'Kyiv or Kiev'),
	                                                (182,'Abu Dhabi'),
	                                                (182,'Dubai'),
	                                                (182,'Sharjah'),
	                                                (182,'Ajman'),
	                                                (182,'Umm Al-Quwain'),
	                                                (182,'Ras Al Khaimah'),
	                                                (182,'Fujairah'),
                                                    (183,'London'),
                                                    (184,'Washington D.C.'),
                                                    (185,'Montevideo'),
                                                    (186,'Tashkent'),
                                                    (187,'Port Vila'),
                                                    (188,'Vatican City'),
                                                    (189,'Caracas'),
                                                    (190,'Hanoi'),
                                                    (191,'Sana''a'),
                                                    (192,'Lusaka'),
                                                    (193,'Harare');
                            "
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
                        //add column Diagnosis to Visits
                        new DatabaseMigration
                        {
                            Version = "20261016_00014",
                            Description = "Add Diagnosis column to visits",
                            Sql = @"
                                ALTER TABLE visits
                                ADD COLUMN Diagnosis VARCHAR(500) NOT NULL DEFAULT('');
                            "
                        },
                        //add col [VisitNotes] [nvarchar](max) NULL, to Visits
                        new DatabaseMigration
                        {
                            Version = "20261016_00015",
                            Description = "Add VisitNotes column to visits",
                            Sql = @"
                                ALTER TABLE visits
                                ADD COLUMN VisitNotes VARCHAR(500) NOT NULL DEFAULT('');
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
                        new DatabaseMigration
                        {
                            Version = "20261017_XRAY_001",
                            Description = "Create radiology test catalog",
                            Sql = @"CREATE TABLE IF NOT EXISTS xraytests (
                                XRayTestID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                                TestName VARCHAR(200) NOT NULL,
                                Description TEXT NULL,
                                Price DECIMAL(12,2) NOT NULL DEFAULT 0,
                                IsActive TINYINT(1) NOT NULL DEFAULT 1
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"
                        },
                        new DatabaseMigration
                        {
                            Version = "20261017_XRAY_002",
                            Description = "Create radiology requests with stored image paths",
                            Sql = @"CREATE TABLE IF NOT EXISTS xrayrequests (
                                XRayRequestID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                                VisitID INT NOT NULL,
                                XRayTestID INT NOT NULL,
                                RequestedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                Status VARCHAR(30) NOT NULL DEFAULT 'Ordered',
                                ClinicalNotes TEXT NULL,
                                Findings TEXT NULL,
                                Impression TEXT NULL,
                                ImagePath VARCHAR(1000) NULL,
                                ResultedAt DATETIME NULL,
                                INDEX IX_XRayRequests_Visit (VisitID),
                                INDEX IX_XRayRequests_Test (XRayTestID),
                                INDEX IX_XRayRequests_Status (Status),
                                CONSTRAINT FK_XRayRequests_Visit FOREIGN KEY (VisitID) REFERENCES visits(VisitID),
                                CONSTRAINT FK_XRayRequests_Test FOREIGN KEY (XRayTestID) REFERENCES xraytests(XRayTestID)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"
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

                        bool shouldExecute = true;

                        // BillNumber may already exist in older databases.
                        if (migration.Version == "20261016_021")
                        {
                            using (MySqlCommand checkCommand = new MySqlCommand(@"
            SELECT COUNT(*)
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = 'bills'
              AND COLUMN_NAME = 'BillNumber';",
                                connection))
                            {
                                long columnCount =
                                    Convert.ToInt64(checkCommand.ExecuteScalar());

                                if (columnCount > 0)
                                {
                                    shouldExecute = false;

                                    Debug.WriteLine(
                                        "BillNumber already exists. Skipping ALTER TABLE.");
                                }
                            }
                        }

                        if (shouldExecute)
                        {
                            using (MySqlCommand command =
                                new MySqlCommand(migration.Sql, connection))
                            {
                                command.ExecuteNonQuery();
                            }
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
