using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Data;

namespace Dmarc.Admin.Api.Test.Dao
{
    public static class TestHelpers
    {
        public static int CreateDomain(string connectionString, string domainName)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, $@"INSERT INTO `domain`(`name`, `publish`) VALUES('{domainName}', b'1');");
            return (int)(ulong)MySqlHelper.ExecuteScalar(connectionString, "SELECT LAST_INSERT_ID();");
        }

        public static int CreateGroup(string connectionString, string groupName)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, $@"INSERT INTO `group`(`name`) VALUES('{groupName}');");
            return (int)(ulong)MySqlHelper.ExecuteScalar(connectionString, "SELECT LAST_INSERT_ID();");
        }

        public static int CreateUser(string connectionString, string firstName, string lastName, string email)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, $@"INSERT INTO `user`(`firstname`,`lastname`,`email`) VALUES('{firstName}','{lastName}','{email}');");
            return (int)(ulong)MySqlHelper.ExecuteScalar(connectionString, "SELECT LAST_INSERT_ID();");
        }

        public static void CreateGroupUserMapping(string connectionString, List<Tuple<int, int>> groupUsers)
        {
            string values = string.Join(",", groupUsers.Select(_ => $"({_.Item1},{_.Item2})"));

            MySqlHelper.ExecuteNonQuery(connectionString, $@"INSERT INTO `group_user_mapping`(`group_id`, `user_id`) VALUES {values};");
        }

        public static void CreateGroupDomainMapping(string connectionString, List<Tuple<int, int>> domainUsers)
        {
            string values = string.Join(",", domainUsers.Select(_ => $"({_.Item1},{_.Item2})"));

            MySqlHelper.ExecuteNonQuery(connectionString, $@"INSERT INTO `group_domain_mapping`(`group_id`, `domain_id`) VALUES {values};");
        }

        public static List<Api.Domain.Domain> GetAllDomains(string connectionString)
        {
            List<Api.Domain.Domain> domains = new List<Api.Domain.Domain>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(connectionString, "SELECT * FROM domain"))
            {
                while (reader.Read())
                {
                    Api.Domain.Domain domain = new Api.Domain.Domain(
                        reader.GetInt32("id"),
                        reader.GetString("name"));

                    domains.Add(domain);
                }
            }
            return domains;
        }

        public static List<Api.Domain.Group> GetAllGroups(string connectionString)
        {
            List<Api.Domain.Group> groups = new List<Api.Domain.Group>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(connectionString, "SELECT * FROM `group`"))
            {
                while (reader.Read())
                {
                    Api.Domain.Group group = new Api.Domain.Group(
                        reader.GetInt32("id"),
                        reader.GetString("name"));

                    groups.Add(group);
                }
            }
            return groups;
        }

        public static List<Api.Domain.User> GetAllUsers(string connectionString)
        {
            List<Api.Domain.User> users = new List<Api.Domain.User>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(connectionString, "SELECT * FROM user"))
            {
                while (reader.Read())
                {
                    Api.Domain.User user = new Api.Domain.User(
                        reader.GetInt32("id"),
                        reader.GetString("firstname"),
                        reader.GetString("lastname"),
                        reader.GetString("email"),
                        reader.GetBoolean("global_admin") ? RoleType.Admin : RoleType.Standard
                        );

                    users.Add(user);
                }
            }
            return users;
        }

        public static List<Tuple<int, int>> GetAllGroupDomains(string connectionString)
        {
            List<Tuple<int, int>> groupDomains = new List<Tuple<int, int>>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(connectionString, "SELECT * FROM `group_domain_mapping` ORDER BY group_id, domain_id;"))
            {
                while (reader.Read())
                {
                    Tuple<int, int> groupDomain = Tuple.Create(reader.GetInt32("group_id"), reader.GetInt32("domain_id"));

                    groupDomains.Add(groupDomain);
                }
            }
            return groupDomains;
        }

        public static List<Tuple<int, int>> GetAllGroupUsers(string connectionString)
        {
            List<Tuple<int, int>> groupUsers = new List<Tuple<int, int>>();
            using (DbDataReader reader = MySqlHelper.ExecuteReader(connectionString, "SELECT * FROM `group_user_mapping` ORDER BY group_id, user_id;"))
            {
                while (reader.Read())
                {
                    Tuple<int, int> groupDomain = Tuple.Create(reader.GetInt32("group_id"), reader.GetInt32("user_id"));

                    groupUsers.Add(groupDomain);
                }
            }
            return groupUsers;
        }
    }
}