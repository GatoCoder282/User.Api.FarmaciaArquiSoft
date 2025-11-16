using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Domain.Ports;
using User.Infraestructure.Data;

namespace User.Infraestructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConnection _db;

        public UserRepository()
        {
            _db = DatabaseConnection.Instance;
        }

        public async Task<UserEntity> Create(UserEntity entity)
        {

            string query = @"
INSERT INTO users 
(first_name, last_first_name, last_second_name,  username, password,has_changed_password,password_version, mail, phone, ci, role, created_at, created_by, is_deleted) 
VALUES 
(@first_name,@last_first_name, @last_second_name, @username,@password,@has_changed_password,@password_version, @mail, @phone, @ci, @role, @created_at, @created_by, @is_deleted)";

            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            using var comand = new MySqlCommand(query, connection);
            comand.Parameters.AddWithValue("@first_name", entity.first_name);
            comand.Parameters.AddWithValue("@last_first_name", entity.last_first_name);
            comand.Parameters.AddWithValue("@last_second_name", entity.last_second_name);
            comand.Parameters.AddWithValue("@username", entity.username);
            comand.Parameters.AddWithValue("@password", entity.password);
            comand.Parameters.AddWithValue("@has_changed_password", entity.has_changed_password);
            comand.Parameters.AddWithValue("@password_version", entity.password_version);
            comand.Parameters.AddWithValue("@mail", entity.mail);
            comand.Parameters.AddWithValue("@phone", entity.phone);
            comand.Parameters.AddWithValue("@ci", entity.ci);
            comand.Parameters.AddWithValue("@role", entity.role.ToString());
            comand.Parameters.AddWithValue("@created_at", entity.created_at);
            comand.Parameters.AddWithValue("@created_by", (object?)entity.created_by ?? DBNull.Value);
            comand.Parameters.AddWithValue("@is_deleted", entity.is_deleted);

            await comand.ExecuteNonQueryAsync();
            entity.id = (int)comand.LastInsertedId;
            return entity;
        }

        public async Task<UserEntity?> GetById(UserEntity entity)
        {
            string query = "SELECT * FROM users WHERE id = @id AND is_deleted = FALSE";
            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            using var comand = new MySqlCommand(query, connection);
            comand.Parameters.AddWithValue("@id", entity.id);

            using var reader = await comand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var u = new UserEntity
                {
                    id = reader.GetInt32("id"),

                    first_name = reader.GetString("first_name"),
                    last_first_name = reader.GetString("last_first_name"),
                    last_second_name = reader.GetString("last_second_name"),

                    username = reader.GetString("username"),
                    password = reader.GetString("password"),
                    has_changed_password = reader.GetBoolean("has_changed_password"),
                    password_version = reader.GetInt32("password_version"),
                    last_password_changed_at = reader.IsDBNull(reader.GetOrdinal("last_password_changed_at")) ? null : reader.GetDateTime("last_password_changed_at"),
                    mail = reader.GetString("mail"),
                    phone = reader.GetString("phone"),
                    ci = reader.GetString("ci"),
                    role = Enum.Parse<UserRole>(reader.GetString("role")),

                    is_deleted = reader.GetBoolean("is_deleted"),
                    created_by = reader.IsDBNull(reader.GetOrdinal("created_by")) ? null : reader.GetInt32("created_by"),
                    updated_by = reader.IsDBNull(reader.GetOrdinal("updated_by")) ? null : reader.GetInt32("updated_by"),
                    created_at = reader.GetDateTime("created_at"),
                    updated_at = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime("updated_at")
                };

                return u;
            }
            return null;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var lista = new List<User>();
            string query = "SELECT * FROM users WHERE is_deleted = FALSE ORDER BY last_first_name ASC, first_name ASC;";

            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            using var comand = new MySqlCommand(query, connection);
            using var reader = await comand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new UserEntity
                {
                    id = reader.GetInt32("id"),

                    first_name = reader.GetString("first_name"),
                    last_first_name = reader.GetString("last_first_name"),
                    last_second_name = reader.GetString("last_second_name"),


                    username = reader.GetString("username"),
                    password = reader.GetString("password"),
                    has_changed_password = reader.GetBoolean("has_changed_password"),
                    password_version = reader.GetInt32("password_version"),
                    last_password_changed_at = reader.IsDBNull(reader.GetOrdinal("last_password_changed_at")) ? null : reader.GetDateTime("last_password_changed_at"),
                    mail = reader.GetString("mail"),
                    phone = reader.GetString("phone"),
                    ci = reader.GetString("ci"),
                    role = Enum.Parse<UserRole>(reader.GetString("role")),

                    is_deleted = reader.GetBoolean("is_deleted"),
                    created_by = reader.IsDBNull(reader.GetOrdinal("created_by")) ? null : reader.GetInt32("created_by"),
                    updated_by = reader.IsDBNull(reader.GetOrdinal("updated_by")) ? null : reader.GetInt32("updated_by"),
                    created_at = reader.GetDateTime("created_at"),
                    updated_at = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime("updated_at")
                });
            }
            return lista;
        }

        public async Task Update(UserEntity entity)
        {
            string query = @"
UPDATE users 
SET first_name=@first_name, 
last_first_name=@last_first_name,
    last_second_name=@last_second_name,  
    username=@username, 
    password=@password,
has_changed_password=@has_changed_password,
password_version=@password_version,
last_password_changed_at=@last_password_changed_at,
    mail=@mail, 
    phone=@phone, 
    ci=@ci, 
    role=@role, 
    updated_at=@updated_at, 
    updated_by=@updated_by 
WHERE id=@id";

            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            using var comand = new MySqlCommand(query, connection);
            comand.Parameters.AddWithValue("@first_name", entity.first_name);
            comand.Parameters.AddWithValue("@last_first_name", entity.last_first_name);
            comand.Parameters.AddWithValue("@last_second_name", entity.last_second_name);


            comand.Parameters.AddWithValue("@username", entity.username);
            comand.Parameters.AddWithValue("@password", entity.password);
            comand.Parameters.AddWithValue("@has_changed_password", entity.has_changed_password);
            comand.Parameters.AddWithValue("@password_version", entity.password_version);
            comand.Parameters.AddWithValue("@last_password_changed_at", (object?)entity.last_password_changed_at ?? DBNull.Value);
            comand.Parameters.AddWithValue("@mail", (object?)entity.mail);
            comand.Parameters.AddWithValue("@phone", entity.phone);
            comand.Parameters.AddWithValue("@ci", entity.ci);
            comand.Parameters.AddWithValue("@role", entity.role.ToString());
            comand.Parameters.AddWithValue("@updated_at", DateTime.Now);
            comand.Parameters.AddWithValue("@updated_by", (object?)entity.updated_by ?? DBNull.Value);
            comand.Parameters.AddWithValue("@id", entity.id);

            await comand.ExecuteNonQueryAsync();
        }

        public async Task Delete(UserEntity entity)
        {
            const string sql = @"
UPDATE users
SET is_deleted = TRUE,
    updated_at = @updated_at,
    updated_by = @updated_by
WHERE id = @id;";

            using var con = _db.GetConnection();
            await con.OpenAsync();

            using var cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", entity.id);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
            cmd.Parameters.AddWithValue("@updated_by", (object?)entity.updated_by ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }


    }
}
