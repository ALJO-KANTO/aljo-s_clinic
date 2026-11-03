-- Run once on clinic_management. Back up the database first.
CREATE TABLE IF NOT EXISTS laboratoryparameterresults (
    ResultID INT NOT NULL AUTO_INCREMENT,
    LabRequestID INT NOT NULL,
    ParameterID INT NOT NULL,
    ResultValue VARCHAR(500) NOT NULL,
    ResultFlag VARCHAR(30) NOT NULL DEFAULT 'Not Classified',
    UnitSnapshot VARCHAR(50) NULL,
    ReferenceRangeSnapshot VARCHAR(200) NULL,
    RecordedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (ResultID),
    UNIQUE KEY UQ_LabParameterResult_RequestParameter (LabRequestID, ParameterID),
    INDEX IX_LabParameterResult_Parameter (ParameterID),
    CONSTRAINT FK_LabParameterResult_Request FOREIGN KEY (LabRequestID)
        REFERENCES labrequests (LabRequestID),
    CONSTRAINT FK_LabParameterResult_Parameter FOREIGN KEY (ParameterID)
        REFERENCES laboratorytestparameters (ParameterID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
