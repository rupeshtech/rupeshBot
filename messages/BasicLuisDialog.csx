using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        string city = null;
        await context.PostAsync($"You asked.{result.Query}");
        foreach(var entity in entities)
        {
            city = entity.Entity;
        }
        var info = new WeatherInfo();
        var weather = info.GetWeatherInfo(city);
        if (weather.data.error != null)
        {
            await context.PostAsync($"Unbale for find answer you question: {result.Query}. \r\n Please check the city name.");

        }
        else
        {
            await context.PostAsync($"Weather info for {weather.data.request.FindLast(x => true).query}. \r\n Temperatur is : {weather.data.current_condition.FindLast(p => true).temp_C}. \r\n Feels like :{weather.data.current_condition.FindLast(l => true).FeelsLikeC}");
        }
            //await context.PostAsync($"End");
         //
        context.Wait(MessageReceived);
    }
}
public class Weather
{
    public List<Request> request;
    public List<Current_condition> current_condition;
    public List<ErrorMessage> error;

}
public class ErrorMessage
{
    public string msg;
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
    public int windspeedMiles;
    public int windspeedKmph;
    public int winddirDegree;
    public string winddir16Point;
    public int humidity;
    public int visibility;
    public int pressure;
    public int cloudcover;
    public int FeelsLikeC;
    public int FeelsLikeF;
}
public class WeatherResponse
{
    public Weather data;
}
public class WeatherInfo
{
    public WeatherResponse GetWeatherInfo(string city)
    {
        WebClient client = new WebClient();
        string response = client.DownloadString($"http://api.worldweatheronline.com/premium/v1/weather.ashx?q={city}&key=cce343f9e6bd4d159bc133122171803&format=json");
        var weatherInfo = JsonConvert.DeserializeObject<WeatherResponse>(response);
        return weatherInfo; 
    }
}