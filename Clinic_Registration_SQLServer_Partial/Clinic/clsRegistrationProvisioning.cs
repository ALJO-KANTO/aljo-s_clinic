using System;
using Clinic_DataAccess;

namespace Clinic_Business
{
    public static class clsRegistrationProvisioning
    {
        public static int CreateApplicationUser(
            string fullName,
            DateTime dateOfBirth,
            short gender,
            string phone,
            string email,
            string address,
            int sqlServerCountryId,
            string username,
            string passwordHash,
            int roleId)
        {
            if (sqlServerCountryId <= 0)
                throw new ArgumentException(
                    "A valid SQL Server country is required.");

            if (roleId <= 0)
                throw new ArgumentException(
                    "A valid application role is required.");

            if (clsUserData.IsUserExist(username))
                throw new InvalidOperationException(
                    "Username already exists in the application.");

            int personId = clsPersonData.AddNewPerson(
                fullName,
                dateOfBirth,
                gender,
                phone,
                email ?? "",
                address,
                "",
                sqlServerCountryId
            );

            if (personId <= 0)
                throw new InvalidOperationException(
                    "Failed to create person record.");

            int userId = clsUserData.AddNewUser(
                personId,
                username,
                passwordHash,
                true,
                roleId
            );

            if (userId <= 0)
                throw new InvalidOperationException(
                    "Person was created, but user creation failed.");

            return userId;
        }
    }
}