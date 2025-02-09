using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



var builder = Host.CreateApplicationBuilder();

builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), "llama3"));

var app = builder.Build();

var chatClient = app.Services.GetRequiredService<IChatClient>();


//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Simple Chat Completion
//var chatCompletion = await chatClient.CompleteAsync("What is .NET? Reply in 50 words max.");
//Console.WriteLine(chatCompletion.Message.Text);
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------




//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Implementing Chat with History
var chatHistory = new List<ChatMessage>();

while (true)
{
	Console.WriteLine("Enter your prompt:");
	var userPrompt = Console.ReadLine();
	chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

	Console.WriteLine("Response from AI:");
	var chatResponse = "";
	await foreach (var item in chatClient.CompleteStreamingAsync(chatHistory))
	{
		// We're streaming the response, so we get each message as it arrives
		Console.Write(item.Text);
		chatResponse += item.Text;
	}
	chatHistory.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
	Console.WriteLine();
}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------



//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Getting Practical: Article Summarization
//var posts = Directory.GetFiles("posts").Take(5).ToArray();
//foreach (var post in posts)
//{
//	string prompt = $$"""
//	                  You will receive an input text and the desired output format.
//	                  You need to analyze the text and produce the desired output format.
//	                  You not allow to change code, text, or other references.

//	                  # Desired response

//	                  Only provide a RFC8259 compliant JSON response following this format without deviation.

//	                  {
//	                     "title": "Title pulled from the front matter section",
//	                     "summary": "Summarize the article in no more than 100 words"
//	                  }

//	                  # Article content:

//	                  {{File.ReadAllText(post)}}
//	                  """;

//	var chatCompletion = await chatClient.CompleteAsync(prompt);
//	Console.WriteLine(chatCompletion.Message.Text);
//	Console.WriteLine(Environment.NewLine);
//}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------





//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Taking It Further: Smart Categorization


//var posts = Directory.GetFiles("posts").Take(5).ToArray();
//foreach (var post in posts)
//{
//	string prompt = $$"""
//	                  You will receive an input text and the desired output format.
//	                  You need to analyze the text and produce the desired output format.
//	                  You not allow to change code, text, or other references.

//	                  # Desired response

//	                  Only provide a RFC8259 compliant JSON response following this format without deviation.

//	                  {
//	                     "title": "Title pulled from the front matter section",
//	                     "tags": "Array of tags based on analyzing the article content. Tags should be lowercase."
//	                  }

//	                  # Article content:

//	                  {{File.ReadAllText(post)}}
//	                  """;

//	var chatCompletion = await chatClient.CompleteAsync<PostCategory>(prompt);

//	Console.WriteLine(
//		$"{chatCompletion.Result.Title}. Tags: {string.Join(",", chatCompletion.Result.Tags)}");
//}


//class PostCategory
//{
//	public string Title { get; set; } = string.Empty;
//	public string[] Tags { get; set; } = [];
//}


//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Flexibility with Different LLM Providers


// Using Azure OpenAI
//builder.Services.AddChatClient(new AzureOpenAIClient(
//		new Uri("AZURE_OPENAI_ENDPOINT"),
//		new DefaultAzureCredential())
//	.AsChatClient());

//// Using OpenAI
//builder.Services.AddChatClient(new OpenAIClient("OPENAI_API_KEY").AsChatClient());