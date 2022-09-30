using DataEditLib.Enums;
using DataEditLib.Interfaces;
using DataEditLib.Interfaces.Crud;
using DataEditLib.Models;
using DataEditLib.Models.MessagesTypes;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace DataEditLib.Data
{
    public class SqliteDataHandler<T> :  IDataEdit, IDataBaseOptions<T>,
        ICreate<T>, IRead<T>, IUpdate<T>, IDelete<T> where T : MyEntity, new()
    {
        public async Task<IActionResult<T>> Create(ClientMessage message)
        {
            using (var connection = new SqliteConnection(ProjectProperties.SqliteDataString))
            {
                try
                {
                    connection.Open();
                    var list = ToCollection(message.Value);
                    SqliteCommand command = new SqliteCommand();
                    command.Connection = connection;
                    foreach (var item in list)
                    {

                        command.CommandText = $"INSERT INTO MyEntities (Name, ScopeOfWork, UnitPrice, AccruedEarnings) " +
                            $"VALUES ('{item.Name}', '{item.ScopeOfWork}', '{item.UnitPrice}', '{item.AccruedEarnings}')";
                        var number = await command.ExecuteNonQueryAsync();
                    }
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Ok, ProjectProperties.OkCreatedInfo);


                }
                catch (Exception ex)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Error, ex);
                }
            }
        }

        public async Task<IActionResult<T>> CreateDataBase(ClientMessage message)
        {
            using (var connection = new SqliteConnection(ProjectProperties.SqliteDataString))
            {
                try
                {
                    connection.Open();
                    var list = ToCollection(message.Value);
                    SqliteCommand command = new SqliteCommand();
                    command.Connection = connection;
                    command.CommandText = "CREATE TABLE MyEntities(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, Name TEXT, ScopeOfWork DOUBLE, UnitPrice DOUBLE, AccruedEarnings DOUBLE)";
                    int number = await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Ok, $"Database was created");
                }

                catch (Exception ex)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Error, ex);
                }
            }
        }

        public async Task<ServerMessage> ParseMessageType(ClientMessage message)
        {
            IActionResult<T> actionResult;
            try
            {
                if (message == null)
                    throw new ArgumentNullException();

                if (message.MsgType == MessageType.CreateDatabase)
                    actionResult = await CreateDataBase(message);
                else if (message.MsgType == MessageType.DeleteDatabase)
                    actionResult = await DeleteDataBase(message);
                else if (message.MsgType == MessageType.Create)
                    actionResult = await Create(message);
                else if (message.MsgType == MessageType.ReadOne)
                    actionResult = await ReadOne(message);
                else if (message.MsgType == MessageType.ReadAll)
                    actionResult = await ReadAll(message);
                else if (message.MsgType == MessageType.Update)
                    actionResult = await Update(message);
                else if (message.MsgType == MessageType.Delete)
                    actionResult = await Delete(message);
                else
                    throw new ArgumentException(ProjectProperties.MessageTypeNotDefine);

                return ServerMessage.ServerResponse((IActionResult<MyEntity>)actionResult, message);
            }
            catch (Exception ex)
            {
                actionResult = new ActionResult<T>(message, MessageStatus.Error, ex);
                return ServerMessage.ServerResponse((IActionResult<MyEntity>)actionResult, message);
            }
        }

        public async Task<IActionResult<T>> Delete(ClientMessage message)
        {
            using (var connection = new SqliteConnection(ProjectProperties.SqliteDataString))
            {
                try
                {
                    connection.Open();
                    if (int.TryParse(message.Value, out int id))
                    {
                        SqliteCommand command = new SqliteCommand();
                        command.Connection = connection;
                        command.CommandText = $"DELETE FROM MyEntities WHERE Id='{id}'";
                        var number = await command.ExecuteNonQueryAsync();
                    }
                    else throw new ArgumentException(message.Value);

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Ok, ProjectProperties.OkDeletedInfo);
                }
                catch (Exception ex)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Error, ex);
                }
            }
        }

        public async Task<IActionResult<T>> DeleteDataBase(ClientMessage message)
        {
            try
            {
                if (File.Exists(ProjectProperties.SqliteFilePath))
                {
                    SqliteConnection.ClearAllPools();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(ProjectProperties.SqliteFilePath);
                    return new ActionResult<T>(message, MessageStatus.Ok, $"Database was deleted");
                }
                else throw new Exception("File error");
            }
            catch (Exception ex)
            {
                return new ActionResult<T>(message, MessageStatus.Error, ex);
            }
        }

        public async Task<IActionResult<T>> ReadAll(ClientMessage message)
        {
            string result = string.Empty;
            using (var connection = new SqliteConnection(ProjectProperties.SqliteDataString))
            {
                try
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM MyEntities", connection);
                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            var list = new List<T>();
                            while (reader.Read())
                            {
                                var tmp = new T();
                                tmp.Id = reader.GetInt32(0);
                                tmp.Name = reader.GetString(1);
                                tmp.ScopeOfWork = reader.GetDouble(2);
                                tmp.UnitPrice = reader.GetDouble(3);
                                tmp.AccruedEarnings = reader.GetDouble(4);

                                list.Add(tmp);
                            }
                            for (int i = 0; i < list.Count; i++)
                            {
                                result += list[i].ToString() + '\n';
                            }
                            await reader.CloseAsync();
                            await reader.DisposeAsync();
                        }
                        else throw new Exception("Empty databse");
                    }

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Ok, result);

                }
                catch (Exception ex)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Error, ex);
                }
            }
        }

        public async Task<IActionResult<T>> ReadOne(ClientMessage message)
        {
            using (var connection = new SqliteConnection(ProjectProperties.SqliteDataString))
            {
                try
                {
                    connection.Open();
                    if (int.TryParse(message.Value, out int id))
                    {
                        SqliteCommand command = new SqliteCommand($"SELECT * FROM MyEntities WHERE Id = '{id}'", connection);
                        using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                var tmp = new T();
                                while (reader.Read())
                                {
                                    tmp.Id = reader.GetInt32(0);
                                    tmp.Name = reader.GetString(1);
                                    tmp.ScopeOfWork = reader.GetDouble(2);
                                    tmp.UnitPrice = reader.GetDouble(3);
                                    tmp.AccruedEarnings = reader.GetDouble(4);
                                }

                                await reader.CloseAsync();
                                await reader.DisposeAsync();
                                await connection.CloseAsync();
                                await connection.DisposeAsync();
                                return new ActionResult<T>(message, MessageStatus.Ok, tmp.ToString());
                            }
                            else throw new Exception("Empty db response");
                        }
                    }
                    else throw new KeyNotFoundException();

                }
                catch (Exception ex)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Error, ex);
                }
            }
        }

        public async Task<IActionResult<T>> Update(ClientMessage message)
        {
            using (var connection = new SqliteConnection(ProjectProperties.SqliteDataString))
            {
                try
                {
                    connection.Open();
                    var obj = ToCollection(message.Value).First();
                    SqliteCommand command = new SqliteCommand();
                    command.Connection = connection;
                    if (obj != null)
                    {
                        command.CommandText = $"UPDATE MyEntities SET Name='{obj.Name}', ScopeOfWork='{obj.ScopeOfWork}', UnitPrice='{obj.UnitPrice}', AccruedEarnings='{obj.AccruedEarnings}' WHERE Id='{obj.Id}'";
                        var number = await command.ExecuteNonQueryAsync();
                    }
                    else throw new NullReferenceException();

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Ok, ProjectProperties.OkUpdatedInfo);

                }

                catch (Exception ex)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                    return new ActionResult<T>(message, MessageStatus.Error, ex);
                }
            }
        }

        private IEnumerable<T> ToCollection(string line)
        {
            var list = new List<T>();
            var lines = line.Replace("\r\n", string.Empty)
                .Replace("\r\n", string.Empty)
                .Split(';')
                .Where(x => x != string.Empty)
                .Select(x => x += ";").ToArray();

            for (int i = 0; i < lines.Count(); i++)
                list.Add((T)new T().ToObjectFromText(lines.ElementAt(i)));

            return list;
        }
    }
}
