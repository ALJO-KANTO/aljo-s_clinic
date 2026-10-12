CREATE DATABASE IF NOT EXISTS clinic_management CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE clinic_management;

CREATE TABLE IF NOT EXISTS tbl_country (
    CountryID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CountryName VARCHAR(120) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS tbl_registration_account (
    RegistrationID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(200) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender TINYINT UNSIGNED NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(150) NULL,
    Address VARCHAR(500) NOT NULL,
    CountryID INT NOT NULL,
    Username VARCHAR(20) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 0,
    ApprovalStatus VARCHAR(20) NOT NULL DEFAULT 'Pending',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ApprovedAt DATETIME NULL,
    ApprovedBy VARCHAR(100) NULL,
    CONSTRAINT fk_registration_country FOREIGN KEY (CountryID)
        REFERENCES tbl_country(CountryID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO tbl_country (CountryName) VALUES
('United Arab Emirates'), ('India'), ('Oman'), ('Saudi Arabia'),
('Qatar'), ('Kuwait'), ('Bahrain'), ('Pakistan'), ('Philippines'),
('United Kingdom'), ('United States');

-- Registration records are pending and inactive by default.
-- Integrate approval with your existing user/person tables and login flow.
