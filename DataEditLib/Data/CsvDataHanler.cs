using DataEditLib.Enums;
using DataEditLib.Interfaces;
using DataEditLib.Interfaces.Crud;
using DataEditLib.Models;
using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Data
{
    public class CsvDataHanler<T>
        : IDataEdit, IRead<T>, ICreate<T>, IUpdate<T>, IDelete<T> where T : MyEntity, new()

    {
        public async Task<IActionResult<T>> Create(ClientMessage message)
        {
            try
            {

                var objs = await InsertCurrentData(message);
                await File.WriteAllLinesAsync(ProjectProperties.TextFilePath, objs);
                return new ActionResult<T>(message, MessageStatus.Ok, ProjectProperties.OkCreatedInfo);
            }
            catch (Exception ex)
            {
                return new ActionResult<T>(message, MessageStatus.Error, ex);
            }
        }

        public async Task<ServerMessage> ParseMessageType(ClientMessage message)
        {
            IActionResult<T> actionResult;
            try
            {
                if (message == null)
                    throw new ArgumentNullException();

                if (!File.Exists(ProjectProperties.TextFilePath))
                {
                    File.Create(ProjectProperties.TextFilePath);
                }

                if (message.MsgType == MessageType.Create)
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

        public async Task<IActionResult<T>> ReadOne(ClientMessage message)
        {
            try
            {
                var text = (await File.ReadAllTextAsync(ProjectProperties.TextFilePath));
                var collection = ToCollection(text);

                if (int.TryParse(message.Value, out int id))
                    foreach (var item in collection)
                        if (item.Id == id)
                            return new ActionResult<T>(message, MessageStatus.Ok, item.ToString());

                throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                return new ActionResult<T>(message, MessageStatus.Error, ex);
            }
        }


        public async Task<IActionResult<T>> ReadAll(ClientMessage message)
        {
            try
            {
                var text = await File.ReadAllTextAsync(ProjectProperties.TextFilePath);
                return new ActionResult<T>(message, MessageStatus.Ok, text);
            }
            catch (IOException ex)
            {
                return new ActionResult<T>(message, MessageStatus.Error, ex);
            }
        }

        public async Task<IActionResult<T>> Update(ClientMessage message)
        {
            try
            {
                var text = (await File.ReadAllTextAsync(ProjectProperties.TextFilePath));
                var collection = ToCollection(text);

                if (int.TryParse(message.Value.Split(',')[0], out int id))
                {
                    foreach (var item in collection)
                    {
                        if (item.Id == id)
                        {
                            var tmp = new T();
                            item.UpdateCurrent(tmp.ToObjectFromText(message.Value));
                            await RewriteBase(collection);
                            return new ActionResult<T>(message, MessageStatus.Ok, ProjectProperties.OkUpdatedInfo);
                        }
                    }
                }

                throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                return new ActionResult<T>(message, MessageStatus.Error, ex);
            }
        }

        public async Task<IActionResult<T>> Delete(ClientMessage message)
        {
            try
            {
                var text = (await File.ReadAllTextAsync(ProjectProperties.TextFilePath));
                var collection = ToCollection(text);

                if (int.TryParse(message.Value.Split(',')[0], out int id))

                foreach (var item in collection)
                {
                    if (item.Id == id)
                    {
                        (collection as List<T>).Remove(item);
                        await RewriteBase(collection);
                        return new ActionResult<T>(message, MessageStatus.Ok, ProjectProperties.OkDeletedInfo);
                    }
                }

                throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                return new ActionResult<T>(message, MessageStatus.Error, ex);
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

        private async Task RewriteBase(IEnumerable<T> data)
        {
            File.Delete(ProjectProperties.TextFilePath);
            var list = new List<string>();
            foreach (var item in data)
                list.Add(item.ToString());
            await File.WriteAllLinesAsync(ProjectProperties.TextFilePath, list);
        }

        private async Task<string[]> InsertCurrentData(ClientMessage message)
        {
            string toWrite = (await ReadAll(message)).Value;

            var objs = message.Value.Insert(0, toWrite)
                .Replace("\r\n", string.Empty)
                .Split(';')
                .Where(x => x != string.Empty)
                .Select(x => x += ";").ToArray();
            return objs;
        }
    }
}
