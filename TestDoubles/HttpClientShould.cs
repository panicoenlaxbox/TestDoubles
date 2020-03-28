using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestDoubles
{
    public class HttpClientShould
    {
        [Fact]
        async Task intercept_request_and_use_a_fake_message_handler()
        {
            using var client = new HttpClient(new FakeMessageHandler());
            var response = await client.GetStringAsync("http://fakerestapi.azurewebsites.net/api/Authors");
            if (string.IsNullOrWhiteSpace(response))
            {
                return;
            }
            var authors = JsonConvert.DeserializeObject<IEnumerable<Author>>(response);
            foreach (var author in authors)
            {
                Console.WriteLine(author);
            }
        }
    }

    public class FakeMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authors = new List<Author>
            {
                new Author(1, 1, "foo", "bar")
            };
            var result = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(authors), Encoding.UTF8, "application/json")
            };

            return Task.FromResult(result);
        }
    }


    public class Author
    {
        public Author(int iD, int iDBook, string firstName, string lastName)
        {
            ID = iD;
            IDBook = iDBook;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }

        public int ID { get; set; }
        public int IDBook { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ID)}={ID}, {nameof(IDBook)}={IDBook}, {nameof(FirstName)}={FirstName}, {nameof(LastName)}={LastName}}}";
        }
    }
}
