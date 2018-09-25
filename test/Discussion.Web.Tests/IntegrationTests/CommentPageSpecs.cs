using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discussion.Web.Tests.Specs.Controllers;
using Xunit;

namespace Discussion.Web.Tests.IntegrationTests
{
    [Collection("AppSpecs")]
    public class CommentPageSpecs
    {
        private readonly TestApplication _theApp;

        public CommentPageSpecs(TestApplication theApp)
        {
            _theApp = theApp.Reset();
        }

        [Fact]
        public async Task should_comment_a_topic_by_an_authorized_user()
        {
            _theApp.MockUser();
            var (topic, userId) = CommentControllerSpecs.CreateTopic(_theApp);

            var response = await RequestToCreateComment(topic.Id);

            response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task should_not_be_able_to_comment_a_topic_by_anonymous_user()
        {
            _theApp.MockUser();
            var (topic, userId) = CommentControllerSpecs.CreateTopic(_theApp);
            
            _theApp.Reset();
            var response = await RequestToCreateComment(topic.Id);

            response.StatusCode.ShouldEqual(HttpStatusCode.Redirect);
            response.Headers.Location.ToString().Contains("signin").ShouldEqual(true);
        }
        
        
        private async Task<HttpResponseMessage> RequestToCreateComment(int topicId)
        {
            var request = _theApp.Server
                .CreateRequest($"/topics/{topicId}/comments/")
                .WithFormContent(new Dictionary<string, string>
                {
                    {"Content", "comment content" }
                });

            return await request.PostAsync();
        }
        
    }
}