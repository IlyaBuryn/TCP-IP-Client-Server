// Server Console
using DataEditLib.Data;
using DataEditLib.Interfaces;
using DataEditLib.Logger;
using DataEditLib.Models;
using ManagementServer.Servers;


// Setting variables

//IDataEdit editService = new CsvDataHanler<MyEntity>();
IDataEdit editService = new SqliteDataHandler<MyEntity>();
ILogger logger = new CL();


// Starting server
var _server = new TcpServerSocket(editService, logger);
_server.Start();


