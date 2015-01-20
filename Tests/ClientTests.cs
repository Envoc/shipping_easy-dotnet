﻿using System;
using NUnit.Framework;
using ShippingEasy;

namespace Tests
{
    [TestFixture]
    public class ClientTests
    {
        private const string OrderJson = @"{
  ""external_order_identifier"": ""ABC-100"",
  ""ordered_at"": ""2014-01-16T14:37:56-06:00"",
  ""recipients"": [
    {
      ""first_name"": ""Colin"",
      ""last_name"": ""Smith"",
      ""address"": ""1600 Pennsylvania Ave"",
      ""line_items"": [
        {
          ""item_name"": ""Sprocket"",
          ""quantity"": 7
        }
      ]
    }
  ]
}";

        [Test]
        public void CreateInstanceWithValidUrl()
        {
            Assert.DoesNotThrow(() =>
            {
                var client = new Client("apiKey", "apiSecret", "https://api.example.com:8080");
                Assert.IsInstanceOf<Client>(client);
            });
        }

        [Test]
        public void CreateInstanceWithInvalidUrl()
        {
            Assert.Catch<UriFormatException>(() =>
            {
                var client = new Client("apiKey", "apiSecret", "badexample");
                Assert.IsInstanceOf<Client>(client);
            });
        }

        [Test]
        public void CreateOrderFromJson()
        {
            var order = new Client().ParseOrder(OrderJson);
            Assert.AreEqual("ABC-100", order.OrderIdentifier);
            var date = new DateTimeOffset(2014, 1, 16, 14, 37, 56, new TimeSpan(-6, 0, 0));
            Assert.AreEqual(date, order.OrderedAt);

            Assert.AreEqual(1, order.Recipients.Count);
            var recipient = order.Recipients[0];

            Assert.AreEqual("Colin", recipient.FirstName);
            Assert.AreEqual("Smith", recipient.LastName);
            Assert.AreEqual(1, recipient.LineItems.Count);
            var lineItem = recipient.LineItems[0];

            Assert.AreEqual("Sprocket", lineItem.ItemName);
            Assert.AreEqual(7, lineItem.Quantity);
        }

        [Test]
        public void CreateJsonFromOrder()
        {
            var order = new Order();
            order.OrderIdentifier = "ABC-100";
            order.OrderedAt = new DateTimeOffset(2014, 1, 16, 14, 37, 56, new TimeSpan(-6, 0, 0));
            var recipient = new Recipient();
            recipient.FirstName = "Colin";
            recipient.LastName = "Smith";
            recipient.Address = "1600 Pennsylvania Ave";
            var lineItem = new LineItem();
            lineItem.ItemName = "Sprocket";
            lineItem.Quantity = 7;
            recipient.LineItems.Add(lineItem);
            order.Recipients.Add(recipient);

            var json = new Client().OrderToJson(order);
            Assert.AreEqual(OrderJson, json);
        }
    }
}