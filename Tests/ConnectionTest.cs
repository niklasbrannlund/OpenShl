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
            var credentialsSentInRequest = JsonSerializer.Deserialize<ConnectionOptions>(content);
            
            // assert
            Assert.That(credentialsSentInRequest.ClientId, Is.EqualTo(_myClientId), "Wrong ClientId sent in request");
            Assert.That(credentialsSentInRequest.ClientSecret, Is.EqualTo(_myClientSecret), "Wrong ClientId sent in request");

        }
    }
}