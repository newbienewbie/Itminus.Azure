
The Honor Blongs to Microsoft . Some codes within this library are shamelessly copied from [BotBuilder Framework](https://github.com/Microsoft/botbuilder-dotnet). 


## QnA

- *Why another wrapper to QnAMaker ?* 

The original library has a dependency on `BotBuilder`. I don't think that's a modular way. 


- *Will this library run  on .NET Framework* 

This library targets to `netstandard2.0`. Surely it runs on `.NET Framework` or other platform.


- *Does this library have other dependencies?*

Yes. It depends on `.NET Standard` and `Newtonsoft.Json`.

## Qucik Start

1. Build Service:

```csharp
    var qaService= new QnAMakerServiceBuilder()
        .SetHost("https://<app-service-name>.azurewebsites.net/qnamaker")
        .SetPath("/knowledgebases/<kb-id>/generateAnswer")
        .SetEndPointKey("your-endpoint-key")
        .Build()
        ;
```

2. Query and Get Answers :

```csharp
    var x = await qaService.GetAnswer("hi, what's your name?");
```

Or  with an option :
```csharp
    var opts = new QnAMakerOptions(){
        // ....
    };
    var x = await qaService.GetAnswer("hi, what's your name?", opts);
```