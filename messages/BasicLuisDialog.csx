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
        var entities = result.Entities;
        string greeting = null;
        foreach (var entity in entities)
        {
            greeting = entity.Entity;
        }
        if (greeting == null) greeting = "hello";
        await context.PostAsync($"{greeting} .. How are you?"); //
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
            await context.PostAsync($"Weather info for {weather.data.request.FindLast(x => true).query}. \r\n Temperatur is : {weather.data.current_condition.FindLast(p => true).temp_C} degree celsius. \r\n Feels like :{weather.data.current_condition.FindLast(l => true).FeelsLikeC} degree celsius");
        }
            //await context.PostAsync($"End");
         //
        context.Wait(MessageReceived);
    }

    [LuisIntent("Hypotheker")]
    public async Task Hypotheker(IDialogContext context, LuisResult result)
    {
        var entities = result.Entities;
        int j = 0;
        string annualSalary = null;
        string age = null;
        await context.PostAsync($"You asked.{result.Query}");
        if(result.Query.ToLower().Contains("mortgage")|| result.Query.ToLower().Contains("loan")|| result.Query.ToLower().Contains("lenen"))
        {
            await context.PostAsync($"Please type: My Annual salary is ursalary euro and my age is urage. \r\n For ex: My Annual salary is 50000 euro and my age is 30 ");
        }
        foreach (var entity in entities)
        {
            //await context.PostAsync($"entity is : {j}. value is {entity.Entity}");
            if(j==0)annualSalary = entity.Entity;
            if (j == 1) age = entity.Entity;
            j++;
        }
        var info = new HypothekerInfo();
        int annualSalaryInt = 0;
        int ageInt = 0;
        if (int.TryParse(annualSalary, out annualSalaryInt) && int.TryParse(age, out ageInt))
        {
            if (!string.IsNullOrEmpty(annualSalary) && !string.IsNullOrEmpty(age))
            {
                var hypothekerInfo = info.GetMortgageInfo(annualSalary, age);
                if (hypothekerInfo == null)
                {
                    await context.PostAsync($"Unbale for find answer you question: {result.Query}. \r\n Please check annual salary and age.");

                }
                else
                {
                    await context.PostAsync($"{hypothekerInfo}");
                    await context.PostAsync($"Thanks");
                }
            }
            else
            {
                await context.PostAsync($"Unbale for find answer you question: {result.Query}. \r\n Please check annual salary and age.");
            }
        }
        else
        {
            if (!int.TryParse(annualSalary, out annualSalaryInt) || !int.TryParse(age, out ageInt))
                await context.PostAsync($"Unbale for get mortgage amount.\r\n Please make sure n annual salary and age are only numbers");
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
public class HypothekerInfo
{
    public string GetMortgageInfo(string annualIncome, string age)
    {
        WebClient client = new WebClient();
        client.Headers.Add("Content-Type", "application/json");
        string data = "{ApplicantYearlyIncome:" + annualIncome + ",ApplicantAge:" + age + "}";
        string hypothekerInfo = client.UploadString($"https://api.hypotheker.nl/Calculations/CalculateMaximumMortgageByIncome","POST",data);
        return hypothekerInfo;
    }
}