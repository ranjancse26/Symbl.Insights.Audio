using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SymblAISharp.Async.TextApi;
using SymblAISharp.Async.JobApi;
using Symbl.Insights.Audio.Speech;
using SymblAISharp.Conversation;
using System.Linq;

namespace Symbl.Insights.Audio
{
    class Program
    {
        private static string userId = "ranjancse@gmail.com";
        private static string sentence =
              @"Hello. So this is a live demo that we are trying to give 
                where we are going to show how the platform detects various
                insights can do transcription in real time and also the 
                different topics of discussions, which would be generated 
                after the call is over, and they will be an email that will
                be sent to the inbox.  So that is the idea.  
                So I am going to do a quick conversation.  
                I would say where I will demonstrate all of this great 
                catching up.  Thanks for calling good to hear.  From you. 
                And I would love to hear more about what you have to offer?
                I will set up a time and appointment probably sometime 
                tomorrow evening where we can go over the documents 
                that you're providing.  I love all the plants.  
                I just need to discuss with my family in terms of which 
                one will we go forward with it?  It very excited to hear 
                from you and the discount and look forward to talking 
                sharply.  I have a quick question though.  
                Is there basically website?  
                Where I can go to and look at all these details myself.  
                It will be very helpful.  Can you also share the quotation 
                to me on email so that I can go ahead and talk about it 
                with my other kind of folks in the family?  That's it.  
                Thanks a lot.  Thanks for calling good catching up.  
                Talk soon.";
    
        private static string appId = "";
        private static string appSecret = "";
        private static string baseUrl = "https://api-labs.symbl.ai";

        private static string jobId = "50f9fdde-706d-46b7-b2a6-9fad620299a5";
        private static string conversationId = "5096056086855680";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Started Sybml Insights");
            string url = $"{baseUrl}/v1/process/text?enableSummary=true&refresh=true";

            SymblAuth symblAuth = new SymblAuth(appId, appSecret);
            var authResponse = symblAuth.GetAuthToken();

            if(authResponse != null)
            {
                if(jobId != "" && conversationId != "")
                {
                   await ProcessByExistingIds(authResponse.accessToken);
                }
                else
                {

                    var textPostResponse = await SymblTextApi.Post(url, authResponse.accessToken,
                        new TextPostRequest
                        {
                            messages = GetMessages()
                        });

                    if (textPostResponse != null)
                    {
                        string conversationId = textPostResponse.conversationId;
                        string jobId = textPostResponse.jobId;

                        IJobApi jobApi = new JobApi(authResponse.accessToken,
                            baseUrl);
                        var jobResponse = await jobApi.GetJobResponse(jobId);

                        while (jobResponse.status != "completed")
                        {
                            Thread.Sleep(2000);
                            jobResponse = await jobApi.GetJobResponse(jobId);
                        }

                        HandleSummary(conversationId, authResponse.accessToken);
                    }
                }
            }

            Console.WriteLine("Completed Symbl Insights");
            Console.ReadLine();
        }

        private static async Task ProcessByExistingIds(string accessToken)
        {
            IJobApi jobApi = new JobApi(accessToken,
                           baseUrl);
            var jobResponse = await jobApi.GetJobResponse(jobId);

            while (jobResponse.status != "completed")
            {
                Thread.Sleep(2000);
                jobResponse = await jobApi.GetJobResponse(jobId);
            }

            HandleSummary(conversationId, accessToken);
            HandleTopics(conversationId, accessToken);
            HandleFollowUps(conversationId, accessToken);
        }

        private static void HandleTopics(string conversationId,
            string accessToken)
        {
            IConversationApi conversationApi = new ConversationApi(accessToken);
            var topicResponse = conversationApi.GetTopics(conversationId);

     
            if(topicResponse != null && topicResponse.topics.Count > 0)
            {
                var topicsString = string.Join(" ", topicResponse.topics.Select(topic => topic.text).ToList());
                string formattedTopics = "The following are the topics" +
                    $" gathered as part of the Conversation API {topicsString}";

                SpeechHelper.ConvertToAudio(formattedTopics, "topics.mp4",
                           new VoiceHint
                           {
                               Age = 22,
                               VoiceAge = System.Speech.Synthesis.VoiceAge.Teen,
                               VoiceGender = System.Speech.Synthesis.VoiceGender.Female
                           });
            }
        }

        private static void HandleFollowUps(string conversationId,
            string accessToken)
        {
            IConversationApi conversationApi = new ConversationApi(accessToken);
            var followUpResponse = conversationApi.GetFollowUps(conversationId);

            if (followUpResponse != null && followUpResponse.followUps.Count > 0)
            {
                var followUpsString = string.Join(" ", followUpResponse.followUps.Select(followUp => followUp.text).ToList());
                string formattedFollowUps = "The following are the follow ups" +
                    $" gathered as part of the Conversation API {followUpsString}";

                SpeechHelper.ConvertToAudio(formattedFollowUps, "followUps.mp4",
                           new VoiceHint
                           {
                               Age = 22,
                               VoiceAge = System.Speech.Synthesis.VoiceAge.Teen,
                               VoiceGender = System.Speech.Synthesis.VoiceGender.Female
                           });
            }
        }

        private static void HandleSummary(string conversationId, 
            string accessToken)
        {
            try
            {
                string summaryGetUrl = $"{baseUrl}/v1/conversations/{conversationId}/summary";
                var summaryRoot = SymblTextApi.GetSummary(summaryGetUrl, accessToken);
                if (summaryRoot != null)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(
                        summaryRoot.summary));

                    if(summaryRoot.summary.Count > 0)
                    {
                        SpeechHelper.ConvertToAudio(summaryRoot.summary[0].text, "summary.mp4",
                           new VoiceHint
                           {
                               Age = 22,
                               VoiceAge = System.Speech.Synthesis.VoiceAge.Teen,
                               VoiceGender = System.Speech.Synthesis.VoiceGender.Female
                           });
                    }                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static List<Message> GetMessages()
        {
            return new List<Message>
            {
                new Message
                {
                    payload = new Payload
                    {
                        contentType = "text/plain",
                        content = sentence
                    },
                    from = new From
                    {
                        name = "Ranjan",
                        userId = userId
                    }
                }
            };
        }
    }
}
