using ExampleBot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using UserStep;

namespace Telegram.Bot.Examples.Polling;

public static class UpdateHandlers
{
    public static Step<StepEnums.UserEnum> UsersStepManagment { get; set; } = new Step<StepEnums.UserEnum>();


    #region Use For Example
    public static string NumberPhone { get; set; }
    public static string Email { get; set; }
    public static string BirthDay { get; set; }

    #endregion

    public static Task PollingErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
            UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
            _ => UnknownUpdateHandlerAsync(botClient, update)
        };

        try
        {
            await handler;
        }
#pragma warning disable CA1031
        catch (Exception exception)
#pragma warning restore CA1031
        {
            await PollingErrorHandler(botClient, exception, cancellationToken);
        }
    }

    private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
    {
        Console.WriteLine($"Receive message type: {message.Type}");
        if (message.Text is not { } text)
            return;
        var FromId = message.From!.Id;
        var MessageId = message.MessageId;

        // if user exists Ignore ...
        UsersStepManagment.SetStep(FromId, StepEnums.UserEnum.Main);

        // getStep user
        var userStep = UsersStepManagment.GetStep(FromId).Step;

        if (userStep == StepEnums.UserEnum.Main)
        {
            if (text == "/start")
            {
                await SendReplyKeyboard(botClient, message);
            }
            else if (text == "Register")
            {
                //Update stepUer
                UsersStepManagment.UpdateStep(FromId, StepEnums.UserEnum.waitToResponsePhoneNumber);
                await botClient.SendTextMessageAsync(FromId, "Please Send numberPhone", replyToMessageId: MessageId);
            }
        }
        else
        {
            if (userStep == StepEnums.UserEnum.waitToResponsePhoneNumber)
            {
                NumberPhone = text;
                UsersStepManagment.UpdateStep(FromId, StepEnums.UserEnum.waitToResponseEmail);
                await botClient.SendTextMessageAsync(FromId, "Please Send email", replyToMessageId: MessageId);
            }
            else if (userStep == StepEnums.UserEnum.waitToResponseEmail)
            {
                Email = text;
                UsersStepManagment.UpdateStep(FromId, StepEnums.UserEnum.waitToResponseBirhDay);
                await botClient.SendTextMessageAsync(FromId, "Please Send bithDay", replyToMessageId: MessageId);
            }
            else if (userStep == StepEnums.UserEnum.waitToResponseBirhDay)
            {
                BirthDay = text;
                UsersStepManagment.UpdateStep(FromId, StepEnums.UserEnum.Main);
                await botClient.SendTextMessageAsync(FromId, $"Your registration was successful : \nNumber : {NumberPhone}\nEmail : {Email}\nBirthDay : {BirthDay}", replyToMessageId: MessageId);
            }
        }




        static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "Register", },
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Choose",
                                                        replyMarkup: replyKeyboardMarkup);
        }


    }



    private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}