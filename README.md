# natti-chat-bot
A welcoming bot for t.me/natti_jun_front_chat. Currently greets every new user with a DiscoElysium sticker, and supports new members and messages counters, and daily alerts, since v1.2. 

## Usage

Configure and deploy the bot, add it to your chat and enable counters (only >= v1.2) by sending `/enable_counter` command. You can disable it (surprise!) with `/disable_counter` command.

## Configuration and installation
### Version <= 1.1
The easiest way to run this bot is pulling a docker image:

```bash
sudo docker pull eveloth/natti-chat-bot:{specify-tag}

sudo docker run \
  --name natti-chat-bot \
  -d \
  -e "BotConfiguration:BotToken={your-token}" \
  -e "BotConfiguration:HostAddress={your-domain}" \
  -e "BotConfiguration:SecretToken={e.g.-random-guid}" \
  -p 443:443 \
  eveloth/natti-chat-bot:{specify-tag}
```
**NOTE:** you MUST have an SSL certificate! Otherwise webhook won't be set. See: https://core.telegram.org/bots/webhooks

### Version > 1.1

There is now a single chat counter support since v1.2. It uses Hangfire with Redis storage for scheduling alerts, so you'll either need a Redis instanse running or you can just use `docker compose`. 

Ports and redis configurations live inside the `.env` file, and bot configuration lives inside the `bot-config.env`.
The former is pretty much self explanatory and the latter goes like this:

```env
<...>

This is your group id. Bot will only react to commands and messages from this group:

BotConfiguration__ChatId=''

This is the 'entitled user' id. In order to prevent abuse only admins of the chat can enable and disable counters. However you might want to make some user a 'bot admin', while not giving him an admin role in your group. If so, specify their user id here:

BotConfiguration__EntitledUserId=''

```

#### How do I get the ids?

By adding @RawDataBot in your group -- this will give you a group id. (Don't forget to remove it or your chat members will be buired under the json mountain). To get any user's id, simply forward any message from that user to the data bot.

## Future plans

- Add multi-chat support

## Don't stop there!

This bot is configured to run behind nginx as well. You should use docker port 80 for that, not 443 (e.g. -p 5000:80 or the same in the docker-compose).

Learn nginx basics here: https://www.freecodecamp.org/news/the-nginx-handbook/

Learn how to host ASP.NET Core web apps behind nginx: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-7.0

And keep coding, as they say.
