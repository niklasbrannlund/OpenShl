using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using OpenShl;
using OpenShl.Models;

namespace Tests
{
    [TestFixture]
    public class ConnectionTest
    {
        private Connection _connection;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private AccessToken _expectedAccessToken;
        private HttpRequestMessage _sentRequestMessage;
        private string _myClientId = "myClientId";
        private string _myClientSecret = "myClientSecret";
        
        
        [SetUp]
        public void Setup()
        {
            _expectedAccessToken = new AccessToken {Token = "ABC123", ExpiresIn = 3600};
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((r, _) => _sentRequestMessage = r) 
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(_expectedAccessToken))
                });
            
            var options = new ConnectionOptions(_myClientId, _myClientSecret);
            _connection = new Connection(options, new HttpClient(_httpMessageHandlerMock.Object));
        }
        
        [Test]
        public void VerifyCorrectHttpMethod()
        {
            // act
            _connection.Connect();
         
            // assert
            Assert.That(_sentRequestMessage.Method, Is.EqualTo(HttpMethod.Post), "Wrong HttpMethod");
        }
        
        [Test]
        public void VerifyCorrectUri()
        {
            // act
            _connection.Connect();
         
            // assert
            Assert.That(_sentRequestMessage.RequestUri, Is.EqualTo(new Uri("https://openapi.shl.se/oauth2/token")), "Wrong RequestUri");
        }

        [Test]
        public async Task VerifyClientIdAndClientSecretsAreSentInRequest()
        {
            // act
            await _connection.Connect();
            var content = await _sentRequestMessage.Content.ReadAsStringAsync();
            var credentialsSentInRequest = content.Split("&");
            var clientId = credentialsSentInRequest[0];
            var clientSecret = credentialsSentInRequest[1];
            
            // assert
            Assert.That(clientId, Is.EqualTo($"client_id={_myClientId}"), "Wrong ClientId sent in request");
            Assert.That(clientSecret, Is.EqualTo($"client_secret={_myClientSecret}"), "Wrong ClientSecret sent in request");
        }

        [Test]
        public void VerifyConnectOnlyMakesOneRequest()
        {
            // act
            _connection.Connect();

            // assert
            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public void VerifyGetThrowsExceptionIfAutoConnectIsFalseAndNoAuthTokenHasBeenFetched()
        {
            // arrange
            var options = new ConnectionOptions {AutoConnect = false};
            _connection = new Connection(options, new HttpClient(_httpMessageHandlerMock.Object));

            // act 
            var exc = Assert.ThrowsAsync<Exception>(() => _connection.Get("/api/endpoint"));

            // assert
            Assert.That(exc, Is.Not.Null, "Exception should be caught");
            Assert.That(exc.Message,
                Is.EqualTo(
                    "No auth-token has been retrieved. Either set AutoConnect-flag to true or explicitly call Connect() before you make this call"));

        }
    }
}