using AzureCalculator.Helper;
using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AzureCalculator.DAO
{
    public class UserDAO
    {
        public static int Create(User user)
        {
            SqlConnection conn = null;
            try
            {
                conn = DatabaseHelper.CreateConnection();

                String sql = "INSERT INTO UserNew( UserName,  CompanyName,  Email,  LoginId,  LoginPassword,  ContactNumber,  RequestedOn,  TestDriveId,  Status) OUTPUT INSERTED.UserId " +
                             "             VALUES(@UserName, @CompanyName, @Email, @LoginId, @LoginPassword, @ContactNumber, @RequestedOn, @TestDriveId, @Status)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@CompanyName", user.CompanyName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@LoginId", String.IsNullOrEmpty(user.LoginId) ? "NotProvided" : user.LoginId);
                cmd.Parameters.AddWithValue("@LoginPassword", user.LoginPassword);
                cmd.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);
                cmd.Parameters.AddWithValue("@TestDriveId", user.TestDriveId);
                cmd.Parameters.AddWithValue("@Status", StatusType.STATUS_REQUESTED);
                cmd.Parameters.AddWithValue("@RequestedOn", DateTime.Now);

                return (Int32) cmd.ExecuteScalar();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public static void UpdateSiteName(int userId, String siteName)
        {
            SqlConnection conn = null;
            try
            {
                conn = DatabaseHelper.CreateConnection();

                String sql = "UPDATE UserNew SET SiteName = @SiteName WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SiteName", siteName);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public static User Get(int userId)
        {
            String query = "SELECT UserId, TestDriveId, UserName, ContactNumber, Email, RequestedOn, AvailableOn, ExpireOn, RemovedOn, LoginId, LoginPassword, SiteName, Status FROM UserNew AS u WHERE u.UserId=@UserId";
            return DatabaseHelper.Get<User>(query, new {UserId = userId}).SingleOrDefault();
        }

        public static void UpdateAvailabilityDate(int userId, String status)
        {
            SqlConnection conn = null;
            try
            {
                conn = DatabaseHelper.CreateConnection();

                String sql = "UPDATE UserNew SET AvailableOn = @AvailableOn, Status = @Status WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@AvailableOn", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public static void UpdateStatus(int userId, String status)
        {
            SqlConnection conn = null;
            try
            {
                conn = DatabaseHelper.CreateConnection();

                String sql = "UPDATE UserNew SET Status = @Status WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public static Status StartStepExecution(Status status)
        {
            SqlConnection conn = null;
            try
            {
                conn = DatabaseHelper.CreateConnection();

                status.StepStatus = StatusType.STATUS_PROVISIONING;

                String sql = "INSERT INTO Status( UserId,  StepId, StartTime,  StepStatus,  SequenceNumber) OUTPUT INSERTED.StatusId " +
                             "            VALUES(@UserId, @StepId, @StartTime, @StepStatus, @SequenceNumber)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", status.UserId);
                cmd.Parameters.AddWithValue("@StepId", status.StepId);
                cmd.Parameters.AddWithValue("@SequenceNumber", status.SequenceNumber);
                cmd.Parameters.AddWithValue("@StepStatus", status.StepStatus);
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);

                status.StatusId = (Int32)cmd.ExecuteScalar();
                return status;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public static void EndStepExecution(Status status)
        {
            SqlConnection conn = null;
            try
            {
                conn = DatabaseHelper.CreateConnection();

                String sql = "UPDATE Status SET StepStatus = @StepStatus, EndTime = @EndTime, Remark = @Remark WHERE StatusId = @StatusId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@StepStatus", status.StepStatus);
                cmd.Parameters.AddWithValue("@Remark", status.Remark);
                cmd.Parameters.AddWithValue("@StatusId", status.StatusId);
                cmd.Parameters.AddWithValue("@EndTime", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public static List<Status> GetDriveStatus(int userId)
        {
            String query = "SELECT StatusId, UserId, StepId, StartTime, EndTime, StepStatus, TimeTaken, Remark, SequenceNumber FROM Status WHERE UserId = @UserId ORDER BY SequenceNumber";
            return DatabaseHelper.Get<Status>(query, new { UserId = userId });
        }
    }
}