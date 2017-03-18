using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the none intent. You said: {result.Query}"); //
        context.Wait(MessageReceived);
    }

    // Go to https://luis.ai and create a new intent, then train/publish your luis app.
    // Finally replace "MyIntent" with the name of your newly created intent in the following handler
    [LuisIntent("Greetings")]
    public async Task MyIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Hello Sir .. How are you?"); //
        context.Wait(MessageReceived);
    }
    [LuisIntent("WeatherIntent")]
    public async Task WeatherIntent(IDialogContext context, LuisResult result)
    {
        var entities = result.Entities;
        int j = 0;
        await context.PostAsync($"Y asked about weather.{result.Query}");
        foreach(var entity in entities)
        {
            var city = entity.Entity;
         await context.PostAsync($"entity number is {j} .. value is {city}"); 
         j++;   
        }
        var info = new WeatherInfo();
        var weather = info.GetWeatherInfo("Amsterdam");
        await context.PostAsync($"{weather.request.ToString()}");
        await context.PostAsync($"End");
         //
        context.Wait(MessageReceived);
    }
}
public class Weather
{
    public Request request;
    public Current_condition current_condition;

}
public class Request
{
    public string type;
    public string query;
}
public class Current_condition
{
    public int temp_C;
    public int temp_F;
}
public class WeatherInfo
{
    public Weather GetWeatherInfo(string city)
    {
        WebClient client = new WebClient();
        string response = client.DownloadString($"http://api.worldweatheronline.com/premium/v1/weather.ashx?q={city}&key=cce343f9e6bd4d159bc133122171803");
        XmlSerializer serializer = new XmlSerializer(typeof(Weather), new XmlRootAttribute("data"));
        MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(response));
        var weatherInfo = (Weather)serializer.Deserialize(memStream);
        return weatherInfo;
    }
}