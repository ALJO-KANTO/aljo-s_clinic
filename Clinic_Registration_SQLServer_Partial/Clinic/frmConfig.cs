using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Clinic
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtServerName.Text.Trim()))
			{
				MessageBox.Show("Enter server / host name");
				return;
			}

			if (string.IsNullOrEmpty(txtDataBaseName.Text.Trim()))
			{
				MessageBox.Show("Enter database name");
				return;
			}

			if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
			{
				MessageBox.Show("Enter user name");
				return;
			}
			//if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
			//{
			//    MessageBox.Show("Enter password");
			//    return;
			//}
			string server = txtServerName.Text.Trim();
			string db = txtDataBaseName.Text.Trim();
			string user = txtUserName.Text.Trim();
			string pass = txtPassword.Text.Trim();

			string connStr = $"Server={server};Database={db};Uid={user};";
			if (!string.IsNullOrEmpty(pass))
			{
				connStr += $"Pwd={pass};";
			}
			connStr += "Max Pool Size=1000;charset=utf8mb4;";

			Environment.SetEnvironmentVariable("clinic_connection", CryptoHelper.Encrypt(connStr), EnvironmentVariableTarget.User);
			MessageBox.Show("Server connection saved. Please restart...", "Saved");

			createCompanyIfNot();

			//MigrateDatabaseIfNotExists();
		}

		private void MigrateDatabaseIfNotExists()
		{
			string server = txtServerName.Text.Trim();
            string db = "clinic_management";
            string user = txtUserName.Text.Trim();
            string pass = txtPassword.Text.Trim();

            string serverOnlyConnStr = $"Server={server};Uid={user};Pwd={pass};Max Pool Size=1000;";

            try
            {

                using (var conn = new MySqlConnection(serverOnlyConnStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS clinic_management CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // STEP 1: Connect to MySQL
                using (var connection = ClinicDatabase.GetConnection())
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine(
                        "Connected to MySQL: " +
                        connection.DataSource + " / " +
                        connection.Database);
                }


                ClinicDatabase.ApplyMigrations();

                System.Diagnostics.Debug.WriteLine(
                    "Database migrations completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database initialization failed:\n\n" +
                    ex.ToString(),
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
        }

        private void createCompanyIfNot()
        {
            string server = txtServerName.Text.Trim();
            string db = "clinic_management";
            string user = txtUserName.Text.Trim();
            string pass = txtPassword.Text.Trim();

            string serverOnlyConnStr = $"Server={server};Uid={user};Pwd={pass};Max Pool Size=1000;";

            try
            {
                using (var conn = new MySqlConnection(serverOnlyConnStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS clinic_management CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    conn.ChangeDatabase("clinic_management");

                    string createTable = @"
                        USE `clinic_management`;

						-- Dumping structure for table clinic_management.countries
						CREATE TABLE IF NOT EXISTS `countries` (
						  `CountryID` int NOT NULL AUTO_INCREMENT,
						  `CountryName` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
						  PRIMARY KEY (`CountryID`),
						  UNIQUE KEY `CountryName` (`CountryName`)
						) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

						-- Dumping data for table clinic_management.countries: ~24 rows (approximately)

						INSERT INTO `countries` (`CountryID`, `CountryName`) VALUES
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
							(193, 'Zimbabwe');

						-- Dumping structure for table clinic_management.days
						CREATE TABLE IF NOT EXISTS `days` (
						  `DayID` int NOT NULL,
						  `DayName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
						  PRIMARY KEY (`DayID`)
						) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

						-- Dumping data for table clinic_management.days: ~7 rows (approximately)
						INSERT INTO `days` (`DayID`, `DayName`) VALUES
							(1, 'Saturday'),
							(2, 'Sunday'),
							(3, 'Monday'),
							(4, 'Tuesday'),
							(5, 'Wednesday'),
							(6, 'Thursday'),
							(7, 'Friday');

						-- Dumping structure for table clinic_management.bloodtypes
						CREATE TABLE IF NOT EXISTS `bloodtypes` (
						  `BloodTypeID` int NOT NULL AUTO_INCREMENT,
						  `BloodTypeName` varchar(20) NOT NULL,
						  PRIMARY KEY (`BloodTypeID`)
						) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping data for table clinic_management.bloodtypes: ~8 rows (approximately)
						INSERT INTO `bloodtypes` (`BloodTypeID`, `BloodTypeName`) VALUES
							(1, 'O+'),
							(2, 'O-'),
							(3, 'A+'),
							(4, 'A-'),
							(5, 'B+'),
							(6, 'B-'),
							(7, 'AB+'),
							(8, 'AB-');

						-- Dumping structure for table clinic_management.roles
						CREATE TABLE IF NOT EXISTS `roles` (
						  `RoleID` int NOT NULL AUTO_INCREMENT,
						  `RoleName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
						  PRIMARY KEY (`RoleID`),
						  UNIQUE KEY `RoleName` (`RoleName`)
						) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

						-- Dumping data for table clinic_management.roles: ~5 rows (approximately)
						INSERT INTO `roles` (`RoleID`, `RoleName`) VALUES
							(1, 'Administrator'),
							(2, 'Doctor'),
							(3, 'Nurse'),
							(4, 'Receptionist'),
							(5, 'Staff');
							(6, 'Pharmacist');
							(7, 'Cashier');

						-- Dumping structure for table clinic_management.specializations
						CREATE TABLE IF NOT EXISTS `specializations` (
						  `SpecializationID` int NOT NULL AUTO_INCREMENT,
						  `SpecializationName` varchar(150) NOT NULL,
						  PRIMARY KEY (`SpecializationID`)
						) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.appointmenttypes
						CREATE TABLE IF NOT EXISTS `appointmenttypes` (
						  `AppointmentTypeID` int NOT NULL AUTO_INCREMENT,
						  `AppointmentTypeName` varchar(150) NOT NULL,
						  `Description` varchar(500) DEFAULT NULL,
						  PRIMARY KEY (`AppointmentTypeID`)
						) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping data for table clinic_management.appointmenttypes: ~3 rows (approximately)
						INSERT INTO `appointmenttypes` (`AppointmentTypeID`, `AppointmentTypeName`, `Description`) VALUES
							(1, 'Consultation', 'Standard doctor consultation'),
							(2, 'Follow-up', 'Review after consultation'),
							(3, 'Emergency', 'Urgent assessment');

						-- Dumping structure for table clinic_management.discounts
						CREATE TABLE IF NOT EXISTS `discounts` (
						  `DiscountID` int NOT NULL AUTO_INCREMENT,
						  `DiscountName` varchar(150) NOT NULL,
						  `DiscountPercentage` decimal(5,2) NOT NULL DEFAULT '0.00',
						  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
						  PRIMARY KEY (`DiscountID`)
						) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping data for table clinic_management.discounts: ~3 rows (approximately)
						INSERT INTO `discounts` (`DiscountID`, `DiscountName`, `DiscountPercentage`, `IsActive`) VALUES
							(1, 'Senior Citizen', 10.00, 1),
							(2, 'Staff Benefit', 15.00, 1),
							(3, 'Promotional', 5.00, 1);

						-- Dumping structure for table clinic_management.services
						CREATE TABLE IF NOT EXISTS `services` (
						  `ServiceID` int NOT NULL AUTO_INCREMENT,
						  `ServiceName` varchar(200) NOT NULL,
						  `ServicePrice` decimal(12,2) NOT NULL DEFAULT '0.00',
						  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
						  PRIMARY KEY (`ServiceID`)
						) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.medicines
						CREATE TABLE IF NOT EXISTS `medicines` (
						  `MedicineID` int NOT NULL AUTO_INCREMENT,
						  `MedicineName` varchar(200) NOT NULL,
						  `MedicinePrice` decimal(12,2) NOT NULL DEFAULT '0.00',
						  `CurrentStock` decimal(14,3) NOT NULL DEFAULT '0.000',
						  `ReorderLevel` decimal(14,3) NOT NULL DEFAULT '0.000',
						  `TaxRate` decimal(6,3) NOT NULL DEFAULT '0.000',
						  PRIMARY KEY (`MedicineID`)
						) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.tbl_country
						CREATE TABLE IF NOT EXISTS `tbl_country` (
						  `CountryID` int NOT NULL AUTO_INCREMENT,
						  `CountryName` varchar(120) NOT NULL,
						  PRIMARY KEY (`CountryID`),
						  UNIQUE KEY `CountryName` (`CountryName`)
						) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping data for table clinic_management.tbl_country: ~24 rows (approximately)
						INSERT INTO `tbl_country` (`CountryID`, `CountryName`) VALUES
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
							(193, 'Zimbabwe');

						-- Dumping structure for table clinic_management.persons
						CREATE TABLE IF NOT EXISTS `persons` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

						-- Dumping structure for table clinic_management.users
						CREATE TABLE IF NOT EXISTS `users` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

						-- Dumping data for table clinic_management.users: ~3 rows (approximately)
						-- INSERT INTO `users` (`UserID`, `PersonID`, `UserName`, `Password`, `IsActive`, `RoleID`, `CreatedAt`) VALUES
						--	(1, 1, 'admin', 'LhfUPfmKuVB9hc8e1mNmObb3TlwXJ7DaAS1SkzKssqc=', 1, 1, '2026-10-10 12:28:05'),

						-- Dumping structure for table clinic_management.doctors
						CREATE TABLE IF NOT EXISTS `doctors` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.patients
						CREATE TABLE IF NOT EXISTS `patients` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.doctorspecialties
						CREATE TABLE IF NOT EXISTS `doctorspecialties` (
						  `DoctorID` int NOT NULL,
						  `SpecializationID` int NOT NULL,
						  PRIMARY KEY (`DoctorID`,`SpecializationID`),
						  KEY `fk_ds_specialization` (`SpecializationID`),
						  CONSTRAINT `fk_ds_doctor` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`),
						  CONSTRAINT `fk_ds_specialization` FOREIGN KEY (`SpecializationID`) REFERENCES `specializations` (`SpecializationID`)
						) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

						-- Dumping structure for table clinic_management.doctorworkingdays
						CREATE TABLE IF NOT EXISTS `doctorworkingdays` (
						  `DoctorID` int NOT NULL,
						  `DayID` int NOT NULL,
						  `StartTime` time NOT NULL,
						  `EndTime` time NOT NULL,
						  PRIMARY KEY (`DoctorID`,`DayID`),
						  KEY `fk_dwd_day` (`DayID`),
						  CONSTRAINT `fk_dwd_day` FOREIGN KEY (`DayID`) REFERENCES `days` (`DayID`),
						  CONSTRAINT `fk_dwd_doctor` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`)
						) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

						-- Dumping structure for table clinic_management.appointments
						CREATE TABLE IF NOT EXISTS `appointments` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.visits
						CREATE TABLE IF NOT EXISTS `visits` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.bills
						CREATE TABLE IF NOT EXISTS `bills` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.prescriptions
						CREATE TABLE IF NOT EXISTS `prescriptions` (
						  `PrescriptionID` int NOT NULL AUTO_INCREMENT,
						  `VisitID` int NOT NULL,
						  `PrescriptionDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
						  `PrescriptionNotes` text,
						  `PrescriptionStatus` varchar(40) DEFAULT NULL,
						  `Prescriptiontype` varchar(40) DEFAULT NULL,
						  PRIMARY KEY (`PrescriptionID`),
						  KEY `fk_prescriptions_visit` (`VisitID`),
						  CONSTRAINT `fk_prescriptions_visit` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`)
						) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.vitals
						CREATE TABLE IF NOT EXISTS `vitals` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.billservices
						CREATE TABLE IF NOT EXISTS `billservices` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.payments
						CREATE TABLE IF NOT EXISTS `payments` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.invoices
						CREATE TABLE IF NOT EXISTS `invoices` (
						  `InvoiceID` int NOT NULL AUTO_INCREMENT,
						  `BillID` int DEFAULT NULL,
						  `InvoiceDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
						  `TotalAmount` decimal(12,2) NOT NULL DEFAULT '0.00',
						  `InvoiceStatus` varchar(40) NOT NULL DEFAULT 'Open',
						  PRIMARY KEY (`InvoiceID`),
						  KEY `fk_invoices_bill` (`BillID`),
						  CONSTRAINT `fk_invoices_bill` FOREIGN KEY (`BillID`) REFERENCES `bills` (`BillID`)
						) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.salesreturns
						CREATE TABLE IF NOT EXISTS `salesreturns` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.prescriptiondetails
						CREATE TABLE IF NOT EXISTS `prescriptiondetails` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.inventorytransactions
						CREATE TABLE IF NOT EXISTS `inventorytransactions` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.systemlogs
						CREATE TABLE IF NOT EXISTS `systemlogs` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=132 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping structure for table clinic_management.tbl_registration_account
						CREATE TABLE IF NOT EXISTS `tbl_registration_account` (
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
						) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

						-- Dumping data for table clinic_management.tbl_registration_account: ~3 rows (approximately)
						-- INSERT INTO `tbl_registration_account` (`RegistrationID`, `FullName`, `DateOfBirth`, `Gender`, `Phone`, `Email`, `Address`, `CountryID`, `Username`, `PasswordHash`, `IsActive`, `ApprovalStatus`, `CreatedAt`, `ApprovedAt`, `ApprovedBy`) VALUES
						--	(1, 'admin', '2026-01-01', 0, '000000000', 'admin@gmail.com', 'admin', 16, 'admin', 'LhfUPfmKuVB9hc8e1mNmObb3TlwXJ7DaAS1SkzKssqc=', 1, 'Approved', '2026-10-10 12:06:15', '2026-10-10 12:06:15', NULL),

						-- Dumping structure for view clinic_management.view_doctorsdetails
						-- Creating temporary table to overcome VIEW dependency errors
						CREATE TABLE `view_doctorsdetails` (
							`DoctorID` INT(10) NULL,
							`PersonID` INT(10) NULL,
							`FullName` VARCHAR(200) NULL COLLATE 'utf8mb4_unicode_ci',
							`LicenseNumber` VARCHAR(100) NULL COLLATE 'utf8mb4_0900_ai_ci',
							`ConsultationFees` DECIMAL(12,2) NULL,
							`Specializations` TEXT NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
							`WorkingDays` TEXT NOT NULL COLLATE 'utf8mb4_unicode_ci',
							`CountryName` VARCHAR(150) NOT NULL COLLATE 'utf8mb4_unicode_ci',
							`IsActive` TINYINT(1) NULL
						) ENGINE=MyISAM;

						-- Dumping structure for view clinic_management.view_visitdetails
						-- Creating temporary table to overcome VIEW dependency errors
						CREATE TABLE `view_visitdetails` (
							`VisitID` INT(10) NOT NULL,
							`PatientID` INT(10) NOT NULL,
							`DoctorID` INT(10) NOT NULL,
							`AppointmentID` INT(10) NULL,
							`PatientName` VARCHAR(200) NULL COLLATE 'utf8mb4_unicode_ci',
							`DoctorName` VARCHAR(200) NULL COLLATE 'utf8mb4_unicode_ci',
							`VisitDate` DATETIME NOT NULL,
							`VisitStatus` VARCHAR(40) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
							`Diagnosis` CHAR NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
							`StatusText` VARCHAR(40) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
							`CreatedByUserID` INT(10) NULL
						) ENGINE=MyISAM;

						-- Dumping structure for view clinic_management.view_doctorsdetails
						-- Removing temporary table and create final VIEW structure
						DROP TABLE IF EXISTS `view_doctorsdetails`;
						CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `view_doctorsdetails` AS select `d`.`DoctorID` AS `DoctorID`,`d`.`PersonID` AS `PersonID`,`p`.`FullName` AS `FullName`,`d`.`LicenseNumber` AS `LicenseNumber`,`d`.`ConsultationFees` AS `ConsultationFees`,coalesce((select group_concat(distinct `s`.`SpecializationName` order by `s`.`SpecializationName` ASC separator ', ') from (`doctorspecialties` `ds` join `specializations` `s` on((`s`.`SpecializationID` = `ds`.`SpecializationID`))) where (`ds`.`DoctorID` = `d`.`DoctorID`)),'') AS `Specializations`,coalesce((select group_concat(`dy`.`DayName` order by `dw`.`DayID` ASC separator ', ') from (`doctorworkingdays` `dw` join `days` `dy` on((`dy`.`DayID` = `dw`.`DayID`))) where (`dw`.`DoctorID` = `d`.`DoctorID`)),'') AS `WorkingDays`,coalesce(`c`.`CountryName`,'') AS `CountryName`,`d`.`IsActive` AS `IsActive` from ((`doctors` `d` left join `persons` `p` on((`p`.`PersonID` = `d`.`PersonID`))) left join `countries` `c` on((`c`.`CountryID` = `p`.`NationalityCountryID`)));

						-- Dumping structure for view clinic_management.view_visitdetails
						-- Removing temporary table and create final VIEW structure
						DROP TABLE IF EXISTS `view_visitdetails`;
						CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `view_visitdetails` AS select `v`.`VisitID` AS `VisitID`,`v`.`PatientID` AS `PatientID`,`v`.`DoctorID` AS `DoctorID`,`v`.`AppointmentID` AS `AppointmentID`,`pp`.`FullName` AS `PatientName`,`dp`.`FullName` AS `DoctorName`,`v`.`VisitDate` AS `VisitDate`,`v`.`VisitStatus` AS `VisitStatus`,'' AS `Diagnosis`,(case when (`v`.`VisitStatus` = 'Open') then 'Open' when (`v`.`VisitStatus` = 'Completed') then 'Completed' else coalesce(`v`.`VisitStatus`,'') end) AS `StatusText`,`v`.`CreatedByUserID` AS `CreatedByUserID` from ((((`visits` `v` left join `patients` `p` on((`p`.`PatientID` = `v`.`PatientID`))) left join `persons` `pp` on((`pp`.`PersonID` = `p`.`PersonID`))) left join `doctors` `d` on((`d`.`DoctorID` = `v`.`DoctorID`))) left join `persons` `dp` on((`dp`.`PersonID` = `d`.`PersonID`)));
						";

                    using (var cmd = new MySqlCommand(createTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Application.Restart();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MySQL error:\n{ex.Message}", "MySQL Error");
                Application.Restart();
            }
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string server = txtServerName.Text.Trim();
            string user = txtUserName.Text.Trim();
            string pass = txtPassword.Text.Trim();
            //"Server=127.0.0.1;Port=3306;Uid=root;Pwd=;SslMode=None;";
            string connStr = $"Server={server};Uid={user};";
            if (!string.IsNullOrEmpty(pass))
            {
                connStr += $"Pwd={pass};";
            }
            connStr += "Max Pool Size=1000;";
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MessageBox.Show("Connection successful!", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed:\n{ex.Message}", "Error");
            }
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            string con = Environment.GetEnvironmentVariable("clinic_connection", EnvironmentVariableTarget.User);
            string connStr = string.IsNullOrEmpty(con) ? "" : CryptoHelper.Decrypt(con);

            // If the connection string exists, set it to the text box
            if (!string.IsNullOrEmpty(connStr))
            {
                var parameters = connStr.Split(';');

                foreach (var parameter in parameters)
                {
                    var keyValue = parameter.Split('=');

                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        
                        switch (key.ToLower())
                        {
                            case "server":
                                txtServerName.Text = value;
                                break;
                            case "database":
                                txtDataBaseName.Text = value;
                                break;
                            case "uid":
                                txtUserName.Text = value;
                                break;
                            case "pwd":
                                txtPassword.Text = value;
                                break;
                            case "max pool size":
                                // You might want to add logic for max pool size, or ignore it
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No connection string found. Please set it first.", "Info");
            }
        }
    }
}
