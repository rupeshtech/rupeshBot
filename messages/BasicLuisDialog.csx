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
        var intents = result.Intents;
        var entities = result.Entities;
        var dialog = result.Dialog;
        int i=0;
        int j=0;
        await context.PostAsync($"Y asked about weather.{result.Query}");
        foreach(var intent in intents)
        {
         await context.PostAsync($"Intenttion number is {i} .. value is {intent.Intent}"); 
         i++;
        }
        foreach(var entity in entities)
        {
         await context.PostAsync($"entity number is {j} .. value is {entity.Entity}"); 
         j++;   
        }
        await context.PostAsync($"End");
         //
        context.Wait(MessageReceived);
    }
}