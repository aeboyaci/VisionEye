# Public APIs

## Text-to-Speech API:
https://cloud.google.com/text-to-speech
Google Text-to-Speech allows developers to create natural-sounding, synthetic human speech as playable audio. This way, the created audio can be integrated with the developed applications. We will use this API to get compelling audio that is relevant to our story. Instead of having to record voices, we will produce them from text.

### How to Use Text-to-Speech API
In order to get an audio file for the given text (the process is called synthesis), a POST request should be sent to the synthesize endpoint.

POST https://texttospeech.googleapis.com/v1/text:synthesize

The body of the request should include three essential components which are:
  
 &emsp; **Input:** 
  The input can be a plain text or an SSML document.

 &emsp; **Voice:**
  The voice parameter is used to specify the language,  selection of the voice (there are several options) and the gender selection (which is optional and might not be available with a specific language or voice selection). 

 &emsp; **AudioConfig:**
  The AudioConfig parameter is used to determine the configuration of API’s output audio file. It must contain the encoding information. There are also numerous options that can be used for the configuration that can be found at the figure below:


### Pricing
Google does not provide a free-of-charge version of the Text-to-Speech API. However, every new  Google Cloud user gets a 300$ worth of free credit. This way, we can use the API for our application without having to pay.

## Trivia:
https://the-trivia-api.com/docs
The puzzles in the game might be containing some quizzes that dynamically change every time a player encounters them. We will be using the Trivia API in order to get that dynamic questions. The integration of the Trivia API will be integrated through our own API. The users will access our API’s endpoint to get the quizzes while we get them through the Trivia API.

### How to Use the Trivia API:
The main functionality that we will be using is getting the questions. In order to make this happen, a GET request will be sent to /api/questions endpoint.
The API lets the user choose between different categories, to choose a difficulty level, and region in order to get appropriate quizzes for the purpose.
The Trivia API also allows the user to create sessions so no duplicate questions will be sent. However, this is not available in the free version of the API. Since our players will be accessing the quizzes through our own API, we can filter the duplicates by ourselves.
## How Does the Response Look Like:
When the Trivia API is accessed, it returns a JSON formatted data which contains the question, the answer options, the correct answer  and the metadata of the question which includes category, tags etc.








## The License and Pricing
It is allowed to use the questions from the API for non-commercial use (educational, personal etc). The free version has some limitations about the features and the rate limit. However, I believe we can use our own API to get our project running with the free version. If not, the paid versions are not too expensive so using a paid version is an option as well. 



