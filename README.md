smssender
=========

Little C# class that can send text messages, a.k.a. SMSs, using a (almost any) mobile phone connected to a PC.

Here is how to use it (you would have never guessed):

    SmsSender sms_sender = new SmsSender();
    sms_sender.Open();
    sms_sender.SendSms("+358504073515", "Thank you for providing this useful code.");
    sms_sender.Close();