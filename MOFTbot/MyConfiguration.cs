using System;
using Microsoft.Extensions.Configuration;

namespace MOFTbot;

public class MyConfiguration
{
    private MyConfiguration()
    {
        var builder = new ConfigurationBuilder()
            //когда будем релизить - удалим секреты и раскоментируем appsettings
            //куда положим настоящие настройки
            //.AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>();

        _cfg = builder.Build();
    }
    public static void AddConfiguration()
    {
        if(_cfg == null)
            new MyConfiguration();
    }
    private static IConfigurationRoot? _cfg = null;
    public static string Token { get { return _cfg!.GetSection("Token").Value; } }
    public static string ConnectionString { get{ return _cfg!.GetSection("ConnectionString").Value; } }
}

