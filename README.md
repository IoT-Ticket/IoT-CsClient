# RestClient

## Getting started

Create your own account at https://my.iot-ticket.com/Dashboard/. 

### Register device 

A device Id is automatically assigned to a newly registered device; the API Client should store this
device Id as it uniquely identifies the device and will be used in subsequent calls. Client should
avoid multiple registration call as this might result to duplicate devices being created. When
in doubt, a good flow will be to get the list of already created devices and validate the device’s
existence on the server through its name and attributes. Once the device is registered and the
device id is obtained, clients can immediately start sending measurement values to the server. 

<pre><code>
  var username = "my_user";
  var pwd = new SecureString();
  "my_secret".ToList().ForEach(pwd.AppendChar);

  var client = new IoTTicketClient(new Uri("https://my.iot-ticket.com/api/v1/"), username, pwd);
  
  var device = new Device
  {
    Name = "MyTestDevice",
    Description = "Testing my device",
    Manufacturer = "My",
    Type = "PC",
    Attributes =
        new List<DeviceAttribute>
        {
            new DeviceAttribute("testKey1", "testValue1"),
            new DeviceAttribute("testKey2", "testValue2")
        }
  };
  
  var deviceDetails = client.RegisterDeviceAsync(device).Result;
  
  Console.WriteLine("This is the device id that should be used in subsequent calls when sending measurement data: " + deviceDetails.Id);
  
</code></pre>

### Send measurement data

<pre><code>
  var datapoint = new DatanodeWritableValue
  {
      Name = "Temperature",
      Path = "Engine",
      Value = 87.4,
      Unit = "c",
      DataType="double",
      Timestamp = DateTime.Now
  };
  
  var writeResult = client.WriteDatapointAsync(DeviceId, datapoint).Result;
  
  //dispose client when not needed anymore 
</code></pre>

## API documentation

https://www.iot-ticket.com/images/Files/IoT-Ticket.com_IoT_API.pdf

## NuGet Package

https://www.nuget.org/packages/Wapice.IoTTicket.RestClient/
